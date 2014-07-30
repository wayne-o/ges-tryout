
namespace Conversations.Events
{
    using System;

    using Raven.Client;

    public class ConversationDenormalizer : IHandlesEvent<ConversationStarted>
    {
        private readonly IDocumentStore documentStore;

        public ConversationDenormalizer(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

     
        public void Consume(ConversationStarted @event)
        {
            var conversation = new DenormalizedConversation
            {
                Body = @event.Body,
                Subject = @event.Subject
            };

            using (var session = documentStore.OpenSession())
            {
                session.Store(conversation);

                session.SaveChanges();
            }
        }
    }
}