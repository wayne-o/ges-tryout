namespace Conversations.DomainService
{
    using System.ComponentModel;
    using System.Net;

    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Conversations.Commands;
    using Conversations.Installers;

    using EventStore.ClientAPI;

    using MassTransit;

    using Component = Castle.MicroKernel.Registration.Component;

    public class CommandHandlerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.Connect();
            container.Register(Component.For<IEventStoreConnection>().Instance(connection).LifestyleSingleton());

            container.Register(Classes.FromAssemblyContaining(typeof(CreateNewConversationCommandHandler)).Where(x => x.GetInterface(typeof(Consumes<>.Context).Name) != null));
            container.Install(new IWindsorInstaller[] { new ServicesInstaller(Keys.DomainServiceEndpoint), new DomainServiceInstaller(),  });

            
        }
    }
}