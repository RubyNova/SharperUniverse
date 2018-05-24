using System;
using System.Collections.Generic;
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
        protected readonly GameRunner Game;

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
        public List<T> Components { get; }

        /// <summary>
        /// Base constructor for any <see cref="BaseSharperSystem{T}"/>. This must be called for the system to function correctly. Do not pass <see cref="ISharperSystem{T}"/> types in an overriden constructor. Instead, use <see cref="SharperInjectAttribute"/>.
        /// </summary>
        /// <param name="game">The current GameRunner this system should be registered to.</param>
        protected BaseSharperSystem(GameRunner game)
        {
            Components = new List<T>();
            game.RegisterSystem(this);
            Game = game;
        }



        /// <summary>
        /// The logic that runs in an <see cref="ISharperSystem{T}"/> once per update cycle. The update cycle is defined by the <see cref="GameRunner"/> on instantiation.
        /// </summary>
        /// <param name="outputHandler"></param>
        /// <returns></returns>
        public abstract Task CycleUpdateAsync(Func<string, Task> outputHandler);

        /// <summary>
        /// Registers a component of type <typeparamref name="T"/> name="T"/> and assigns it to the target <see cref="SharperEntity"/>.
        /// </summary>
        /// <param name="entity">The target entity.</param>
        /// <param name="args">the values for the new component.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the registration work.</returns>
        public Task RegisterComponentAsync(SharperEntity entity, params object[] args)
        {
            var inputArr = new object[args.Length + 1];
            inputArr[0] = entity;

            for (int i = 1; i < inputArr.Length; i++)
            {
                inputArr[i] = args[i - 1];
            }

            T component = (T)Activator.CreateInstance(typeof(T), inputArr);

            entity.AddComponentAsync(component);

            Components.Add(component);

            ComponentRegistered?.Invoke(this, new SharperComponentEventArgs(component));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Destroys a component of type 
        /// </summary>
        /// <param name="component">The component to remove from the game.</param>
        /// <returns>Returns a <see cref="Task"/> representing the removal work.</returns>
        public Task UnregisterComponentAsync(T component)
        {
            Components.Remove(component);
            ComponentUnRegistered?.Invoke(this, new SharperComponentEventArgs(component));
            return Task.CompletedTask;
        }
    }
}
