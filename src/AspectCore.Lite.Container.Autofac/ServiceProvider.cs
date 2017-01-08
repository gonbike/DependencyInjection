using Autofac;
using System;

namespace AspectCore.Lite.Container.Autofac
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly IComponentContext componentContext;

        public ServiceProvider(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public object GetService(Type serviceType)
        {
            return this.componentContext.Resolve(serviceType);
        }
    }
}
