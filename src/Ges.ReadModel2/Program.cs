using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ges.ReadModel2
{
    using System.Net;

    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    class Program
    {
        static void Main(string[] args)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));

            connection.Connect();

            connection.SubscribeToStream("$et-ConversationStarted", false,
                (subscription, @event) => Console.WriteLine("recieved!!" + @event.OriginalEvent.EventId),
                (subscription, reason, arg3) => { Console.WriteLine("error!!!"); }, new UserCredentials("admin", "changeit"));

            Console.ReadLine();
        }
    }
}
