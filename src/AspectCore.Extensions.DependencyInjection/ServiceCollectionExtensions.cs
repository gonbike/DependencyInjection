using System;
using AspectCore.Abstractions;
using AspectCore.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspectCore.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAspectCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddScoped<IAspectActivator, AspectActivator>();

            services.TryAddScoped<IAspectBuilderProvider, AspectBuilderProvider>();

            //services.TryAddScoped<IInterceptorSelector, ConfigureInterceptorSelector>();


            services.AddTransient<IServiceInstanceProvider, ServiceInstanceProvider>();

            //services.TryAddSingleton<IInterceptorMatcher, InterceptorMatcher>();

            //services.TryAddSingleton<IAspectConfigure, AspectConfigure>();

            //services.TryAddSingleton<IAspectValidator, AspectValidator>();

            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();

            return services;
        }

        internal static IServiceCollection AddInterceptorProvider(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IInterceptorProvider, InterceptorProvider>();
            services.AddTransient<IInterceptorSelector, ConfigureInterceptorSelector>();
            services.AddTransient<IInterceptorSelector, TypeInterceptorSelector>();
            services.AddTransient<IInterceptorSelector, MethodInterceptorSelector>();

            return services;
        }

        internal static IServiceCollection AddInterceptorActivator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<ITypedInterceptorActivator, ReflectionTypedInterceptorActivator>();
            services.AddTransient<ITypedInterceptorActivator, ActivatorUtilitieInterceptorActivator>();

            return services;
        }

        internal static IServiceCollection AddInterceptorInjector(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IInterceptorInjectorProvider, InterceptorInjectorProvider>();
            services.AddTransient<IPropertyInjectorSelector, PropertyInjectorSelector>();

            return services;
        }

        internal static IServiceCollection ConfigureAspectCore(this IServiceCollection services, Action<IAspectConfigure> configure)
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
