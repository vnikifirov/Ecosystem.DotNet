using System;
using System.Collections;
using System.Collections.Generic;

namespace MyIEnumerable
{
    class Program
    {
        static void Main(string[] args)
        {
            var enumerable = new MyIEnumerable();
            foreach (var item in enumerable)
            {
                Console.WriteLine($"Element: {item}");
            }

            Console.ReadKey();
        }
    }

    public class MyIEnumerable : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            return new MyEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyEnumerator();
        }
    }

    public class MyEnumerator : IEnumerator<int>
    {
        public int Current
        {
            get
            {
                return Elements[Index];
            }
        } 

        object IEnumerator.Current
        {
            get
            {
                return Elements[Index];
            }
        }

        private int Index = -1;

        private int[] Elements = new int[3] { 1, 2, 3 };

        public void Dispose()
        {
            Elements = null;
        }

        public bool MoveNext()
        {
            if (Index == 2)
                return false;
            else
            {
               Index++;
               return true;
            }
            
        }

        public void Reset()
        {
            Index = 0;
        }
    }
}
