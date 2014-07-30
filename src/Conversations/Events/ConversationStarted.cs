namespace Conversations.Events
{
    using System;

    /// <summary>
    /// The conversation started.
    /// </summary>
    public class ConversationStarted : IDomainEvent
    {
        public ConversationStarted(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; private set; }

        public uint Version { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }
    }
}