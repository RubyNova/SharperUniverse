using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
    class TestSystem : BaseSharperSystem<TestComponent>
    {
        private readonly Dictionary<TestComponent, bool> _prevStates;
        private float _updateTime;
        private readonly EmptySystem _emptySystem;

        public TestSystem(GameRunner game, EmptySystem emptySystem) : base(game)
        {
            _prevStates = new Dictionary<TestComponent, bool>();
            ComponentRegistered += OnComponentRegistered;
            ComponentUnRegistered += OnComponentUnRegistered;
            _emptySystem = emptySystem;
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            _emptySystem.TestSwitch = true;
            if (_updateTime > 1000f)
            {
                foreach (var comp in Components)
                {
                    comp.State = !comp.State;
                }
                _updateTime = 0f;
            }
            _updateTime += 50f;
            return Task.CompletedTask;
        }

        private void OnComponentUnRegistered(object sender, SharperComponentEventArgs e)
        {
            _prevStates.Remove((TestComponent)e.SharperComponent);
        }

        private void OnComponentRegistered(object sender, SharperComponentEventArgs e)
        {
            var comp = (TestComponent)e.SharperComponent;
            _prevStates.Add(comp, comp.State);
        }
    }
}
