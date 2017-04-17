﻿using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Abstractions;

namespace AspectCore.Extensions.DependencyInjection.Test
{
    public class SpecificationTests: DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            serviceCollection.AddAspectCore();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            return aspectCoreServiceProviderFactory.CreateServiceProvider(serviceCollection);
        }
    }
}
