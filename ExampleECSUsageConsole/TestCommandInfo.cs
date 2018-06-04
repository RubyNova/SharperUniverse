using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestCommandInfo : IUniverseCommandInfo
    {
        public bool NewState { get; private set; }

        public Task ProcessArgsAsync(List<string> args)
        {
            if (args.Count >= 1 && bool.TryParse(args[0], out var result))
            {
                NewState = result;
            }
            return Task.CompletedTask;
        }
    }
}
