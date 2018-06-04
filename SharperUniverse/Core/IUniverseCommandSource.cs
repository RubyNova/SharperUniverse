using System;
using System.Collections.Generic;
using System.Text;

namespace SharperUniverse.Core
{
    public interface IUniverseCommandSource
    {
        bool SourceIsSameAsBindingSource(IUniverseCommandSource bindingSource);
    }
}
