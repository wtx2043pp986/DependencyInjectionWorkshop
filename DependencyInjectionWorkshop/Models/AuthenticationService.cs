using System.Net.Http.Formatting;
using DependencyInjectionWorkshop.Adapters;
using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.Repository;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otpRemoteProxy;

        public AuthenticationService(IProfile profile,
            IHash hash, IOtp otpRemoteProxy)
        {
            _profile = profile;
            _hash = hash;
            _otpRemoteProxy = otpRemoteProxy;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var hashedPasswordFromDb = _profile.GetPassword(accountId);

            var hashedPassword = _hash.GetHash(password);

            var currentOtp = _otpRemoteProxy.GetCurrentOtp(accountId);

            if (hashedPasswordFromDb == hashedPassword && currentOtp == otp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}