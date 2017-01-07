using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCore.Lite.Container.Autofac
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
