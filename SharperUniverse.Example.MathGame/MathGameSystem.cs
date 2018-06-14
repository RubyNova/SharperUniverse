using System;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Example.MathGame
{
    public class MathGameSystem : BaseSharperSystem<MathGameComponent>
    {
        enum GameState
        {
            Question,
            Answer
        };

        private SharperInputSystem _input;
        private GameRunner _game;
        private GameState _gameState;
        private Random _random;
        private int _guess;
        private int _answer;

        public MathGameSystem(GameRunner game, SharperInputSystem input) : base(game)
        {
            _game = game;
            _input = input;
            _random = new Random();

            _input.NewInputEntityCreated += async (s, e) =>
            {
                await RegisterComponentAsync(await Game.CreateEntityAsync(), e.Entity);
            };
        }

        public override async Task CycleUpdateAsync(Func<string, Task> outputHandler)
        {
            var commands = await _input.GetEntitiesByCommandInfoTypesAsync(typeof(ExitCommandInfo), typeof(AnswerCommandInfo));
            foreach (var command in commands)
            {
                switch (command.Value)
                {
                    case ExitCommandInfo _:
                        _game.StopGame();
                        break;
                    case AnswerCommandInfo a:
                        _guess = a.Guess;
                        _gameState = GameState.Answer;

                        var commandEntityContext = command.Key;
                        var componentICareAbout = Components.Single(c => c.ControllingEntity == commandEntityContext);

                        break;
                    default:
                        break;
                }
            }

            switch (_gameState)
            {
                case GameState.Question:
                    var left = _random.Next(1, 10);
                    var right = _random.Next(1, 10);
                    _answer = left + right;
                    await outputHandler.Invoke($"{left} + {right}");
                    _gameState = GameState.Answer;
                    break;
                case GameState.Answer:
                    if (_guess == _answer)
                    {
                        _guess = -1;
                        await outputHandler.Invoke("Correct!\n");
                        _gameState = GameState.Question;
                    }
                    break;
            }
        }
    }
}
