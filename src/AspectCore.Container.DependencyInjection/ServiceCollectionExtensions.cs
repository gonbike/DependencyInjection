using AspectCore.Abstractions;
using AspectCore.Abstractions.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace AspectCore.Container.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection TryAddAspectCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.TryAddTransient<IAspectActivator, AspectActivator>();
            services.TryAddTransient<IAspectBuilder, AspectBuilder>();
            services.TryAddScoped<IInterceptorInjector, InterceptorInjector>();
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.TryAddSingleton<IAspectValidator, AspectValidator>();
            services.TryAddSingleton<IInterceptorMatcher, InterceptorMatcher>();
            services.TryAddSingleton<IAspectConfiguration, AspectConfiguration>();
            return services;
        }

        public static IServiceCollection AddAspectConfiguration(this IServiceCollection services, Action<IAspectConfiguration> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var aspectConfiguration = new AspectConfiguration();
            configure.Invoke(aspectConfiguration);
            services.AddSingleton<IAspectConfiguration>(aspectConfiguration);
            return services;
        }
    }
}
