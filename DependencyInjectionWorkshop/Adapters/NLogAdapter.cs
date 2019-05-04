using NLog;
using ILogger = DependencyInjectionWorkshop.Adapters.Interfaces.ILogger;


namespace DependencyInjectionWorkshop.Adapters
{
    public class NLogAdapter : ILogger
    {
        public virtual void Info(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(message);
        }
    }
}