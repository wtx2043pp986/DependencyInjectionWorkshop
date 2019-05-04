using DependencyInjectionWorkshop.Adapters;
using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.Models.Interfaces;
using DependencyInjectionWorkshop.Repository;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IFailedCounter _failedCounter;
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otpRemoteProxy;
        private readonly ILogger _logger;
        private readonly INotification _slackAdapter;

        public AuthenticationService()
        {
            _failedCounter = new FailedCounter();
            _profile = new ProfileRepository();
            _hash = new Sha256Adapter();
            _otpRemoteProxy = new OtpRemoteProxy();
            _logger = new NLogAdapter();
            _slackAdapter = new SlackAdapter();
        }

        public AuthenticationService(IFailedCounter failedCounter, IProfile profile, 
            IHash hash, IOtp otpRemoteProxy, 
            ILogger logger, INotification slackAdapter)
        {
            _failedCounter = failedCounter;
            _profile = profile;
            _hash = hash;
            _otpRemoteProxy = otpRemoteProxy;
            _logger = logger;
            _slackAdapter = slackAdapter;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLocked(accountId);

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

                _slackAdapter.Notify(accountId);

                return false;
            }

        }
    }
}