using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public class GameBuilder
    {
        private GameRunner _game;

        public GameBuilder()
        {
            // Prime our game runner, and add to it as we construct the builder
            _game = new GameRunner();
        }

        public IOHandlerBuilder AddCommand<T>(string name) where T : IUniverseCommandBinding
        {
            var binding = (IUniverseCommandBinding)Activator.CreateInstance(typeof(T), name);
            _game.CommandRunner.AddCommandBinding(binding);

            return new IOHandlerBuilder(_game);
        }
    }

    public class IOHandlerBuilder
    {
        private GameRunner _game;

        public IOHandlerBuilder(GameRunner game)
        {
            _game = game;
        }

        public SystemBuilder AddIOHandler<T>() where T : IIOHandler
        {
            var ioHandler = Activator.CreateInstance<T>();
            return AddIOHandler(ioHandler);
        }

        public SystemBuilder AddIOHandler(IIOHandler handler)
        {
            _game.IOHandler = handler;
            return new SystemBuilder(_game);
        }
    }

    public class SystemBuilder
    {
        private GameRunner _game;
        private readonly List<ConstructorInfo> _systemBuilders;

        public SystemBuilder(GameRunner game)
        {
            _game = game;
            _systemBuilders = new List<ConstructorInfo>();
        }

        public SystemBuilder AddSystem<TSystem>() where TSystem : ISharperSystem<BaseSharperComponent>
        {
            var systemConstructors = typeof(TSystem).GetConstructors();
            foreach (var systemConstructor in systemConstructors)
            {
                foreach (var parameter in systemConstructor.GetParameters())
                {
                    if (parameter.ParameterType != typeof(GameRunner))
                    {
                        // Maybe we want to break; here - not sure... for now just throwing because who knows what's expected
                        throw new InvalidOperationException("Systems must have only one constructor and its parameter must be a GameRunner");
                    }
                }

                _systemBuilders.Add(systemConstructor);
                break;
            }

            return this;
        }

        public EntityBuilder ComposeSystems()
        {
            // Simply invoking this allows the implicit registration under the hood from the system's base
            foreach (var systemBuilder in _systemBuilders)
            {
                systemBuilder.Invoke(new[] { _game });
            }

            return new EntityBuilder(_game);
        }
    }

    public class EntityBuilder
    {
        private GameRunner _game;
        private List<SharperEntity> _entities;

        public EntityBuilder(GameRunner game)
        {
            _game = game;
            _entities = new List<SharperEntity>();
        }

        public EntityBuilder AddEntity()
        {
            _entities.Add(new SharperEntity());
            return this;
        }

        public EntityBuilder WithComponent<T>(params object[] args) where T : BaseSharperComponent
        {
            foreach (var system in _game.Systems)
            {
                var componentToMatch = system.GetType().BaseType.GetGenericArguments().SingleOrDefault(c => c.ToString().Equals(typeof(T).ToString()));
                if (componentToMatch == null) continue;

                // If we're calling this, we're always going to attach it to the most recently created entity (from the builder pattern)
                system.RegisterComponentAsync(_entities.Last(), args).GetAwaiter().GetResult();

                break;
            }

            return this;
        }

        public EntityBuilder ComposeEntities()
        {
            foreach (var entity in _entities)
            {
                _game.Entities.AddRange(_entities);
            }

            return this;
        }

        public OptionsBuilder WithOptions()
        {
            return new OptionsBuilder(_game);
        }

        public ComposeBuilder Build()
        {
            return new ComposeBuilder(_game);
        }
    }

    public class OptionsBuilder
    {
        private GameRunner _game;

        public OptionsBuilder(GameRunner game)
        {
            _game = game;
        }

        public OptionsBuilder SetLoopDelay(int ms)
        {
            _game.DeltaMs = ms;
            return this;
        }

        public ComposeBuilder Build()
        {
            return new ComposeBuilder(_game);
        }
    }

    public class ComposeBuilder
    {
        private GameRunner _game;

        public ComposeBuilder(GameRunner game)
        {
            _game = game;
        }

        public async Task StartGameAsync()
        {
            await _game.RunGameAsync();
        }
    }
}
