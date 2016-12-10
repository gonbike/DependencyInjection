using AspectCore.Lite.Abstractions;
using AspectCore.Lite.DynamicProxy.Container;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutofacDef = Autofac.Builder;

namespace AspectCore.Lite.Container.Autofac
{
    public static class RegistrationExtensions
    {
        private static readonly AspectServiceProvider aspectServiceProvider = new AspectServiceProvider();
        internal static void RegisterAspectCoreLite(this ContainerBuilder builder, Action<IInterceptorTable> configure = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var serviceDescriptions = ContainerHelper.GetAspectServiceDescriptions();
            foreach (var description in serviceDescriptions)
                builder.RegisterType(description.ImplementationType).As(description.ServiceType).ConfigureLifecycle(description.Lifetime);
            if (configure != null)
            {
                var interceptorCollection = ContainerHelper.GetInterceptorTable();
                configure(interceptorCollection);
                builder.RegisterInstance(interceptorCollection).SingleInstance();
            }
        }

        public static void RegisterInterceptor(this ContainerBuilder builder, Action<IInterceptorTable> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var interceptorCollection = (IInterceptorTable)aspectServiceProvider.GetService(typeof(IInterceptorTable));
            configure(interceptorCollection);
        }

        public static IRegistrationBuilder<TImplementer, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterDynamicProxy<TImplementer>(this ContainerBuilder builder)
        {
            return RegisterDynamicProxy<TImplementer, TImplementer>(builder);
        }

        public static IRegistrationBuilder<TImplementer, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterDynamicProxy<TService, TImplementer>(this ContainerBuilder builder) where TImplementer : TService
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var registration = RegistrationBuilder.ForDynamicProxy<TService, TImplementer>();

            builder.RegisterCallback(cr => AutofacDef.RegistrationBuilder.RegisterSingleComponent(cr, registration));

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

            builder.RegisterCallback(cr => AutofacDef.RegistrationBuilder.RegisterSingleComponent(cr, registration));

            return registration.RegisterRuntimeProxy();
        }

        private static IRegistrationBuilder<TLimit, ServiceConcreteReflectionActivatorData, TRegistrationStyle> RegisterRuntimeProxy<TLimit, TRegistrationStyle>(this IRegistrationBuilder<TLimit, ServiceConcreteReflectionActivatorData, TRegistrationStyle> registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            var aspectValidator = (IAspectValidator)aspectServiceProvider.GetService(typeof(IAspectValidator));

            if(!CanProxy(registration.ActivatorData.ServiceType, registration.ActivatorData.ImplementationType, aspectValidator))
            {
                return registration; 
            }

            var activator = registration.ActivatorData.Activator;
            registration.ActivatorData.ImplementationType = ContainerHelper.CreateAspectType(registration.ActivatorData.ServiceType, registration.ActivatorData.ImplementationType, aspectServiceProvider);

            registration.OnPreparing(args =>
            {
                var parameters = args.Parameters.ToList();
                var targetProvider = new AutofacTargetServiceProvider(activator.ActivateInstance(args.Context, parameters));
                parameters.Add(new PositionalParameter(parameters.Count, aspectServiceProvider));
                parameters.Add(new PositionalParameter(parameters.Count, targetProvider));
                args.Parameters = parameters;
            });

            return registration;
        }

        private static bool CanProxy(Type serviceType, Type implementationType, IAspectValidator aspectValidator)
        {

            if (!CanProxy(serviceType.GetTypeInfo(), aspectValidator))
            {
                return false;
            }

            if (!CanInherited(implementationType.GetTypeInfo()))
            {
                return false;
            }

            return true;
        }

        private static bool CanProxy(TypeInfo typeInfo, IAspectValidator aspectValidator)
        {
            if (typeInfo.IsValueType)
            {
                return false;
            }

            return typeInfo.DeclaredMethods.Any(method => aspectValidator.Validate(method));
        }

        private static bool CanInherited(TypeInfo typeInfo)
        {
            return typeInfo.IsClass && (typeInfo.IsPublic || (typeInfo.IsNested && typeInfo.IsNestedPublic)) &&
                   !typeInfo.IsSealed && !typeInfo.IsGenericTypeDefinition;
        }

        private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
               this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
               Lifetime lifecycleKind)
        {
            switch (lifecycleKind)
            {
                case Lifetime.Singleton:
                    registrationBuilder.SingleInstance();
                    break;
                case Lifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                case Lifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
            }

            return registrationBuilder;
        }

    }
}
