using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public class UniverseCommandRunner
    {
        private readonly List<IUniverseCommandBinding> _bindings;

        public UniverseCommandRunner()
        {
            _bindings = new List<IUniverseCommandBinding>();
        }

        public void AddCommandBinding(IUniverseCommandBinding binding)
        {
            _bindings.Add(binding);
        }

        public UniverseCommandRunner(List<IUniverseCommandBinding> bindings)
        {
            _bindings = bindings;
        }

        public async Task AttemptExecuteAsync(string command, List<string> args)
        {
            var bla = _bindings.FirstOrDefault(x => x.CommandName.ToUpper() == command.ToUpper());
            if (bla == null) return;
            await bla.ProcessCommandAsync(args);
        }
    }
}
