using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TimeZoneTest
{
    public class Conc
    {
        public List<int> intList = new List<int>(1000);
        public ConcurrentBag<int> intListConcurent = new();

        public List<string> words = new List<string> { "11", "22", "33", "44", "55", "66", "77", "88", "99", "00" };
        public Dictionary<string, string> dict = new();
        public ConcurrentDictionary<string, string> dictConcurent = new();

        public void GlobalAdd()
        {
            var task1 = Task.Run(() => AddValues());
            var task2 = Task.Run(() => AddValues());

            var taskC1 = Task.Run(() => AddValuesConcurent());
            var taskC2 = Task.Run(() => AddValuesConcurent());

            var taskDC1 = Task.Run(() => AddDictC());
            var taskDC2 = Task.Run(() => AddDictC());

            var taskD1 = Task.Run(() => AddDict());
            var taskD2 = Task.Run(() => AddDict());

            Task.WaitAll(task1, taskC1, task2, taskC2);
            Task.WaitAll(taskD1, taskDC1, taskD2, taskDC2);

            Console.WriteLine(intList.Count);
            Console.WriteLine(intListConcurent.Count);

            foreach(var word in dictConcurent.Keys)
            {
                Console.WriteLine(word + " " + dictConcurent[word]);
            }
            foreach (var word in dict.Keys)
            {
                Console.WriteLine(word + " " + dictConcurent[word]);
            }

            Console.ReadLine();
        }

        public void AddValues()
        {
            for (var i = 0; i < 200; i++)
                intList.Add(i);
        }
        public void AddValuesConcurent()
        {
            for (var i = 0; i < 200; i++)
            {
                intListConcurent.Add(i);
            }
        }
        public void AddDictC()
        {
            foreach(var word in words)
            {
                dictConcurent.TryAdd(word, Thread.CurrentThread.ManagedThreadId.ToString());
            }
        }
        public void AddDict()
        {
            foreach (var word in words)
            {
                dict.Add(word, Thread.CurrentThread.ManagedThreadId.ToString());
            }
        }
    }

}
