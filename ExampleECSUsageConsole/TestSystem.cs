using System;
using System.Collections.Generic;
using System.Text;
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
            ComponentAdded += OnComponentAdded;
            ComponentRemoved += OnComponentRemoved;
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
