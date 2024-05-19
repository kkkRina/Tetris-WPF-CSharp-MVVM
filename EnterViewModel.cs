using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class EnterViewModel : INotifyPropertyChanged
    {
        private User _user = new();
        private GameWindow _game;
        public User CurUser
        {
            get => _user;
            private set => SetField(ref _user, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private Command _loginCommand;
        public Command LoginCommand => _loginCommand; 

        private Command _removeCommand;
        public Command RemoveCommand => _removeCommand ??= new Command(
            CurUser.DeleteUser, _ => CurUser.IsExists);


        public EnterViewModel()
        {
            User.LoadUsers();
            _game = new(_user);
            _loginCommand = new(
                p =>
                {
                    CurUser.SaveOrUpdateUser(p);
                    _game.Show();
                },
                _ => CurUser.IsExists
            );

        }
    }
}
