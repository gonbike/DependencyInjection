using AspectCore.Lite.DynamicProxy;
using System;

namespace AspectCore.Lite.Container.Autofac
{
    internal class AutofacTargetServiceProvider : ITargetServiceProvider
    {
        private readonly object instance;

        public AutofacTargetServiceProvider(object instance)
        {
            this.instance = instance;
        }
        public void Dispose()
        {
        }

        public object GetTarget(Type serviceType)
        {
            return instance;
        }
    }
}
