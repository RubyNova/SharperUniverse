using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestCommandBinding : IUniverseCommandBinding
    {
        private IIOHandler _ioHandler;
        private TestSystem _system;
        public string CommandName { get; }

        public TestCommandBinding(string commandName, TestSystem system, IIOHandler ioHandler)
        {
            CommandName = commandName;
            _ioHandler = ioHandler;
            _system = system;
        }

        public async Task ProcessCommandAsync(List<string> args)
        {
            if (int.TryParse(args[0], out var intResult) && bool.TryParse(args[1], out var boolResult))
            {
                await ExecuteCommandAsync(intResult, boolResult);
            }
            else
            {
                await _ioHandler.SendOutputAsync("I'm afraid I can't do that, Dave. Reason: You fudged the args weeee!");
            }
        }

        private Task ExecuteCommandAsync(int index, bool result)
        {
            _system.Components[index].State = result;
            return Task.CompletedTask;
        }
    }
}
