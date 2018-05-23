using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public interface IIOHandler
    {
        Task<(string commandName, List<string> args)> GetInputAsync();
        Task SendOutputAsync(string outputText);
    }
}