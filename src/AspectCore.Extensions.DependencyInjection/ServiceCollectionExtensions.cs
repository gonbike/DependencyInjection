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
            return AddAspectCore(services, null);
        }

        public static IServiceCollection AddAspectCore(this IServiceCollection services, Action<AspectCoreOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddAspectBuilder();
            services.AddAspectConfigure(options);
            services.AddAspectValidator();
            services.AddInterceptorActivator();
            services.AddInterceptorInjector();
            services.AddInterceptorProvider();
            return services;
        }

        internal static IServiceCollection AddAspectBuilder(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IAspectActivator, AspectActivator>();

            services.AddScoped<IAspectBuilderProvider, AspectBuilderProvider>();

            services.AddScoped<IServiceInstanceProvider, ServiceInstanceProvider>();

            services.AddScoped<IProxyGenerator, ProxyGenerator>();

            services.AddScoped<IRealServiceProvider>(p => new RealServiceProvider(p));

            return services;
        }

        internal static IServiceCollection AddAspectConfigure(this IServiceCollection services, Action<AspectCoreOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var aspectCoreOptions = new AspectCoreOptions();

            options?.Invoke(aspectCoreOptions);

            services.AddSingleton<IAspectConfigureProvider>(new AspectConfigureProvider(aspectCoreOptions));

            return services;
        }

        internal static IServiceCollection AddAspectValidator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IAspectValidatorBuilder, AspectValidatorBuilder>();
            services.AddTransient<IAspectValidationHandler, AccessibleAspectValidationHandler>();
            services.AddTransient<IAspectValidationHandler, AttributeAspectValidationHandler>();
            services.AddTransient<IAspectValidationHandler, CacheAspectValidationHandler>();
            services.AddTransient<IAspectValidationHandler, ConfigureAspectValidationHandler>();
            services.AddTransient<IAspectValidationHandler, DynamicallyAspectValidationHandler>();
            services.AddTransient<IAspectValidationHandler, NonAspectValidationHandler>();

            return services;
        }

        internal static IServiceCollection AddInterceptorProvider(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IInterceptorProvider, InterceptorProvider>();
            services.AddScoped<IInterceptorSelector, ConfigureInterceptorSelector>();
            services.AddScoped<IInterceptorSelector, TypeInterceptorSelector>();
            services.AddScoped<IInterceptorSelector, MethodInterceptorSelector>();

            return services;
        }

        internal static IServiceCollection AddInterceptorActivator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<ITypedInterceptorActivator, ReflectionTypedInterceptorActivator>();
            services.AddScoped<ITypedInterceptorActivator, ActivatorUtilitieInterceptorActivator>();

            return services;
        }

        internal static IServiceCollection AddInterceptorInjector(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IInterceptorInjectorProvider, InterceptorInjectorProvider>();
            services.AddScoped<IPropertyInjectorSelector, PropertyInjectorSelector>();

            return services;
        }

        public static IServiceProvider BuildAspectCoreServiceProvider(this IServiceCollection services)
        {
            return new AspectCoreServiceProviderFactory().CreateServiceProvider(services);
        }
    }
}
