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

        private void ThrowExceptionWhenAccountLocked(string accountId)
        {
            var isAccountLocked = _failedCounter.CheckAccountIsLocked(accountId);
            if (isAccountLocked)
            {
                var errorMessage = $"{accountId} has been locked, ";
                throw new FailedTooManyTimeException(errorMessage);
            }
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            ThrowExceptionWhenAccountLocked(accountId);

            var isValid = base.Verify(accountId, password, otp);

            if (isValid)
            {
                ResetFailedCounter(accountId);
            }
            else
            {
                AddFailedCounter(accountId);
            }

            return isValid;
        }

        private void ResetFailedCounter(string accountId)
        {
            _failedCounter.Reset(accountId);
        }

        private void AddFailedCounter(string accountId)
        {
            _failedCounter.Add(accountId);
        }
    }
}