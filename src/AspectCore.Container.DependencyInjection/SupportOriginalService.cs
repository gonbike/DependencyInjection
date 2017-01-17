using AspectCore.Abstractions;
using System;

namespace AspectCore.Container.DependencyInjection
{
    internal sealed class SupportOriginalService : IOriginalServiceProvider
    {
        private readonly IServiceProvider serviceProvider;

        public SupportOriginalService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }
    }
}
