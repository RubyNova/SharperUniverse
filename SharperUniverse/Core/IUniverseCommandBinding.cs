using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public interface IUniverseCommandBinding
    {
        string CommandName { get; }
        Task ProcessCommandAsync(List<string> args);
    }
}