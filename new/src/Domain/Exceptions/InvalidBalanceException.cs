using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class InvalidBalanceException : Exception
    {
        public InvalidBalanceException(string value)
            : base($"Invalid balance {value}")
        {
        }
    }
}