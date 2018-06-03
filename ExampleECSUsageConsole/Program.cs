using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var builder = new GameBuilder()
                .AddCommand<TestCommandBinding>("s")
                .AddIOHandler<ConsoleIOHandler>()
                .AddSystem<TestSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<TestComponent>(true)
                .ComposeEntities()
                .Build();

            await builder.StartGameAsync();
        }
    }
}