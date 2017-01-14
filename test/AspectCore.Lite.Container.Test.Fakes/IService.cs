using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCore.Lite.Container.Test.Fakes
{
    public interface IService
    {
        [CacheInterceptor]
        Model Get(int id);
    }
}
