using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class PlayerSystem : BaseSharperSystem<PlayerComponent>
    {
        private readonly SharperInputSystem _inputSystem;

        public int PlayerCount => Components.Count;

        public PlayerSystem(GameRunner game, SharperInputSystem inputSystem) : base(game)
        {
            ComponentRegistered += PlayerSystem_ComponentRegistered;
            _inputSystem = inputSystem;
        }

        private void PlayerSystem_ComponentRegistered(object sender, SharperComponentEventArgs e)
        {
            (e.SharperComponent as PlayerComponent).PlayerNumber = PlayerCount;
        }

        public override async Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            var commands = await _inputSystem.GetEntitiesByCommandInfoTypesAsync(typeof(GuessCommandInfo));
            foreach (var command in commands)
            {
                switch (command.Value)
                {
                    case GuessCommandInfo g:
                        var guess = g.Guess;
                        break;
                    default:
                        break;
                }
            }
        }

        public void ResetPlayers()
        {
            Components?.ForEach(c =>
            {
                c.GuessCount = 0;
            });
        }

        public PlayerComponent GetCurrentPlayer(int currentTurn)
        {
            if (currentTurn == 0) return null;

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

            if (PlayerCount == 1 || currentTurn == 1)
            {
                return Components.Single(p => p.PlayerNumber == 1);
            }
            else if (PlayerCount >= currentTurn)
            {
                return Components.Single(p => p.PlayerNumber == currentTurn);
            }
            else
            {
                int modResult = currentTurn % PlayerCount;
                return Components.Single(p => p.PlayerNumber == (modResult == 0 ? PlayerCount : modResult));
            }

            throw new InvalidOperationException("Something horribly wrong happened - who's turn is it?!");
        }
    }
}