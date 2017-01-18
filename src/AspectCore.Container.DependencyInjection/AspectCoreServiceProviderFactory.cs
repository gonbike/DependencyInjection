using AspectCore.Abstractions;
using AspectCore.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace AspectCore.Container.DependencyInjection
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
                ServiceTargetInstanceProvider.MapServiceDescriptor(descriptor);
            }

            dynamicProxyServices.AddScoped<IOriginalServiceProvider>(p =>
            {
                var scopedProvider = serviceProvider.GetService<IServiceScopeFactory>().CreateScope().ServiceProvider;
                return new OriginalServiceProvider(scopedProvider);
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
