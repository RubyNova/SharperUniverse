using System;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
    class EmptySystem : BaseSharperSystem<EmptyComponent>
    {
        public bool TestSwitch { get; set; }

        public EmptySystem(GameRunner game) : base(game)
        {
            TestSwitch = false;
        }

        public override Task CycleUpdateAsync(int deltaMs)
        {
            return Task.CompletedTask;
        }
    }
}
