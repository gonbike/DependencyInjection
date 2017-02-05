using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Abstractions;

namespace AspectCore.Container.DependencyInjection.Test
{
    public class SpecificationTests: DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAspectConfigure, NonIgnoreAspectConfiguration>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            return aspectCoreServiceProviderFactory.CreateServiceProvider(serviceCollection);
        }
    }
}
