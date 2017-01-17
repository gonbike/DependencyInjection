using Autofac.Core;
using System;

namespace AspectCore.Container.Autofac
{
    public static class ServiceExtensions
    {
        public static Type GetServiceType(this Service service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return ((IServiceWithType)service).ServiceType;
        }
    }
}
