﻿using static System.Console;
using Sudoku.Business.Implementations;
using Sudoku.Business.Interfaces;

namespace Sudoku.Console
{
    class Program
    {
        private static readonly ISudokuServiceHasSet _sudokuServiceHasSet = new SudokuService();
        //private static readonly ISudokuServiceLINQ _sudokuServiceLINQ = new SudokuService();

        static void Main(string[] args)
        {
            var matrix = new int[9, 9]
            {
                { 2, 4, 9, 5, 7, 6, 1, 8, 3 },
                { 8, 3, 5, 4, 1, 2, 7, 6, 9 },
                { 6, 7, 1, 3, 8, 9, 5, 2, 4 },
                { 3, 6, 8, 2, 4, 1, 9, 5, 7 },
                { 7, 9, 2, 8, 5, 3, 4, 1, 6 },
                { 1, 5, 4, 6, 9, 7, 8, 3, 2 },
                { 5, 1, 6, 9, 3, 4, 2, 7, 8 },
                { 4, 2, 7, 1, 6, 8, 3, 9, 5 },
                { 9, 8, 3, 7, 2, 5, 6, 4, 1 },
            };

            
            var isSolvedHasSet = _sudokuServiceHasSet.IsRightFilled(matrix);
            //var isSolvedLINQ = _sudokuServiceLINQ.IsRightFilled(matrix);

            WriteLine($"Sudoku HasSet - Is the puzzle solved? (true/false): {isSolvedHasSet}");
            //WriteLine($"Sudoku LINQ - Is the puzzle solved? (true/false): {isSolvedLINQ}");
            ReadKey();
        }
    }
}
