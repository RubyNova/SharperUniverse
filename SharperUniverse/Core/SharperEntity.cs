using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The type that represents all entites within a game. This class cannot be inherited. It should not be instantiated directly. Instead, call <see cref="GameRunner.CreateEntityAsync"/> to get a new <see cref="SharperEntity"/>.
    /// </summary>
    public sealed class SharperEntity
    {
        /// <summary>
        /// The components attached to this entity.
        /// </summary>
        public List<BaseSharperComponent> Components { get; }


        /// <summary>
        /// Creates a new instance of <see cref="SharperEntity"/>. It should not be instantiated directly. Instead, call <see cref="GameRunner.CreateEntityAsync"/> to get a new <see cref="SharperEntity"/>.
        /// </summary>
        public SharperEntity()
        {
            Components = new List<BaseSharperComponent>();
        }

        /// <summary>
        /// Asynchronously attaches a <see cref="BaseSharperComponent"/> to this entity.
        /// </summary>
        /// <param name="component">The <see cref="BaseSharperComponent"/> to attach.</param>
        /// <returns>Returns a <see cref="Task"/> representing the attachment work.</returns>
        public Task AddComponentAsync(BaseSharperComponent component)
        {
            Components.Add(component);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously removes the specific <see cref="BaseSharperComponent"/> from the entity.
        /// </summary>
        /// <param name="component">The <see cref="BaseSharperComponent"/> to remove.</param>
        /// <returns></returns>
        public Task RemoveComponentAsync(BaseSharperComponent component)
        {
            Components.Remove(component);
            return Task.CompletedTask;
        }
    }
}
