using SharperUniverse.Networking;

namespace SharperUniverse.Core
{
    public class SharperInputComponent : BaseSharperComponent
    {
        public ISharperConnection BindingSource { get; }
        public IUniverseCommandInfo CurrentCommand { get; set; }

        public SharperInputComponent(SharperEntity entity, ISharperConnection bindingSource) : base(entity)
        {
            BindingSource = bindingSource;
        }

    }
}
