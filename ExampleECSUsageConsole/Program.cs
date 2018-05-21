using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var commandRunner = new UniverseCommandRunner();
            var io = new ConsoleIOHandler();
            var runner = new GameRunner(commandRunner, io);
            var system = new TestSystem(runner);
            var command = new TestCommandBinding("switch", system, io);
            commandRunner.AddCommandBinding(command);
            var ent = await runner.CreateEntityAsync();
            await system.RegisterComponentAsync(ent, true);

            await runner.RunGameAsync();


        }
    }
}
