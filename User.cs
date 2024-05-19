using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tetris
{
    public class User : INotifyPropertyChanged
    {
        private string _nickname = " ";
        private string _name = " ";
        private DateTime _birth = DateTime.Now.Date.AddYears(-6);
        private static readonly List<User> _users = new();
        public string Nick
        {
            get => _nickname;
            set => SetField(ref _nickname, value);
        }
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
        public DateTime Birth
        {
            get => _birth;
            set => SetField(ref _birth, value);
        }

        [JsonIgnore] 
        public bool IsValid =>
            !Nick.Trim().Equals("")
            && !Name.Trim().Equals("")
            && Birth <= DateTime.Now.Date.AddYears(-6)
            && Birth >= DateTime.Now.Date.AddYears(-120);

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
        public static void LoadUsers()
        {
            try
            {
                using FileStream fs = new("Users.Info", FileMode.Open);
                _users.Clear();
                JsonSerializer.Deserialize<List<User>>(fs)?.ForEach(_users.Add);
            }
            catch { }

        }
        public void SaveOrUpdateUser(object? p)
        {

            var u = _users.FirstOrDefault(user => user.Nick.Equals(Nick));
            if (u != null)
            {
                u.Name = Name;
                u.Birth = Birth;
            }
            else
            {
                _users.Add(Clone());
            }
            using FileStream fs = new("Users.Info", FileMode.Create);
            JsonSerializer.Serialize(fs, _users);
        }
        public bool IsExists => _users.FirstOrDefault(user => user.Nick.Equals(Nick)) != null;
        public void DeleteUser(object? p)
        {
            var u = _users.FirstOrDefault(user => user.Nick.Equals(Nick));
            if (u != null)
            {
                _users.Remove(u);
            }
            using FileStream fs = new("Users.Info", FileMode.Create);
            JsonSerializer.Serialize(fs, _users);
        }
        public User Clone()
        {
            var c = new User();
            c.Name = Name;
            c.Nick = Nick;
            c.Birth = Birth;
            return c;
        }
    }
}
