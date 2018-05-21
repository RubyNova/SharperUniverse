using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public class SharperEntity
    {
        public List<BaseSharperComponent> Components { get; }

        public SharperEntity()
        {
            Components = new List<BaseSharperComponent>();
        }

        public Task AddComponentAsync(BaseSharperComponent component)
        {
            Components.Add(component);
            return Task.CompletedTask;
        }

        public Task RemoveComponentAsync(BaseSharperComponent component)
        {
            Components.Remove(component);
            return Task.CompletedTask;
        }
    }
}
