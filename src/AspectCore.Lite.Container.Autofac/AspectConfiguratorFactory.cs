using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using Autofac;
using System;
using System.Collections.Concurrent;

namespace AspectCore.Lite.Container.Autofac
{
    internal sealed class AspectConfiguratorFactory : IAspectConfiguratorFactory<ContainerBuilder>
    {
        private readonly ConcurrentDictionary<int, IAspectConfigurator> configuratorCache;

        public AspectConfiguratorFactory()
        {
            configuratorCache = new ConcurrentDictionary<int, IAspectConfigurator>();
        }

        public IAspectConfigurator CreateConfigurator(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }
            return configuratorCache.GetOrAdd(containerBuilder.GetHashCode(), _ => new AspectConfigurator());
        }
    }
}
