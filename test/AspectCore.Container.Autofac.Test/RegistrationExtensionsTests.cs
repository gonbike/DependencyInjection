using AspectCore.Abstractions;
using System;
using Xunit;
using Autofac;
using AspectCore.Container.Test.Fakes;
using System.Reflection;
using AspectCore.Abstractions.Extensions;

namespace AspectCore.Container.Autofac.Test
{
    public class RegistrationExtensionsTests
    {
        private ContainerBuilder CreateBuilder()
        {
            return new ContainerBuilder().RegisterAspectCore();
        }

        [Theory]
        [InlineData(typeof(IServiceProvider))]
        [InlineData(typeof(IOriginalServiceProvider))]
        [InlineData(typeof(IAspectActivator))]
        [InlineData(typeof(IAspectBuilderProvider))]
        [InlineData(typeof(IInterceptorSelector))]
        [InlineData(typeof(IAspectConfiguration))]
        [InlineData(typeof(IInterceptorInjectorProvider))]
        [InlineData(typeof(IInterceptorMatcher))]
        [InlineData(typeof(IAspectValidator))]
        [InlineData(typeof(IProxyGenerator))]
        public void RegisterAspectCore_Test(Type serviceType)
        {
            var container = CreateBuilder().Build();
            Assert.True(container.IsRegistered(serviceType));
        }

        [Fact]
        public void AsProxy_Test()
        {
            var builder = CreateBuilder();
            builder.RegisterType<Service>().AsProxy(typeof(IService));
            var container = builder.Build();
            var proxyService = container.Resolve<IService>();
            Assert.True(proxyService.GetType().GetTypeInfo().IsDynamically());
            Assert.Equal(proxyService.Get(1), proxyService.Get(1));
        }

        [Fact]
        public void AsNamedProxy_Test()
        {
            var builder = CreateBuilder();
            builder.RegisterType<Service>().AsNamedProxy("proxy", typeof(IService));
            var container = builder.Build();
            var proxyService = container.ResolveNamed<IService>("proxy");
            Assert.True(proxyService.GetType().GetTypeInfo().IsDynamically());
            Assert.Equal(proxyService.Get(1), proxyService.Get(1));
        }

        [Fact]
        public void AsKeyedProxy_Test()
        {
            var builder = CreateBuilder();
            var key = new object();
            builder.RegisterType<Service>().AsKeyedProxy(key, typeof(IService));
            var container = builder.Build();
            var proxyService = container.ResolveKeyed<IService>(key);
            Assert.True(proxyService.GetType().GetTypeInfo().IsDynamically());
            Assert.Equal(proxyService.Get(1), proxyService.Get(1));
        }

        [Fact]
        public void AsProxyWithParamter_Test()
        {
            var builder = CreateBuilder();
            builder.RegisterType<Service>().AsProxy(typeof(IService));
            builder.RegisterType<Controller>().AsProxy(typeof(IController));
            var container = builder.Build();

            var proxyService = container.Resolve<IService>();
            Assert.True(proxyService.GetType().GetTypeInfo().IsDynamically());
            Assert.Equal(proxyService.Get(1), proxyService.Get(1));

            var proxyController = container.Resolve<IController>();
            Assert.Equal(proxyService.Get(100), proxyController.Execute());
        }

        [Fact]
        public void OriginalServiceProvider_Test()
        {
            var builder = CreateBuilder();
            builder.RegisterType<Service>().AsProxy(typeof(IService));
            var container = builder.Build();
            var proxyService = container.Resolve<IOriginalServiceProvider>().GetService<IService>();
            Assert.False(proxyService.GetType().GetTypeInfo().IsDynamically());
            Assert.NotEqual(proxyService.Get(1), proxyService.Get(1));
        }

        [Fact]
        public void OriginalServiceProviderWithParameter_Test()
        {
            var builder = CreateBuilder();
            builder.RegisterType<Service>().AsProxy(typeof(IService));
            builder.RegisterType<Controller>().AsProxy(typeof(IController));

            var container = builder.Build();

            var proxyService = container.Resolve<IOriginalServiceProvider>().GetService<IService>();

            Assert.False(proxyService.GetType().GetTypeInfo().IsDynamically());

            var proxyController = container.Resolve<IOriginalServiceProvider>().GetService<IController>();

            Assert.False(proxyController.GetType().GetTypeInfo().IsDynamically());
            Assert.False(proxyController.Service.GetType().GetTypeInfo().IsDynamically());

            Assert.NotEqual(proxyService.Get(100), proxyController.Execute());
        }
    }
}
