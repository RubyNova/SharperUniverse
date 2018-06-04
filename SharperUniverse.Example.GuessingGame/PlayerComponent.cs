using SharperUniverse.Core;

namespace SharperUniverse.Example.GuessingGame
{
    public class PlayerComponent : BaseSharperComponent
    {
        public int PlayerNumber { get; set; }
        public int Score { get; set; } = 0;
        public int GuessCount { get; set; } = 0;

        public PlayerComponent(SharperEntity entity) : base(entity)
        {

        }

        public override string ToString()
        {
            return PlayerNumber.ToString();
        }
    }
}