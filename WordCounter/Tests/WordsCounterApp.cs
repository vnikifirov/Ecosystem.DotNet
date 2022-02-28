using Business.Services.Implementations;
using Business.Services.Interfaces;
using NUnit.Framework;

namespace Tests
{
    public class WordsCounterApp
    {
        public ITextReaderService _readerService { get; set; }
        public IWordsCounterService _counterService { get; set; }

        [SetUp]
        public void Setup()
        {
            _readerService = new WebSiteTextReaderService();
            _counterService = new WordsCounterService();
        }

        [Test, TestCase("https://www.simbirsoft.com/en/")]
        public void WordsCounter_CountWordsFromUrl_ShouldOk(string url)
        {
            var text = _readerService.ReadTextFrom(url);

            Assert.NotNull(text);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(text));
            Assert.IsTrue(text.Length > 0);

            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        // TODO: SOFTWARE By Google Find on the Page should be - 11 
        [Test, TestCase("Software", 11, "https://www.simbirsoft.com/en/")]
        public void WordsCounter_CountSpecificWordFromUrl_ShouldOk(string target, int numWords, string url)
        {
            var text = _readerService.ReadTextFrom(url);

            Assert.NotNull(text);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(text));
            Assert.IsTrue(text.Length > 0);

            var result = _counterService.CountAllWordsIn(text);

            Assert.NotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(numWords, result[target]);
        }
    }
}
