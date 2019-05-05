using System;
using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.CustomExceptions;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Models.Interfaces;
using DependencyInjectionWorkshop.Repository;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private const string DefaultAccountId = "roberson";
        private const string DefaultHashPassword = "my_hash_password";
        private const string DefaultPassword = "password";
        private const string DefaultOtp = "123456";
        private const int DefaultFailedCount = 87;
        private IProfile _profile;
        private IOtp _otpRemoteProxy;
        private IHash _hash;
        private INotification _notification;
        private IFailedCounter _failedCounter;
        private ILogger _logger;
        private IAuthentication _authenticationService;

        [SetUp]
        public void Setup()
        {
            //Use NSubstitute to mock objects.
            _profile = Substitute.For<IProfile>();
            _otpRemoteProxy = Substitute.For<IOtp>();
            _hash = Substitute.For<IHash>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _logger = Substitute.For<ILogger>();

            var authenticationService = new AuthenticationService(_failedCounter, _profile, _hash, _otpRemoteProxy, _logger);
            var notificationDecorator = new NotificationDecorator(authenticationService, _notification);
            _authenticationService = new FailedCounterDecorator(notificationDecorator, _failedCounter);

        }

        [Test]
        public void is_valid()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);
            
            ShouldBeValid(isValid);
        }


        [Test]
        public void is_invalid_when_wrong_otp()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
            
            ShouldBeInvalid(isValid);
        }

        [Test]
        public void notify_user_when_invalid()
        {
            WhenInvalid();
            ShouldNotifyUser();
        }

        [Test]
        public void reset_failed_count_when_valid()
        {
            WhenValid();
            _failedCounter.Received(1).Reset(DefaultAccountId);
        }
        
        [Test]
        public void account_is_locked()
        {
            _failedCounter.CheckAccountIsLocked(DefaultAccountId).ReturnsForAnyArgs(true);

            TestDelegate action = () => _authenticationService.Verify(DefaultAccountId, DefaultHashPassword, DefaultOtp);
            Assert.Throws<FailedTooManyTimeException>(action);
        }

        private bool WhenValid()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);
            return isValid;
        }
        
        private void ShouldNotifyUser()
        {
            _notification.Received(1).PushMessage(Arg.Any<string>());
        }
        
        
        [Test]
        public void Log_account_failed_count_when_invalid()
        {
            GivenFailedCount();
            
            WhenInvalid();

            LogShouldContains(DefaultAccountId);
        }

        private void GivenFailedCount()
        {
            _failedCounter.Get(DefaultAccountId).ReturnsForAnyArgs(DefaultFailedCount);
        }

        private void LogShouldContains(string accountId)
        {
            _logger.Received(1).Info(Arg.Is<string>(x => x.Contains(accountId) && x.Contains(DefaultFailedCount.ToString())));
        }

        private bool WhenInvalid()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
            return isValid;
        }

        private static void ShouldBeInvalid(bool verify)
        {
            Assert.IsFalse(verify);
        }


        private static void ShouldBeValid(bool verify)
        {
            Assert.IsTrue(verify);
        }

        private void GivenOtp(string accountId, string otp)
        {
            _otpRemoteProxy.GetCurrentOtp(accountId).ReturnsForAnyArgs(otp);
        }

        private void GivenHash(string hashPassword, string password)
        {
            _hash.GetHash(password).ReturnsForAnyArgs(hashPassword);
        }

        private void GivenPassword(string accountId, string hashPassword)
        {
            _profile.GetPassword(accountId).ReturnsForAnyArgs(hashPassword);
        }

        private bool WhenVerify(string accountId, string password, string otp)
        {
            return _authenticationService.Verify(accountId, password, otp);
        }
    }
}