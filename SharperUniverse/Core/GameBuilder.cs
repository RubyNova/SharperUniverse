using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public class GameBuilder
    {
        public IOHandlerBuilder AddCommand<T>(string name) where T : IUniverseCommandBinding
        {
            var binding = (IUniverseCommandBinding)Activator.CreateInstance(typeof(T), name);
            GameRunner.Instance._commandRunner.AddCommandBinding(binding);

            return new IOHandlerBuilder(this);
        }
    }

    public class IOHandlerBuilder
    {
        private GameBuilder _gameBuilder;

        public IOHandlerBuilder(GameBuilder gameBuilder)
        {
            _gameBuilder = gameBuilder;
        }

        public SystemBuilder AddIOHandler<T>() where T : IIOHandler
        {
            var ioHandler = Activator.CreateInstance<T>();
            return AddIOHandler(ioHandler);
        }

        public SystemBuilder AddIOHandler(IIOHandler handler)
        {
            GameRunner.Instance._ioHandler = handler;
            return new SystemBuilder(_gameBuilder);
        }
    }

    public class SystemBuilder
    {
        private GameBuilder _gameBuilder;
        private readonly List<ConstructorInfo> _systemBuilders;

        public SystemBuilder(GameBuilder gameBuilder)
        {
            _gameBuilder = gameBuilder;
            _systemBuilders = new List<ConstructorInfo>();
        }

        public SystemBuilder AddSystem<TSystem>() where TSystem : ISharperSystem<BaseSharperComponent>
        {
            var systemConstructors = typeof(TSystem).GetConstructors();
            foreach (var systemConstructor in systemConstructors)
            {
                var parameters = systemConstructor.GetParameters();
                List<object> builtParameters = new List<object>();

                foreach (var parameter in parameters)
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
                systemBuilder.Invoke(new[] { GameRunner.Instance });
            }

            return new EntityBuilder(_gameBuilder);
        }
    }

    public class EntityBuilder
    {
        private GameBuilder _gameBuilder;
        private List<SharperEntity> _entities;

        public EntityBuilder(GameBuilder gameBuilder)
        {
            _gameBuilder = gameBuilder;
            _entities = new List<SharperEntity>();
        }

        public SharperEntity AddEntity()
        {
            var entity = new SharperEntity();
            GameRunner.Instance._entities.Add(entity);
            return entity;
        }

        public async Task StartGame()
        {
            await GameRunner.Instance.RunGameAsync();
        }
    }

    public static class SharperEntityExtensions
    {
        public static async Task<SharperEntity> WithComponentAsync<T>(this SharperEntity self, params object[] args) where T : BaseSharperComponent
        {
            foreach (var system in GameRunner.Instance._systems)
            {
                var componentToMatch = system.GetType().BaseType.GetGenericArguments().SingleOrDefault(c => c.ToString().Equals(typeof(T).ToString()));
                if (componentToMatch == null) continue;

                await system.RegisterComponentAsync(self, args);
                break;
            }

            return self;
        }
    }
}
