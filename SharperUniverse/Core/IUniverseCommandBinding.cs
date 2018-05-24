using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The interface for all game commands. Your commands must implement this in order to function correctly. Do not pass <see cref="IIOHandler"/> types or <see cref="ISharperSystem{T}"/> types via a constructor that implements this interface. Instead, see <see cref="SharperInjectAttribute"/>.
    /// </summary>
    public interface IUniverseCommandBinding
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Runs the command logic in the <see cref="IUniverseCommandBinding"/>. This is the entry point for executing the command logic.
        /// </summary>
        /// <param name="args">The arguments, in text form, parsed in from the <see cref="IIOHandler"/>.</param>
        /// <returns></returns>
        Task ProcessCommandAsync(List<string> args);
    }
}