using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core
{
    public class GameRunner
    {
        private readonly List<ISharperSystem<BaseSharperComponent>> _systems;
        private readonly UniverseCommandRunner _commandRunner;
        private readonly IIOHandler _ioHandler;
        private readonly List<SharperEntity> _entities;

        public GameRunner(UniverseCommandRunner commandRunner, IIOHandler ioHandler)
        {
            _systems = new List<ISharperSystem<BaseSharperComponent>>();
            _commandRunner = commandRunner;
            _ioHandler = ioHandler;
            _entities = new List<SharperEntity>();
        }

        public void RegisterSystem(ISharperSystem<BaseSharperComponent> system)
        {
            if (_systems.Contains(system) || _systems.Any(x => x.GetType() == system.GetType())) throw new DuplicateSharperObjectException();

            _systems.Add(system);
        }

        public Task<SharperEntity> CreateEntityAsync()
        {
            var ent = new SharperEntity();
            _entities.Add(ent);
            return Task.FromResult(ent); //I have no idea if this is ok LUL
        }

        public async Task RunGameAsync()
        {
            _commandRunner.ComposeCommands(_ioHandler, _systems);
            ComposeSystems();
            Task<(string commandName, List<string> args)> inputTask = Task.Run(() => _ioHandler.GetInputAsync());
            Func<string, Task> outputDel = _ioHandler.SendOutputAsync;
            while (true)
            {
                if (inputTask.IsCompleted)
                {
                    await _commandRunner.AttemptExecuteAsync(inputTask.Result.commandName, inputTask.Result.args);
                    inputTask = Task.Run(() => _ioHandler.GetInputAsync());
                }
                else if (inputTask.IsFaulted)
                {
                    var exception = inputTask.Exception ?? new Exception("REEEEEEEEEEEEEEEEEEE WTF DID YOU DO TO MY POOR ENGINE???");
                    throw exception;
                }
                foreach (var sharperSystem in _systems)
                {
                    await sharperSystem.CycleUpdateAsync(outputDel);
                }
                await Task.Delay(50);
            }
        }

        private void ComposeSystems()
        {
            foreach (var sharperSystem in _systems)
            {
                var sysType = sharperSystem.GetType();
                var method = sysType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(x => x.GetCustomAttribute<SharperInjectAttribute>() != null);

                if (method == null) continue;

                var parameters = method.GetParameters();

                if (parameters.Any(x => !typeof(BaseSharperSystem<>).IsSubclassOfRawGeneric(x.ParameterType))) throw new NotSupportedException($"One or more parameters in type {sharperSystem.GetType().Name} are not BaseSharperSystems. This is not supported. Please pass in any dependencies that aren't systems via the constructor.");

                var systems = new List<object>();
                foreach (var parameterInfo in parameters)
                {
                    systems.Add(_systems.First(x => x.GetType() == parameterInfo.ParameterType));
                }
                method.Invoke(sharperSystem, systems.ToArray());
            }
        }
    }
}
