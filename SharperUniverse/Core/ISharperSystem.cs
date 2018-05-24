using System;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The interface <see cref="BaseSharperSystem{T}"/> inherits from. You should not create types directly from this interface. Instead, inherit <see cref="BaseSharperSystem{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="BaseSharperSystem{T}"/> this system governs.</typeparam>
    public interface ISharperSystem<out T> where T : BaseSharperComponent
    {
        /// <summary>
        /// The logic that runs in an <see cref="ISharperSystem{T}"/> once per update cycle. The update cycle is defined by the <see cref="GameRunner"/> on instantiation.
        /// </summary>
        /// <param name="outputHandler"></param>
        /// <returns></returns>
        Task CycleUpdateAsync(Func<string, Task> outputHandler);
    }
}
