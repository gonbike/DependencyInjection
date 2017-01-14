using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Extensions;
using AspectCore.Lite.Abstractions.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public class AspectCoreServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.TryAddAspectCore().BuildServiceProvider();

            var aspectValidator = serviceProvider.GetRequiredService<IAspectValidator>();

            var dynamicProxyServices = new ServiceCollection();

            var generator = serviceProvider.GetService<IProxyGenerator>();

            foreach (var descriptor in serviceCollection)
            {
                if (!Validate(descriptor, aspectValidator))
                {
                    dynamicProxyServices.Add(descriptor);
                    continue;
                }
                var proxyType = generator.CreateType(descriptor.ServiceType, descriptor.ImplementationType);
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

        private static bool Validate(ServiceDescriptor descriptor, IAspectValidator aspectValidator)
        {
            var implementationType = descriptor.ImplementationType;
            if (implementationType == null)
            {
                return false;
            }

            if (!aspectValidator.Validate(descriptor.ServiceType))
            {
                return false;
            }

            if (implementationType.GetTypeInfo().IsDynamically())
            {
                return false;
            }

            if (!implementationType.GetTypeInfo().CanInherited())
            {
                return false;
            }

            return true;
        }
    }
}
