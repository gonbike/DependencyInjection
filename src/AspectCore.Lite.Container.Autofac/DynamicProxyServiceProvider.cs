using Autofac;
using System;

namespace AspectCore.Lite.Container.Autofac
{
    internal class DynamicProxyServiceProvider : IServiceProvider
    {
        private readonly IComponentContext componentContext;

        public DynamicProxyServiceProvider(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public object GetService(Type serviceType)
        {
            return this.componentContext.Resolve(serviceType);
        }
    }
}
