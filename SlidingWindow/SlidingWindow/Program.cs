using System;

namespace SlidingWindow
{
    /// <summary>
    /// Дается массив 10 тыс елементов и еще некое целое число - N. N у нас меньше длинны массива.
    /// Можно взять и определить сумму N последовательных чисел, например, от 0 до N, или от 1 до N + 1 и тд
    /// N и массив определить наибольшую из Sum последовательных чисел.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var arr = new int[] { 4, 8, 2, 10, 4, 5 }; // L = 6x elem
            // Кол-во елементов в послеовательности
            var num = 3; // N = 3x elem

            // L = 2N then is O(N)
            var firstRes = Sliding.GetMaxSequence(arr, num);
            var secondRes = Sliding.GetMaxSequenceBySliding(arr, num);

            Console.WriteLine($"Наибольшая из Sum последовательных чисел: {firstRes}");
            Console.WriteLine($"Наибольшая из Sum последовательных чисел: {secondRes}");
            Console.ReadKey();
        }
    }

    public class Sliding
    {
        public static int GetMaxSequence(int[] arr, int num)
        {
            if (arr.Length < num || num <= 0)
                throw new ArgumentOutOfRangeException();
           
            var maxSumSeq = 0;
            // массив на Length елементов
            // Time complicity - best case O(L - N) worst case L = 2N then is O(N)
            for (int i = 0; i < arr.Length - num; i++)
            {
                var sumSeq = 0;
                // последовательных чисел N, от 0 до N или от 1 до N + 1 и тд

                // Time complicity - best case O(N)
                for (int j = i; j < num + i; j++)
                {
                    sumSeq += arr[j];
                }

                // O(1)
                maxSumSeq = Math.Max(maxSumSeq, sumSeq);
            }
            
            // Time complicity - best case O(L - N) * N worst case O(N^2)
            return maxSumSeq;
        }

        public static int GetMaxSequenceBySliding(int[] arr, int num)
        {
            if (arr.Length < num || num <= 0)
                throw new ArgumentOutOfRangeException();

            var maxSumSeq = 0;
            // Time complicity - best case O(N) worst case O(N)
            for (int i = 0; i < num; i++)
            {
                maxSumSeq += arr[i];
            }

            var localSum = maxSumSeq;
            // Time complicity - best case O(L - N) worst case O(L - N)
            for (int i = 1; i < arr.Length - num; i++)
            {
                localSum = localSum + arr[i + (num - 1)];
                localSum = localSum - arr[i - 1];

                maxSumSeq = Math.Max(maxSumSeq, localSum);
            }

            // Time complicity - best case O(L) worst case O(N)
            return maxSumSeq;
        }
    }
}
