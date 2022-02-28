using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest
{
    public static class Fibonacci
    {
        // 1 итерация - 1 операции(сложение),
        // 2 итерация - 2 операции
        // 3 итерация - 4 операции
        // ...
        // n итерация - 2^n операций - O(2^n) ~ O(e^n) - экспоненциальная сложность n = 10  - 1024
        // много памяти из-за создания функций в стеке
        public static Func<long, long> FibonacciRec = (n) => n > 2 ? FibonacciRec(n - 1) + FibonacciRec(n - 2) : 1;

        // линейная рекурсия - каждый вызов порождает 1 вызов, а не 2
        public static long FibonacciCache(long n, int[] mass)
        {
            if (mass[mass.Length - 1] == 0)
            {
                mass[n - 2] = 1;
                mass[n - 1] = 1;
            }
            if (n > 2)
            {
                mass[n - 3] = mass[n - 2] + mass[n - 1];
                return FibonacciCache(n - 1, mass);
            }
            else
            {
                return mass[0];
            }
        }



        public static long FibonacciLin(long n) // 3*n операций - O(n) - 1 цикл - линейная сложность n = 10 - 30
        {
            if (n <= 2) return 1;

            var f1 = 1L;
            var f2 = 1L;

            for (var i = 0; i < n - 2; i++) // n
            {
                var tempF = F(f1, f2);
                f1 = f2;
                f2 = tempF;
            }

            return f2;
        }

        static Func<long, long, long> F = (n, m) => n + m;
    }
    
}

// m * n - полный перебор , m ~ n
// m - строка 
// n - массив