using AspectCore.Lite.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using AspectCore.Lite.Abstractions.Resolution;

namespace AspectCore.Lite.Container.DependencyInjection
{
    internal sealed class AspectConfiguratorFactory : IAspectConfiguratorFactory<IServiceCollection>
    {
        private readonly ConcurrentDictionary<int, IAspectConfigurator> configuratorCache;

        public AspectConfiguratorFactory()
        {
            configuratorCache = new ConcurrentDictionary<int, IAspectConfigurator>();
        }

        public IAspectConfigurator CreateConfigurator(IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return configuratorCache.GetOrAdd(serviceCollection.GetHashCode(), _ => new AspectConfigurator());
        }
    }
}
