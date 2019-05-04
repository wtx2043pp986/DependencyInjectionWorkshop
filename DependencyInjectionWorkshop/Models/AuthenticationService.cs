using System.Net.Http.Formatting;
using DependencyInjectionWorkshop.Adapters;
using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.CustomExceptions;
using DependencyInjectionWorkshop.Models.Interfaces;
using DependencyInjectionWorkshop.Repository;

namespace DependencyInjectionWorkshop.Models
{
    public interface IAuthentication
    {
        bool Verify(string accountId, string password, string otp);
    }

    public class AuthenticationService : IAuthentication
    {
        private readonly IFailedCounter _failedCounter;
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otpRemoteProxy;
        private readonly ILogger _logger;
        private readonly INotification _notification;
        
        //public AuthenticationService()
        //{
        //    _failedCounter = new FailedCounter();
        //    _profile = new ProfileRepository();
        //    _hash = new Sha256Adapter();
        //    _otpRemoteProxy = new OtpRemoteProxy();
        //    _logger = new NLogAdapter();
        //    _notification = new SlackAdapter();
        //}

        public AuthenticationService(IFailedCounter failedCounter, IProfile profile, 
            IHash hash, IOtp otpRemoteProxy, 
            ILogger logger, INotification notification)
        {
            _failedCounter = failedCounter;
            _profile = profile;
            _hash = hash;
            _otpRemoteProxy = otpRemoteProxy;
            _logger = logger;
            _notification = notification;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var isAccountLocked = _failedCounter.CheckAccountIsLocked(accountId);
            if (isAccountLocked)
            {
                var errorMessage = $"{accountId} has been locked, ";
                throw new FailedTooManyTimeException(errorMessage);
            }

            var hashedPasswordFromDb = _profile.GetPassword(accountId);

            var hashedPassword = _hash.GetHash(password);

            var currentOtp = _otpRemoteProxy.GetCurrentOtp(accountId);

            if (hashedPasswordFromDb == hashedPassword && currentOtp == otp)
            {
                _failedCounter.Reset(accountId);

                return true;
            }
            else
            {
                _failedCounter.Add(accountId);

                var failedCount = _failedCounter.Get(accountId);

                _logger.Info($"{accountId} has already verified failed {failedCount}");

                _notification.PushMessage(accountId);

                return false;
            }

        }
    }
}