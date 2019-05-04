using DependencyInjectionWorkshop.Repository;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly ProfileRepository _profileRepository = new ProfileRepository();
        private readonly Sha256Adapter _sha256Adapter = new Sha256Adapter();
        private readonly OtpRemoteProxy _otpRemoteProxy = new OtpRemoteProxy();
        private readonly FailedCounter _failedCounter = new FailedCounter();
        private readonly NLogAdapter _nLogAdapter = new NLogAdapter();
        private readonly SlackAdapter _slackAdapter = new SlackAdapter();

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLocked(accountId);

            var hashedPasswordFromDb = _profileRepository.GetPasswordFromDb(accountId);

            var hashedPassword = _sha256Adapter.GetHashedPassword(password);

            var currentOtp = _otpRemoteProxy.GetCurrentOtp(accountId);

            if (hashedPasswordFromDb == hashedPassword && currentOtp == otp)
            {
                _failedCounter.ResetFailedCounter(accountId);

                return true;
            }
            else
            {
                _failedCounter.AddFailedCounter(accountId);

                var failedCount = _failedCounter.GetFailedCount(accountId);

                _nLogAdapter.LogFailedCount($"{accountId} has already verified failed {failedCount}");

                _slackAdapter.Notify(accountId);

                return false;
            }

        }
    }
}