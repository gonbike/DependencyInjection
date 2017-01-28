using AspectCore.Abstractions;
using AspectCore.Abstractions.Extensions;
using AspectCore.Abstractions.Resolution;
using System;
using System.Linq;
using System.Reflection;

namespace AspectCore.Container.Autofac
{
    internal class AutofacAspectValidator : IAspectValidator
    {
        public bool Validate(MethodInfo method)
        {
            var declaringType = method.DeclaringType.GetTypeInfo();

            if (declaringType.IsDynamically())
            {
                return false;
            }

            if (AspectValidator.IsNonAspect(method) || AspectValidator.IsNonAspect(declaringType))
            {
                return false;
            }

            if (!AspectValidator.IsAccessibility(declaringType) || !AspectValidator.IsAccessibility(method))
            {
                return false;
            }

            return true;
        }

        public void ValidateAndThrowException(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (serviceType.GetTypeInfo().DeclaredMethods.All(m => !Validate(m)))
            {
                throw new InvalidOperationException($"Validate '{serviceType}' failed because the type  does not satisfy the conditions of the generate proxy class.");
            }
        }

        public void ValidateInheritedAndThrowException(Type implementationType)
        {
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            if (!implementationType.GetTypeInfo().CanInherited())
            {
                throw new InvalidOperationException($"Validate '{implementationType}' failed because the type does not satisfy the condition to be inherited.");
            }
        }
    }
}
