using System;
using System.Collections.Generic;
using System.Text;

namespace SharperUniverse.Core
{
    public class SharperInputComponent : BaseSharperComponent
    {
        public IUniverseCommandSource BindingSource { get; }
        public IUniverseCommandInfo CurrentCommand { get; set; }

        public SharperInputComponent(SharperEntity entity, IUniverseCommandSource bindingSource) : base(entity)
        {
            BindingSource = bindingSource;
        }

    }
}
