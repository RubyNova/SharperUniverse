using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    /// <summary>
    /// The base type for all System types. You should inherit this type if you are developing a System type.
    /// </summary>
    /// <typeparam name="T">The Component type this System manages.</typeparam>
    public abstract class BaseSharperSystem<T> : ISharperSystem<T> where T : BaseSharperComponent
    {
        /// <summary>
        /// The instance of <see cref="GameRunner"/> this System is registered to.
        /// </summary>
        protected readonly IGameRunner Game;

        /// <summary>
        /// Fires whenever a component is registered in the system.
        /// </summary>
        protected event EventHandler<SharperComponentEventArgs> ComponentRegistered;

        /// <summary>
        /// Fires whenever a component is removed from the system.
        /// </summary>
        protected event EventHandler<SharperComponentEventArgs> ComponentUnRegistered;

        /// <summary>
        /// All the Components of the generic type currently in existence.
        /// </summary>
        protected List<T> Components { get; }

        /// <summary>
        /// Base constructor for any <see cref="BaseSharperSystem{T}"/>. This must be called for the system to function correctly. Do not pass <see cref="ISharperSystem{T}"/> types in an overriden constructor. Instead, use <see cref="SharperInjectAttribute"/>.
        /// </summary>
        /// <param name="game">The current GameRunner this system should be registered to.</param>
        protected BaseSharperSystem(IGameRunner game)
        {
            Components = new List<T>();
            game.RegisterSystem(this);
            Game = game;
        }

        /// <summary>
        /// The logic that runs in an <see cref="ISharperSystem{T}"/> once per update cycle. The update cycle is defined by the <see cref="GameRunner"/> on instantiation.
        /// </summary>
        /// <param name="deltaMs"></param>
        /// <returns></returns>
        public abstract Task CycleUpdateAsync(int deltaMs);

        public Task RegisterComponentAsync(BaseSharperComponent component)
        {
            if (component.GetType() != typeof(T))
            {
                throw new SharperComponentMismatchException();
            }
            
            Components.Add((T)component);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers a component of type <typeparamref name="T"/> and assigns it to the target <see cref="SharperEntity"/>.
        /// </summary>
        /// <param name="component">The instantiated component to register.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the registration work.</returns>
        public Task RegisterComponentAsync(T component)
        {

            Components.Add(component);

            ComponentRegistered?.Invoke(this, new SharperComponentEventArgs(component));
            return Task.CompletedTask;
        }

	    public Task RegisterComponentsAsync(params BaseSharperComponent[] components)
	    {
		    foreach (var component in components)
		    {
				Components.Add((T)component);
			    ComponentRegistered?.Invoke(this, new SharperComponentEventArgs(component));   
		    }
		    return Task.CompletedTask;
	    }

        public Task<bool> EntityHasComponentOfManagingTypeAsync(SharperEntity entity)
        {
            return Task.FromResult(Components.Any(x => x.Entity == entity));
        }

	    public bool EntityHasComponent(T component, SharperEntity entity)
	    {
		    return Components.Find(c => c == component).Entity == entity;
	    }

	    public async Task<bool> EntityHasComponent(BaseSharperComponent component, SharperEntity entity)
	    {
		    return Components.Find(c => c == component).Entity == entity;
	    }

	    /// <summary>
        /// Destroys a component of the type the system manages.
        /// </summary>
        /// <param name="component">The component to remove from the game.</param>
        /// <returns>Returns a <see cref="Task"/> representing the removal work.</returns>
        public Task UnregisterComponentAsync(T component)
        {
            Components.Remove(component);
            ComponentUnRegistered?.Invoke(this, new SharperComponentEventArgs(component));
            return Task.CompletedTask;
        }

        public Task UnregisterAllComponentsByEntityAsync(SharperEntity entity)
        {
            Components.RemoveAll(x => x.Entity == entity);
            return Task.CompletedTask;
        }
    }
}
