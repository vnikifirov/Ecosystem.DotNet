using System;
using System.Collections.Concurrent;

namespace TimeZoneTest
{
    public class Robot
    {
        public int GetPath(int m, int n)
        {
            if (n < 1 || m < 1)
                return 0;
            if (n == 1 && m == 1)
                return 1;
            return GetPath(m - 1, n) + GetPath(m, n - 1);
        }
        public int GetPathMemoize(int m, int n, int[][] arr)
        {
            if (n < 1 || m < 1)
                return 0;
            if (n == 1 && m == 1)
                return 1;

            if (arr[m][n] != 0)
                return arr[m][n];

            arr[m][n] = GetPathMemoize(m - 1, n, arr) + GetPathMemoize(m, n - 1, arr);
            return arr[m][n];
        }
        public static Func<int, int, int> GetPathF = (int m, int n) =>
        {
            if (n < 1 || m < 1)
                return 0;
            if (n == 1 && m == 1)
                return 1;
            return GetPathF(m - 1, n) + GetPathF(m, n - 1);
        };
        public static Func<int, int, int> GetPathFMemoize => GetPathF.Memoize();
        public static Func<int, int, int> GetPathFMemoizeLazy => GetPathF.LazyMemoize();
    }

    public static class Ext
    {
        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        {
            var cache = new ConcurrentDictionary<Tuple<T1, T2>, TResult>();
            return (a, b) =>
            {
                var tuple = new Tuple<T1, T2>(a, b);
                var cached = cache.GetOrAdd(tuple, f(a, b));
                return cached;
            };
        }
        public static Func<T1, T2, TResult> LazyMemoize<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        {
            var cache = new ConcurrentDictionary<Tuple<T1, T2>, Lazy<TResult>>();
            return (a, b) =>
            {
                var tuple = new Tuple<T1, T2>(a, b);
                var cached = cache.GetOrAdd(tuple, new Lazy<TResult>(() => f(a, b))).Value;
                return cached;
            };
        }
    }
}
