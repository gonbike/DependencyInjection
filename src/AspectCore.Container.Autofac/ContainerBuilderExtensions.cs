using AspectCore.Abstractions;
using AspectCore.Abstractions.Resolution;
using AspectCore.Abstractions.Resolution.Internal;
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

        public static ContainerBuilder RegisterAspectCore(this ContainerBuilder builder, Action<IAspectConfigure> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerDependency();

            builder.RegisterType<AutofacOriginalServiceProvider>().As<IOriginalServiceProvider>().InstancePerDependency();

            builder.RegisterType<AspectActivator>().As<IAspectActivator>().InstancePerLifetimeScope();

            builder.RegisterType<AspectBuilderProvider>().As<IAspectBuilderProvider>().InstancePerLifetimeScope();

            builder.RegisterType<InterceptorSelector>().As<IInterceptorSelector>().InstancePerLifetimeScope();

            builder.RegisterType<InterceptorInjectorProvider>().As<IInterceptorInjectorProvider>().InstancePerLifetimeScope();

            builder.RegisterType<PropertyInjectorSelector>().As<IPropertyInjectorSelector>().InstancePerLifetimeScope();

            builder.RegisterType<InterceptorMatcher>().As<IInterceptorMatcher>().InstancePerLifetimeScope();

            builder.RegisterType<AspectValidator>().As<IAspectValidator>().InstancePerLifetimeScope();

            builder.RegisterType<ProxyGenerator>().As<IProxyGenerator>().InstancePerLifetimeScope();

            var aspectConfigure = new AspectConfigure();
            configure?.Invoke(aspectConfigure);
            builder.RegisterInstance<IAspectConfigure>(aspectConfigure).SingleInstance();

            return builder;
        }
    }
}
