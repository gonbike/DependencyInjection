using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace AspectCore.Container.Autofac
{
    internal class ParameterConstants
    {
        internal readonly static Parameter[] DefaultParameters = new Parameter[] { new AutowiringParameter(), new DefaultValueParameter() };
    }
}
