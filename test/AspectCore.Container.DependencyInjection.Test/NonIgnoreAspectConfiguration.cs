using AspectCore.Abstractions;
using System;
using System.Collections.Concurrent;

namespace AspectCore.Container.DependencyInjection.Test
{
    public class NonIgnoreAspectConfiguration : IAspectConfigure
    {
        private readonly ConcurrentDictionary<Type, object> optionCache;

        public NonIgnoreAspectConfiguration()
        {
            optionCache = new ConcurrentDictionary<Type, object>();
        }

        public IAspectConfigureOption<TOption> GetConfigureOption<TOption>()
        {
            return (ConfigurationOption<TOption>)optionCache.GetOrAdd(typeof(TOption), key => new ConfigurationOption<TOption>());
        }
    }
}
