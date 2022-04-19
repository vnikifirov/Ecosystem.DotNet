using System;
namespace Sudoku.Business.Interfaces
{
    public interface ISudokuServiceHasSet
    {
        public bool IsRightFilled(int[,] matrix);
    }
}
