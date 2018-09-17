using System.Threading.Tasks;
using SharperUniverse.Logging;

namespace SharperUniverse.Core.Builder
{
    /// <summary>
    /// A final builder exposing the exeuction of a Sharper Universe game.
    /// </summary>
    public class ComposeBuilder
    {
        private readonly GameRunner _game;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposeBuilder"/> class.
        /// </summary>
        /// <param name="game">The <see cref="GameRunner"/> being built by this <see cref="GameBuilder"/>.</param>
        internal ComposeBuilder(GameRunner game)
        {
            _game = game;
        }

        /// <summary>
        /// Starts a Sharper Universe game.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this <see cref="GameRunner"/> execution.</returns>
        public async Task StartGameAsync()
        {
            ServerLog.LogInfo("Game server now launching...");
	        _game.PeristenceManager?.Connect();
	        _game.Server.Start();
            await _game.RunGameAsync();
        }
    }
}
