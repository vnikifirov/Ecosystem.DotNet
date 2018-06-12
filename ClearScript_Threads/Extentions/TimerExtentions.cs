
using System;
using System.Diagnostics;

namespace _20180507_ClearScript_001.Helpers
{
    public static class Estimator
    {
        public static TimeSpan Estimate(Action toTime)
        {
            // Arrange
            var timer = Stopwatch.StartNew();

            // Act
            toTime();
            timer.Stop();

            // Result
            return timer.Elapsed;
        }
    }
}
