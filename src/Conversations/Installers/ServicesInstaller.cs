namespace Conversations.Installers
{
    using System;
    using System.Linq;

    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Conversations.Configuration;

    using MassTransit;
    using MassTransit.NLogIntegration;

    using Raven.Client;
    using Raven.Client.Document;

    public class DomainServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container,
                            IConfigurationStore store)
        {
            container.Register(Component.For<IEndpoint>().UsingFactoryMethod(GetEndpoint).LifeStyle.Transient);
        }

        private static IEndpoint GetEndpoint(IKernel kernel)
        {
            var bus = kernel.Resolve<IServiceBus>(Keys.DomainBusName);
            var domainService = bus.GetEndpoint(new Uri(Keys.DomainServiceEndpoint));
            return domainService;
        }
    }

    public class ServicesInstaller : IWindsorInstaller
    {
        private readonly string endpoint;

        public ServicesInstaller(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            IDocumentStore rdb = new DocumentStore
            {
                Url = AppSettings.GetConfigurationString("SonatribeConnectionString"),
                DefaultDatabase = AppSettings.GetConfigurationString("SonatribeConnectionStringDBName")
            }.Initialize();

            rdb.Conventions.IdentityPartsSeparator = "-";

            container.Register(Component.For<IDocumentStore>().Instance(rdb));

            if (!container.ResolveAll<IServiceBus>().Any())
            {
                container.Register(
                        Component
                        .For<IServiceBus>()
                        .UsingFactoryMethod(() =>
                            ServiceBusFactory.New(sbc =>
                                                    {
                                                        sbc.ReceiveFrom(endpoint);
                                                        sbc.UseRabbitMq();
                                                        sbc.UseNLog();
                                                        sbc.EnableMessageTracing();
                                                        sbc.SetPurgeOnStartup(true);
                                                        sbc.Subscribe(c => c.LoadFrom(container));
                                                    })).LifeStyle.Singleton,
                Component.For<IBus>()
					.UsingFactoryMethod((k, c) => 
						new MassTransitPublisher(k.Resolve<IServiceBus>()))
					.LifeStyle.Singleton);
            }
        }
    }
}