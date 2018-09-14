using System;
using System.Threading.Tasks;
using SharperUniverse.Core.Builder;

namespace ExampleECSUsageConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("setting up game...");
            var builder = new GameBuilder()
                .AddCommand<TestCommandInfo>("s")
                .CreateSystem()
                .AddSystem<TestSystem>()
                .ComposeSystems()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(23)
                .Build();
            Console.WriteLine("Setup complete! Game will now begin.");
            await builder.StartGameAsync();
        }
    }
}