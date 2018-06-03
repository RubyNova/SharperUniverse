using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class RoundComponent : BaseSharperComponent
    {
        public enum RoundState
        {
            Start,
            NewTurn,
            Input,
            Finish
        }

        public int Answer { get; set; }

        public RoundState State { get; set; }

        public int RoundNumber { get; set; }

        public RoundComponent(SharperEntity entity)
        {
            Entity = entity;
            State = RoundState.Start;
        }
    }
}