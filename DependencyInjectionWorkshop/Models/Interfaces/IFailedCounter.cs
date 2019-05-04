namespace DependencyInjectionWorkshop.Models.Interfaces
{
    public interface IFailedCounter
    {
        void Reset(string accountId);
        void Add(string accountId);
        bool CheckAccountIsLocked(string accountId);
        int Get(string accountId);
    }
}