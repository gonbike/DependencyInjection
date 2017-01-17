﻿using AspectCore.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspectCore.Container.DependencyInjection.Test
{
    public class ConfigurationOption<TOption> : IConfigurationOption<TOption>
    {
        private readonly ICollection<Func<MethodInfo, TOption>> collection = new List<Func<MethodInfo, TOption>>();

        public void Add(Func<MethodInfo, TOption> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            collection.Add(configuration);
        }

        public IEnumerator<Func<MethodInfo, TOption>> GetEnumerator()
        {
            foreach (var item in collection.ToArray())
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
