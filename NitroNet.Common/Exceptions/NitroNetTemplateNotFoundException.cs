using System;

namespace NitroNet.Common.Exceptions
{
    public class NitroNetTemplateNotFoundException : Exception
    {
        public NitroNetTemplateNotFoundException()
        {
        }

        public NitroNetTemplateNotFoundException(string message)
            : base(message)
        {
        }

        public NitroNetTemplateNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
