using System;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
    class FooSystem : BaseSharperSystem<FooComponent>
    {
        public bool TestSwitch { get; set; }

        private BarSystem _barSystem;

        public FooSystem(GameRunner game, BarSystem barSystem) : base(game)
        {
            TestSwitch = false;
            _barSystem = barSystem;
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            return Task.CompletedTask;
        }
    }
}
