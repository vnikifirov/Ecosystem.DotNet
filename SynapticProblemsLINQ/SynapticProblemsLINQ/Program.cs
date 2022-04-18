using System;
using System.Linq;
using System.Collections.Generic;

namespace SynapticProblemsLINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 18. Задача на знание LINQ: из исходной коллекции данных следует получить новую 
             * коллекцию по определенным требованиям (например, отсортированную, 
             * без дубликатов и т.п.).
             */
            var values = new List<int>(){ 1, 2, 3, 3, 2, 1 };
            values
                .Distinct()
                .OrderBy(value => value);

            /*
             * 20.Реализуйте свой Select, Where, SelectMany со всеми характеристикам
             * и LINQ(отложенное исполнение, сиюминутная (eager) проверка входных данных).
             */
            var result = values.CustomWhere( value => value == 3);
        }
    }

    static class CustomLINQ
    {
        public static IEnumerable<T> CustomWhere<T>(this IEnumerable<T> elements, Func<T, bool> func)
        {
            // Cheacks
            if (func.IsNull())
                throw new ArgumentNullException(nameof(func));

            if (elements.IsNull())
                throw new ArgumentNullException(nameof(elements));

            // Algo
            foreach (var item in elements)
            {
                if (func(item))
                    yield return item;
            }
        }

        public static T CustomFirst<T>(this IEnumerable<T> elements, Func<T, bool> func)
        {
            // Cheacks
            if (func.IsNull())
                throw new ArgumentNullException(nameof(func));

            if (elements.IsNull())
                throw new ArgumentNullException(nameof(elements));

            // Algo
            foreach (var item in elements)
            {
                if (func(item))
                    return item;
            }

            throw new InvalidOperationException();
        }

        public static T CustomFirstOrDefault<T>(this IEnumerable<T> elements, Func<T, bool> func)
        {
            // Cheacks
            if (func.IsNull())
                throw new ArgumentNullException(nameof(func));

            if (elements.IsNull())
                throw new ArgumentNullException(nameof(elements));

            // Algo
            foreach (var item in elements)
            {
                if (func(item))
                    return item;
            }

            return default(T);
        }

        public static bool IsNull<T>(this T value) => value is null;
    }
}
