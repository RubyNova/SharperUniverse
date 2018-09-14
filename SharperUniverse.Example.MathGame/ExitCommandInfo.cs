using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    public class ExitCommandInfo : IUniverseCommandInfo
    {
        public Task ProcessArgsAsync(List<string> args)
        {
            return Task.CompletedTask;
        }
    }
}
