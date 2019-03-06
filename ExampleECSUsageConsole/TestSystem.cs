using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestSystem : BaseSharperSystem<TestComponent>
    {
        private readonly Dictionary<TestComponent, bool> _prevStates;
        private SharperInputSystem _inputSystem;

        public TestSystem(IGameRunner game, SharperInputSystem inputSystem) : base(game)
        {
            _prevStates = new Dictionary<TestComponent, bool>();
            _inputSystem = inputSystem;
            _inputSystem.NewInputEntityCreated += OnNewInputEntity;
            ComponentRegistered += OnComponentRegistered;
            ComponentUnRegistered += OnComponentUnRegistered;
        }

        private void OnNewInputEntity(object sender, SharperEntityEventArgs e)
        {
            var entity = new TestComponent(Game.CreateEntityAsync().GetAwaiter().GetResult(), false, e.Entity);
            RegisterComponentAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task CycleUpdateAsync(int deltaMs)
        {
            await ResolveCommandsAsync(await _inputSystem.GetEntitiesByCommandInfoTypesAsync(typeof(TestCommandInfo)));
            foreach (var comp in Components)
            {
                if (comp.State !=_prevStates[comp])
                {
                    var connection = _inputSystem.GetConnectionByEntity(comp.OwnerEntity);
                    connection.Send($"State is now {comp.State}");
                }
                _prevStates[comp] = comp.State;
            }
        }

        private Task ResolveCommandsAsync(Dictionary<SharperEntity, IUniverseCommandInfo> commandData)
        {
            foreach (var inputEntity in commandData.Keys)
            {
                Components.First(x => x.OwnerEntity == inputEntity).State = ((TestCommandInfo) commandData[inputEntity]).NewState;
            }

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
