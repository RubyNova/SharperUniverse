using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharperUniverse.Core.Builder
{
    /// <summary>
    /// The builder for <see cref="SharperEntity"/> and <see cref="BaseSharperComponent"/>.
    /// </summary>
    public class EntityBuilder
    {
        private readonly GameRunner _game;
        private readonly List<SharperEntity> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBuilder"/> class.
        /// </summary>
        /// <param name="game">The <see cref="GameRunner"/> being built by this <see cref="GameBuilder"/>.</param>
        internal EntityBuilder(GameRunner game)
        {
            _game = game;
            _entities = new List<SharperEntity>();
        }

        /// <summary>
        /// Add a <see cref="SharperEntity"/> to the Sharper Universe.
        /// </summary>
        /// <returns>An <see cref="EntityBuilder"/>, for adding multiple <see cref="SharperEntity"/> or <see cref="BaseSharperComponent"/> to the Sharper Universe.</returns>
        public EntityBuilder AddEntity()
        {
            _entities.Add(new SharperEntity());
            return this;
        }

        /// <summary>
        /// Attach a <see cref="BaseSharperComponent"/> to the most previously created <see cref="SharperEntity"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="BaseSharperComponent"/> to attach to the previously created <see cref="SharperEntity"/>.</typeparam>
        /// <param name="args">Any parameters that your <see cref="BaseSharperComponent"/> takes in its constructor must be pased through this parameter.</param>
        /// <returns>An <see cref="EntityBuilder"/>, for adding multiple <see cref="SharperEntity"/> or <see cref="BaseSharperComponent"/> to the Sharper Universe.</returns>
        /// <remarks>Even if your <see cref="BaseSharperComponent"/> takes a <see cref="SharperEntity"/> as its first parameter, do not add it to the <paramref name="args"/> array. It is automatically provided for you. Only add parameters beyond the <see cref="SharperEntity"/> to <paramref name="args"/>.</remarks>
        public EntityBuilder WithComponent<T>(params object[] args) where T : BaseSharperComponent
        {
            foreach (var system in _game.Systems)
            {
                var componentToMatch = system.GetType().BaseType.GetGenericArguments().SingleOrDefault(c => c.ToString().Equals(typeof(T).ToString()));
                if (componentToMatch == null) continue;

                // POSSIBLE HACK - MIGHT ACTUALLY WORK OUT 100% OF THE TIME
                // If we're calling this, we're always going to attach it to the most recently created entity (from the builder pattern)
                // It's possible we could maybe improve this by forcing it to always follow the creation of an entity
                // TODO: Possible refactor
                system.RegisterComponentAsync(_entities.Last(), args).GetAwaiter().GetResult();

                break;
            }

            return this;
        }

        /// <summary>
        /// Signals that no more <see cref="SharperEntity"/> or <see cref="BaseSharperComponent"/> will be needed to build this Sharper Universe.
        /// </summary>
        /// <returns>An <see cref="EntityBuilder"/>, for adding multiple <see cref="SharperEntity"/> or <see cref="BaseSharperComponent"/> to the Sharper Universe.</returns>
        public EntityBuilder ComposeEntities()
        {
            // TODO: Probably should add a bool class member to track whether this method has been called or not
            //  If it has been called, then set the member to true, and check its true-ness in the other creation methods in this class
            //  We return an EntityBuilder here for convenience, in-case they don't want to add an OptionsBuilder

            _game.Entities.AddRange(_entities);

            return this;
        }

        public NetworkBuilder SetupNetwork()
        {
            return new NetworkBuilder(_game);
        }
    }
}
