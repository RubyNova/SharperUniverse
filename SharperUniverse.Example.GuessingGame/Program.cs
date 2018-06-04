using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new GameBuilder()
                .AddCommand<GuessCommandInfo>("g")
                .AddIOHandler<ConsoleIOHandler>()
                .AddSystem<PlayerSystem>()
                .AddSystem<RoundSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<PlayerComponent>()
                .AddEntity().WithComponent<PlayerComponent>()
                .AddEntity().WithComponent<RoundComponent>()
                .ComposeEntities().Build();

            await builder.StartGameAsync();
        }
    }
}