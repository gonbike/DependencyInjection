using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Lazy<IAspectConfigurator> AspectConfiguration = new Lazy<IAspectConfigurator>(() => new AspectConfigurator(), LazyThreadSafetyMode.ExecutionAndPublication);
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
            services.TryAddSingleton<IAspectConfigurator>(p => AspectConfiguration.Value);

            return services;
        }

        public static IServiceCollection AddInterceptorConfiguration(this IServiceCollection services, Action<IAspectConfigurator> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure.Invoke(AspectConfiguration.Value);
            services.TryAddSingleton<IAspectConfigurator>(p => AspectConfiguration.Value);
            return services;
        }
    }
}
