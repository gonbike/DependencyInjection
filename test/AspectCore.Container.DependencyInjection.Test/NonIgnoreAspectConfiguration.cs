using AspectCore.Abstractions;
using System;
using System.Collections.Concurrent;

namespace AspectCore.Container.DependencyInjection.Test
{
    public class NonIgnoreAspectConfiguration : IAspectConfiguration
    {
        private readonly ConcurrentDictionary<Type, object> optionCache;

        public NonIgnoreAspectConfiguration()
        {
            optionCache = new ConcurrentDictionary<Type, object>();
        }

        public IConfigurationOption<TOption> GetConfigurationOption<TOption>()
        {
            return (ConfigurationOption<TOption>)optionCache.GetOrAdd(typeof(TOption), key => new ConfigurationOption<TOption>());
        }
    }
}
