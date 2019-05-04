using System.Text;
using DependencyInjectionWorkshop.Adapters.Interfaces;

namespace DependencyInjectionWorkshop.Adapters
{
    public class Sha256Adapter : IHash
    {
        public  string GetHash(string plainText)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            foreach (var theByte in crypto)
                hash.Append(theByte.ToString("x2"));

            var hashedPassword = hash.ToString();
            return hashedPassword;
        }
    }
}