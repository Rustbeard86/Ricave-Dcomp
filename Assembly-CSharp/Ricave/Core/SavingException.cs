using System;

namespace Ricave.Core
{
    public class SavingException : Exception
    {
        public SavingException(string message)
            : base(message)
        {
        }
    }
}