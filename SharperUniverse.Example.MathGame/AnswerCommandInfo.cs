using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    public class AnswerCommandInfo : IUniverseCommandInfo
    {
        public int Guess { get; private set; }

        public Task ProcessArgsAsync(List<string> args)
        {
            int.TryParse(args[0], out int guess);
            Guess = guess;

            return Task.CompletedTask;
        }
    }
}
