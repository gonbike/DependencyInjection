using Autofac.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;

namespace AspectCore.Lite.Container.Autofac
{
    public class ServiceConcreteReflectionActivatorData : ConcreteReflectionActivatorData, IConcreteActivatorData
    {
        public Type ServiceType { get; }

        public ServiceConcreteReflectionActivatorData(Type serviceType, Type implementer) : base(implementer)
        {
            this.ServiceType = serviceType;
        }
    }
}
