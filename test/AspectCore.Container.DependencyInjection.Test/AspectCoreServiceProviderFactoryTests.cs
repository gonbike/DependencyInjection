using AspectCore.Abstractions;
using AspectCore.Abstractions.Extensions;
using AspectCore.Abstractions.Resolution.Internal;
using AspectCore.Container.Test.Fakes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Xunit;

namespace AspectCore.Container.DependencyInjection.Test
{
    public class AspectCoreServiceProviderFactoryTests
    {
        [Fact]
        public void CreateBuilderWithProxy_Test()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServices = aspectCoreServiceProviderFactory.CreateBuilder(services);
            var descriptor = Assert.Single(proxyServices, d => d.ServiceType == typeof(IService));
            Assert.NotNull(descriptor.ImplementationType);
            Assert.Equal(descriptor.Lifetime, ServiceLifetime.Transient);
            Assert.IsNotType<Service>(descriptor.ImplementationType);
            Assert.True(descriptor.ImplementationType.GetTypeInfo().IsDynamically());
        }

        [Theory]
        [InlineData(typeof(IAspectActivator))]
        [InlineData(typeof(IAspectBuilderProvider))]
        [InlineData(typeof(IInterceptorSelector))]
        [InlineData(typeof(IInterceptorInjectorProvider))]
        [InlineData(typeof(IServiceInstanceProvider))]
        [InlineData(typeof(IPropertyInjectorSelector))]
        [InlineData(typeof(IInterceptorMatcher))]
        [InlineData(typeof(IAspectConfiguration))]
        [InlineData(typeof(IAspectValidator))]
        [InlineData(typeof(IProxyGenerator))]
        public void CreateBuilderWithAspectCoreServices_Test(Type serviceType)
        {
            var services = new ServiceCollection();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServices = aspectCoreServiceProviderFactory.CreateBuilder(services);
            var descriptor = Assert.Single(proxyServices, d => d.ServiceType == serviceType);
            Assert.False(descriptor.ImplementationType.GetTypeInfo().IsDynamically());
        }
    }
}
