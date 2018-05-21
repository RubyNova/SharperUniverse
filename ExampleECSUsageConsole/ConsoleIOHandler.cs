using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperUniverse.Core;
using SharperUniverse.Utilities;

namespace ExampleECSUsageConsole
{
    class ConsoleIOHandler : IIOHandler
    {
        public async Task<(string commandName, List<string> args)> GetInputAsync()
        {
            var inputStr = await Console.In.ReadLineAsync();
            var inputSplit = inputStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var returnList = inputSplit.SubArray(1, inputSplit.Length - 1);
            return (inputSplit[0], returnList.ToList());
        }

        public async Task SendOutputAsync(string outputText)
        {
            await Console.Out.WriteLineAsync(outputText);
        }
    }
}
