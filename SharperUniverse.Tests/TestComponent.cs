using SharperUniverse.Core;

namespace SharperUniverse.Tests
{
    class TestComponent : BaseSharperComponent
    {
        public bool State { get; set; }

        public TestComponent(SharperEntity entity, bool startState) : base(entity)
        {
            State = startState;
        }
    }
}
