using System;
using System.Reflection;
using AspectCore.Abstractions;
using AspectCore.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspectCore.Extensions.DependencyInjection
{
    public class AspectCoreServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var aspectValidator = serviceProvider.GetRequiredService<IAspectValidatorBuilder>().Build();

            var dynamicProxyServices = new ServiceCollection();

            var generator = serviceProvider.GetRequiredService<IProxyGenerator>();

            foreach (var descriptor in serviceCollection)
            {
                if (!Validate(descriptor, aspectValidator))
                {
                    dynamicProxyServices.Add(descriptor);
                    continue;
                }
                Type proxyType;
                if (descriptor.ServiceType.GetTypeInfo().IsInterface)
                {
                    proxyType = generator.CreateInterfaceProxyType(descriptor.ServiceType, descriptor.ImplementationType, descriptor.ServiceType.GetTypeInfo().GetInterfaces());
                }
                else
                {
                    proxyType = generator.CreateClassProxyType(descriptor.ServiceType, descriptor.ImplementationType, descriptor.ServiceType.GetTypeInfo().GetInterfaces());
                }
                dynamicProxyServices.Add(ServiceDescriptor.Describe(descriptor.ServiceType, proxyType, descriptor.Lifetime));
                ServiceInstanceProvider.MapServiceDescriptor(descriptor);
            }

            dynamicProxyServices.AddScoped<IRealServiceProvider>(p => new RealServiceProvider(serviceProvider.CreateScope().ServiceProvider));

            return dynamicProxyServices;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }
            return CreateBuilder(containerBuilder).BuildServiceProvider();
        }

        private static bool Validate(ServiceDescriptor descriptor, IAspectValidator aspectValidator)
        {
            var implementationType = descriptor.ImplementationType;
            if (implementationType == null)
            {
                return false;
            }

            if (!aspectValidator.Validate(descriptor.ServiceType.GetTypeInfo()))
            {
                return false;
            }

            if (implementationType.GetTypeInfo().IsDefined(typeof(DynamicallyAttribute)))
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
