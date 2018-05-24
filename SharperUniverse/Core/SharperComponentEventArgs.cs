namespace SharperUniverse.Core
{
    /// <summary>
    /// Event arguments for all <see cref="BaseSharperComponent"/> based events.
    /// </summary>
    public class SharperComponentEventArgs
    {
        /// <summary>
        /// the <see cref="BaseSharperComponent"/> associated with the event call.
        /// </summary>
        public BaseSharperComponent SharperComponent { get; }

        /// <summary>
        /// Creates a new instance of <see cref="SharperComponentEventArgs"/>.
        /// </summary>
        /// <param name="component">The <see cref="BaseSharperComponent"/> associated with the event call.</param>
        public SharperComponentEventArgs(BaseSharperComponent component)
        {
            SharperComponent = component;
        }
    }
}
