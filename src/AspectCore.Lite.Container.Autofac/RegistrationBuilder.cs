using Autofac.Builder;
using Autofac.Core;
using System;

namespace AspectCore.Lite.Container.Autofac
{
    public static class RegistrationBuilder
    {
        public static IRegistrationBuilder<TImplementer, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> ForDynamicProxy<TService, TImplementer>()
        {
            return new RegistrationBuilder<TImplementer, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle>(
                new TypedService(typeof(TImplementer)),
                new ServiceConcreteReflectionActivatorData(typeof(TService), typeof(TImplementer)),
                new SingleRegistrationStyle());
        }

        public static IRegistrationBuilder<object, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle> ForDynamicProxy(Type serviceType, Type implementationType)
        {
            return new RegistrationBuilder<object, ServiceConcreteReflectionActivatorData, SingleRegistrationStyle>(
                new TypedService(implementationType),
                new ServiceConcreteReflectionActivatorData(serviceType, implementationType),
                new SingleRegistrationStyle());
        }
    }
}
