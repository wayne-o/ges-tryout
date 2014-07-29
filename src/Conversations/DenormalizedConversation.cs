namespace Conversations
{
    using System;

    using MassTransit;

    using RabbitMQ.Client.Impl;

    public class DenormalizedConversation
    {
        public string Body { get; set; }

        public string Subject { get; set; }
    }

    public interface IBus
    {
        //void RegisterHandler<T>(Action<T> handler) where T : class, IDomainEvent;

        void Send<T>(T command) where T : class, ICommand;
    }

    public interface ICommand
    {
        /// <summary>
        /// Gets or sets AggregateId.
        /// </summary>
        Guid AggregateId { get; set; }

        /// <summary>
        /// Gets or sets Version.
        /// </summary>
        uint Version { get; set; }
    }

    public class MassTransitPublisher : IBus
        //,IDispatchCommits
    {
        private readonly IServiceBus _Bus;

        public MassTransitPublisher(IServiceBus bus)
        {
            _Bus = bus;
        }

        //void IBus.RegisterHandler<T>(Action<T> handler) where T : class
        //{
        //    _Bus.SubscribeHandler(handler);
        //}

        void IBus.Send<T>(T command)
        {
            _Bus.Publish(command);

        }

        //void IDispatchCommits.Dispatch(Commit commit)
        //{
        //    commit.Events.ForEach(@event => this.FastInvoke("PublishEvent", @event.Body));
        //}

        public void Dispose()
        {
            _Bus.Dispose();
        }

        private void PublishEvent<T>(T message) where T : class
        {
            _Bus.Publish(message);
        }
    }
}