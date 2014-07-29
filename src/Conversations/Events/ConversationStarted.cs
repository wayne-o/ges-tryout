namespace Conversations.Events
{
    using System;

    /// <summary>
    /// The conversation started.
    /// </summary>
    public class ConversationStarted 
    {
        public ConversationStarted(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; private set; }
    }
}