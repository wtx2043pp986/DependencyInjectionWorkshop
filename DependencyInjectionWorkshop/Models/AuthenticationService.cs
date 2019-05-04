﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using Dapper;
using SlackAPI;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {

        public bool Verify(string accountId, string password, string otp)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") };
            var isLockedResponse = httpClient.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();
            var isAccountLocked = isLockedResponse.Content.ReadAsAsync<bool>().Result;
            if (isAccountLocked)
            {
                var errorMessage = $"{accountId} has been locked, ";
                throw new AccountLockedException(errorMessage);
            }
            
            string hashedPasswordFromDb;
            using (var connection = new SqlConnection("my connection string"))
            {
                hashedPasswordFromDb = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
                hash.Append(theByte.ToString("x2"));

            var hashedPassword = hash.ToString();

            var otpHttpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
            var otpApiResponse = otpHttpClient.PostAsJsonAsync("api/otps", accountId).Result;

            string currentOtp;
            if (otpApiResponse.IsSuccessStatusCode)
            {
                currentOtp = otpApiResponse.Content.ReadAsAsync<string>().Result;
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            var failedCounterHttpClient = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") };

            if (hashedPasswordFromDb == hashedPassword && currentOtp == otp)
            {
                var resetFailedCounterApiResponse = failedCounterHttpClient
                    .PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
                resetFailedCounterApiResponse.EnsureSuccessStatusCode();

                return true;
            }
            else
            {
                var addFailedCounterApiResponse = failedCounterHttpClient.
                    PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
                addFailedCounterApiResponse.EnsureSuccessStatusCode();

                var slackClient = new SlackClient("my api token");
                var message = $"{accountId} try to verify failed";
                slackClient.PostMessage(res => { }, "my channel", message, "my bot name");
                return false;
            }

        }
    }

    public class AccountLockedException : Exception
    {
        public AccountLockedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}