using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using System;
using System.Linq;
using System.Reflection;

namespace AspectCore.Lite.Container.Autofac
{
    public static class RegistrationExtensions
    {
        public static IAspectConfigurator RegisterAspectConfiguration(this ContainerBuilder builder, Action<IAspectConfigurator> configure = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.RegisterType<AspectActivator>().As<IAspectActivator>().InstancePerDependency();
            builder.RegisterType<AspectBuilder>().As<IAspectBuilder>().InstancePerDependency();
            builder.RegisterType<DynamicProxyServiceProvider>().As<IServiceProvider>().InstancePerDependency();

            builder.RegisterType<InterceptorInjector>().As<IInterceptorInjector>().InstancePerLifetimeScope();
            builder.RegisterType<AspectValidator>().As<IAspectValidator>().SingleInstance();
            builder.RegisterType<InterceptorMatcher>().As<IInterceptorMatcher>().SingleInstance();

            var aspectConfigurator = new AspectConfigurator();
            configure?.Invoke(aspectConfigurator);
            builder.RegisterInstance<IAspectConfigurator>(aspectConfigurator).SingleInstance();

            return aspectConfigurator;
        }

        public static void AsProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            AsServiceProxy(registration, new TypedService(serviceType));
        }

        public static void AsProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, Type serviceType, IAspectConfigurator configurator)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            AsServiceProxy(registration, new TypedService(serviceType), configurator);
        }

        public static void AsNamedProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, string serviceName, Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }
            AsServiceProxy(registration, new KeyedService(serviceName, serviceType));
        }

        public static void AsNamedProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, string serviceName, Type serviceType, IAspectConfigurator configurator)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }
            AsServiceProxy(registration, new KeyedService(serviceName, serviceType), configurator);
        }

        public static void AsKeyedProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, object serviceKey, Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (serviceKey == null)
            {
                throw new ArgumentNullException(nameof(serviceKey));
            }
            AsServiceProxy(registration, new KeyedService(serviceKey, serviceType));
        }

        public static void AsKeyedProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, object serviceKey, Type serviceType, IAspectConfigurator configurator)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (serviceKey == null)
            {
                throw new ArgumentNullException(nameof(serviceKey));
            }
            AsServiceProxy(registration, new KeyedService(serviceKey, serviceType), configurator);
        }

        public static void AsServiceProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, Service service)
        {
            AsServiceProxy(registration, service, null);
        }

        public static void AsServiceProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, Service service, IAspectConfigurator configurator)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var serviceWithType = service as IServiceWithType;
            var aspectValidator = new AspectValidator(configurator ?? new AspectConfigurator());
            Validate(serviceWithType.ServiceType, registration.ActivatorData.ImplementationType, aspectValidator);

            registration.As(service);
            var activator = registration.ActivatorData.Activator;
            registration.ActivatorData.ImplementationType = serviceWithType.ServiceType.CreateProxyType(registration.ActivatorData.ImplementationType, aspectValidator);

            registration.OnPreparing(args =>
            {
                var parameters = args.Parameters.ToList();
                var supportOriginalService = new SupportOriginalService(activator.ActivateInstance(args.Context, parameters));
                parameters.Add(new ResolvedParameter((p, ctx) => p.ParameterType == typeof(IServiceProvider), (p, ctx) =>
                {
                    return ctx.Resolve(p.ParameterType);
                }));
                parameters.Add(new PositionalParameter(parameters.Count, supportOriginalService));
                args.Parameters = parameters;
            });
        }

        private static void Validate(Type serviceType, Type implementationType, IAspectValidator aspectValidator)
        {
            if (!implementationType.GetTypeInfo().CanInherited())
            {
                throw new InvalidOperationException($"Register dynamicProxy failed.Type {implementationType.FullName} cannot be inherited.");
            }
        }
    }
}
