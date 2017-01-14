using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using System;
using System.Linq;

namespace AspectCore.Lite.Container.Autofac
{
    public static class RegistrationExtensions
    {
        public static ContainerBuilder RegisterAspectCore(this ContainerBuilder builder)
        {
            return RegisterAspectCore(builder, null);
        }

        public static ContainerBuilder RegisterAspectCore(this ContainerBuilder builder, Action<IAspectConfiguration> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            builder.RegisterType<AspectActivator>().As<IAspectActivator>().InstancePerDependency();
            builder.RegisterType<AspectBuilder>().As<IAspectBuilder>().InstancePerDependency();
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>().InstancePerDependency();

            builder.RegisterType<InterceptorInjector>().As<IInterceptorInjector>().InstancePerLifetimeScope();
            builder.RegisterType<AspectValidator>().As<IAspectValidator>().SingleInstance();
            builder.RegisterType<InterceptorMatcher>().As<IInterceptorMatcher>().SingleInstance();
            builder.RegisterType<ProxyGenerator>().As<IProxyGenerator>().SingleInstance();

            var aspectConfiguration = new AspectConfiguration();
            configure?.Invoke(aspectConfiguration);
            builder.RegisterInstance<IAspectConfiguration>(aspectConfiguration).SingleInstance();

            return builder;
        }

        public static void AsProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            AsServiceProxy(registration, new TypedService(serviceType));
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

        public static void AsServiceProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> registration, Service service)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            registration.As(service);

            var activatorData = registration.ActivatorData;

            registration.OnActivating(args =>
            {
                var serviceType = service.GetServiceType();

                var parameters = args.Parameters.ToList();

                parameters.Add(new ResolvedParameter((p, ctx) => p.ParameterType == typeof(IServiceProvider), (p, ctx) =>
                {
                    return ctx.Resolve(p.ParameterType);
                }));

                parameters.Add(new PositionalParameter(parameters.Count, new SupportOriginalService(args.Instance)));

                var proxyGenerator = args.Context.Resolve<IProxyGenerator>();

                var proxyType = proxyGenerator.CreateType(serviceType, activatorData.ImplementationType);

                var proxyActivator = new ReflectionActivator(proxyType, activatorData.ConstructorFinder,
                    activatorData.ConstructorSelector, parameters, activatorData.ConfiguredProperties);

                args.ReplaceInstance(proxyActivator.ActivateInstance(args.Context, parameters));
            });
        }
    }
}
