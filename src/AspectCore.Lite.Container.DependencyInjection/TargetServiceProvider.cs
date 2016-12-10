using AspectCore.Lite.DynamicProxy;
using System;

namespace AspectCore.Lite.Container.DependencyInjection
{
    internal sealed class TargetServiceProvider : ITargetServiceProvider
    {

        private readonly IServiceProvider serviceProvider;

        public TargetServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            var disposable = serviceProvider as IDisposable;
            disposable?.Dispose();
        }

        public object GetTarget(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }
    }
}
