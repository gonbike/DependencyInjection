﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AspectCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AspectCore.Extensions.DependencyInjection
{
    internal sealed class ServiceInstanceProvider : IServiceInstanceProvider, IDisposable
    {
        private readonly static ConcurrentDictionary<Type, IList<ServiceDescriptor>> ServiceDescriptorCache = new ConcurrentDictionary<Type, IList<ServiceDescriptor>>();

        private readonly static ConcurrentDictionary<IServiceProvider, ConcurrentDictionary<Type, object>> ScopedResolvedServiceCache = new ConcurrentDictionary<IServiceProvider, ConcurrentDictionary<Type, object>>();

        private readonly IServiceProvider _serviceProvider;

        private readonly object _resolveLock = new object();

        public ServiceInstanceProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        public object GetInstance(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var descriptorList = default(IList<ServiceDescriptor>);

            if (!ServiceDescriptorCache.TryGetValue(serviceType, out descriptorList))
            {
                return _serviceProvider.GetRequiredService(serviceType);
            }

            var descriptor = descriptorList.Last();

            var factory = GetObjectFactory();

            if (descriptor.Lifetime == ServiceLifetime.Transient)
            {
                return factory(_serviceProvider, descriptor.ImplementationType);
            }

            var resolvedServices = GetOrAddResolvedCache();

            return resolvedServices.GetOrAdd(serviceType, _ => factory(_serviceProvider, descriptor.ImplementationType));
        }

        private Func<IServiceProvider, Type, object> GetObjectFactory()
        {
            return (servicrProvider, type) => ActivatorUtilities.CreateInstance(servicrProvider, type);
        }

        private ConcurrentDictionary<Type, object> GetOrAddResolvedCache()
        {
            var resolvedCache = ScopedResolvedServiceCache.GetOrAdd(_serviceProvider, _ => new ConcurrentDictionary<Type, object>());
            return resolvedCache;
        }

        internal static void MapServiceDescriptor(ServiceDescriptor descriptor)
        {
            var descriptorList = ServiceDescriptorCache.GetOrAdd(descriptor.ServiceType, _ => new List<ServiceDescriptor>());

            if (descriptor.ImplementationType != null)
            {
                descriptorList.Add(ServiceDescriptor.Describe(descriptor.ServiceType, descriptor.ImplementationType, descriptor.Lifetime));
            }
        }

        public void Dispose()
        {
            ConcurrentDictionary<Type, object> resolvedServices;
            if (ScopedResolvedServiceCache.TryRemove(_serviceProvider, out resolvedServices))
            {
                resolvedServices.Clear();
            }
        }
    }
}