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
        public int AppliedEventCount { get; private set; }

        public void ProduceEvents(int count)
        {
            for (int i = 0; i < count; i++)
                RaiseEvent(new WoftamEvent("Woftam1-" + i, "Woftam2-" + i));
        }

        public void ProduceWikiWahEvents(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.RaiseEvent(new WikiWikiWahEvent("blah-" + i));
            }
        }

        public Conversation(Guid aggregateId)
            : this()
        {
            RaiseEvent(new ConversationStarted(aggregateId));
        }

        private Conversation()
        {
            Register<ConversationStarted>(e => Id = e.AggregateId);
            Register<WoftamEvent>(e => AppliedEventCount++);
            Register<WikiWikiWahEvent>(e => AppliedEventCount++);
        }
    }

  

    public class WikiWikiWahEvent
    {
        public string Something { get; set; }

        public WikiWikiWahEvent(string something)
        {
            this.Something = something;
        }
    }

    public class WoftamEvent
    {
        public WoftamEvent(string property1, string property2)
        {
            Property1 = property1;
            Property2 = property2;
        }

        public string Property1 { get; private set; }
        public string Property2 { get; private set; }
    }
}
