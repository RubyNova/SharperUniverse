using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new GameBuilder()
                .AddCommand<TestCommandInfo>("s")
                .AddIOHandler<ConsoleIOHandler>()
                .AddSystem<TestSystem>()
                .ComposeSystems()
                .ComposeEntities()
                .Build();

            await builder.StartGameAsync();
        }
    }
}