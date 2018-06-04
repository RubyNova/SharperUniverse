namespace SharperUniverse.Core
{
    /// <summary>
    /// The base type for all game components. This class cannot be instantiated by itself. You must derive from this class.
    /// </summary>
    public abstract class BaseSharperComponent
    {
        /// <summary>
        /// The <see cref="SharperEntity"/> this Component is attached to.
        /// </summary>
        public SharperEntity Entity { get; }

        protected BaseSharperComponent(SharperEntity entity)
        {
            Entity = entity;
        }
    }
}
