using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class ConsoleIOHandler : IIOHandler
    {
        public async Task<(string CommandName, List<string> Args, IUniverseCommandSource CommandSource)> GetInputAsync()
        {
            var input = await Console.In.ReadLineAsync();
            var split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length >= 2)
            {
                return (split[0], new List<string> { split[1] }, new GuessCommandSource(0));
            }

            return (string.Empty, new List<string>(), new GuessCommandSource(0));
        }

        public Task SendOutputAsync(string outputText)
        {
            Console.Out.WriteLine(outputText);
            return Task.CompletedTask;
        }
    }
}