using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    class GuessCommandSource : IUniverseCommandSource
    {
        public int TestIdentifierThing { get; }

        public GuessCommandSource(int testId)
        {
            TestIdentifierThing = testId;
        }

        public bool SourceIsSameAsBindingSource(IUniverseCommandSource bindingSource)
        {
            var source = (GuessCommandSource)bindingSource;

            return TestIdentifierThing == source.TestIdentifierThing;
        }
    }
}
