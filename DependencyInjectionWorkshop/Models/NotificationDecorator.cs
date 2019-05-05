using DependencyInjectionWorkshop.Adapters.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public class NotificationDecorator : IAuthentication
    {
        private readonly IAuthentication _authentication;
        private readonly INotification _notification;

        public NotificationDecorator(IAuthentication authentication, INotification notification)
        {
            _authentication = authentication;
            _notification = notification;
        }

        private void NotifyVerify(string accountId)
        {
            _notification.PushMessage(accountId);
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var isValid = _authentication.Verify(accountId, password, otp);

            if (!isValid)
            {
                NotifyVerify(accountId);
            }

            return isValid;
        }
    }
}