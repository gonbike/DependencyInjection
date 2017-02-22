using System;
using AspectCore.Abstractions;
using AspectCore.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspectCore.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection TryAddAspectCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddScoped<IAspectActivator, AspectActivator>();

            services.TryAddScoped<IAspectBuilderProvider, AspectBuilderProvider>();

            services.TryAddScoped<IInterceptorSelector, InterceptorSelector>();

            services.TryAddScoped<IInterceptorInjectorProvider, InterceptorInjectorProvider>();

            services.TryAddScoped<IServiceInstanceProvider, ServiceInstanceProvider>();

            services.TryAddSingleton<IPropertyInjectorSelector, PropertyInjectorSelector>();

            services.TryAddSingleton<IInterceptorMatcher, InterceptorMatcher>();

            services.TryAddSingleton<IAspectConfigure, AspectConfigure>();

            services.TryAddSingleton<IAspectValidator, AspectValidator>();

            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();  
               
            return services;
        }

        public static IServiceCollection ConfigureAspectCore(this IServiceCollection services, Action<IAspectConfigure> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var aspectConfigure = new AspectConfigure();

            configure.Invoke(aspectConfigure);

            services.AddSingleton<IAspectConfigure>(aspectConfigure);

            return services;
        }

        public static IServiceProvider BuildAspectCoreServiceProvider(this IServiceCollection services)
        {
            return new AspectCoreServiceProviderFactory().CreateServiceProvider(services);
        }
    }
}
