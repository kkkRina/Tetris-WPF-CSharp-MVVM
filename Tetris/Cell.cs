using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Cell : NotifyPropertyChanged
    {
        private bool _state;

        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged(); // сообщает интерфейсу что надо обновить эту ячейку
            }
        }
    }
}
