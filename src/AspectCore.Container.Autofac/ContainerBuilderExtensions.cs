using AspectCore.Abstractions;
using AspectCore.Abstractions.Resolution;
using Autofac;
using System;

namespace AspectCore.Container.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterAspectCore(this ContainerBuilder builder)
        {
            return RegisterAspectCore(builder, null);
        }

        public static ContainerBuilder RegisterAspectCore(this ContainerBuilder builder, Action<IAspectConfiguration> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.RegisterType<AspectActivator>().As<IAspectActivator>().InstancePerDependency();
            builder.RegisterType<AspectBuilder>().As<IAspectBuilder>().InstancePerDependency();
            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerDependency();
            builder.RegisterType<AutofacOriginalServiceProvider>().As<IOriginalServiceProvider>().InstancePerDependency();

            builder.RegisterType<InterceptorInjector>().As<IInterceptorInjector>().InstancePerLifetimeScope();
            builder.RegisterType<AspectValidator>().As<IAspectValidator>().SingleInstance();
            builder.RegisterType<InterceptorMatcher>().As<IInterceptorMatcher>().SingleInstance();
            builder.RegisterType<ProxyGenerator>().As<IProxyGenerator>().SingleInstance();

            var aspectConfiguration = new AspectConfiguration();
            configure?.Invoke(aspectConfiguration);
            builder.RegisterInstance<IAspectConfiguration>(aspectConfiguration).SingleInstance();

            return builder;
        }
    }
}
