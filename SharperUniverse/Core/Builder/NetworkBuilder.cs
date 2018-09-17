using SharperUniverse.Logging;
using SharperUniverse.Networking;

namespace SharperUniverse.Core.Builder
{
    public class NetworkBuilder
    {
        private readonly GameRunner _game;

        public NetworkBuilder(GameRunner game)
        {
            ServerLog.LogInfo("Now composing defined network behaviour...");
            _game = game;
        }

        public OptionsBuilder RegisterServer(ISharperServer sharperServer)
        {
            ServerLog.LogInfo($"Using ISharperServer of type {sharperServer.GetType().FullName} as server mainframe.");
            _game.Server = sharperServer;
            return new OptionsBuilder(_game);
        }

        public OptionsBuilder DefaultServer(int port)
        {
            ServerLog.LogInfo($"Using default telnet ISharperServer type as server mainframe.");
            _game.Server = new Server(port);
            return new OptionsBuilder(_game);
        }
        
        /// <summary>
        /// Signals that your Sharper Universe will want to add additional, non-required options to the built <see cref="GameRunner"/>.
        /// </summary>
        /// <returns>An <see cref="OptionsBuilder"/>, for altering optional settings of the Sharper Universe.</returns>
        public OptionsBuilder WithOptions()
        {
            return new OptionsBuilder(_game);
        }
    }
}
