namespace SharperUniverse.Core
{
    public class SharperComponentEventArgs
    {
        public BaseSharperComponent BaseSharperComponent { get; }

        public SharperComponentEventArgs(BaseSharperComponent component)
        {
            BaseSharperComponent = component;
        }
    }
}
