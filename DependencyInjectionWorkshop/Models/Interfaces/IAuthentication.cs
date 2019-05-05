namespace DependencyInjectionWorkshop.Models.Interfaces
{
    public interface IAuthentication
    {
        bool Verify(string accountId, string password, string otp);
    }
}