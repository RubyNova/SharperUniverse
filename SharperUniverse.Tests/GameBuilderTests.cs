using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSubstitute;
using NUnit.Framework;
using SharperUniverse.Core;
using SharperUniverse.Core.Builder;
using SharperUniverse.Networking;
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

        [SetUp]
        public void SetUp()
        {
            _ioHandler = Substitute.For<IIOHandler>();

            var tuple = ("test", new List<string>(), new TestCommandSource());
            _ioHandler.GetInputAsync().Returns(Task.FromResult(tuple));
            _ioHandler.SendOutputAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        }

        [TearDown]
        public void TearDown()
        {
            _ioHandler = null;
        }

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
                .Build();

            _runQuickGame(builder);
        }
    }
}
