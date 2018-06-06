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
        internal Dictionary<string, Type> CommandBindings { get; set; }
        internal List<ISharperSystem<BaseSharperComponent>> Systems { get; set; }
        internal List<SharperEntity> Entities { get; set; }
        internal IIOHandler IOHandler { get; set; }
        internal int DeltaMs { get; set; }

        internal GameRunner()
        {
            Systems = new List<ISharperSystem<BaseSharperComponent>>();
            Entities = new List<SharperEntity>();
            DeltaMs = 50;
            CommandBindings = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="GameRunner"/> with the specified <see cref="UniverseCommandRunner"/> and <see cref="IIOHandler"/> instances, along with the delta time, in milliseconds.
        /// </summary>
        /// <param name="commandRunner">Your instance of <see cref="UniverseCommandRunner"/>.</param>
        /// <param name="ioHandler">Your implementation of the IO logic.</param>
        /// <param name="deltaMs">The frequency of the update cycle, in milliseconds.</param>
        public GameRunner(IIOHandler ioHandler, Dictionary<string, Type> commandBindings, int deltaMs)
        {
            CommandBindings = commandBindings;
            Systems = new List<ISharperSystem<BaseSharperComponent>>();
            IOHandler = ioHandler;
            Entities = new List<SharperEntity>();
            DeltaMs = deltaMs;
        }

        public GameRunner(IIOHandler ioHandler, int deltaMs)
        {
            CommandBindings = new Dictionary<string, Type>();
            Systems = new List<ISharperSystem<BaseSharperComponent>>();
            IOHandler = ioHandler;
            Entities = new List<SharperEntity>();
            DeltaMs = deltaMs;
        }

        /// <summary>
        /// Registers an <see cref="ISharperSystem{T}"/> to the Game. This should not be called from external code.
        /// </summary>
        /// <param name="system">The target system to register.</param>
        public void RegisterSystem(ISharperSystem<BaseSharperComponent> system)
        {
            if (Systems.Contains(system) || Systems.Any(x => x.GetType() == system.GetType())) throw new DuplicateSharperObjectException();

            Systems.Add(system);
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="SharperEntity"/> and registers it to the game.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that will contain the new <see cref="SharperEntity"/> on completion.</returns>
        public Task<SharperEntity> CreateEntityAsync()
        {
            var ent = new SharperEntity();
            Entities.Add(ent);
            return Task.FromResult(ent); //I have no idea if this is ok LUL
        }

        /// <summary>
        /// Launches the Game. This task runs for as long as the game is running.
        /// </summary>
        /// <returns>A <see cref="Task"/> represnting the asynchronous game loop.</returns>
        public async Task RunGameAsync()
        {
            ComposeSystems();
            Task<(string CommandName, List<string> Args, IUniverseCommandSource CommandSource)> inputTask = Task.Run(() => IOHandler.GetInputAsync());
            Func<string, Task> outputDel = IOHandler.SendOutputAsync;
            var inputSystem = (SharperInputSystem)Systems.First(x => x is SharperInputSystem);
            while (true)
            {
                if (inputTask.IsCompleted)
                {
                    var commandModel = (IUniverseCommandInfo)Activator.CreateInstance(CommandBindings[inputTask.Result.CommandName]);
                    await commandModel.ProcessArgsAsync(inputTask.Result.Args);
                    await inputSystem.AssignNewCommandAsync(commandModel, inputTask.Result.CommandSource);
                    inputTask = Task.Run(() => IOHandler.GetInputAsync());
                }
                else if (inputTask.IsFaulted)
                {
                    var exception = inputTask.Exception ?? new Exception("REEEEEEEEEEEEEEEEEEE WTF DID YOU DO TO MY POOR ENGINE???");
                    throw exception;
                }
                foreach (var sharperSystem in Systems)
                {
                    await sharperSystem.CycleUpdateAsync(outputDel);
                    var entitiesToDestroy = Entities.Where(x => x.ShouldDestroy).ToList();
                    for (var i = entitiesToDestroy.Count - 1; i > -1; i--)
                    {
                        var entity = entitiesToDestroy[i];
                        await sharperSystem.UnregisterAllComponentsByEntityAsync(entity);
                    }
                }
                await inputSystem.CycleCommandFlushAsync();
                await Task.Delay(DeltaMs);
            }
        }

        private void ComposeSystems()
        {
            foreach (var sharperSystem in Systems)
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
                    systems.Add(Systems.First(x => x.GetType() == parameterInfo.ParameterType));
                }
                method.Invoke(sharperSystem, systems.ToArray());
            }
        }
    }
}
