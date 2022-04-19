using System;
using Sudoku.Business.Interfaces;
using System.Collections.Generic;

namespace Sudoku.Business.Implementations
{
    public class SudokuService : ISudokuServiceHasSet
    {
        public bool IsRightFilled(int[,] matrix)
        {
            if (!IsNotNullAndRightSize(matrix))
                throw new ArgumentException(nameof(matrix));

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                var xValues = new HashSet<int>();
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (!IsValueInRange(matrix[row, col]) || !xValues.Add(matrix[row, col]))
                        return false;
                }
            }

            return true; 
        }

        private bool IsValueInRange(int value) => value > 0 && value <= 9;

        private bool IsNotNullAndRightSize(int[,] matrix) => !(matrix is null) && (matrix.GetLength(0) == 9) && (matrix.GetLength(1) == 9);
    }
}
