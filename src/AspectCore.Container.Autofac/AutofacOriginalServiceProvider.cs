﻿using AspectCore.Abstractions;
using AspectCore.Abstractions.Extensions;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspectCore.Container.Autofac
{
    internal class AutofacOriginalServiceProvider : IOriginalServiceProvider
    {
        private readonly static ConcurrentDictionary<Type, ConcreteReflectionActivatorData> ActivatorCache = new ConcurrentDictionary<Type, ConcreteReflectionActivatorData>();

        private readonly IComponentContext componentContext;

        public AutofacOriginalServiceProvider(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var activatorData = default(ConcreteReflectionActivatorData);
            if (!ActivatorCache.TryGetValue(serviceType, out activatorData))
            {
                return componentContext.Resolve(serviceType);
            }

            var prioritisedParameters = activatorData.ConfiguredParameters.ToArray().Concat(ParameterConstants.DefaultParameters).ToArray();
            var targetConstructor = GetTargetConstructor(componentContext, activatorData, prioritisedParameters);
            var resolvedParameters = GetResolvedParameters(componentContext, targetConstructor, prioritisedParameters);

            return activatorData.Activator.ActivateInstance(componentContext, resolvedParameters);
        }

        private IEnumerable<Parameter> GetResolvedParameters(IComponentContext context, ConstructorInfo targetConstructor, IEnumerable<Parameter> prioritisedParameters)
        {
            var constructorParameters = targetConstructor.GetParameters();

            var resolvedParameters = new Parameter[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; ++i)
            {
                var pi = constructorParameters[i];
                foreach (var param in prioritisedParameters)
                {
                    Func<object> valueRetriever;
                    if (param.CanSupplyValue(pi, componentContext, out valueRetriever))
                    {
                        var value = valueRetriever();
                        if (value.GetType().GetTypeInfo().IsDynamically())
                        {
                            resolvedParameters[i] = new PositionalParameter(i, GetService(pi.ParameterType));
                        }
                        else
                        {
                            resolvedParameters[i] = param;
                        }
                        break;
                    }
                }
            }

            return resolvedParameters;
        }

        private ConstructorInfo GetTargetConstructor(IComponentContext context, ConcreteReflectionActivatorData data, IEnumerable<Parameter> parameters)
        {
            var constructors = data.ConstructorFinder.FindConstructors(data.ImplementationType);
            var constructorBindings = GetConstructorBindings(componentContext, parameters, constructors);
            var validBindings = constructorBindings.Where(cb => cb.CanInstantiate).ToArray();

            if (validBindings.Length == 0)
                throw new DependencyResolutionException(GetBindingFailureMessage(constructorBindings, data));

            return data.ConstructorSelector.SelectConstructorBinding(validBindings).TargetConstructor;
        }

        private IEnumerable<ConstructorParameterBinding> GetConstructorBindings(IComponentContext context, IEnumerable<Parameter> parameters, IEnumerable<ConstructorInfo> constructorInfo)
        {
            return constructorInfo.Select(ci => new ConstructorParameterBinding(ci, parameters, context));
        }

        private string GetBindingFailureMessage(IEnumerable<ConstructorParameterBinding> constructorBindings, ConcreteReflectionActivatorData data)
        {
            var reasons = new StringBuilder();

            foreach (var invalid in constructorBindings.Where(cb => !cb.CanInstantiate))
            {
                reasons.AppendLine();
                reasons.Append(invalid.Description);
            }

            return string.Format("NoConstructorsBindable", data.ConstructorFinder, data.ImplementationType, reasons);
        }

        internal static void MapActivatorData(Type serviceType, ConcreteReflectionActivatorData concreteReflectionActivatorData)
        {
            ActivatorCache.TryAdd(serviceType, concreteReflectionActivatorData);
        }
    }
}
