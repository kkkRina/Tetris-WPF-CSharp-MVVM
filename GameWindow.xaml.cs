using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly GameViewModel? _viewModel;
        public GameWindow(User player)
        {
            _viewModel = new(player);
            InputBindings.Add(new KeyBinding(_viewModel.LeftMoveCommand, Key.Left, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(_viewModel.RightMoveCommand, Key.Right, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(_viewModel.RightRotateCommand, Key.Up, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(_viewModel.LeftRotateCommand, Key.Down, ModifierKeys.None));
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
