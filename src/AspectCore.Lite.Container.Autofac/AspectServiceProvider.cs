using Autofac;
using System;

namespace AspectCore.Lite.Container.Autofac
{
    internal class AspectServiceProvider : IServiceProvider
    {
        private readonly IContainer container;

        public AspectServiceProvider()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterAspectCoreLite();
            builder.RegisterInstance<IServiceProvider>(this);
            container = builder.Build();
        }

        public object GetService(Type serviceType)
        {
            return container.ResolveOptional(serviceType);
        }
    }
}
