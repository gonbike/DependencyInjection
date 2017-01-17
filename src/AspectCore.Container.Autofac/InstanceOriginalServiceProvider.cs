using AspectCore.Abstractions;
using System;

namespace AspectCore.Container.Autofac
{
    internal class InstanceOriginalServiceProvider : IOriginalServiceProvider
    {
        private readonly object instance;

        public InstanceOriginalServiceProvider(object instance)
        {
            this.instance = instance;
        }

        public object GetService(Type serviceType)
        {
            return instance;
        }
    }
}
