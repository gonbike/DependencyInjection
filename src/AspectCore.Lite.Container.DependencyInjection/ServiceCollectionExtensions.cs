using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IAspectConfiguratorFactory<IServiceCollection> aspectConfiguratorFactory = new AspectConfiguratorFactory();

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
            services.TryAddSingleton<IAspectConfigurator>(aspectConfiguratorFactory.CreateConfigurator(services));
            return services;
        }

        public static IServiceCollection AddAspectConfiguration(this IServiceCollection services, Action<IAspectConfigurator> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var aspectConfigurator = aspectConfiguratorFactory.CreateConfigurator(services);
            configure.Invoke(aspectConfigurator);
            services.TryAddSingleton<IAspectConfigurator>(aspectConfigurator);
            return services;
        }
    }
}
