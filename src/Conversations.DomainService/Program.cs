namespace Conversations.DomainService
{
    using System.Threading;

    using Castle.Windsor;

    using EventStore.ClientAPI;

    using MassTransit;

    using NLog;
    using NLog.Config;

    using Topshelf;

    internal class Program
    {
        private static IEventStoreConnection connection;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IWindsorContainer container;

        private IServiceBus bus;

        private static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Domain Service Main Thread";
            HostFactory.Run(x =>
            {
                x.Service<Program>(s =>
                {
                    s.ConstructUsing(name => new Program());
                    s.WhenStarted(p => p.Start());
                    s.WhenStopped(p => p.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Handles the domain logic for the Sonatribe Application.");
                x.SetDisplayName("Sonatribe Domain Service");
                x.SetServiceName("Sonatribe.Domain.Service");
            });
        }

        private void Stop()
        {
            _logger.Info("shutting down Domain Service");
            
            container.Dispose();
        }

        private void Start()
        {
            SimpleConfigurator.ConfigureForConsoleLogging();
            container = new WindsorContainer();
            container.Install(new CommandHandlerInstaller());

            bus = container.Resolve<IServiceBus>();
        }
    }
}
