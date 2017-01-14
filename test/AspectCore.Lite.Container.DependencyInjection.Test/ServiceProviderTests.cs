using AspectCore.Lite.Container.Test.Fakes;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Lite.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using AspectCore.Lite.Abstractions.Resolution;

namespace AspectCore.Lite.Container.DependencyInjection.Test
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
            var supportOriginalService = proxyServiceProvider.GetService<ISupportOriginalService>();
            var service = (IService)supportOriginalService.GetService(typeof(IService));
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
