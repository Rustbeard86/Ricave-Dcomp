using System;

namespace Ricave.Core
{
    public class LoadingException : Exception
    {
        public LoadingException(string message)
            : base(message)
        {
        }
    }
}