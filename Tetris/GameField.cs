using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameField : IEnumerable
    {
        private readonly int _rows;
        private readonly int _cols;

        private readonly Cell[] _data;

        public int Rows => _rows;
        public int Cols => _cols;


        public bool this[int row, int col]
        {
            get => _data[row * Cols + col].State;
            set => _data[row * Cols + col].State = value;
        }

        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
                _data[i].State = false;
        }

        public GameField(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _data = new Cell[rows * cols];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = new Cell();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => _data.GetEnumerator();
    }
}
