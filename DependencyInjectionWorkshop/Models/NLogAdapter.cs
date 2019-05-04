using NLog;

namespace DependencyInjectionWorkshop.Models
{
    public class NLogAdapter
    {
        public void LogFailedCount(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(message);
        }
    }
}