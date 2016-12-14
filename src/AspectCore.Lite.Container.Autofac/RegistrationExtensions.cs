using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using AutofacRegistrationBuilder = Autofac.Builder.RegistrationBuilder;

namespace AspectCore.Lite.Container.Autofac
{
    public static class RegistrationExtensions
    {
        private static readonly Lazy<IAspectConfigurator> AspectConfiguration = new Lazy<IAspectConfigurator>(() => new AspectConfigurator(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static void ConfiguringAspect(this ContainerBuilder builder, Action<IAspectConfigurator> configure = null)
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

            configure?.Invoke(AspectConfiguration.Value);
            builder.Register<IAspectConfigurator>(ctx => AspectConfiguration.Value).SingleInstance();
        }

        public static IRegistrationBuilder<TImplementer, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterDynamicProxy<TImplementer>(this ContainerBuilder builder)
        {
            return RegisterDynamicProxy<TImplementer, TImplementer>(builder);
        }

        public static IRegistrationBuilder<TImplementer, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterDynamicProxy<TService, TImplementer>(this ContainerBuilder builder) where TImplementer : TService
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var registration = RegistrationBuilder.ForDynamicProxy<TService, TImplementer>();

            builder.RegisterCallback(cr => AutofacRegistrationBuilder.RegisterSingleComponent(cr, registration));

            return registration.RegisterRuntimeProxy();
        }

        public static IRegistrationBuilder<object, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterDynamicProxy(this ContainerBuilder builder, Type serviceType)
        {
            return RegisterDynamicProxy(builder, serviceType, serviceType);
        }

        public static IRegistrationBuilder<object, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterDynamicProxy(this ContainerBuilder builder, Type serviceType, Type implementationType)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            var registration = RegistrationBuilder.ForDynamicProxy(serviceType, implementationType);

            builder.RegisterCallback(cr => AutofacRegistrationBuilder.RegisterSingleComponent(cr, registration));

            return registration.RegisterRuntimeProxy();
        }

        private static IRegistrationBuilder<TLimit, ServiceConcreteReflectionActivatorData, TRegistrationStyle> RegisterRuntimeProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ServiceConcreteReflectionActivatorData, TRegistrationStyle> registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            var aspectValidator = new AspectValidator(AspectConfiguration.Value);

            Validate(registration.ActivatorData.ServiceType, registration.ActivatorData.ImplementationType, aspectValidator);

            var activator = registration.ActivatorData.Activator;
            registration.ActivatorData.ImplementationType = registration.ActivatorData.ServiceType.CreateProxyType(registration.ActivatorData.ImplementationType, aspectValidator);

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

            return registration;
        }

        private static void Validate(Type serviceType, Type implementationType, IAspectValidator aspectValidator)
        {

            if (!serviceType.GetTypeInfo().ValidateAspect(aspectValidator))
            {
                throw new InvalidOperationException($"Register dynamicProxy failed.Service {serviceType.FullName} cannot be validated.");
            }

            if (!implementationType.GetTypeInfo().CanInherited())
            {
                throw new InvalidOperationException($"Register dynamicProxy failed.Type {implementationType.FullName} cannot be inherited.");
            }
        }
    }
}
