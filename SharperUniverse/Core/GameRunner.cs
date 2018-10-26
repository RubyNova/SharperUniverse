using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharperUniverse.Logging;
using SharperUniverse.Networking;

namespace SharperUniverse.Core
{
    /// <summary>
    /// This is the entry point and main manager for any game that uses SharperUniverse.
    /// </summary>
    public class GameRunner : IGameRunner
    {
        internal List<ISharperSystem> Systems{ get; set; }
        internal List<SharperEntity> Entities { get; set; }
        internal ISharperServer Server { get; set; }
        internal int DeltaMs { get; set; }

        private readonly CancellationTokenSource _cancellationTokenSource;

        internal GameRunner()
        {
            Systems = new List<ISharperSystem>();
            Entities = new List<SharperEntity>();
            DeltaMs = 50;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Registers an <see cref="ISharperSystem{T}"/> to the Game. This should not be called from external code.
        /// </summary>
        /// <param name="system">The target system to register.</param>
        public void RegisterSystem<T>(T system) where T : ISharperSystem
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

        public void StopGame()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Launches the Game. This task runs for as long as the game is running.
        /// </summary>
        /// <returns>A <see cref="Task"/> represnting the asynchronous game loop.</returns>
        public async Task RunGameAsync()
        {
            var inputSystem = (SharperInputSystem)Systems.First(x => x is SharperInputSystem);
            Server.NewConnectionMade += inputSystem.OnNewInputConnectionAsync;
            ServerLog.LogInfo("Server launch successful.");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                foreach (var sharperSystem in Systems)
                {
                    await sharperSystem.CycleUpdateAsync(DeltaMs);
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
    }
}
