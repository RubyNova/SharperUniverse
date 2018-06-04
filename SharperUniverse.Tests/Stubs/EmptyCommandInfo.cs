using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
    public class EmptyCommandInfo : IUniverseCommandInfo
    {
        private IIOHandler _ioHandler;

        [SharperInject]
        private void InitializeCommandRequirements(IIOHandler ioHandler)
        {
            _ioHandler = ioHandler;
        }

        public Task ProcessArgsAsync(List<string> args)
        {
            return Task.CompletedTask;
        }

        private Task ExecuteCommandAsync(int index, bool result)
        {
            return Task.CompletedTask;
        }
    }
}
