using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.Models.Interfaces;

namespace DependencyInjectionWorkshop.Models.Decorators
{
    public class NotificationDecorator : AuthenticationBaseDecorator
    {
        private readonly INotification _notification;

        public NotificationDecorator(IAuthentication authentication, INotification notification) : base(authentication)
        {
            _notification = notification;
        }

        private void NotifyVerify(string accountId)
        {
            _notification.PushMessage(accountId);
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var isValid = base.Verify(accountId, password, otp);

            if (!isValid)
            {
                NotifyVerify(accountId);
            }

            return isValid;
        }
    }
}