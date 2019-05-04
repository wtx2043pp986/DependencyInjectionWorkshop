using DependencyInjectionWorkshop.Adapters.Interfaces;
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
        private const string DefautOtp = "123456";
        private IProfile _profile;
        private IOtp _otpRemoteProxy;
        private IHash _hash;
        private INotification _notification;
        private IFailedCounter _failedCounter;
        private ILogger _logger;
        private AuthenticationService _authenticationService;

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
            _authenticationService =
                new AuthenticationService(_failedCounter, _profile, _hash, _otpRemoteProxy, _logger, _notification);
        }

        [Test]
        public void is_valid()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefautOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefautOtp);
            
            ShouldBeValid(isValid);
        }


        [Test]
        public void is_invalid_when_wrong_otp()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefautOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
            
            ShouldBeInvalid(isValid);
        }

        [Test]
        public void notify_user_when_invalid()
        {
            WhenInvalid();
            ShouldNotifyUser();
        }

        private void ShouldNotifyUser()
        {
            _notification.Received(1).Notify(Arg.Any<string>());
        }

        private void WhenInvalid()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHash(DefaultHashPassword, DefaultPassword);
            GivenOtp(DefaultAccountId, DefautOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
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