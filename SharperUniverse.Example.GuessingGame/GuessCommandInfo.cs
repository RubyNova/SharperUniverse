using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class GuessCommandInfo : IUniverseCommandInfo
    {
        public int Guess { get; private set; }

        public Task ProcessArgsAsync(List<string> args)
        {
            if (args.Any())
            {
                // Grab the user's guess, and store it on this command info
                //  This should be retrievable from a system, right? Does that system need a sharper input system?
                Guess = int.Parse(args.First());
            }

            return Task.CompletedTask;
        }
    }
}