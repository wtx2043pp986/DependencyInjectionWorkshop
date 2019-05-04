using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public class OtpRemoteProxy
    {
        public string GetCurrentOtp(string accountId)
        {
            var otpApiResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/otps", accountId).Result;
            string currentOtp;
            if (otpApiResponse.IsSuccessStatusCode)
            {
                currentOtp = otpApiResponse.Content.ReadAsAsync<string>().Result;
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            return currentOtp;
        }
    }
}