namespace Conversations
{
    using System;
    using System.Collections.Generic;

    using MassTransit;

    using Newtonsoft.Json;

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

    public interface IHandlesEvent<T> : Consumes<T>.All
        where T : class, IDomainEvent
    {
    }

    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the aggregate root id of the domain event.
        /// </summary>
        Guid AggregateId { get;}

        /// <summary>
        /// Gets or sets the version of the aggregate which this event corresponds to.
        /// E.g. CreateNewCustomerCommand would map to (:NewCustomerCreated).Version = 1,
        /// as that event corresponds to the creation of the customer.
        /// </summary>
        uint Version { get; set; }
    }

    public class EventMessage<T>
    {
        public Guid EventId { get; set; }

        public string StreamName { get; set; }

        public int EventNumber { get; set; }

        public string EventType { get; set; }

        public Dictionary<string, object> MetaData { get; set; }

        public T Data { get; set; }

        public string EventClrTypeName { get; set; }
    }

    public class Constants
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

    }
}