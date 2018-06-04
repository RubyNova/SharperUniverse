using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The interface for all game commands. Your commands must implement this in order to function correctly. Do not pass <see cref="IIOHandler"/> types or <see cref="ISharperSystem{T}"/> types via a constructor that implements this interface. Instead, see <see cref="SharperInjectAttribute"/>.
    /// </summary>
    public interface IUniverseCommandInfo
    {
        Task ProcessArgsAsync(List<string> args);
    }
}