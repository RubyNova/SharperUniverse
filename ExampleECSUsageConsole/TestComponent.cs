using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    public class TestComponent : BaseSharperComponent
    {
        public bool State { get; set; }
        public SharperEntity OwnerEntity { get; }

        public TestComponent(SharperEntity entity, bool startState, SharperEntity ownerEntity) : base(entity)
        {
            State = startState;
            OwnerEntity = ownerEntity;
        }
    }
}
