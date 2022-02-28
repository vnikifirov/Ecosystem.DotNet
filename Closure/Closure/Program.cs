using System;
using System.Collections.Generic;

namespace Closure
{
    class Program
    {
        static void Main(string[] args)
        {
            var instatance = new Closure();

            instatance.EnumerateClosure();

            Console.WriteLine("---------------------------------");

            instatance.EnumerateNotClosure();

            Console.ReadLine();
        }
    }

    public class Closure
    {
        public void EnumerateClosure()
        {
            var actions = new List<Action>();
            for (var i = 0; i < 3; i++)
            {
                actions.Add(() => Console.WriteLine(i));
            }

            foreach (var action in actions)
            {
                action();
            }
        }

        public void EnumerateNotClosure()
        {
            var actions = new List<Action>();
            for (var i = 0; i < 3; i++)
            {
                var j = i;
                actions.Add(() => Console.WriteLine(j));
            }

            foreach (var action in actions)
            {
                action();
            }
        }
    }
}
