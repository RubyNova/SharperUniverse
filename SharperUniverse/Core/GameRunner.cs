using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core
{
    /// <summary>
    /// This is the entry point and main manager for any game that uses SharperUniverse.
    /// </summary>
    public class GameRunner
    {
        private static GameRunner _instance;

        internal static GameRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameRunner();
                }

                return _instance;
            }
        }

        internal List<ISharperSystem<BaseSharperComponent>> _systems;
        internal UniverseCommandRunner _commandRunner;
        internal IIOHandler _ioHandler;
        internal List<SharperEntity> _entities;
        internal int _deltaMs;

        internal GameRunner()
        {
            _systems = new List<ISharperSystem<BaseSharperComponent>>();
            _commandRunner = new UniverseCommandRunner();
            _entities = new List<SharperEntity>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="GameRunner"/> with the specified <see cref="UniverseCommandRunner"/> and <see cref="IIOHandler"/> instances, along with the delta time, in milliseconds.
        /// </summary>
        /// <param name="commandRunner">Your instance of <see cref="UniverseCommandRunner"/>.</param>
        /// <param name="ioHandler">Your implementation of the IO logic.</param>
        /// <param name="deltaMs">The frequency of the update cycle, in milliseconds.</param>
        public GameRunner(UniverseCommandRunner commandRunner, IIOHandler ioHandler, int deltaMs)
        {
            _systems = new List<ISharperSystem<BaseSharperComponent>>();
            _commandRunner = commandRunner;
            _ioHandler = ioHandler;
            _entities = new List<SharperEntity>();
            _deltaMs = deltaMs;
        }

        /// <summary>
        /// Registers an <see cref="ISharperSystem{T}"/> to the Game. This should not be called from external code.
        /// </summary>
        /// <param name="system">The target system to register.</param>
        public void RegisterSystem(ISharperSystem<BaseSharperComponent> system)
        {
            if (_systems.Contains(system) || _systems.Any(x => x.GetType() == system.GetType())) throw new DuplicateSharperObjectException();

            _systems.Add(system);
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="SharperEntity"/> and registers it to the game.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that will contain the new <see cref="SharperEntity"/> on completion.</returns>
        public Task<SharperEntity> CreateEntityAsync()
        {
            var ent = new SharperEntity();
            _entities.Add(ent);
            return Task.FromResult(ent); //I have no idea if this is ok LUL
        }

        /// <summary>
        /// Launches the Game. This task runs for as long as the game is running.
        /// </summary>
        /// <returns>A <see cref="Task"/> represnting the asynchronous game loop.</returns>
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
                await Task.Delay(_deltaMs);
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
