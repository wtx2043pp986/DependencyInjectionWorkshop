using System;
using System.Net.Http;
using DependencyInjectionWorkshop.CustomExceptions;
using DependencyInjectionWorkshop.Models.Interfaces;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounter : IFailedCounter
    {
        private readonly HttpClient _httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.dev/")};

        public  void Reset(string accountId)
        {
            var resetFailedCounterApiResponse = _httpClient
                .PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetFailedCounterApiResponse.EnsureSuccessStatusCode();
        }

        public  void Add(string accountId)
        {
            var addFailedCounterApiResponse = _httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCounterApiResponse.EnsureSuccessStatusCode();
        }

        public bool CheckAccountIsLocked(string accountId)
        {
            var isLockedResponse = _httpClient.PostAsJsonAsync("api/failedCounter/isLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();
            var isAccountLocked = isLockedResponse.Content.ReadAsAsync<bool>().Result;

            return isAccountLocked;
        }

        public int Get(string accountId)
        {
            var getFailedCounterApiResponse = _httpClient.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
            getFailedCounterApiResponse.EnsureSuccessStatusCode();
            var failedCount = getFailedCounterApiResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }
    }
}