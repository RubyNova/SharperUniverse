using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new GameBuilder()
                .AddCommand<GuessCommandBinding>("g")
                .AddIOHandler<ConsoleIOHandler>()
                .AddSystem<PlayerSystem>()
                .AddSystem<RoundSystem>()
                .ComposeSystems()
                .AddEntity().WithComponent<PlayerComponent>().WithComponent<PlayerComponent>()
                .AddEntity().WithComponent<RoundComponent>()
                .ComposeEntities().Build();

            await builder.StartGameAsync();
        }
    }

    public class RoundComponent : BaseSharperComponent
    {
        public enum RoundState
        {
            Start,
            NewTurn,
            Input,
            Finish
        }

        public int Answer { get; set; }

        public RoundState State { get; set; }

        public int RoundNumber { get; set; }

        public RoundComponent(SharperEntity entity)
        {
            Entity = entity;
            State = RoundState.Start;
        }
    }

    public class RoundSystem : BaseSharperSystem<RoundComponent>
    {
        static Random _random = new Random();

        const int AnswerLowerBound = 1;
        const int AnswerUpperBound = 5;

        private int _currentTurn;

        private RoundComponent _roundComponent;
        private PlayerSystem _playerSystem;

        public RoundSystem(GameRunner game) : base(game)
        {
            ComponentRegistered += RoundSystem_ComponentRegistered;
        }

        [SharperInject]
        private void InitializeCommandRequirements(PlayerSystem playerSystem)
        {
            _playerSystem = playerSystem;
        }

        private void RoundSystem_ComponentRegistered(object sender, SharperComponentEventArgs e)
        {
            if (_roundComponent == null)
            {
                _roundComponent = e.SharperComponent as RoundComponent;
            }
            else
            {
                // this is a single instance component system - destroy any components registered beyond the first
                UnregisterComponentAsync(e.SharperComponent as RoundComponent);
            }
        }

        // our iohandler is the console, so don't worry about performance hit on multiple repeated output invokes
        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            if (_roundComponent != null)
            {
                switch (_roundComponent.State)
                {
                    case RoundComponent.RoundState.Start:
                        PrimeNewRound();

                        _roundComponent.Answer = _random.Next(AnswerLowerBound, AnswerUpperBound + 1);
                        outputHandler.Invoke($"Begin round {_roundComponent.RoundNumber} with {_playerSystem.PlayerCount} participants");
                        break;
                    case RoundComponent.RoundState.NewTurn:
                        _currentTurn += 1;
                        outputHandler.Invoke($"\nPlayer {GetCurrentPlayer()}'s turn");
                        outputHandler.Invoke($"Guess a number between 1 and {AnswerUpperBound}");
                        _roundComponent.State = RoundComponent.RoundState.Input;
                        break;
                    case RoundComponent.RoundState.Finish:
                        outputHandler.Invoke($"\n> Player {GetCurrentPlayer()} wins! - It took them {GetCurrentPlayer()?.GuessCount} guesses\n");
                        _roundComponent.State = RoundComponent.RoundState.Start;
                        break;
                }
            }

            return Task.CompletedTask;
        }

        public PlayerComponent GetCurrentPlayer()
        {
            if (_playerSystem != null)
            {
                /*
                 * Figures out who's turn it is
                 *  if playercount or currentturn is 1, then return player1
                 *  if playercount is >= currentturn, then return player{currentturn}
                 *  otherwise, modulus the currentturn and the playercount to get the current player
                 *  
                 *  ie:
                 *   playercount = 1
                 *   currentturn = 1
                 *    > 1
                 *   
                 *   playercount = 3
                 *   currentturn = 2
                 *    > 2
                 *   
                 *   playercount = 2
                 *   currentturn = 5
                 *    > 5 % 2 = 1
                 */
                if (_playerSystem.PlayerCount == 1 || _currentTurn == 1)
                {
                    return _playerSystem.Components.Single(p => p.PlayerNumber == 1);
                }
                else if (_playerSystem.PlayerCount >= _currentTurn)
                {
                    return _playerSystem.Components.Single(p => p.PlayerNumber == _currentTurn);
                }
                else
                {
                    int modResult = _currentTurn % _playerSystem.PlayerCount;
                    return _playerSystem.Components.Single(p => p.PlayerNumber == (modResult == 0 ? _playerSystem.PlayerCount : modResult));
                }
            }

            throw new InvalidOperationException("Something horribly wrong happened - who's turn is it?!");
        }

        private void PrimeNewRound()
        {
            _roundComponent.RoundNumber += 1;
            _currentTurn = 0;
            _playerSystem.Components.ForEach(p => p.GuessCount = 0);
            _roundComponent.State = RoundComponent.RoundState.NewTurn;
        }
    }

    public class PlayerComponent : BaseSharperComponent
    {
        public int PlayerNumber { get; set; }
        public int Score { get; set; }
        public int GuessCount { get; set; }

        public PlayerComponent(SharperEntity entity)
        {
            Entity = entity;
            Score = 0;
            GuessCount = 0;
        }

        public override string ToString()
        {
            return PlayerNumber.ToString();
        }
    }

    public class PlayerSystem : BaseSharperSystem<PlayerComponent>
    {
        public int PlayerCount => Components.Count;

        public PlayerSystem(GameRunner game) : base(game)
        {
            ComponentRegistered += PlayerSystem_ComponentRegistered;
        }

        private void PlayerSystem_ComponentRegistered(object sender, SharperComponentEventArgs e)
        {
            (e.SharperComponent as PlayerComponent).PlayerNumber = PlayerCount;
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            return Task.CompletedTask;
        }
    }

    public class ConsoleIOHandler : IIOHandler
    {
        public Task<(string commandName, List<string> args)> GetInputAsync()
        {
            var input = Console.In.ReadLine();
            var split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length >= 2)
            {
                return Task.FromResult((split[0], new List<string> { split[1] }));
            }

            return Task.FromResult((string.Empty, new List<string>()));
        }

        public Task SendOutputAsync(string outputText)
        {
            Console.Out.WriteLine(outputText);
            return Task.CompletedTask;
        }
    }

    public class GuessCommandBinding : IUniverseCommandBinding
    {
        private IIOHandler _ioHandler;
        private RoundSystem _roundSystem;
        private PlayerSystem _playerSystem;

        public string CommandName { get; }

        public GuessCommandBinding(string commandName)
        {
            CommandName = commandName;
        }

        [SharperInject]
        private void InitializeCommandRequirements(IIOHandler ioHandler, RoundSystem roundSystem, PlayerSystem playerSystem)
        {
            _ioHandler = ioHandler;
            _roundSystem = roundSystem;
            _playerSystem = playerSystem;
        }

        public Task ProcessCommandAsync(List<string> args)
        {
            if (args.Any())
            {
                var roundComponent = _roundSystem.Components.Single();
                var playerComponent = _roundSystem.GetCurrentPlayer();

                if (int.TryParse(args[0], out int guess) && playerComponent != null && roundComponent != null)
                {
                    playerComponent.GuessCount += 1;

                    if (guess == roundComponent.Answer)
                    {
                        playerComponent.Score += 1;
                        roundComponent.State = RoundComponent.RoundState.Finish;
                    }
                    else
                    {
                        _ioHandler.SendOutputAsync($"{guess} is too {(guess > roundComponent.Answer ? "high" : "low")}");
                        roundComponent.State = RoundComponent.RoundState.NewTurn;
                    }
                }
            }
            else
            {
                _ioHandler.SendOutputAsync("You forgot to guess a number");
            }

            return Task.CompletedTask;
        }
    }
}