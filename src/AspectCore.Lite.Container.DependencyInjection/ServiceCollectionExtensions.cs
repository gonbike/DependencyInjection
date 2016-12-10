using AspectCore.Lite.Abstractions;
using AspectCore.Lite.DynamicProxy;
using AspectCore.Lite.DynamicProxy.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider BuildAspectServiceProvider(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceProvider = TryAddAspectCoreLite(services).BuildServiceProvider();
            var aspectServices = new ServiceCollection();

            foreach (var descriptor in services)
            {
                if (!CanProxy(descriptor, serviceProvider))
                {
                    aspectServices.Add(descriptor);
                    continue;
                }
                var proxyType = ContainerHelper.CreateAspectType(descriptor.ServiceType, descriptor.ImplementationType, serviceProvider);
                aspectServices.Add(ServiceDescriptor.Describe(descriptor.ServiceType, proxyType, descriptor.Lifetime));
            }

            aspectServices.AddScoped<ITargetServiceProvider>(p =>
            {
                var scopedProvider = serviceProvider.GetService<IServiceScopeFactory>().CreateScope().ServiceProvider;
                return new TargetServiceProvider(scopedProvider);
            });

            return aspectServices.BuildServiceProvider();
        }

        public static IServiceCollection TryAddAspectCoreLite(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceDescriptions = ContainerHelper.GetAspectServiceDescriptions();
            foreach (var description in serviceDescriptions)
            {
                services.TryAdd(GetServiceDescriptor(description));
            }

            return services;
        }

        public static IServiceCollection AddInterceptor(this IServiceCollection services, Action<IInterceptorTable> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var interceptorTable = ContainerHelper.GetInterceptorTable();
            configure(interceptorTable);
            return services.AddSingleton(interceptorTable);
        }

        private static ServiceDescriptor GetServiceDescriptor(ServiceDescription description)
        {
            switch (description.Lifetime)
            {
                case Lifetime.Scoped:
                    return ServiceDescriptor.Scoped(description.ServiceType, description.ImplementationType);
                case Lifetime.Singleton:
                    return ServiceDescriptor.Singleton(description.ServiceType, description.ImplementationType);
                default:
                    return ServiceDescriptor.Transient(description.ServiceType, description.ImplementationType);
            }
        }

        private static bool CanProxy(ServiceDescriptor serviceDescriptor, IServiceProvider provider)
        {
            var implementationType = serviceDescriptor.ImplementationType;
            if (implementationType == null)
            {
                return false;
            }

            var aspectValidator = provider.GetRequiredService<IAspectValidator>();

            if (!CanProxy(serviceDescriptor.ServiceType.GetTypeInfo(), aspectValidator))
            {
                return false;
            }

            if (!CanInherited(serviceDescriptor.ImplementationType.GetTypeInfo()))
            {
                return false;
            }

            return true;
        }

        public static bool CanProxy(TypeInfo typeInfo, IAspectValidator aspectValidator)
        {
            if (typeInfo.IsValueType)
            {
                return false;
            }

            return typeInfo.DeclaredMethods.Any(method => aspectValidator.Validate(method));
        }

        private static bool CanInherited(TypeInfo typeInfo)
        {
            return typeInfo.IsClass && (typeInfo.IsPublic || (typeInfo.IsNested && typeInfo.IsNestedPublic)) &&
                   !typeInfo.IsSealed && !typeInfo.IsGenericTypeDefinition;
        }
    }
}
