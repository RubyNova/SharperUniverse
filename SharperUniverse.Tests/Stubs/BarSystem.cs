using System;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
    class BarSystem : BaseSharperSystem<BarComponent>
    {
        public bool TestSwitch { get; set; }

        public BarSystem(IGameRunner game) : base(game)
        {
            TestSwitch = false;
        }

        public override Task CycleUpdateAsync(int deltaMs)
        {
            return Task.CompletedTask;
        }
    }
}
