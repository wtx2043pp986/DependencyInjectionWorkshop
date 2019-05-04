using SlackAPI;

namespace DependencyInjectionWorkshop.Models
{
    public class SlackAdapter
    {
        public void Notify(string accountId)
        {
            var slackClient = new SlackClient("my api token");
            var message = $"{accountId} try to verify failed";
            slackClient.PostMessage(res => { }, "my channel", message, "my bot name");
        }
    }
}