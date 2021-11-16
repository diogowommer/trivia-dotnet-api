using CrossCutting.MessageHelpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossCutting.MessageHelpers
{
    public class MessageStatus
        : Enumeration
    {
        public static MessageStatus Error = new MessageStatus(1, nameof(Error).ToLowerInvariant());
        public static MessageStatus Success = new MessageStatus(2, nameof(Success).ToLowerInvariant());
        public static MessageStatus Warning = new MessageStatus(3, nameof(Warning).ToLowerInvariant());
        public static MessageStatus Wait = new MessageStatus(4, nameof(Wait).ToLowerInvariant());

        public MessageStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<MessageStatus> List() =>
            new[] { Error, Success, Warning, Wait };

        public static MessageStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new MessageStatusException($"Possible values for MessageStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static MessageStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new MessageStatusException($"Possible values for MessageStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
