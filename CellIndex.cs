using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tetris
{
    
    public class CellIndex(int row, int col) : Cell(row, col)
    {
        public string Id => $"{Row},{Col}";
        public int Left { get; set; }
        public int Top { get; set; }
        public int Size { get; set; }
        public Thickness Margin => new(Left, Top, 0, 0);

        public static event Action? CellUpdateNeeded;
        public static event Action<Cell>? CellOpened;
        public SolidColorBrush Color
        {
            get
            {
                if (this.Type == CellType.White)
                    return new SolidColorBrush(Colors.White);
                return new SolidColorBrush(Colors.Gray);
            }
        }
    }
}
