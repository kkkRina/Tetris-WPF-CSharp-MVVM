using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public enum GameState
    {
        Over,
        Running,
        Paused
    }

    public class Game
    {
        public event Action<int> ScoreChanged; //возникает при изменении очков
        public event Action<GameState> StateChanged; // возникает при изменении состояния игры

        private int _score;
        private GameState _state;
        private readonly GameField _field;
        private CancellationTokenSource _cts;

        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                ScoreChanged?.Invoke(value);
            }
        }

        public GameState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }

        public Game(GameField field)
        {
            _field = field;
        }

        public async void Start()
        {
            // если игра уже запущена, повторно не запускать
            if (State == GameState.Running)
                return;

            // пример работы с полем
            _field[3, 1] = true;
            _field[4, 1] = true;
            _field[5, 1] = true;
            _field[5, 2] = true;

            _field[3, 4] = true;
            _field[4, 4] = true;
            _field[5, 4] = true;
            _field[4, 5] = true;

            _field[4, 7] = true;
            _field[5, 7] = true;
            _field[4, 8] = true;
            _field[5, 8] = true;

            //запуск игры
            State = GameState.Running;
            using (_cts = new CancellationTokenSource())
            {
                await RunGameLoop(_cts.Token);
            }
            _cts = null;
        }

        private async Task RunGameLoop(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    Score += 10;
                    await Task.Delay(1000, token);
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Pause()
        {
            if (State != GameState.Running)
                return;
            _cts?.Cancel(); // остановить игру
            _field.Clear(); // очистить поле

            State = GameState.Paused;
        }
    }
}
