using _20180507_ClearScript_001.Helpers;
using Microsoft.ClearScript.V8;
using Microsoft.ClearScript.Windows;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace _20180507_ClearScript_001.Tests
{
    /// <summary>
    /// Summary: This is Unit Tests are releted to a following quastion
    /// Link: https://stackoverflow.com/questions/50218834/using-clearscript-inside-threads/50221675#50221675
    /// </summary>
    [TestFixture]
    public class V8ScriptEngineTests
    {
        private V8ScriptEngine _engine = null;

        [SetUp]
        public void TestSetup()
        {
            _engine = new V8ScriptEngine();
        }

        [Test]        
        public void Evaluated_ExpectedResult_2()
        {
            // Arrange
            var engine = new V8ScriptEngine();
            int actual = 0;
            int expected = 2;

            // Act
            try
            {
                actual = (int)engine.Evaluate("1+1");
            }
            finally
            {
                engine.Dispose();
            }

            // Expect
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Evaluating4000Records_ExpectedTime_200ms()
        {
            // Arrange
            var engine = new V8ScriptEngine();

            var exprected_time = TimeSpan.FromMilliseconds(200);
            var exprected_count = 4000;

            var actual_time = new TimeSpan();
            var actual_results = new Stack<int>(4000);

            // Act
            try
            {
                actual_time = Estimator.Estimate(() =>
                {
                    for (int i = 0; i < 4000; i++)
                    {
                        actual_results.Push((int)engine.Evaluate("1+1"));
                    }
                });
            }
            finally
            {
                engine.Dispose();
            }

            // Expect            
            Assert.AreEqual(exprected_count, actual_results.Count);
            Assert.That(actual_time, Is.LessThanOrEqualTo(exprected_time));
        }

        [Test]
        public void Evaluating4000Records_ExpectedTime_100ms()
        {
            // Arrange
            var vbengines = new VBScriptEngine[Environment.ProcessorCount];
            var checkPoint = new ManualResetEventSlim();

            var exprected_time = TimeSpan.FromMilliseconds(100);
            var exprected_count = Environment.ProcessorCount;

            var actual_time = new TimeSpan();

            // Act
            for (var index = 0; index < vbengines.Length; ++index)
            {
                var thread = new Thread(indexArg =>
                {
                    using (var engine = new VBScriptEngine())
                    {
                        vbengines[(int)indexArg] = engine;
                        checkPoint.Set();
                        Dispatcher.Run();
                    }
                });

                thread.Start(index);
                checkPoint.Wait();
                checkPoint.Reset();
            }

            Parallel.ForEach(Enumerable.Range(0, 4000), item => {
                var engine = vbengines[item % vbengines.Length];

                engine.Dispatcher.Invoke(() => {
                    ThreadedFunc(new Myobj() { Vbengine = engine, Index = item });
                });
            });

            Array.ForEach(vbengines, engine => engine.Dispatcher.InvokeShutdown());

            // Expect
            Assert.AreEqual(exprected_count, vbengines.Length);
            Assert.That(actual_time, Is.LessThanOrEqualTo(exprected_time));
        }

        static void ThreadedFunc(object obj)
        {
            Console.WriteLine(((Myobj)obj).Index.ToString() + ": " + ((Myobj)obj).Vbengine.Evaluate("1+1").ToString());
        }

        internal class Myobj
        {
            public VBScriptEngine Vbengine { get; set; }
            public int Index { get; set; }
        }
    }
}
