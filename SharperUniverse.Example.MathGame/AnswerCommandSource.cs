using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    public class AnswerCommandSource : IUniverseCommandSource
    {
        public int ID { get; set; }

        public AnswerCommandSource(int id)
        {
            ID = id;
        }

        public bool SourceIsSameAsBindingSource(IUniverseCommandSource bindingSource)
        {
            return ID == (bindingSource as AnswerCommandSource).ID;
        }
    }
}
