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
            await ResolveCommandsAsync(await _inputSystem.GetEntitiesByCommandInfoTypesAsync(typeof(GuessCommandInfo))); //you could grab as many command types as you want from this method
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

        private Task ResolveCommandsAsync(Dictionary<SharperEntity, IUniverseCommandInfo> commandData)
        {
            foreach (var inputEntity in commandData.Keys)
            {
                //var componentToUpdate = (RoundComponent)Components.First(); //we can define context here, but since these components currently have no relation to the input entities, we can't apply context just yet
                //ProcessGuess(((GuessCommandInfo)commandData[inputEntity]).Guess);
            }

            return Task.CompletedTask;
        }
    }
}