using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TimeZoneTest
{
    public class Sem
    {
        private const int taskCount = 1000;

        private Task[] tasks = new Task[taskCount];

        //private static Semaphore semaphore = new Semaphore(1, 1, "Global_sema");
        private static SemaphoreSlim semaphore = new SemaphoreSlim(10, 10);

        private int xSync;
        private X xAsync = new X { sum = 0 };

        public void Count()
        {
            for (var i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    int semephoreCount;

                    await semaphore.WaitAsync();

                    // get info from site

                    //semaphore.WaitOne();

                    //lock(semaphore)
                    //{
                        xAsync.sum += i;
                        await Task.Delay(100);// do long work
                    //}
                    
                    try
                    {
                        Interlocked.Increment(ref xSync);

                        await Task.Delay(300);
                    }
                    finally
                    {
                        semephoreCount = semaphore.Release();
                    }

                    Console.WriteLine(semephoreCount);
                });
            }

            Thread.Sleep(500);

            Console.WriteLine("semaphore count: " + semaphore.CurrentCount);

            Task.WaitAll(tasks);

            Console.WriteLine("sync: " + xSync);
            Console.WriteLine("async: " + xAsync.sum);
        }
    }
}
