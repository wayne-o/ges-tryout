namespace Conversations.Api.App_Start
{
    using System;
    using System.Reflection;

    using Castle.MicroKernel.Registration;

    public class ApplicationAssemblyFilter : AssemblyFilter
    {
        public ApplicationAssemblyFilter()
            : base(AppDomain.CurrentDomain.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".*.dll") { }
    }
}