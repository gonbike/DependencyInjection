using Autofac;
using System;

namespace AspectCore.Container.Autofac
{
    internal class AutofacServiceProvider : IServiceProvider
    {
        private readonly IComponentContext componentContext;

        public AutofacServiceProvider(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public object GetService(Type serviceType)
        {
            return this.componentContext.Resolve(serviceType);
        }
    }
}
