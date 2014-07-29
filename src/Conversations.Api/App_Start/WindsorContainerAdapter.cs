namespace Conversations.Api.App_Start
{
    using System;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    using Conversations.Installers;

    using ServiceStack.Configuration;

    public class WindsorContainerAdapter : IContainerAdapter, IDisposable
    {
        private readonly IWindsorContainer _container;

        public WindsorContainerAdapter()
        {
            _container = new WindsorContainer();
            //.Install(FromAssembly.InThisApplication(),
            //    FromAssembly.InDirectory(new ApplicationAssemblyFilter()));

            _container.Install(new IWindsorInstaller[] { new ServicesInstaller(Keys.WebEndpoint) });
        }

        public T TryResolve<T>()
        {
            if (this._container.Kernel.HasComponent(typeof(T)))
            {
                return (T)this._container.Resolve(typeof(T));
            }

            return default(T);
        }

        public T Resolve<T>()
        {
            return this._container.Resolve<T>();
        }

        public void Dispose()
        {
            this._container.Dispose();
        }
    }
}