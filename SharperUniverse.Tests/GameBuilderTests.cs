using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SharperUniverse.Core;
using Moq;
using System;
using SharperUniverse.Tests.Stubs;

namespace SharperUniverse.Tests
{
    [TestFixture]
    public class GameBuilderTests
    {
        private Mock<IIOHandler> _ioHandler;
        private Action<ComposeBuilder> _runQuickGame = b =>
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
            _ioHandler = new Mock<IIOHandler>();

            var tuple = ("foo", new List<string>());
            _ioHandler.Setup(c => c.GetInputAsync()).Returns(Task.FromResult(tuple));
            _ioHandler.Setup(c => c.SendOutputAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        [Test]
        public void CanDoBasicBuilderConstruction()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioHandler.Object)
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
               .AddCommand<EmptyCommandBinding>("a")
               .AddCommand<EmptyCommandBinding>("b")
               .AddCommand<EmptyCommandBinding>("c")
               .AddCommand<EmptyCommandBinding>("d")
               .AddCommand<EmptyCommandBinding>("e");
        }

        [Test]
        public void CanAddMultipleSystems()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioHandler.Object)
                .AddSystem<EmptySystem>()
                .AddSystem<TestSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<EmptyComponent>()
                .ComposeEntities()
                .Build();

            _runQuickGame(builder);
        }

        [Test]
        public void CanAddMultipleEntities()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioHandler.Object)
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
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioHandler.Object)
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
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioHandler.Object)
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
