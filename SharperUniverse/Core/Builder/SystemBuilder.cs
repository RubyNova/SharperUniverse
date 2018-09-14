using System;
using System.Collections.Generic;
using System.Reflection;
using SharperUniverse.Logging;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core.Builder
{
    /// <summary>
    /// The builder for <see cref="ISharperSystem{T}"/>.
    /// </summary>
    public class SystemBuilder
    {
        private readonly GameRunner _game;
        private readonly List<ConstructorInfo> _systemBuilders;
        private readonly Dictionary<Type, object> _registeredSystemParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBuilder"/> class.
        /// </summary>
        /// <param name="game">The <see cref="GameRunner"/> being built by this <see cref="GameBuilder"/>.</param>
        internal SystemBuilder(GameRunner game)
        {
            ServerLog.LogInfo("Now attempting to compose ISharperSystem types...");
            _game = game;
            _systemBuilders = new List<ConstructorInfo>();
            _registeredSystemParameters = new Dictionary<Type, object>
            {
                {typeof(GameRunner), _game}
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
            ServerLog.LogInfo($"Attaching system of type {typeof(TSystem).FullName}.");
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
                        throw new InvalidOperationException(
                            "Systems only support GameRunner and other systems as parameters.");
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
                _registeredSystemParameters.Add(systemBuilder.DeclaringType,
                    systemBuilder.Invoke(systemParameters.ToArray()));
            }
            
            ServerLog.LogInfo("Systems attached.");

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
}
