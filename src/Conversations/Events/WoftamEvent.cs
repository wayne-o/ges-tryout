namespace Conversations.Events
{
    using System;

    public class WoftamEvent : IDomainEvent
    {
        public WoftamEvent(string property1, string property2)
        {
            this.Property1 = property1;
            this.Property2 = property2;
        }

        public string Property1 { get; private set; }
        public string Property2 { get; private set; }

        public Guid AggregateId { get; private set; }

        public uint Version { get; set; }
    }
}