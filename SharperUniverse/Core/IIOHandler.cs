using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The base interface your IO type must implement in order to get proper asynchronous input and output from the <see cref="GameRunner"/>.
    /// </summary>
    public interface IIOHandler
    {
        /// <summary>
        /// Asynchronously handles getting input from a source.
        /// </summary>
        /// <returns>Returns a <see cref="Task{TResult}"/> which contains the filtered command name and arguments for the command in question.</returns>
        Task<(string commandName, List<string> args)> GetInputAsync();

        /// <summary>
        /// Asynchronously pushes output from the <see cref="GameRunner"/>.
        /// </summary>
        /// <param name="outputText">The text the <see cref="GameRunner"/> will output.</param>
        /// <returns></returns>
        Task SendOutputAsync(string outputText);
    }
}