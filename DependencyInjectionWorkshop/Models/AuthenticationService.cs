﻿using System.Net.Http.Formatting;
using DependencyInjectionWorkshop.Adapters;
using DependencyInjectionWorkshop.Adapters.Interfaces;
using DependencyInjectionWorkshop.CustomExceptions;
using DependencyInjectionWorkshop.Models.Interfaces;
using DependencyInjectionWorkshop.Repository;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IFailedCounter _failedCounter;
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otpRemoteProxy;
        private readonly ILogger _logger;

        public AuthenticationService(IFailedCounter failedCounter, IProfile profile, 
            IHash hash, IOtp otpRemoteProxy, ILogger logger)
        {
            _failedCounter = failedCounter;
            _profile = profile;
            _hash = hash;
            _otpRemoteProxy = otpRemoteProxy;
            _logger = logger;
        }

        public IFailedCounter FailedCounter
        {
            get { return _failedCounter; }
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
                //AddFailedCounter(accountId);

                var failedCount = _failedCounter.Get(accountId);

                _logger.Info($"{accountId} has already verified failed {failedCount}");
                
                return false;
            }
        }
    }
}