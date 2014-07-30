using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ges.ReadModel
{
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Conversations;
    using Conversations.Configuration;
    using Conversations.Events;

    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    using MassTransit;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Raven.Client;
    using Raven.Client.Document;

    public class EventStorePosition
    {
        public string Id { get; set; }
        public long PreparePosition { get; set; }
        public long CommitPosition { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));

            connection.Connect();

            var container = new WindsorContainer();

            container.Register(Classes.FromAssemblyContaining<ConversationStarted>().BasedOn(typeof(IHandlesEvent<>)).WithServiceAllInterfaces());

            IDocumentStore rdb = new DocumentStore
            {
                Url = AppSettings.GetConfigurationString("SonatribeConnectionString"),
                DefaultDatabase = AppSettings.GetConfigurationString("SonatribeConnectionStringDBName")
            }.Initialize();

            rdb.Conventions.IdentityPartsSeparator = "-";

            container.Register(Component.For<IDocumentStore>().Instance(rdb));

            EventStorePosition position = null;

            using (var session = container.Resolve<IDocumentStore>().OpenSession())
            {
                position = session.Load<EventStorePosition>("EventStorePostitions-0");
            }

            if (position == null)
            {
                position = new EventStorePosition()
                {
                    Id = "EventStorePostitions-0",
                    CommitPosition = 0,
                    PreparePosition = 0
                };
            }

            connection.SubscribeToAllFrom(
                new Position(position.CommitPosition, position.PreparePosition),
                false,
                (subscription, @event) =>
                {
                    try
                    {
                        Console.WriteLine("recieved!!" + @event.OriginalEvent.EventId);
                        var processedEvent = ProcessRawEvent(@event);

                        if (processedEvent != null && processedEvent.Data != null)
                        {
                            var t = Type.GetType(processedEvent.EventClrTypeName);
                            var type = typeof(IHandlesEvent<>).MakeGenericType(t);
                            var allHandlers = container.ResolveAll(type);

                            foreach (var allHandler in allHandlers)
                            {
                                var method = allHandler.GetType().GetMethod("Consume", new[] { t });
                                Console.WriteLine(JsonConvert.DeserializeObject(processedEvent.Data.ToString()));
                                method.Invoke(allHandler, new[] { JsonConvert.DeserializeObject(processedEvent.Data.ToString(), t) });
                            }

                            using (var session = container.Resolve<IDocumentStore>().OpenSession())
                            {
                                session.Store(new EventStorePosition
                                {
                                    Id = "EventStorePostitions-0",
                                    CommitPosition = @event.OriginalPosition.Value.CommitPosition,
                                    PreparePosition = @event.OriginalPosition.Value.PreparePosition
                                });
                                session.SaveChanges();
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                },
                (subscription) =>
                {
                    Console.WriteLine("Live!!!");
                },
                (subscription, reason, arg3) =>
                {
                    Console.WriteLine("error!!!");
                    Console.WriteLine(reason);
                    Console.WriteLine(subscription.StreamId);
                }, new UserCredentials("admin", "changeit"));

            Console.ReadLine();
        }

        static EventMessage<object> ProcessRawEvent(ResolvedEvent rawEvent)
        {
            if (rawEvent.OriginalEvent.Metadata.Length > 0 &&
                rawEvent.OriginalEvent.Data.Length > 0 &&
                !rawEvent.OriginalEvent.EventType.StartsWith("$"))
                return DeserializeEvent(rawEvent.OriginalEvent);
            return null;
        }

        static EventMessage<object> DeserializeEvent(RecordedEvent originalEvent)
        {
            try
            {
                var headers =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(originalEvent.Metadata), Constants.JsonSerializerSettings);
                object data = null;
                string eventClrTypeName = string.Empty;

                try
                {
                    data = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(originalEvent.Data), Constants.JsonSerializerSettings);
                    eventClrTypeName = headers["EventClrTypeName"].ToString();
                }
                catch (Exception)
                {
                }

                var e = new EventMessage<object>
                {
                    EventId = originalEvent.EventId,
                    StreamName = originalEvent.EventStreamId,
                    EventNumber = originalEvent.EventNumber,
                    EventType = originalEvent.EventType,
                    EventClrTypeName = eventClrTypeName,
                    MetaData = headers,
                    Data = data,
                };

                return e;
            }
            catch (Exception exc)
            {
                Console.WriteLine("static EventMessage<object> DeserializeEvent(RecordedEvent originalEvent)");
                Console.WriteLine(exc.Message);
            }

            return null;
        }
    }
}
