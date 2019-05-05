using DependencyInjectionWorkshop.CustomExceptions;
using DependencyInjectionWorkshop.Models.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounterDecorator : AuthenticationBaseDecorator
    {
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthentication authentication, IFailedCounter failedCounter) : base(authentication)
        {
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

        public override bool Verify(string accountId, string password, string otp)
        {
            CheckAccountIsLocked(accountId);

            var isValid = base.Verify(accountId, password, otp);

            if (isValid)
            {
                _failedCounter.Reset(accountId);
            }
            else
            {
                _failedCounter.Add(accountId);
            }

            return isValid;
        }
    }
}