using SharperUniverse.Logging;

namespace SharperUniverse.Core.Builder
{

    /// <summary>
    /// The provider of options within a <see cref="GameBuilder"/>.
    /// </summary>
    public class OptionsBuilder
    {
        private readonly GameRunner _game;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsBuilder"/> class.
        /// </summary>
        /// <param name="game">The <see cref="GameRunner"/> being built by this <see cref="GameBuilder"/>.</param>
        internal OptionsBuilder(GameRunner game)
        {
            ServerLog.LogInfo("Adjusting settings based on the following options...");
            _game = game;
        }

        /// <summary>
        /// Sets the delay between game loops within the Sharper Universe.
        /// </summary>
        /// <param name="ms">The delay between game loops.</param>
        /// <returns>An <see cref="OptionsBuilder"/>, for setting additional options on the <see cref="GameRunner"/>.</returns>
        public OptionsBuilder SetLoopDelay(int ms)
        {
            ServerLog.LogInfo($"Setting the delta time of the game loop at {ms} milliseconds.");
            _game.DeltaMs = ms;
            return this;
        }

        /// <summary>
        /// Signals that all composition of a <see cref="GameRunner"/> is complete, and allows the exeuction of the Sharper Universe.
        /// </summary>
        /// <returns>A <see cref="ComposeBuilder"/>, used for finalizing the construction of a <see cref="GameBuilder"/>.</returns>
        public ComposeBuilder Build()
        {
            return new ComposeBuilder(_game);
        }
    }
}
