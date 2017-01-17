using AspectCore.Abstractions;
using AspectCore.Abstractions.Extensions;
using AspectCore.Container.Test.Fakes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace AspectCore.Container.DependencyInjection.Test
{
    public class ServiceProviderTests
    {
        [Fact]
        public void ServiceProvider_GetService_IsDynamically_Tests()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServiceProvider = aspectCoreServiceProviderFactory.CreateServiceProvider(services);
            var proxyService = proxyServiceProvider.GetService<IService>();
            Assert.True(proxyService.GetType().GetTypeInfo().IsDynamically());
        }

        [Fact]
        public void SupportOriginalService_Test()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServiceProvider = aspectCoreServiceProviderFactory.CreateServiceProvider(services);
            var originalServiceProvider = proxyServiceProvider.GetService<IOriginalServiceProvider>();
            var service = originalServiceProvider.GetService<IService>();
            Assert.False(service.GetType().GetTypeInfo().IsDynamically());
            Assert.NotEqual(service.Get(1), service.Get(1));
        }

        [Fact]
        public void ServiceProvider_GetService_WithInterceptor_Tests()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServiceProvider = aspectCoreServiceProviderFactory.CreateServiceProvider(services);
            var proxyService = proxyServiceProvider.GetService<IService>();
            Assert.Equal(proxyService.Get(1), proxyService.Get(1));
        }
    }
}
