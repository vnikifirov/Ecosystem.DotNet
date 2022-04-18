using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Tests.Performance
{
    public class WebSiteTextReaderServicePerformanceTests
    {
        private ITextReaderService _readerService { get; set; }
        public string URL { get; set; } = "http://yandex.ru";

        [SetUp]
        public void Setup() => _readerService = new WebSiteTextReaderService();

        [Test, TestCase(300)]
        public async Task Reader_ReadTextFromAsync_Performance(int expectedTime)
        {
            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 10; i++)
                await _readerService.ReadTextFromAsync(URL);
            watch.Stop();

            Assert.LessOrEqual(watch.Elapsed.Milliseconds, expectedTime);
        }
    }
}
