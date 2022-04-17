using NUnit.Framework;
using AutoFixture;
using SentenceComposer.Business.Services.Implementations;
using System;
using System.Diagnostics;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Tests
{
    public class ComposerServiceFastPerformanceTests
    {
        private IComposerService _composerService { get; set; }
        private IFixture _fixture { get; set; }
        //private string[] _baseString = { "Мама", "мыла", "раму" };

        [SetUp]
        public void Setup()
        {
            _composerService = new ComposerService();
            _fixture = new Fixture();
        }

        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(1_000, 50, 100)]
        public void Composer_ComposeSentenceFast_Performance(int baseSize, int targetSize, int expectedTime)
        {
            var baseString = new string[baseSize];
            var targetString = new string[targetSize];

            for (int i = 0; i < targetString.Length; i++)
                targetString[i] = _fixture.Create<string>();

            for (int i = 0; i < baseString.Length; i++)
                baseString[i] = _fixture.Create<string>();

            for (int i = 0; i < targetString.Length; i++)
                baseString[new Random().Next(baseSize - 1)] = targetString[i];

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000; i++)
                _composerService.CheckWordsFast(baseString, targetString);
            watch.Stop();

            Assert.LessOrEqual(watch.Elapsed.Milliseconds, expectedTime);
        }

        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(1_000, 50, 1_000)]
        public void Composer_ComposeSentenceLINQ_Performance(int baseSize, int targetSize, int expectedTime)
        {
            var baseString = new string[baseSize];
            var targetString = new string[targetSize];

            for (int i = 0; i < targetString.Length; i++)
                targetString[i] = _fixture.Create<string>();

            for (int i = 0; i < baseString.Length; i++)
                baseString[i] = _fixture.Create<string>();

            for (int i = 0; i < targetString.Length; i++)
                baseString[new Random().Next(baseSize - 1)] = targetString[i];

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000; i++)
                _composerService.CheckWordsLINQ(baseString, targetString);
            watch.Stop();

            Assert.LessOrEqual(watch.Elapsed.Milliseconds, expectedTime);
        }
    }
}
