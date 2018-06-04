using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SharperUniverse.Core;
using SharperUniverse.Tests.Stubs;

namespace SharperUniverse.Tests
{
    public class ECSTests
    {
        private IIOHandler _ioMock;
        private UniverseCommandRunner _commandRunner;
        private string _uiOutput;
        private bool _inputEntered;
        private GameRunner _gameRunner;

        [SetUp]
        public void SetupTestGame()
        {
            _ioMock = Substitute.For<IIOHandler>();

            _ioMock.SendOutputAsync(Arg.Any<string>()).Returns(Task.CompletedTask)
                .AndDoes(x => _uiOutput = (string)x[0]);

            _ioMock.GetInputAsync().Returns(Task.FromResult(("say", new List<string> { "hello" }))).AndDoes(async x =>
               {
                   while (!_inputEntered)
                   {
                       await Task.Delay(500);
                   }
               });

            _commandRunner = new UniverseCommandRunner();
            _gameRunner = new GameRunner(_commandRunner, _ioMock, 50);
        }

        [TearDown]
        public void Reset()
        {
            _ioMock = null;
            _commandRunner = null;
            _gameRunner = null;
            _uiOutput = string.Empty;
            _inputEntered = false;
        }

        [Test]
        public void CanRunEmptyGame()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var theTask = Task.Run(async () => await _gameRunner.RunGameAsync());
                await Task.Delay(3000);
                if (theTask.IsFaulted)
                {
                    throw theTask.Exception;
                }
            });
        }

        [Test]
        public async Task CanRunGameWithOneTestCommandThatOutputsToUI()
        {
            var command = Substitute.For<IUniverseCommandBinding>();
            command.CommandName.Returns("say");
            command.ProcessArgsAsync(Arg.Any<List<string>>()).Returns(Task.CompletedTask)
                .AndDoes(x => _ioMock.SendOutputAsync(x.Arg<List<string>>()[0]));
            _commandRunner.AddCommandBinding(command);
            var gameRunTask = Task.Run(async () => await _gameRunner.RunGameAsync());
            _inputEntered = true;
            await Task.Delay(2000);
            Assert.AreEqual("hello", _uiOutput);
        }

        [Test]
        public async Task CanRunGameWithOneComponentAndSystem()
        {
            var entity = await _gameRunner.CreateEntityAsync();
            var system = new TestSystem(_gameRunner);
            new EmptySystem(_gameRunner);
            await system.RegisterComponentAsync(entity, false);
            //_gameRunner.RegisterSystem(system);
            var gameRunTask = _gameRunner.RunGameAsync();
            await Task.Delay(1500);
            Assert.IsTrue(system.Components[0].State);
        }

        [Test]
        public async Task CanRunSystemWithMultipleComponents()
        {
            var entity = await _gameRunner.CreateEntityAsync();
            new EmptySystem(_gameRunner);
            var system = new TestSystem(_gameRunner);
            await system.RegisterComponentAsync(entity, false);
            await system.RegisterComponentAsync(entity, false);
            await system.RegisterComponentAsync(entity, false);
            await system.RegisterComponentAsync(entity, false);
            await system.RegisterComponentAsync(entity, false);
            //_gameRunner.RegisterSystem(system);
            var gameRunTask = _gameRunner.RunGameAsync();
            await Task.Delay(1500);
            foreach (var systemComponent in system.Components)
            {
                Assert.IsTrue(systemComponent.State);
            }
        }

        [Test]
        public void CannotAddTwoSystemsOfTheSameType()
        {
            Assert.Throws<DuplicateSharperObjectException>(() =>
            {
                new TestSystem(_gameRunner);
                new TestSystem(_gameRunner);
            });
        }

        [Test]
        public void CanRunGameWithMultipleSystems()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioMock)
                .AddSystem<TestSystem>()
                .AddSystem<EmptySystem>()
                .ComposeSystems()
                .Build();

            Assert.DoesNotThrowAsync(async () =>
            {
                Task.Run(() => builder.StartGameAsync());
                await Task.Delay(3000);
            });
        }

        [Test]
        public async Task CanRunWithSystemsCallingEachOther()
        {
            var builder = new GameBuilder()
                .AddCommand<EmptyCommandBinding>("")
                .AddIOHandler(_ioMock)
                .AddSystem<TestSystem>()
                .AddSystem<EmptySystem>()
                .ComposeSystems();


            var sysOne = new TestSystem(_gameRunner);
            var sysTwo = new EmptySystem(_gameRunner);
            var gameTask = _gameRunner.RunGameAsync();
            await Task.Delay(2000);
            Assert.IsTrue(sysTwo.TestSwitch);
        }
    }
}