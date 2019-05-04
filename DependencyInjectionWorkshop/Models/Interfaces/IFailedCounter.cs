namespace DependencyInjectionWorkshop.Models.Interfaces
{
    public interface IFailedCounter
    {
        void Reset(string accountId);
        void Add(string accountId);
        void CheckAccountIsLocked(string accountId);
        int Get(string accountId);
    }
}