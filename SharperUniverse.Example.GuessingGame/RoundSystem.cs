using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class RoundSystem : BaseSharperSystem<RoundComponent>
    {
        static Random _random = new Random();

        const int AnswerLowerBound = 1;
        const int AnswerUpperBound = 5;

        private int _currentTurn;

        private RoundComponent _roundComponent;
        private PlayerSystem _playerSystem;
        //private readonly SharperInputSystem _inputSystem;
        private PlayerComponent _currentPlayer;

        public RoundSystem(GameRunner game, PlayerSystem playerSystem) : base(game)
        {
            _playerSystem = playerSystem;
            ComponentRegistered += RoundSystem_ComponentRegistered;
        }

        private void OnNewInputEntity(object sender, SharperEntityEventArgs e)
        {
            RegisterComponentAsync(Game.CreateEntityAsync().GetAwaiter().GetResult()).GetAwaiter().GetResult();
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

        internal void ProcessGuess(int guess)
        {
            _currentPlayer.GuessCount += 1;

            if (guess == _roundComponent.Answer)
            {
                _currentPlayer.Score += 1;
                _roundComponent.State = RoundComponent.RoundState.Finish;
            }
            else
            {
                //_ioHandler.SendOutputAsync($"{guess} is too {(guess > roundComponent.Answer ? "high" : "low")}");
                _roundComponent.State = RoundComponent.RoundState.NewTurn;
            }
        }

        // our iohandler is the console, so don't worry about performance hit on multiple repeated output invokes
        public override async Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            if (_roundComponent != null)
            {
                // Do I need this?
                //await ResolveCommandsAsync(await _inputSystem.GetEntitiesByCommandInfoTypesAsync(typeof(GuessCommandInfo))); //you could grab as many command types as you want from this method
                
                // I need to get the Guess from the Command Info here and do something with it

                _currentPlayer = _playerSystem.GetCurrentPlayer(_currentTurn);

                switch (_roundComponent.State)
                {
                    case RoundComponent.RoundState.Start:
                        PrimeNewRound();

                        _roundComponent.Answer = _random.Next(AnswerLowerBound, AnswerUpperBound + 1);
                        await outputHandler.Invoke($"Begin round {_roundComponent.RoundNumber} with {_playerSystem.PlayerCount} participants");
                        break;
                    case RoundComponent.RoundState.NewTurn:
                        _currentTurn += 1;
                        await outputHandler.Invoke($"\nPlayer {_currentPlayer}'s turn");
                        await outputHandler.Invoke($"Guess a number between 1 and {AnswerUpperBound}");
                        _roundComponent.State = RoundComponent.RoundState.Input;
                        break;
                    case RoundComponent.RoundState.Finish:
                        await outputHandler.Invoke($"\n> Player {_currentPlayer} wins! - It took them {_currentPlayer?.GuessCount} guesses\n");
                        _roundComponent.State = RoundComponent.RoundState.Start;
                        break;
                }
            }
        }

        private Task ResolveCommandsAsync(Dictionary<SharperEntity, IUniverseCommandInfo> commandData)
        {
            foreach (var inputEntity in commandData.Keys)
            {
                //var componentToUpdate = (RoundComponent)Components.First(); //we can define context here, but since these components currently have no relation to the input entities, we can't apply context just yet
                ProcessGuess(((GuessCommandInfo)commandData[inputEntity]).Guess);
            }

            return Task.CompletedTask;
        }

        private void PrimeNewRound()
        {
            _roundComponent.RoundNumber += 1;
            _currentTurn = 0;
            _playerSystem.ResetPlayers();
            _roundComponent.State = RoundComponent.RoundState.NewTurn;
        }
    }
}