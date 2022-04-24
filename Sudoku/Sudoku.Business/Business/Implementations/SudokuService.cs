using System;
using Sudoku.Business.Interfaces;
using System.Collections.Generic;

namespace Sudoku.Business.Implementations
{
    public class SudokuService : ISudokuServiceHasSet
    {
        /// <summary>
        /// Determine if a 9x9 Sudoku board is valid.
        /// </summary>
        /// <param name="matrix">Sudoku board</param>
        /// <returns>Time complicity: worst case O(N^2) best case O(1), Space complicty: O(N)</returns>
        public bool IsRightFilled(int[,] matrix)
        {
            if (!IsNotNullAndRightSize(matrix))
                throw new ArgumentException(nameof(matrix));

            // Y-axis
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                // X-axis
                var xValues = new HashSet<int>();
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (!IsValueInRange(matrix[row, col]) || !xValues.Add(matrix[row, col]))
                        return false;
                }
            }

            // X-axis
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                // Y-axis
                var yValues = new HashSet<int>();
                for (int row = 0; row < matrix.GetLength(0); row++)
                {
                    if (!IsValueInRange(matrix[row, col]) || !yValues.Add(matrix[row, col]))
                        return false;
                }
            }


            return true; 
        }

        private bool IsValueInRange(int value) => value > 0 && value <= 9;

        private bool IsNotNullAndRightSize(int[,] matrix) => !(matrix is null) && (matrix.GetLength(0) == 9) && (matrix.GetLength(1) == 9);
    }
}
