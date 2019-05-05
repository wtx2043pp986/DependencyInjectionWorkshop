using DependencyInjectionWorkshop.Adapters.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public abstract class AuthenticationBaseDecorator : IAuthentication
    {
        private readonly IAuthentication _authentication;

        protected AuthenticationBaseDecorator(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public virtual bool Verify(string accountId, string password, string otp)
        {
            return _authentication.Verify(accountId,password,otp);
        }
    }

    public class NotificationDecorator : AuthenticationBaseDecorator
    {
        private readonly INotification _notification;

        public NotificationDecorator(IAuthentication authentication, INotification notification) : base(authentication)
        {
            _notification = notification;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var isValid = base.Verify(accountId, password, otp);

            if (!isValid)
            {
                _notification.PushMessage(accountId);
            }

            return isValid;
        }

        
    }
}