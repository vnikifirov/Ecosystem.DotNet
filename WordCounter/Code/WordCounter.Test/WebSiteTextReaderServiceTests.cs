using NUnit.Framework;
using Business.Services.Interfaces;
using Business.Services.Implementations;

namespace Tests
{
    public class WebSiteTextReaderServiceTests
    {
        public ITextReaderService _readerService { get; set; }

        [SetUp]
        public void Setup()
        {
            _readerService = new WebSiteTextReaderService();
        }

        [Test, TestCase("https://www.simbirsoft.com/en/")]
        public void Reader_ReadTextFromURL_ShouldOk(string url)
        {
            var result = _readerService.ReadTextFrom(url);

            Assert.NotNull(result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Length > 0);
        }

        [Test, TestCase("https://www.simbirsoft.com/en/", "Software")]
        public void Reader_ReadTextFromURLAndSearchForSpecificWord_ShouldOk(string url, string word)
        {
            var result = _readerService.ReadTextFrom(url);

            Assert.NotNull(result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Length > 0);
            Assert.IsTrue(result.ToLower().Contains(word.ToLower()));
        }

        [Test, TestCase("")]
        public void Reader_ReadTextFromEmptySource_ShouldOk(string url)
        {
            var result = _readerService.ReadTextFrom(url);

            Assert.NotNull(result);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Length == 0);
        }

        [Test, TestCase(null)]
        public void Reader_ReadTextFromNullSource_ShouldOk(string url)
        {
            var result = _readerService.ReadTextFrom(url);

            Assert.NotNull(result);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Length == 0);
        }
    }
}
