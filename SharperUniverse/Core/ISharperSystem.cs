using System;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public interface ISharperSystem<out T> where T : BaseSharperComponent
    {
        Task CycleUpdateAsync(Func<string, Task> outputHandler);
    }
}
