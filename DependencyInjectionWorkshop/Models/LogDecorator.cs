using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.Models.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public class LogDecorator : AuthenticationBaseDecorator
    {
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogDecorator(IAuthentication authentication, IFailedCounter failedCounter, ILogger logger) : base(authentication)
        {
            _failedCounter = failedCounter;
            _logger = logger;
        }

        private void LogFailedCount(string accountId)
        {
            var failedCount = _failedCounter.Get(accountId);
            _logger.Info($"{accountId} has already verified failed {failedCount}");
        }
    }
}