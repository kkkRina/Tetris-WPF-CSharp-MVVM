using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Tetris
{
    public enum GameState
    {
        Over,
        Running,
        Paused
    }
    public class Game : INotifyCollectionChanged
    {
        public event Action<int> ScoreChanged; //возникает при изменении очков
        public event Action<GameState> StateChanged; // возникает при изменении состояния игры

        private int _score;
        private GameState _state;
        private readonly GameField _field;
        private CancellationTokenSource _cts;
        private bool _flag = false; // true если есть активная фигура, false если нужно создать фигуру
        private Cell? _centralCell = new(0,4); // центральная клетка активной фигуры
        private FigureType _type; // тип активной фигуры
        public enum MoveType
        {
            None, LeftMove, RightMove, LeftRotate, RightRotate
        }
        private MoveType _moveType; // тип движения активной фигуры
        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                ScoreChanged?.Invoke(value);
            }
        }

        public GameState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }

        public Game(GameField field)
        {
            _field = field;
        }

        public async void Start()
        {
            // если игра уже запущена, повторно не запускать
            if (State == GameState.Running)
                return;
            //запуск игры
            State = GameState.Running;
            using (_cts = new CancellationTokenSource())
            {
                await RunGameLoop(_cts.Token);
            }
            _cts = null;
        }

        private async Task RunGameLoop(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    Score += 10;
                    if (!_flag && (_field[0,4].Type == CellType.White))
                    {
                        NeedDelete();
                        CreateFigure();
                    }
                    else if (!_flag && _field[0, 4].Type == CellType.Colored)
                    {
                        State = GameState.Over;
                        _cts?.Cancel(); // остановить игру
                        _field.Clear();
                        MessageBox.Show(
                            $"Ваш результат: {_score}",
                            "Game over :("
                            );
                    }
                    else
                    {
                        FigureMove(_centralCell, _type);
                    }
                    await Task.Delay(150, token);                
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Pause()
        {
            if (State != GameState.Running)
                return;
            _cts?.Cancel(); // остановить игру
            State = GameState.Paused;
        }
        public void CreateFigure()
        {
            Random rnd = new Random();
            FigureType type = (FigureType)rnd.Next(7);
            _field.CreateFigure(type,0,4);
            _centralCell.Row = 0;
            _centralCell.Col = 4;
            _type = type;
            _flag = true;
            
        }
        public void FigureMove(Cell cell, FigureType type)
        {
            int Bot=0; // низшая координата фигуры
            List<int> Cols = new();// столбцы клеток из фигуры без повторов
            List<int> Rows = new();// соответствующие им строки
            bool CanMoveDown = true; // флажок - можем ли двигать фигуру
            bool CanMoveRight = true;
            bool CanMoveLeft = true;
            List<int> ColsLeft = new();// аналогичные списки для проверок на движение вправо-влево
            List<int> RowsLeft = new();
            List<int> ColsRight = new();
            List<int> RowsRight = new();
            List<Cell> c = _field.GetFigureCells(cell, type); // все клетки фигуры
            foreach (Cell ce in c) 
            {
                int r = ce.Row;
                if (r > Bot)
                    Bot = r; // обновляю низшую координату фигуры
                int co = ce.Col;
                if (!Cols.Contains(co)) // если нет клеток из этого стобца сразу добавляем
                {
                    
                    Cols.Add(co); // заполняю списки столбцов и строк фигуры
                    Rows.Add(r);
                }
                else
                {
                    int t; // вместо BinarySearch , потому что он отказался тут работать
                    for (int j = 0; j < Cols.Count; j++)
                    {
                        if (Cols[j] == co)
                        {
                            t = j;
                            if (r > Rows[t]) // если в этом столбце есть клетки, оставляем ту, у которой макс индекс строки
                            {
                                Cols.Remove(Cols[t]);
                                Rows.Remove(Rows[t]);
                                Cols.Add(co);
                                Rows.Add(r);
                            }
                        }
                    }
                    
                }
                if (!RowsLeft.Contains(r)) // если нет клеток из этой строки сразу добавляем
                {

                    ColsLeft.Add(co); // заполняю списки столбцов и строк фигуры
                    RowsLeft.Add(r);
                }
                else
                {
                    int t; // вместо BinarySearch , потому что он отказался тут работать
                    for (int j=0; j<RowsLeft.Count; j++)
                    {
                        if (RowsLeft[j] == r)
                        {
                            t = j;
                            if (co < ColsLeft[t]) // если в этой строке есть клетки, оставляем ту, у которой мин индекс столбца
                            {
                                ColsLeft.Remove(ColsLeft[t]);
                                RowsLeft.Remove(RowsLeft[t]);
                                ColsLeft.Add(co);
                                RowsLeft.Add(r);
                            }
                        } 
                    }
                    
                }
                if (!RowsRight.Contains(r)) // если нет клеток из этой строки сразу добавляем
                {

                    ColsRight.Add(co); // заполняю списки столбцов и строк фигуры
                    RowsRight.Add(r);
                }
                else
                {
                    int t; // вместо BinarySearch , потому что он отказался тут работать
                    for (int j = 0; j < RowsRight.Count; j++)
                    {
                        if (RowsRight[j] == r)
                        {
                            t = j;
                            if (co > ColsRight[t]) // если в этой строке есть клетки, оставляем ту, у которой макс индекс столбца
                            {
                                ColsRight.Remove(ColsRight[t]);
                                RowsRight.Remove(RowsRight[t]);
                                ColsRight.Add(co);
                                RowsRight.Add(r);
                            }
                        }
                    }
                    
                }
            }
            for(int i = 0; i < Cols.Count; i++)
            {
                if (Bot >= _field.Rows - 1 || _field[Rows[i]+1, Cols[i]].Type==CellType.Colored)
                {
                    CanMoveDown = false;
                    break;
                }
            }
            for (int i = 0; i < ColsRight.Count; i++)
            {
                if (ColsRight[i] >= 9)
                {
                    CanMoveRight = false;
                    break;
                }
                else if(_field[RowsRight[i], ColsRight[i] + 1].Type == CellType.Colored)
                {
                    CanMoveRight = false;
                    break;
                }
            }
            for (int i = 0; i < ColsLeft.Count; i++)
            {
                if (ColsLeft[i] <= 0  )
                {

                    CanMoveLeft = false;
                    break;
                }
                else if(_field[RowsLeft[i], ColsLeft[i] - 1].Type == CellType.Colored)
                {
                    CanMoveLeft = false;
                    break;
                }
            }
            if (CanMoveDown)
            {
                _field.ClearFigure(cell);
                _field.CreateFigure(type, cell.Row+1, cell.Col);
                _centralCell.Row = _centralCell.Row + 1;
                
            }
            if (!(_moveType == MoveType.None))
            {
                
                if (_moveType == MoveType.LeftMove && CanMoveLeft)
                {
                    _field.ClearFigure(cell);
                    _field.CreateFigure(type, cell.Row, cell.Col - 1);
                    _centralCell.Col = _centralCell.Col - 1;

                }
                else if (_moveType == MoveType.RightMove && CanMoveRight)
                {
                    _field.ClearFigure(cell);
                    _field.CreateFigure(type, cell.Row, cell.Col + 1);
                    _centralCell.Col = _centralCell.Col + 1;
                }
                else if (_type == FigureType.I && CanMoveRight && (_moveType == MoveType.RightRotate|| _moveType == MoveType.LeftRotate))
                {
                    _field.ClearFigure(cell);
                    _field.CreateFigure(FigureType.IRot, cell.Row + 3, cell.Col);
                    _type = FigureType.IRot;
                    _centralCell.Row = _centralCell.Row + 3;
                }
                else if (_type == FigureType.IRot && (_moveType == MoveType.RightRotate || _moveType == MoveType.LeftRotate))
                {
                    _field.ClearFigure(cell);
                    _field.CreateFigure(FigureType.I, cell.Row - 3, cell.Col);
                    _type = FigureType.I;
                    _centralCell.Row = _centralCell.Row - 3;
                }
                else if ((_type == FigureType.S || _type == FigureType.Z || _type == FigureType.SRot || _type == FigureType.ZRot) && (_moveType == MoveType.RightRotate || _moveType == MoveType.LeftRotate))
                {
                    _field.ClearFigure(cell);
                    switch (_type)
                    {
                        case FigureType.S:
                            _field.CreateFigure(FigureType.SRot, cell.Row, cell.Col);
                            _type = FigureType.SRot;
                            break;
                        case FigureType.SRot:
                            _field.CreateFigure(FigureType.S, cell.Row, cell.Col);
                            _type = FigureType.S;
                            break;
                        case FigureType.Z:
                            _field.CreateFigure(FigureType.ZRot, cell.Row, cell.Col);
                            _type = FigureType.ZRot;
                            break;
                        case FigureType.ZRot:
                            _field.CreateFigure(FigureType.Z, cell.Row, cell.Col);
                            _type = FigureType.Z;
                            break;

                    }

                }
                else if ((_type == FigureType.L && _moveType == MoveType.RightRotate || _type == FigureType.J && _moveType == MoveType.LeftRotate) && CanMoveLeft)
                {
                    if (_type == FigureType.L)
                    {
                        _field.ClearFigure(cell);
                        _field.CreateFigure(FigureType.LRRot, cell.Row + 1, cell.Col - 1);
                        _type = FigureType.LRRot;
                    }
                    if (_type == FigureType.J)
                    {
                        _field.ClearFigure(cell);
                        _field.CreateFigure(FigureType.JLRot, cell.Row + 1, cell.Col - 1);
                        _type = FigureType.JLRot;
                    }
                    _centralCell.Row++;
                    _centralCell.Col--;
                }
                else if ((_type == FigureType.L && _moveType == MoveType.LeftRotate || _type == FigureType.J && _moveType == MoveType.RightRotate) && CanMoveRight)
                {
                    if (_type == FigureType.L)
                    {
                        _field.ClearFigure(cell);
                        _field.CreateFigure(FigureType.LLRot, cell.Row + 2, cell.Col);
                        _type = FigureType.LLRot;
                    }
                    if (_type == FigureType.J)
                    {
                        _field.ClearFigure(cell);
                        _field.CreateFigure(FigureType.JRRot, cell.Row +2, cell.Col);
                        _type = FigureType.JRRot;
                    }
                    _centralCell.Row = _centralCell.Row+2;
                }
                else if (_type == FigureType.LLRot || _type == FigureType.LRRot || _type == FigureType.JLRot || _type == FigureType.JRRot)
                {
                    if (_type== FigureType.LLRot)
                    {
                        _field.ClearFigure(cell);
                        switch (_moveType)
                        {
                            case MoveType.LeftRotate:
                                _field.CreateFigure(FigureType.LInv, cell.Row - 2, cell.Col );
                                _type = FigureType.LInv;
                                _centralCell.Row = _centralCell.Row - 2;
                                break;
                            case MoveType.RightRotate:
                                _field.CreateFigure(FigureType.L, cell.Row - 2, cell.Col);
                                _type = FigureType.L;
                                _centralCell.Row = _centralCell.Row - 2;
                                break;
                        }
                    }
                    if (_type == FigureType.LRRot)
                    {
                        _field.ClearFigure(cell);
                        switch (_moveType)
                        {
                            case MoveType.LeftRotate:
                                _field.CreateFigure(FigureType.L, cell.Row - 1, cell.Col);
                                _type = FigureType.L;
                                _centralCell.Row = _centralCell.Row - 1;
                                break;
                            case MoveType.RightRotate:
                                _field.CreateFigure(FigureType.LInv, cell.Row - 1, cell.Col);
                                _type = FigureType.LInv;
                                _centralCell.Row = _centralCell.Row - 1;
                                break;
                        }
                    }
                    if (_type == FigureType.JLRot)
                    {
                        _field.ClearFigure(cell);
                        switch (_moveType)
                        {
                            case MoveType.LeftRotate:
                                _field.CreateFigure(FigureType.JInv, cell.Row - 1, cell.Col);
                                _type = FigureType.JInv;
                                _centralCell.Row = _centralCell.Row - 1;
                                break;
                            case MoveType.RightRotate:
                                _field.CreateFigure(FigureType.J, cell.Row - 1, cell.Col);
                                _type = FigureType.J;
                                _centralCell.Row = _centralCell.Row - 1;
                                break;
                        }
                    }
                    if (_type == FigureType.JRRot)
                    {
                        _field.ClearFigure(cell);
                        switch (_moveType)
                        {
                            case MoveType.LeftRotate:
                                _field.CreateFigure(FigureType.J, cell.Row - 2, cell.Col);
                                _type = FigureType.J;
                                _centralCell.Row = _centralCell.Row - 2;
                                break;
                            case MoveType.RightRotate:
                                _field.CreateFigure(FigureType.JInv, cell.Row - 2, cell.Col);
                                _type = FigureType.JInv;
                                _centralCell.Row = _centralCell.Row - 2;
                                break;
                        }
                    } 
                }
                else if (_type == FigureType.T || _type == FigureType.TRRot || _type == FigureType.TLRot || _type == FigureType.TInv)
                {
                    _field.ClearFigure(cell);
                    switch (_type)
                    {
                        case FigureType.T:
                            if (_moveType== MoveType.LeftRotate)
                            {
                                _field.CreateFigure(FigureType.TLRot, cell.Row, cell.Col);
                                _type = FigureType.TLRot;
                            }
                            if (_moveType == MoveType.RightRotate)
                            {
                                _field.CreateFigure(FigureType.TRRot, cell.Row, cell.Col);
                                _type = FigureType.TRRot;
                            }
                            break;
                        case FigureType.TRRot:
                            if (_moveType == MoveType.LeftRotate)
                            {
                                _field.CreateFigure(FigureType.T, cell.Row, cell.Col);
                                _type = FigureType.T;
                            }
                            if (_moveType == MoveType.RightRotate)
                            {
                                _field.CreateFigure(FigureType.TInv, cell.Row + 1, cell.Col);
                                _type = FigureType.TInv;
                                _centralCell.Row++;
                            }
                            break;
                        case FigureType.TLRot:
                            if (_moveType == MoveType.LeftRotate)
                            {
                                _field.CreateFigure(FigureType.TInv, cell.Row +  1, cell.Col);
                                _type = FigureType.TInv;
                                _centralCell.Row++;
                            }
                            if (_moveType == MoveType.RightRotate)
                            {
                                _field.CreateFigure(FigureType.T, cell.Row, cell.Col);
                                _type = FigureType.T;
                                
                            }
                            break;
                        case FigureType.TInv:
                            if (_moveType == MoveType.LeftRotate)
                            {
                                _field.CreateFigure(FigureType.TRRot, cell.Row - 1, cell.Col);
                                _type = FigureType.TRRot;
                            }
                            if (_moveType == MoveType.RightRotate)
                            {
                                _field.CreateFigure(FigureType.TLRot, cell.Row - 1, cell.Col);
                                _type = FigureType.TLRot;
                            }
                            _centralCell.Row--;
                            break;
                    }
                }
                _moveType = MoveType.None;
            }
            if(!CanMoveDown && (_moveType == MoveType.None))
            {
                _flag = false;
                _centralCell.Row = 0;
                _centralCell.Col = 4;
            }

        }
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public void NeedDelete()
        {
            List<int> RowDel = new List<int>(); // какие строки будем удалять
            for(int i = 0; i < _field.Rows; i++)
            {
                bool need = true; 
                for (int j = 0; j < _field.Cols; j++)
                {
                    if (_field[i, j].Type == CellType.White)
                    {
                        need = false;
                        break;
                    }
                }
                if (need) 
                {
                    RowDel.Add(i);
                }
            }
            foreach (int i in RowDel)
            {
                for(int j = 0; j < _field.Cols; j++)
                {
                    _field[i,j].Type = CellType.White;
                }
                for(int x = i - 1; x >= 0; x--)
                {
                    for(int z = 0; z < _field.Cols; z++)
                    {
                        if (_field[x,z].Type==CellType.Colored && _field[x + 1, z].Type == CellType.White)
                        {
                            _field[x, z].Type = CellType.White;
                            _field[x + 1, z].Type = CellType.Colored;
                        }
                    }
                }
                _score = _score + 100;
            }
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void LeftMoveFigure()
        {
            _moveType = MoveType.LeftMove;
        }
        public void RightMoveFigure()
        {
            _moveType = MoveType.RightMove;
        }
        public void LeftRotateFigure()
        {
            _moveType = MoveType.LeftRotate;
        }
        public void RightRotateFigure()
        {
            _moveType = MoveType.RightRotate;
        }
    }
}
