using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharperUniverse.Core
{
    public abstract class BaseSharperSystem<T> : ISharperSystem<T> where T : BaseSharperComponent
    {
        protected event EventHandler<SharperComponentEventArgs> ComponentAdded;
        protected event EventHandler<SharperComponentEventArgs> ComponentRemoved;

        public List<T> Components { get; }

        protected BaseSharperSystem(GameRunner game)
        {
            Components = new List<T>();
            game.RegisterSystem(this);
        }

        public abstract Task CycleUpdateAsync(Func<string, Task> outputHandler);

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

            ComponentAdded?.Invoke(this, new SharperComponentEventArgs(component));
            return Task.CompletedTask;
        }

        public Task UnregisterComponentAsync(T component)
        {
            Components.Remove(component);
            ComponentRemoved?.Invoke(this, new SharperComponentEventArgs(component));
            return Task.CompletedTask;
        }
    }
}
