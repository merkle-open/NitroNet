using System;

namespace NitroNet.Common.Exceptions
{
    public class NitroNetComponentException : Exception
    {
        public NitroNetComponentException()
        {
        }

        public NitroNetComponentException(string message)
            : base(message)
        {
        }

        public NitroNetComponentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
