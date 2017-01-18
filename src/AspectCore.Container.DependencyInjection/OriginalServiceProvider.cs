using AspectCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspectCore.Container.DependencyInjection
{
    internal sealed class OriginalServiceProvider : IOriginalServiceProvider
    {
        private readonly IServiceProvider serviceProvider;

        public OriginalServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetRequiredService(serviceType);
        }
    }
}
