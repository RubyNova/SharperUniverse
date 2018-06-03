using System;
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
}