using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestSystem : BaseSharperSystem<TestComponent>
    {
        private readonly Dictionary<TestComponent, bool> _prevStates;

        public TestSystem(GameRunner game) : base(game)
        {
            _prevStates = new Dictionary<TestComponent, bool>();
            ComponentRegistered += OnComponentRegistered;
            ComponentUnRegistered += OnComponentUnRegistered;
        }

        public override async Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            foreach (var comp in Components)
            {
                if (comp.State !=_prevStates[comp])
                {
                    await outputHandler.Invoke($"state is now {comp.State}");
                }
                _prevStates[comp] = comp.State;
            }
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
