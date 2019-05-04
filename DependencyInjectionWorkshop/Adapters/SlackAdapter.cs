using DependencyInjectionWorkshop.Adapters.Interfaces;
using SlackAPI;

namespace DependencyInjectionWorkshop.Adapters
{
    public class SlackAdapter : INotification
    {
        public void Notify(string accountId)
        {
            var slackClient = new SlackClient("my api token");
            var message = $"{accountId} try to verify failed";
            slackClient.PostMessage(res => { }, "my channel", message, "my bot name");
        }
    }
}