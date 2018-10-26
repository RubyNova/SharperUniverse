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
        private SharperEntity _inputEntity;

        public MathGameSystem(GameRunner game, SharperInputSystem input) : base(game)
        {
            _game = game;
            _input = input;
            _random = new Random();

            _input.NewInputEntityCreated += async (s, e) =>
            {
                await RegisterComponentAsync(new MathGameComponent(await game.CreateEntityAsync(), e.Entity));
                _inputEntity = e.Entity;
            };
        }

        public override async Task CycleUpdateAsync(int deltaMs)
        {
            var commands = await _input.GetEntitiesByCommandInfoTypesAsync(typeof(ExitCommandInfo), typeof(AnswerCommandInfo));
            foreach (var command in commands)
            {
                switch (command.Value)
                {
                    case ExitCommandInfo _:
                        _input.GetConnectionByEntity(command.Key).Disconnect();
                        break;
                    case AnswerCommandInfo a:
                        _guess = a.Guess;
                        _inputEntity = command.Key;
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

                    //await outputHandler.Invoke($"{left} + {right}");
                    if (_inputEntity != null)
                    {
                        var left = _random.Next(1, 10);
                        var right = _random.Next(1, 10);
                        _answer = left + right;
                        _input.GetConnectionByEntity(_inputEntity).Send($"{left} + {right}");
                        _gameState = GameState.Answer;
                    }
                    break;
                case GameState.Answer:
                    if (_guess == _answer)
                    {
                        _guess = -1;
                        //await outputHandler.Invoke("Correct!\n");
                        _gameState = GameState.Question;
                        _input.GetConnectionByEntity(_inputEntity).Send($"Correct!");
                    }
                    break;
            }
        }
    }
}
