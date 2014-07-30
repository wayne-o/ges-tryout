using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conversations.Domain
{
    using CommonDomain.Core;

    using Conversations.Events;

    public class Conversation : AggregateBase
    {
        public Conversation(Guid aggregateId, string subject, string body)
            : this()
        {
            RaiseEvent(new ConversationStarted(aggregateId)
                           {
                               Body = body,
                               Subject = subject
                           });
        }

        private Conversation()
        {
            Register<ConversationStarted>(e => Id = e.AggregateId);
        }
    }
}
