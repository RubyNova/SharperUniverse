using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public interface ISharperSystem<out T> where T : BaseSharperComponent
    {
        Task CycleUpdateAsync(Func<string, Task> outputHandler);
    }
}
