using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CrossCutting.MessageHelpers
{
    public class Response
    {
        private readonly IDictionary<string, string> messages = new Dictionary<string, string>();

        private readonly MessageStatus Status = MessageStatus.Wait;

        public IDictionary<string, string> Messages { get; }
        public object Result { get; }

        public Response()
        {
            Messages = new ReadOnlyDictionary<string, string>(this.messages);
        }

        public Response(object result) : this()
        {
            Result = result;
        }

        public Response AddError(string key, string value)
        {
            this.messages.Add(key, value);

            return this;
        }
    }
}
