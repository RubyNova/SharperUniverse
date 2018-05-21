using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (_systems.Contains(system)) throw new DuplicateSharperObjectException();

            _systems.Add(system);
        }

        public void RegisterSystems(params ISharperSystem<BaseSharperComponent>[] systems)
        {
            if (_systems.Any(x => systems.Contains(x))) throw new DuplicateSharperObjectException();

            _systems.AddRange(systems);
        }

        public Task<SharperEntity> CreateEntityAsync()
        {
            var ent = new SharperEntity();
            _entities.Add(ent);
            return Task.FromResult(ent); //I have no idea if this is ok LUL
        }

        public async Task RunGameAsync()
        {
            Task<(string commandName, List<string> args)> inputTask = _ioHandler.GetInputAsync(); //debugger stops working here, and nothing gets output when example command is ran
            Func<string, Task> outputDel = _ioHandler.SendOutputAsync;
            while (true)
            {
                if (inputTask.IsCompleted)
                {
                    await _commandRunner.AttemptExecuteAsync(inputTask.Result.commandName, inputTask.Result.args);
                    inputTask = _ioHandler.GetInputAsync();
                }
                else if (inputTask.IsFaulted)
                {
                    throw inputTask.Exception;
                }
                foreach (var sharperSystem in _systems)
                {
                    await sharperSystem.CycleUpdateAsync(outputDel);
                }
                await Task.Delay(100);
            }
        }
    }
}
