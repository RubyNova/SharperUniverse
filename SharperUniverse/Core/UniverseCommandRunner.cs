using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core
{
    public sealed class UniverseCommandRunner
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

        public void ComposeCommands(IIOHandler ioHandler, List<ISharperSystem<BaseSharperComponent>> systems)
        {
            foreach (var commandBinding in _bindings)
            {
                var sysType = commandBinding.GetType();
                var method = sysType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(x => x.GetCustomAttribute<SharperInjectAttribute>() != null);

                if (method == null) continue;

                var parameters = method.GetParameters();

                if (parameters.Any(x => !typeof(BaseSharperSystem<>).IsSubclassOfRawGeneric(x.ParameterType) && x.ParameterType != typeof(IIOHandler))) throw new NotSupportedException($"One or more parameters in type {commandBinding.GetType().Name} are not BaseSharperSystems or an IIOHandler. This is not supported. Please pass in any dependencies that aren't systems via the constructor.");

                var objectsToAdd = new List<object>();
                foreach (var parameterInfo in parameters)
                {
                    if (parameterInfo.ParameterType == typeof(IIOHandler))
                    {
                        objectsToAdd.Add(ioHandler);
                    }
                    else
                    {
                        objectsToAdd.Add(systems.First(x => x.GetType() == parameterInfo.ParameterType));
                    }
                }
                method.Invoke(commandBinding, objectsToAdd.ToArray());
            }



        }

        public async Task AttemptExecuteAsync(string command, List<string> args)
        {
            var bla = _bindings.FirstOrDefault(x => x.CommandName.ToUpper() == command.ToUpper());
            if (bla == null) return;
            await bla.ProcessCommandAsync(args);
        }
    }
}
