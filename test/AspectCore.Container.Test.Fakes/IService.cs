using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCore.Container.Test.Fakes
{
    public interface IService
    {
        [CacheInterceptor]
        Model Get(int id);
    }
}
