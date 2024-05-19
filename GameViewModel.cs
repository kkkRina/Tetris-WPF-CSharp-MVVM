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
        private readonly User _player;
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

        public GameViewModel(User player)
        {
            Field = new GameField(rows, cols);
            _game = new Game(Field);
            _game.ScoreChanged += (s) => Score = s;
            _game.StateChanged += (s) => State = s;
            _player = player;
        }
        private Command _leftMoveCommand;
        public Command LeftMoveCommand => _leftMoveCommand ??= new Command(LeftMoveFigure);
        private Command _rightMoveCommand;
        public Command RightMoveCommand => _rightMoveCommand ??= new Command(RightMoveFigure);
        private Command _leftRotateCommand;
        public Command LeftRotateCommand => _leftRotateCommand ??= new Command(LeftRotateFigure);
        private Command _rightRotateCommand;
        public Command RightRotateCommand => _rightRotateCommand ??= new Command(RightRotateFigure);
        private void LeftMoveFigure(object? param)
        {
            _game?.LeftMoveFigure();
        }
        private void RightMoveFigure(object? param)
        {
            _game?.RightMoveFigure();
        }
        private void LeftRotateFigure(object? param)
        {
            _game?.LeftRotateFigure();
        }
        private void RightRotateFigure(object? param)
        {
            _game?.RightRotateFigure();
        }
    }
}
