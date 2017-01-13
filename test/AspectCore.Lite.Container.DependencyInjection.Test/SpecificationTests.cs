using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Lite.Abstractions;

namespace AspectCore.Lite.Container.DependencyInjection.Test
{
    public class SpecificationTests: DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAspectConfiguration, NonIgnoreAspectConfiguration>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            return aspectCoreServiceProviderFactory.CreateServiceProvider(serviceCollection);
        }
    }
}
