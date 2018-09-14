namespace SharperUniverse.Core
{
    /// <summary>
    /// The type that represents all entites within a game. This class cannot be inherited. It should not be instantiated directly. Instead, call <see cref="GameRunner.CreateEntityAsync"/> to get a new <see cref="SharperEntity"/>.
    /// </summary>
    public sealed class SharperEntity
    {
        public bool ShouldDestroy { get; set; }
    }
}
