using System;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
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
}