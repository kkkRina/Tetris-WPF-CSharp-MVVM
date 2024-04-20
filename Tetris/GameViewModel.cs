using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tetris
{
    public class GameViewModel : NotifyPropertyChanged
    {
        private const int rows = 20;
        private const int cols = 10;

        private int _score;
        private GameField _field;
        private GameState _state;
        private Game _game;
        private ICommand _startCommand;

        public GameField Field
        {
            get => _field;
            set
            {
                _field = value;
                OnPropertyChanged();
            }
        }

        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => _startCommand ??= new Command(parameter =>
        {
            if (State == GameState.Running)
                _game.Pause();
            else
                _game.Start();
        });

        public GameViewModel()
        {
            Field = new GameField(rows, cols);
            _game = new Game(Field);
            _game.ScoreChanged += (s) => Score = s;
            _game.StateChanged += (s) => State = s;
        }
    }
}
