using System;
using DependencyInjectionWorkshop.Adapters.Interfaces;

namespace DependencyInjectionWorkshop.Adapters
{
    public class ConsoleAdapter : ILogger
    {
        public virtual void Info(string message)
        {
            Console.WriteLine(message);
        }
    }
}