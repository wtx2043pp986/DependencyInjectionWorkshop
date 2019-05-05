using DependencyInjectionWorkshop.CustomExceptions;
using DependencyInjectionWorkshop.Models.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounterDecorator : IAuthentication
    {
        private readonly IAuthentication _authentication;
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthentication authentication, IFailedCounter failedCounter)
        {
            _authentication = authentication;
            _failedCounter = failedCounter;
        }

        private void CheckAccountIsLocked(string accountId)
        {
            if (_failedCounter.CheckAccountIsLocked(accountId))
            {
                var errorMessage = $"{accountId} has been locked, ";
                throw new FailedTooManyTimeException(errorMessage);
            }
        }

        public bool Verify(string accountId, string password, string otp)
        {
            CheckAccountIsLocked(accountId);

            var isValid = _authentication.Verify(accountId, password, otp);

            return isValid;
        }
    }
}