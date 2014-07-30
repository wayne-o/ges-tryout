namespace Conversations.Events
{
    using System;

    public class WikiWikiWahEvent : IDomainEvent
    {
        public string Something { get; set; }

        public WikiWikiWahEvent(string something)
        {
            this.Something = something;
        }

        public Guid AggregateId { get; private set; }

        public uint Version { get; set; }
    }
}