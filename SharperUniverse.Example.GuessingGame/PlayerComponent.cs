using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class PlayerComponent : BaseSharperComponent
    {
        public int PlayerNumber { get; set; }
        public int Score { get; set; }
        public int GuessCount { get; set; }

        public PlayerComponent(SharperEntity entity)
        {
            Entity = entity;
            Score = 0;
            GuessCount = 0;
        }

        public override string ToString()
        {
            return PlayerNumber.ToString();
        }
    }
}