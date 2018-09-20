using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;
using SharperUniverse.Logging;

namespace SharperUniverse.Core.Builder
{
    /// <summary>
    /// The builder for a Sharper Universe <see cref="GameRunner"/>.
    /// </summary>
    public class GameBuilder
    {
        private readonly GameRunner _game;
        private Dictionary<string, Type> _commandBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBuilder"/> class.
        /// </summary>
        public GameBuilder()
        {
        _commandBindings = new Dictionary<string, Type>();
            ServerLog.LogInfo($"Game server {Assembly.GetCallingAssembly().GetName().Name} is now building...");
            // Prime our game runner, and add to it as we construct the builder
            _game = new GameRunner();
        }

        /// <summary>
        /// Add a <see cref="IUniverseCommandInfo"/> to the <see cref="GameRunner"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IUniverseCommandInfo"/> to add as a command binding.</typeparam>
        /// <param name="name">The name/identifier of this <see cref="IUniverseCommandInfo"/>.</param>
        /// <returns>An <see cref="IOHandlerBuilder"/>, for building the next phase of the Sharper Universe.</returns>
        public GameBuilder AddCommand<T>(string name) where T : IUniverseCommandInfo
        {
            ServerLog.LogInfo($"Adding command type {typeof(T).FullName} with assigned name of \"{name}\"");
            _commandBindings.Add(name, typeof(T));

            return this;
        }

        public SystemBuilder CreateSystem()
        {
            ServerLog.LogInfo("Beginning System composition...");
            return new SystemBuilder(_game, _commandBindings);
        }
    }
}
