using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new GameBuilder()
                            .AddCommand<ExitCommandInfo>("exit")
                            .AddCommand<AnswerCommandInfo>("a")
                            .AddIOHandler<ConsoleIOHandler>()
                            .AddSystem<MathGameSystem>()
                            .ComposeSystems()
                            .ComposeEntities()
                            .Build();

            await builder.StartGameAsync();
        }
    }
}
