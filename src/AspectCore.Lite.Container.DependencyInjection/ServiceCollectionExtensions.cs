using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection TryAddAspectCoreLite(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddTransient<IAspectActivator, AspectActivator>();
            services.TryAddTransient<IAspectBuilder, AspectBuilder>();
            services.TryAddScoped<IInterceptorInjector, InterceptorInjector>();
            services.TryAddSingleton<IAspectValidator, AspectValidator>();
            services.TryAddSingleton<IInterceptorMatcher, InterceptorMatcher>();
            services.TryAddSingleton<IInterceptorConfiguration, InterceptorConfiguration>();

            return services;
        }

        public static IServiceCollection AddInterceptorConfiguration(this IServiceCollection services, Action<IInterceptorConfiguration> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var configuration = new InterceptorConfiguration();
            configure.Invoke(configuration);

            return services.AddSingleton(configuration);
        }   
    }
}
