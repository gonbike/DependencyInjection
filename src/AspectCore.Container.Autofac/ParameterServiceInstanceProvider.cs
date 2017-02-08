using System;
using AspectCore.Abstractions;

namespace AspectCore.Container.Autofac
{
    internal class ParameterServiceInstanceProvider : IServiceInstanceProvider
    {
        private readonly object instance;

        public ParameterServiceInstanceProvider(object instance)
        {
            this.instance = instance;
        }

        public object GetInstance(Type serviceType)
        {
            return instance;
        }
    }
}
