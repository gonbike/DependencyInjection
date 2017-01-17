using AspectCore.Abstractions;
using Autofac;
using Autofac.Builder;
using System;
using System.Collections.Generic;

namespace AspectCore.Container.Autofac
{
    internal class AutofacOriginalServiceProvider : IOriginalServiceProvider
    {
        private readonly static IDictionary<Type, ConcreteReflectionActivatorData> ActivatorCache = new Dictionary<Type, ConcreteReflectionActivatorData>();

        private readonly IComponentContext componentContext;

        public AutofacOriginalServiceProvider(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var activatorData = default(ConcreteReflectionActivatorData);
            if (ActivatorCache.TryGetValue(serviceType, out activatorData))
            {
                return activatorData.Activator.ActivateInstance(componentContext, activatorData.ConfiguredParameters);
            }
            return componentContext.Resolve(serviceType);
        }

        internal static void MapActivatorData(Type serviceType, ConcreteReflectionActivatorData concreteReflectionActivatorData)
        {
            if (!ActivatorCache.ContainsKey(serviceType))
            {
                ActivatorCache.Add(serviceType, concreteReflectionActivatorData);
            }
        }
    }
}
