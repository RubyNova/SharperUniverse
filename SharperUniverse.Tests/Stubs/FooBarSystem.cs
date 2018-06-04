using System;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
    class FooBarSystem : BaseSharperSystem<FooBarComponent>
    {
        public bool TestSwitch { get; set; }

        private BarSystem _barSystem;
        private readonly FooSystem _foo;
        private readonly TestSystem _test;
        private readonly EmptySystem _empty;

        public FooBarSystem(GameRunner game, FooSystem foo, TestSystem test, EmptySystem empty) : base(game)
        {
            TestSwitch = false;
            _foo = foo;
            _test = test;
            _empty = empty;
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            return Task.CompletedTask;
        }
    }
}
