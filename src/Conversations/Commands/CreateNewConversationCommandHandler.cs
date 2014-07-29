using System;

namespace Conversations.Commands
{
    using System.Net;

    using Conversations.Domain;

    using EventStore.ClientAPI;

    using MassTransit;

    public class CreateNewConversationCommandHandler : Consumes<CreateNewConversation>.Context
    {
        private readonly IEventStoreConnection esConnection;

        public CreateNewConversationCommandHandler(IEventStoreConnection esConnection)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.Connect();
            this.esConnection = connection;
        }

        public void Consume(IConsumeContext<CreateNewConversation> message)
        {
            var repo = new GetEventStoreRepository.GetEventStoreRepository(esConnection);

            var id = Guid.NewGuid();
            var testAggregate = new Conversation(id);
            testAggregate.ProduceEvents(10);
            testAggregate.ProduceWikiWahEvents(20);

            repo.Save(testAggregate, Guid.NewGuid(), d => { });
        }
    }
}