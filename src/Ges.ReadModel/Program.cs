using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ges.ReadModel
{
    using System.Net;
    using System.Reflection;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Conversations;
    using Conversations.Configuration;
    using Conversations.Events;

    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Raven.Client;
    using Raven.Client.Document;

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

            connection.SubscribeToAll(
                false,
                (subscription, @event) =>
                    {
                        Console.WriteLine("recieved!!" + @event.OriginalEvent.EventId);
                        var processedEvent = ProcessRawEvent(@event);

                        var t = Type.GetType(processedEvent.EventClrTypeName);

                        var type = typeof(IHandlesEvent<>).MakeGenericType(t);

                        var allHandlers = container.ResolveAll(type);
                    },
                    (subscription, reason, arg3) => { Console.WriteLine("error!!!"); }, new UserCredentials("admin", "changeit"));

            //connection.SubscribeToStream("$et-ConversationStarted", false,
            //    (subscription, @event) => Console.WriteLine("recieved!!" + @event.OriginalEvent.EventId),
            //    (subscription, reason, arg3) => { Console.WriteLine("error!!!"); }, new UserCredentials("admin", "changeit"));

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
            var headers =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    Encoding.UTF8.GetString(originalEvent.Metadata), Constants.JsonSerializerSettings);
            var data = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(originalEvent.Data),
                                                     Constants.JsonSerializerSettings);
            var eventClrTypeName = headers["EventClrTypeName"].ToString();

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
    }
}
