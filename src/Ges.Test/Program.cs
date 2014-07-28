using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ges.Test
{
    using System.Collections;
    using System.Net;
    using System.Security.AccessControl;

    using CommonDomain;
    using CommonDomain.Core;

    using EventStore.ClientAPI;

    class Program
    {
        private static IEventStoreConnection connection;

        static void Main(string[] args)
        {
            var input = string.Empty;

            connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.Connect();

            while (input != "q")
            {
                input = Console.ReadLine();

                for (int i = 0; i < 100; i++)
                {
                    var retrieved = SaveTestAggregate();

                    Console.WriteLine(retrieved.Id);
                }
            }
        }

        private static TestAggregate SaveTestAggregate()
        {
           
            var repo = new GetEventStoreRepository.GetEventStoreRepository(connection);

            var id = Guid.NewGuid();
            var testAggregate = new TestAggregate(id);
            testAggregate.ProduceEvents(10);
            testAggregate.ProduceWikiWahEvents(20);

            repo.Save(testAggregate, Guid.NewGuid(), d => { });

            var retrieved = repo.GetById<TestAggregate>(id);
            return retrieved;
        }
    }

    public class TestAggregate : AggregateBase
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

        public TestAggregate(Guid aggregateId)
            : this()
        {
            RaiseEvent(new TestAggregateCreated(aggregateId));
        }

        private TestAggregate()
        {
            Register<TestAggregateCreated>(e => Id = e.AggregateId);
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

    public class TestAggregateCreated
    {
        public TestAggregateCreated(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; private set; }
    }
}
