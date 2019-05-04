using System;
using System.Net.Http;
using DependencyInjectionWorkshop.CustomExceptions;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounter
    {
        private readonly HttpClient _httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.dev/")};

        public  void ResetFailedCounter(string accountId)
        {
            var resetFailedCounterApiResponse = _httpClient
                .PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetFailedCounterApiResponse.EnsureSuccessStatusCode();
        }

        public  void AddFailedCounter(string accountId)
        {
            var addFailedCounterApiResponse = _httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCounterApiResponse.EnsureSuccessStatusCode();
        }

        public void CheckAccountIsLocked(string accountId)
        {
            var isLockedResponse = _httpClient.PostAsJsonAsync("api/failedCounter/isLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();
            var isAccountLocked = isLockedResponse.Content.ReadAsAsync<bool>().Result;
            if (isAccountLocked)
            {
                var errorMessage = $"{accountId} has been locked, ";
                throw new FailedTooManyTimeException(errorMessage);
            }
        }

        public int GetFailedCount(string accountId)
        {
            var getFailedCounterApiResponse = _httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
            getFailedCounterApiResponse.EnsureSuccessStatusCode();
            var failedCount = getFailedCounterApiResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }
    }
}