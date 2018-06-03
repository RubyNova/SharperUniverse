using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
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