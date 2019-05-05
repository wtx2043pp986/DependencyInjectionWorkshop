namespace DependencyInjectionWorkshop.Models.Interfaces
{
    public interface IOtp
    {
        string GetCurrentOtp(string accountId);
    }
}