using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests
{
    class TestSystem : BaseSharperSystem<TestComponent>
    {
        private readonly Dictionary<TestComponent, bool> _prevStates;
        private float _updateTime;
        private EmptySystem _system;

        public TestSystem(GameRunner game) : base(game)
        {
            _prevStates = new Dictionary<TestComponent, bool>();
            ComponentAdded += OnComponentAdded;
            ComponentRemoved += OnComponentRemoved;
        }

        [SharperInject]
        private void InjectSharperSystems(EmptySystem emptySys)
        {
            _system = emptySys;
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            _system.TestSwitch = true;
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

        private void OnComponentRemoved(object sender, SharperComponentEventArgs e)
        {
            _prevStates.Remove((TestComponent)e.BaseSharperComponent);
        }

        private void OnComponentAdded(object sender, SharperComponentEventArgs e)
        {
            var comp = (TestComponent)e.BaseSharperComponent;
            _prevStates.Add(comp, comp.State);
        }
    }
}
