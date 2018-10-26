using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public interface IGameRunner
    {
        /// <summary>
        /// Registers an <see cref="ISharperSystem"/> to the Game. This should not be called from external code.
        /// </summary>
        /// <param name="system">The target system to register.</param>
        void RegisterSystem<T>(T system) where T : ISharperSystem;

        /// <summary>
        /// Asynchronously creates a new <see cref="SharperEntity"/> and registers it to the game.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that will contain the new <see cref="SharperEntity"/> on completion.</returns>
        Task<SharperEntity> CreateEntityAsync();

        void StopGame();

        /// <summary>
        /// Launches the Game. This task runs for as long as the game is running.
        /// </summary>
        /// <returns>A <see cref="Task"/> represnting the asynchronous game loop.</returns>
        Task RunGameAsync();
    }
}