using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SharperUniverse.Core;
using SharperUniverse.Core.Builder;
using SharperUniverse.Persistence;
using SharperUniverse.Tests.Stubs;

namespace SharperUniverse.Tests
{
    [TestFixture]
    public class GameBuilderTests
    {
        private readonly Action<ComposeBuilder> _runQuickGame = b =>
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var theTask = Task.Run(async () => await b.StartGameAsync());
                await Task.Delay(3000);
                if (theTask.IsFaulted)
                {
                    throw theTask.Exception;
                }
            });
        };

        [Test]
        public void CanDoBasicBuilderConstruction()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandInfo>("test")
                .CreateSystem()
                .AddSystem<EmptySystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<EmptyComponent>()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(4000)
                .Build();

            _runQuickGame(builder);
        }

        [Test]
        public void CanAddMultipleCommands()
        {
            var builder = new GameBuilder()
               .AddCommand<EmptyCommandInfo>("a")
               .AddCommand<EmptyCommandInfo>("b")
               .AddCommand<EmptyCommandInfo>("c")
               .AddCommand<EmptyCommandInfo>("d")
               .AddCommand<EmptyCommandInfo>("e");
        }

        [Test]
        public void CanAddMultipleSystems()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandInfo>("test")
                .CreateSystem()
                .AddSystem<EmptySystem>()
                .AddSystem<TestSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<EmptyComponent>()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(4000)
                .Build();

            _runQuickGame(builder);
        }

        [Test]
        public void CanAddMultipleEntities()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandInfo>("test")
                .CreateSystem()
                .AddSystem<EmptySystem>()
                .ComposeSystems()
                .AddEntity()
                .AddEntity()
                .AddEntity()
                .AddEntity()
                .AddEntity()
                .AddEntity()
                .AddEntity()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(4000)
                .Build();

            _runQuickGame(builder);
        }

        [Test]
        public void CanAddMultipleComponentsToAnEntity()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandInfo>("test")
                .CreateSystem()
                .AddSystem<EmptySystem>()
                .AddSystem<TestSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<TestComponent>(true).WithComponent<EmptyComponent>()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(4000)
                .Build();

            _runQuickGame(builder);
        }

        [Test]
        public void CanBuildThreeLevelSystemGraph()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandInfo>("test")
                .CreateSystem()
                .AddSystem<EmptySystem>()
                .AddSystem<TestSystem>()
                .AddSystem<FooBarSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<EmptyComponent>()
                .AddEntity().WithComponent<FooBarComponent>()
                .AddEntity().WithComponent<FooComponent>()
                .ComposeEntities()
                .SetupNetwork()
                .DefaultServer(4000)
	            .WithPersistence<LiteDBProvider>()
	            .WithConnectionString($"{Path.GetTempPath()}\\SU_test.db")
                .Build();

            _runQuickGame(builder);
        }
    }
}
