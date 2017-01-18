using AspectCore.Abstractions.Resolution;
using System;

namespace AspectCore.Container.Autofac
{
    internal class ParameterTargetInstanceProvider : TargetInstanceProvider
    {
        private readonly object instance;

        public ParameterTargetInstanceProvider(object instance)
        {
            this.instance = instance;
        }

        public override object GetInstance(Type serviceType)
        {
            return instance;
        }
    }
}
