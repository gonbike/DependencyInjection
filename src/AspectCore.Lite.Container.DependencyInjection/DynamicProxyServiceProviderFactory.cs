using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public class DynamicProxyServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            var serviceProvider = services.TryAddAspectCoreLite().BuildServiceProvider();

            var aspectValidator = serviceProvider.GetRequiredService<IAspectValidator>();

            var dynamicProxyServices = new ServiceCollection();

            foreach (var descriptor in services)
            {
                if (!Validate(descriptor, aspectValidator))
                {
                    dynamicProxyServices.Add(descriptor);
                    continue;
                }
                var proxyType = descriptor.ServiceType.CreateProxyType(descriptor.ImplementationType, aspectValidator);
                dynamicProxyServices.Add(ServiceDescriptor.Describe(descriptor.ServiceType, proxyType, descriptor.Lifetime));
            }

            dynamicProxyServices.AddScoped<ISupportOriginalService>(p =>
            {
                var scopedProvider = serviceProvider.GetService<IServiceScopeFactory>().CreateScope().ServiceProvider;
                return new SupportOriginalService(scopedProvider);
            });

            return dynamicProxyServices;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return CreateBuilder(containerBuilder).BuildServiceProvider();
        }

        public IServiceProvider Create(IServiceCollection services)
        {
            return new DynamicProxyServiceProviderFactory().CreateServiceProvider(services);
        }

        private static bool Validate(ServiceDescriptor serviceDescriptor, IAspectValidator aspectValidator)
        {
            var implementationType = serviceDescriptor.ImplementationType;
            if (implementationType == null)
            {
                return false;
            }

            if (!serviceDescriptor.ServiceType.GetTypeInfo().ValidateAspect(aspectValidator))
            {
                return false;
            }

            if (!serviceDescriptor.ImplementationType.GetTypeInfo().CanInherited())
            {
                return false;
            }

            return true;
        }
    }
}
