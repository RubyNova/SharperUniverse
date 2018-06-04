using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestCommandBinding : IUniverseCommandBinding
    {
        public string CommandName { get; }

        public bool NewState { get; private set; }

        public TestCommandBinding(string commandName, List<string> args)
        {
            CommandName = commandName;
            if (args.Count >= 1 && bool.TryParse(args[1], out var boolResult))
            {
                NewState = boolResult;
            }
        }
    }
}
