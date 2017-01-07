using AspectCore.Lite.Abstractions.Resolution;
using System;

namespace AspectCore.Lite.Container.DependencyInjection
{
    internal sealed class SupportOriginalService : ISupportOriginalService
    {

        private readonly IServiceProvider serviceProvider;

        public SupportOriginalService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        public void Dispose()
        {
            var disposable = serviceProvider as IDisposable;
            disposable?.Dispose();
        }

    }
}
