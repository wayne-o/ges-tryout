﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ges.ReadModel
{
    using System.Net;

    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    class Program
    {
        static void Main(string[] args)
        {

            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));

            connection.Connect();

            connection.SubscribeToStream("$et-WoftamEvent", false,
                (subscription, @event) => Console.WriteLine("recieved!!" + @event.OriginalEvent.EventId),
                (subscription, reason, arg3) => { }, new UserCredentials("admin", "changeit"));

            Console.ReadLine();
        }
    }
}