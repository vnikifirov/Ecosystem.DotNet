using System;
using NUnit.Framework;
using Sudoku.Business.Implementations;
using Sudoku.Business.Interfaces;

namespace Sudoku.Tests.Unit
{
    public class SudokuServiceTests
    {
        private ISudokuServiceHasSet _sudokuServiceHasSet;
        private int[,] _correctMatrix = new int[9, 9]
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


        private int[,] _inCorrectMatrix = new int[9, 9]
            {
                { 2, 4, 9, 5, 7, 6, 1, 8, 3 },
                { 10, 3, 5, 4, 1, 2, 7, 6, 9 },
                { 6, 7, 1, 3, 8, 9, 5, 2, 4 },
                { 3, 6, 8, 2, 4, 1, 9, 5, 7 },
                { 7, 9, 2, 8, 5, 3, 4, 1, 6 },
                { 1, 5, 4, 6, 9, 7, 8, 3, 2 },
                { 5, 1, 6, 9, 3, 4, 2, 7, 8 },
                { 4, 2, 7, 1, 6, 8, 3, 9, 5 },
                { 9, 8, 3, 7, 2, 5, 6, 4, 1 },
            };

        private int[,] _inCorrectMatrixWithSimilarCol = new int[9, 9]
            {
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            };

        [SetUp]
        public void Setup() => _sudokuServiceHasSet = new SudokuService();

        [Test]
        public void Sudoku_IsRightFilled_ShouldBeTrue()
        {
            var result = _sudokuServiceHasSet.IsRightFilled(_correctMatrix);

            Assert.IsTrue(result);
        }

        [Test]
        public void Sudoku_IsRightFilledOutOfRangeArg_ShouldBeFalse()
        {
            var result = _sudokuServiceHasSet.IsRightFilled(_inCorrectMatrix);

            Assert.IsFalse(result);
        }

        [Test]
        public void Sudoku_IsRightFilledSimiliarCols_ShouldBeFalse()
        {
            var result = _sudokuServiceHasSet.IsRightFilled(_inCorrectMatrixWithSimilarCol);

            Assert.IsFalse(result);
        }


        [Test]
        public void Sudoku_isRightFilledNullArg_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sudokuServiceHasSet.IsRightFilled(null));
        }

        [Test, TestCase(2, 2)]
        public void Sudoku_isRightFilledNotRightSize_ShouldThrowArgumentException(int row, int col)
        {
            Assert.Throws<ArgumentException>( () => _sudokuServiceHasSet.IsRightFilled(new int[row, col]));
        }

        [Test, TestCase(9, 9)]
        public void Sudoku_isRightFilledEmptyArray_ShouldBeFalse(int row, int col)
        {
            var result = _sudokuServiceHasSet.IsRightFilled(new int[row, col]);

            Assert.IsFalse(result);
        }

    }
}
