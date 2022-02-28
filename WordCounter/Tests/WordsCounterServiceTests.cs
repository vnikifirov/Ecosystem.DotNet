using System.Linq;
using Business.Services.Implementations;
using Business.Services.Interfaces;
using NUnit.Framework;

namespace Tests
{
    public class WordsCounterServiceTests
    {
        public IWordsCounterService _counterService { get; set; }

        [SetUp]
        public void Setup()
        {
            _counterService = new WordsCounterService();
        }

        [Test, TestCase("Software Software Development SimbirSoft Busines Busines Busines")]
        public void Counter_CountWords_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.AreEqual(4, result.Count);
        }

        [Test, TestCase("Software Software")]
        public void Counter_CountWordsSoftware_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Software", result.FirstOrDefault().Key);
            Assert.AreEqual(2, result.FirstOrDefault().Value);
        }

        [Test, TestCase("software Software Software software", false), ]
        public void Counter_CountWordsSoftwareIsNoRegisterSensitive_ShouldOk(string text, bool IsRegisterSensitive)
        {
            var result = _counterService.CountAllWordsIn(text, IsRegisterSensitive);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Software", result.FirstOrDefault().Key);
            Assert.AreEqual(4, result.FirstOrDefault().Value);
        }

        [Test, TestCase("software Software Software software", true),]
        public void Counter_CountWordsSoftwareIsRegisterSensitive_ShouldOk(string text, bool IsRegisterSensitive)
        {
            var result = _counterService.CountAllWordsIn(text, IsRegisterSensitive);

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Software", result.FirstOrDefault().Key);
            Assert.AreEqual(2, result.FirstOrDefault().Value);
            Assert.AreEqual(2, result.LastOrDefault().Value);
        }

        [Test, TestCase("Software, Software, Development, SimbirSoft, Busines, Busines, Busines!")]
        public void Counter_CountWordsWithPunctuation_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.AreEqual(4, result.Count);
        }


        [Test, TestCase("Software, Software.")]
        public void Counter_CountWordsSoftwareWithPunctuation_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Software", result.FirstOrDefault().Key);
            Assert.AreEqual(2, result.FirstOrDefault().Value);
        }

        [Test, TestCase(" \r\n,-!.")]
        public void Counter_DontCountPunctuation_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test, TestCase(null)]
        public void Counter_CountWordsThenTextNull_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.Null(result);
        }

        [Test, TestCase("")]
        public void Counter_CountWordsThenTextEmpty_ShouldOk(string text)
        {
            var result = _counterService.CountAllWordsIn(text);

            Assert.Null(result);
        }
    }
}
