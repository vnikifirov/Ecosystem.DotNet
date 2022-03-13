using System;
using System.Collections.Generic;

namespace ReverseLinkedList
{
    class Program
    {
        static void Main(string[] args)
        {
            var linkedList = new LinkedList<int>(new int[] { 1, 2, 3, 4, 5 });
            var resultFirst = ReverseLinkedList.Reverse(linkedList);
            var resultSecond = ReverseLinkedList.ReverseWithoutMemory(linkedList);

            Console.WriteLine($"Result first: {resultFirst}");
            Console.WriteLine($"Result second: {resultSecond}");
            Console.ReadKey();
        }
    }

    public static class ReverseLinkedList
    {
        public static LinkedList<T> Reverse<T>(LinkedList<T> linkedList)
        {
            // Space complicity O(N) Additional space complicity O(N)
            var innerLinkedList = new LinkedList<T>();
            foreach (var elem in linkedList)
            {
                innerLinkedList.AddFirst(elem);
            }

            return innerLinkedList;
        }

        public static LinkedList<T> ReverseWithoutMemory<T>(LinkedList<T> linkedList)
        {
            var first = linkedList.First;
            var last = linkedList.Last;

            // Time complicity O(N / 2) Space complicity O(1)
            for (int i = 0; i < linkedList.Count / 2; i++)
            {
                var tmpValue = last.Value;
                last.Value = first.Value;        
                first.Value = tmpValue;

                // node1(1nd iter current)  -> node2 (2nd iter current) -> node3
                first = first.Next;
                // nodeN(1nd iter current)  -> nodeN - 1 (2nd iter current) -> nodeN - 2
                last = last.Previous;
            }

            return linkedList;
        }
    }
}
