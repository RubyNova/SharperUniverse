using System.Threading.Tasks;
using SharperUniverse.Core.Builder;

namespace SharperUniverse.Example.MathGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new GameBuilder()
                .AddCommand<ExitCommandInfo>("exit")
                .AddCommand<AnswerCommandInfo>("a")
                .CreateSystem()
                .AddSystem<MathGameSystem>()
                .ComposeSystems()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(23)
                .Build();

            await builder.StartGameAsync();
        }
    }
}
