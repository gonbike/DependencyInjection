using System;
using AspectCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AspectCore.Extensions.DependencyInjection
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
