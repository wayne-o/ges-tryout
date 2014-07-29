namespace Conversations.Installers
{
    using System.Linq;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Conversations.Configuration;

    using MassTransit;
    using MassTransit.NLogIntegration;

    using Raven.Client;
    using Raven.Client.Document;

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
                                                    })).LifeStyle.Singleton);
            }
        }
    }
}