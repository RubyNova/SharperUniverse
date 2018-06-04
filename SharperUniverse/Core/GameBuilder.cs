using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The builder for a Sharper Universe <see cref="GameRunner"/>.
    /// </summary>
    public class GameBuilder
    {
        private GameRunner _game;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBuilder"/> class.
        /// </summary>
        public GameBuilder()
        {
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
            _game.CommandBindings.Add(name, typeof(T));

            return this;
        }

        /// <summary>
        /// Add a <see cref="IIOHandler"/> to the <see cref="GameRunner"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIOHandler"/> to add as the input/output handler for this <see cref="GameRunner"/>.</typeparam>
        /// <returns>A <see cref="SystemBuilder"/>, for building the next phase of the Sharper Universe.</returns>
        public SystemBuilder AddIOHandler<T>() where T : IIOHandler
        {
            var ioHandler = Activator.CreateInstance<T>();
            return AddIOHandler(ioHandler);
        }

        /// <summary>
        /// Add a <see cref="IIOHandler"/> to the <see cref="GameRunner"/>.
        /// </summary>
        /// <typeparam name="handler">The <see cref="IIOHandler"/> to add as the input/output handler for this <see cref="GameRunner"/>.</typeparam>
        /// <returns>A <see cref="SystemBuilder"/>, for building the next phase of the Sharper Universe.</returns>
        public SystemBuilder AddIOHandler(IIOHandler handler)
        {
            _game.IOHandler = handler;
            return new SystemBuilder(_game);
        }
    }

    /// <summary>
    /// The builder for <see cref="ISharperSystem{T}"/>.
    /// </summary>
    public class SystemBuilder
    {
        private GameRunner _game;
        private readonly List<ConstructorInfo> _systemBuilders;
        private readonly Dictionary<Type, object> _registeredSystemParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBuilder"/> class.
        /// </summary>
        /// <param name="game">The <see cref="GameRunner"/> being built by this <see cref="GameBuilder"/>.</param>
        internal SystemBuilder(GameRunner game)
        {
            _game = game;
            _systemBuilders = new List<ConstructorInfo>();
            _registeredSystemParameters = new Dictionary<Type, object>
            {
                {typeof(GameRunner), _game }
            };
            _systemBuilders.AddRange(typeof(SharperInputSystem).GetConstructors());
        }

        /// <summary>
        /// Add a <see cref="ISharperSystem{T}"/> to the <see cref="GameRunner"/>.
        /// </summary>
        /// <typeparam name="TSystem">The <see cref="ISharperSystem{T}"/> to add to the <see cref="GameRunner"/>.</typeparam>
        /// <returns>A <see cref="SystemBuilder"/>, for adding multiple sytsems to the Sharper Universe.</returns>
        public SystemBuilder AddSystem<TSystem>() where TSystem : ISharperSystem<BaseSharperComponent>
        {
            var systemConstructors = typeof(TSystem).GetConstructors();
            foreach (var systemConstructor in systemConstructors)
            {
                foreach (var parameter in systemConstructor.GetParameters())
                {
                    if (parameter.ParameterType != typeof(GameRunner) &&
                        !typeof(BaseSharperSystem<>).IsSubclassOfRawGeneric(parameter.ParameterType))
                    {
                        // Maybe we want to `break;` here?
                        //  For now, assume that if it's not a GameRunner or a system, then it's not supported.
                        throw new InvalidOperationException("Systems only support GameRunner and other systems as parameters.");
                    }
                }

                _systemBuilders.Add(systemConstructor);
                break;
            }

            return this;
        }

        /// <summary>
        /// Signals that no more <see cref="ISharperSystem{T}"/> will be needed to build this Sharper Universe.
        /// </summary>
        /// <returns>An <see cref="EntityBuilder"/>, for building the next phase of the Sharper Universe.</returns>
        public EntityBuilder ComposeSystems()
        {
            // Loop through all requested systems - these are the systems that were requested by the builder config
            foreach (var systemBuilder in _systemBuilders)
            {
                List<object> systemParameters = new List<object>();

                // Systems can depend on 2 types, a GameRunner, and another system
                //  Loop through this particular system's constructor's parameters looking for those types
                //  and instantiating them along the way
                foreach (var parameter in systemBuilder.GetParameters())
                {
                    if (_registeredSystemParameters.TryGetValue(parameter.ParameterType, out var parameterInstance))
                    {
                        // We've already registered this type, so simply retrieve its registered instance and use it
                        systemParameters.Add(parameterInstance);
                    }
                    else
                    {
                        // This type hasn't been registered yet, so recursively look through its
                        //  dependencies and grab/register them as needed
                        var composedSystem = ComposeSystem(parameter.ParameterType);
                        if (composedSystem != null)
                        {
                            systemParameters.Add(composedSystem);
                        }
                    }
                }

                // Finally, register this top-level system into this builder's graph.
                _registeredSystemParameters.Add(systemBuilder.DeclaringType, systemBuilder.Invoke(systemParameters.ToArray()));
            }

            return new EntityBuilder(_game);

            // Local method - recursively called to build dependency graph
            object ComposeSystem(Type type)
            {
                // If we're not GameRunner or a system, then get out of here
                //  We may want to consider throwing InvalidOperationException here?
                if (type != typeof(GameRunner) && !typeof(BaseSharperSystem<>).IsSubclassOfRawGeneric(type))
                {
                    return null;
                }

                object instance = null;

                // Recursively look through this dependency, registering any new systems we come across
                //  and re-using systems/gamerunners we've already registered along the way
                foreach (var ctor in type.GetConstructors())
                {
                    List<object> systemParameters = new List<object>();

                    foreach (var parameter in ctor.GetParameters())
                    {
                        if (_registeredSystemParameters.TryGetValue(parameter.ParameterType, out var parameterInstance))
                        {
                            systemParameters.Add(parameterInstance);
                        }
                        else
                        {
                            systemParameters.Add(ComposeSystem(parameter.ParameterType));
                        }
                    }

                    // Instantiates this system's constructor, implicitly registering itself to the GameRunner
                    //  and caching off its instance for use as a parent system's constructor parameter
                    instance = ctor.Invoke(systemParameters.ToArray());
                }

                _registeredSystemParameters.Add(type, instance);
                return instance;
            }
        }
    }

    /// <summary>
    /// The builder for <see cref="SharperEntity"/> and <see cref="BaseSharperComponent"/>.
    /// </summary>
    public class EntityBuilder
    {
        private GameRunner _game;
        private List<SharperEntity> _entities;

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

        /// <summary>
        /// Signals that your Sharper Universe will want to add additional, non-required options to the built <see cref="GameRunner"/>.
        /// </summary>
        /// <returns>An <see cref="OptionsBuilder"/>, for altering optional settings of the Sharper Universe.</returns>
        public OptionsBuilder WithOptions()
        {
            return new OptionsBuilder(_game);
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

    /// <summary>
    /// The provider of options within a <see cref="GameBuilder"/>.
    /// </summary>
    public class OptionsBuilder
    {
        private GameRunner _game;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsBuilder"/> class.
        /// </summary>
        /// <param name="game">The <see cref="GameRunner"/> being built by this <see cref="GameBuilder"/>.</param>
        internal OptionsBuilder(GameRunner game)
        {
            _game = game;
        }

        /// <summary>
        /// Sets the delay between game loops within the Sharper Universe.
        /// </summary>
        /// <param name="ms">The delay between game loops.</param>
        /// <returns>An <see cref="OptionsBuilder"/>, for setting additional options on the <see cref="GameRunner"/>.</returns>
        public OptionsBuilder SetLoopDelay(int ms)
        {
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

    /// <summary>
    /// A final builder exposing the exeuction of a Sharper Universe game.
    /// </summary>
    public class ComposeBuilder
    {
        private GameRunner _game;

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
            await _game.RunGameAsync();
        }
    }
}
