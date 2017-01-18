using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCore.Container.Test.Fakes
{
    [CacheInterceptor]
    public interface IController
    {
        IService Service { get; }
        Model Execute();
    }
}
