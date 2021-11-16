using System;

namespace CrossCutting.MessageHelpers.Exceptions
{
    public class MessageStatusException : Exception
    {
        public MessageStatusException()
        { }

        public MessageStatusException(string message)
            : base(message)
        { }

        public MessageStatusException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
