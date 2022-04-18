using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Tests.Performance
{
    public class HtmlAgilityPackTextReaderServicePerformanceTests
    {
        private ITextReaderService _readerService { get; set; }
        public string URL { get; set; } = "http://yandex.ru";

        [SetUp]
        public void Setup() => _readerService = new HtmlAgilityPackTextReaderService();

        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(350)]
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
