using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    public class MathGameComponent : BaseSharperComponent
    {
        public SharperEntity ControllingEntity { get; private set; }

        public MathGameComponent(SharperEntity entity, SharperEntity controlingEntity) : base(entity)
        {
            ControllingEntity = controlingEntity;
        }
    }
}
