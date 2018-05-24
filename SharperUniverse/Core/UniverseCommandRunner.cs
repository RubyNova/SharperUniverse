using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core
{
    /// <summary>
    /// This is the type the <see cref="GameRunner"/> uses to process commands from the <see cref="IIOHandler"/>. This class cannot be inherited.
    /// </summary>
    public sealed class UniverseCommandRunner
    {
        private readonly List<IUniverseCommandBinding> _bindings;

        /// <summary>
        /// Creates a new instance of <see cref="UniverseCommandRunner"/>.
        /// </summary>
        public UniverseCommandRunner()
        {
            _bindings = new List<IUniverseCommandBinding>();
        }

        /// <summary>
        /// Adds a new <see cref="IUniverseCommandBinding"/> to this command runner.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public void AddCommandBinding(IUniverseCommandBinding binding)
        {
            _bindings.Add(binding);
        }

        /// <summary>
        /// Creates a new instance of <see cref="UniverseCommandRunner"/> with a set of commands.
        /// </summary>
        /// <param name="bindings">The command bindings to add.</param>
        public UniverseCommandRunner(List<IUniverseCommandBinding> bindings)
        {
            _bindings = bindings;
        }

        /// <summary>
        /// Injects all dependencies involving <see cref="SharperInjectAttribute"/> marked methods. This should not be called from external code.
        /// </summary>
        /// <param name="ioHandler">The <see cref="IIOHandler"/> to pass in to bindings that require it as a dependency.</param>
        /// <param name="systems">The systems registered to the <see cref="GameRunner"/>. These are passed in as dependencies based on the <see cref="IUniverseCommandBinding"/>.</param>
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

        /// <summary>
        /// Asynchronously attempts to execute the specified command from the <see cref="IIOHandler"/>. This should not be called from external code.
        /// </summary>
        /// <param name="command">The name of the command.</param>
        /// <param name="args">The arguments to pass in to the command, in text form.</param>
        /// <returns></returns>
        public async Task AttemptExecuteAsync(string command, List<string> args)
        {
            var bla = _bindings.FirstOrDefault(x => x.CommandName.ToUpper() == command.ToUpper());
            if (bla == null) return;
            await bla.ProcessCommandAsync(args);
        }
    }
}
