using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    class GuessCommandSource : IUniverseCommandSource
    {
        public int ID { get; set; }

        public GuessCommandSource(int id)
        {
            ID = id;
        }

        public bool SourceIsSameAsBindingSource(IUniverseCommandSource bindingSource)
        {
            return ID == (bindingSource as GuessCommandSource).ID;
        }
    }
}
