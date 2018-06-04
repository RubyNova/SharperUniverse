using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    class SharperInputSystem : BaseSharperSystem<SharperInputComponent>
    {
        public SharperInputSystem(GameRunner game) : base(game)
        {
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            return Task.CompletedTask;
        }

        public Task AssignNewCommand(IUniverseCommandBinding commandBinding, IUniverseCommandSource bindingSource)
        {
            Components.First(x => x.BindingSource.SourceIsSameAsBindingSource(bindingSource)).CurrentCommand =
                commandBinding;
            return Task.CompletedTask;
        }

        public Task<SharperEntity[]> GetEntitiesByCommandBindingTypes(params Type[] commandTypes)
        {
            if (commandTypes.Any(x => !typeof(IUniverseCommandBinding).IsAssignableFrom(x))) throw new SharperTypeViolationException("Unexpected non-command type was found. All types used in GetEntitiesByCommandBindingTypes() must implement IUniverseCommandBinding.");

            return Task.FromResult(Components.Where(x => commandTypes.Contains(x.BindingSource.GetType())).Select(x => x.Entity).ToArray());
        }
    }
}
