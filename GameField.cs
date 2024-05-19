using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Tetris
{
    public class GameField : List<CellIndex>, IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private int _width;
        private int _height;
        public int Rows { get; set; }
        public int Cols { get; set; }
        public FigureType Type { get; set; } // тип активной фигуры
        public int Width
        {
            get => _width;
            set
            {
                SetField(ref _width, value);
                UpdateCells();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                SetField(ref _height, value);
                UpdateCells();
            }
        }
        private int CellSize => int.Min(Height / Rows, Width / Cols);
        private int LShift => (Width - CellSize * Cols) / 2;
        private int TShift => (Height - CellSize * Rows) / 2;

        public CellIndex? this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols) return null;
                return this[row * Cols + col];
            }
        }
        public GameField(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            FillField();
        }
        private void FillField()
        {
            Clear();
            for (byte i = 0; i < Rows; i++)
            {
                for (byte j = 0; j < Cols; j++)
                {
                    Add(new CellIndex(i, j));
                }
            }
        }
        public List<Cell> GetFigureCells(Cell cell, FigureType type) // дает координаты всех клеток фигуры
        {
            List<Cell> cellList = new List<Cell>();
            var row = cell.Row;
            var col = cell.Col;
            switch (type)
            {
                case FigureType.O:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col + 1]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.I:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row + 2, col]);
                    cellList.Add(this[row + 3, col]);
                    break;
                case FigureType.S:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row, col + 1]);
                    cellList.Add(this[row + 1, col - 1]);
                    break;
                case FigureType.Z:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row + 1, col + 1]);
                    break;
                case FigureType.L:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row + 2, col]);
                    cellList.Add(this[row + 2, col + 1]);
                    break;
                case FigureType.J:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row + 2, col]);
                    cellList.Add(this[row + 2, col - 1]);
                    break;
                case FigureType.T:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.IRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row, col + 1]);
                    cellList.Add(this[row, col + 2]);
                    cellList.Add(this[row, col + 3]);
                    break;
                case FigureType.SRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row - 1, col - 1]);
                    cellList.Add(this[row, col - 1]);
                    break;
                case FigureType.ZRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col - 1]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row - 1, col]);
                    break;
                case FigureType.LLRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row, col + 1]);
                    cellList.Add(this[row, col + 2]);
                    cellList.Add(this[row - 1, col + 2]);
                    break;
                case FigureType.LRRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row, col + 2 ]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.LInv:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col + 1]);
                    cellList.Add(this[row + 2, col + 1]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.JLRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row + 1, col + 1]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.JRRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row - 1, col - 1]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.JInv:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row + 1, col - 1]);
                    cellList.Add(this[row + 2, col - 1]);
                    break;
                case FigureType.TLRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row - 1, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row, col + 1]);
                    break;
                case FigureType.TRRot:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row - 1, col]);
                    cellList.Add(this[row + 1, col]);
                    cellList.Add(this[row, col - 1]);
                    break;
                case FigureType.TInv:
                    cellList.Add(this[row, col]);
                    cellList.Add(this[row, col - 1]);
                    cellList.Add(this[row, col + 1]);
                    cellList.Add(this[row - 1, col]);
                    break;
            }
            return cellList;
        }
        public void CreateFigure(FigureType type, int row, int col, bool mark = true)
        {
            CellType cellType;
            if (mark == true)
            {
                cellType = CellType.Colored;
            }
            else cellType = CellType.White;
            switch (type)
            {
                case FigureType.O:
                    List<Cell> fo = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fo)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.I:
                    List<Cell> fi = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fi)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.S:
                    List<Cell> fs = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fs)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.Z:
                    List<Cell> fz = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fz)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.L:
                    List<Cell> fl = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fl)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.J:
                    List<Cell> fj = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fj)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.T:
                    List<Cell> ft = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in ft)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.IRot:
                    List<Cell> firot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in firot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.SRot:
                    List<Cell> fsrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fsrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.ZRot:
                    List<Cell> fzrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fzrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.LLRot:
                    List<Cell> fllrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fllrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.LRRot:
                    List<Cell> flrrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in flrrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.LInv:
                    List<Cell> flinv = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in flinv)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.JLRot:
                    List<Cell> fjlrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fjlrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.JRRot:
                    List<Cell> fjrrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fjrrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.JInv:
                    List<Cell> fjinv = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in fjinv)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.TLRot:
                    List<Cell> ftlrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in ftlrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.TRRot:
                    List<Cell> ftrrot = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in ftrrot)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
                case FigureType.TInv:
                    List<Cell> ftinv = GetFigureCells(this[row, col], type);
                    foreach (Cell cell in ftinv)
                    {
                        this[cell.Row, cell.Col].Type = cellType;
                    }
                    break;
            }
            Type = type;
            UpdateCells();
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void ClearFigure(Cell cell)
        {
            var row = cell.Row;
            var col = cell.Col;
            CreateFigure(Type,row,col,false);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public void UpdateCells()
        {
            if (Height > 0 && Width > 0)
            {
                foreach (var cell in this)
                {
                    cell.Left = CellSize * cell.Col + LShift;
                    cell.Top = CellSize * cell.Row + TShift;
                    cell.Size = CellSize;
                }
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
