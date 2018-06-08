using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    class ConsoleIOHandler : IIOHandler
    {
        public async Task<(string CommandName, List<string> Args, IUniverseCommandSource CommandSource)> GetInputAsync()
        {
            var input = (await Console.In.ReadLineAsync()).Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!input.Any()) return (string.Empty, new List<string>(), new EmptySource(2));
            return (input[0], input.Length > 1 ? new List<string>() { input[1] } : new List<string>(), new EmptySource(2));
        }

        public async Task SendOutputAsync(string outputText)
        {
            await Console.Out.WriteLineAsync(outputText);
        }
    }

    class EmptySource : IUniverseCommandSource
    {
        public int ID { get; set; }

        public EmptySource(int id)
        {
            ID = id;
        }

        public bool SourceIsSameAsBindingSource(IUniverseCommandSource bindingSource)
        {
            return ID == (bindingSource as EmptySource).ID;
        }
    }
}
