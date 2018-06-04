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
                //.AddEntity().WithComponent<SharperInputComponent>(new TestCommandSource(0))
                //.AddEntity().WithComponent<TestComponent>(true, ) //this needs a SharperEntity with a SharperInputComponent
                .ComposeEntities()
                .Build();

            await builder.StartGameAsync();
        }
    }
}