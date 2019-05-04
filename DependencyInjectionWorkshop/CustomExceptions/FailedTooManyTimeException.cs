using System;

namespace DependencyInjectionWorkshop.CustomExceptions
{
    public class FailedTooManyTimeException : Exception
    {
        public FailedTooManyTimeException(string errorMessage) : base(errorMessage)
        {
        }
    }
}