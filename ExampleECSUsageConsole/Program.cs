using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var commandRunner = new UniverseCommandRunner();
            commandRunner.AddCommandBinding(new TestCommandBinding("switch"));
            var runner = new GameRunner(commandRunner, new ConsoleIOHandler(), 50);
            var system = new TestSystem(runner);
            var ent = await runner.CreateEntityAsync();
            await system.RegisterComponentAsync(ent, true);
            await runner.RunGameAsync();
        }
    }
}
