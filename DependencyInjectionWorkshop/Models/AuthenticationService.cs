using System;
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
            using (var connection = new SqlConnection("my connection string"))
            {
                var hashedPasswordFromDb = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();

                var crypt = new System.Security.Cryptography.SHA256Managed();
                var hash = new StringBuilder();
                var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
                foreach (var theByte in crypto)
                    hash.Append(theByte.ToString("x2"));

                var hashedPassword = hash.ToString();
                
                var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
                var otpApiResponse = httpClient.PostAsJsonAsync("api/otps", accountId).Result;

                string currentOtp;
                if (otpApiResponse.IsSuccessStatusCode)
                {
                    currentOtp = otpApiResponse.Content.ReadAsAsync<string>().Result;
                }
                else
                {
                    throw new Exception($"web api error, accountId:{accountId}");
                }

                if (hashedPasswordFromDb == hashedPassword && currentOtp == otp)
                {
                    return true;
                }
                else
                {
                    var slackClient = new SlackClient("my api token");

                    var message = $"{accountId} try to verify failed";
                    slackClient.PostMessage(res => { }, "my channel", message, "my bot name");
                    return false;
                }
            }

        }
    }
}