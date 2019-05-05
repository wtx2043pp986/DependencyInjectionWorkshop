using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.Models.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public class LogDecorator : IAuthentication
    {
        private IAuthentication _authentication;
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogDecorator(IAuthentication authentication, IFailedCounter failedCounter, ILogger logger)
        {
            _authentication = authentication;
            _failedCounter = failedCounter;
            _logger = logger;
        }

        private void LogFailedCounter(string accountId)
        {
            var failedCount = _failedCounter.Get(accountId);
            
            _logger.Info($"{accountId} has already verified failed {failedCount}");
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var isValid = _authentication.Verify(accountId, password, otp);

            if (!isValid)
            {
                LogFailedCounter(accountId);
            }

            return isValid;
        }
    }
}