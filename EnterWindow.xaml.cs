using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для EnterWindow.xaml
    /// </summary>
    public partial class EnterWindow : Window
    {
        private EnterViewModel _enterViewModel = new();
        public EnterWindow()
        {
            InitializeComponent();

            Binding bNick = new Binding(nameof(User.Nick))
            {
                Source = _enterViewModel.CurUser,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            TbNick1.SetBinding(TextBox.TextProperty, bNick);
            
            BtnLogin.Command = _enterViewModel.LoginCommand;
            BtnDelete.Command = _enterViewModel.RemoveCommand;
        }
    }
}
