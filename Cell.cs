using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris
{
    public enum CellType
    {
        White, Colored
    }
    public class Cell(int row, int col) : NotifyPropertyChanged
    {
        public int Row { get => row; set => row = value; }
        public int Col { get => col; set => col = value; }
        public CellType Type { get; set; }
        
    }
}
