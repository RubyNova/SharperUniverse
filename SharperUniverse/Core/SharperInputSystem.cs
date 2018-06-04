using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public class SharperInputSystem : BaseSharperSystem<SharperInputComponent>
    {
        public event EventHandler<SharperEntityEventArgs> NewInputEntityCreated;
        public event EventHandler<SharperEntityEventArgs> InputEntityDestroyed;

        public SharperInputSystem(GameRunner game) : base(game)
        {
            ComponentUnRegistered += OnInputComponentUnRegistered;
        }

        private void OnInputComponentUnRegistered(object sender, SharperComponentEventArgs e)
        {
            InputEntityDestroyed?.Invoke(this, new SharperEntityEventArgs(e.SharperComponent.Entity));
        }

        public override Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            return Task.CompletedTask;
        }

        public async Task AssignNewCommandAsync(IUniverseCommandInfo commandInfo, IUniverseCommandSource bindingSource)
        {
            var inputComponent = Components.FirstOrDefault(x => x.BindingSource.SourceIsSameAsBindingSource(bindingSource));

            if (inputComponent == null)
            {
                var newEntity = await Game.CreateEntityAsync();
                NewInputEntityCreated?.Invoke(this, new SharperEntityEventArgs(newEntity));
                await RegisterComponentAsync(newEntity, bindingSource);
                inputComponent = Components.First(x => x.BindingSource.SourceIsSameAsBindingSource(bindingSource));
            }

            inputComponent.CurrentCommand = commandInfo;
        }

        public Task<Dictionary<SharperEntity, IUniverseCommandInfo>> GetEntitiesByCommandInfoTypesAsync(params Type[] commandTypes)
        {
            if (commandTypes.Any(x => !typeof(IUniverseCommandInfo).IsAssignableFrom(x))) throw new SharperTypeViolationException("Unexpected non-command type was found. All types used in GetEntitiesByCommandInfoTypesAsync() must implement IUniverseCommandInfo.");
            var componentsResult = Components.Where(x => x.CurrentCommand != null && commandTypes.Contains(x.CurrentCommand.GetType())).ToList();
            return Task.FromResult(componentsResult.ToDictionary(x => x.Entity, y => y.CurrentCommand));
        }

        public Task CycleCommandFlushAsync()
        {
            foreach (var inputComponent in Components)
            {
                inputComponent.CurrentCommand = null;
            }
            return Task.CompletedTask;
        }
    }
}
