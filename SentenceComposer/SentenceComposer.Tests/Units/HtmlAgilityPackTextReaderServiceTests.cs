using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Tests.Unit
{
    public class HtmlAgilityPackTextReaderServiceTests
    {
        private ITextReaderService _readerService { get; set; }

        [SetUp]
        public void Setup()
        {
            _readerService = new HtmlAgilityPackTextReaderService();
        }

        [Test]
        public async Task Reader_ReadTextFromEmptyStringAsync_ShouldTrue()
        {
            var emptyURL = string.Empty;
            var text = await _readerService.ReadTextFromAsync(emptyURL);

            Assert.NotNull(text);
            Assert.IsEmpty(text);
        }

        [Test]
        public async Task Reader_ReadTextFromNullStringAsync_ShouldTrue()
        {
            var text = await _readerService.ReadTextFromAsync(null);

            Assert.NotNull(text);
            Assert.IsEmpty(text);
        }

        [Test]
        public void Reader_ReadTextFromIncorrectString_ShouldTrue()
        {
            var incorrectURL = "https//blabla.com";

            Assert.Throws<UriFormatException>(() => _readerService.ReadTextFrom(incorrectURL));
        }

        [Test]
        public void Reader_ReadTextFromIncorrectStringAsync_ShouldTrue()
        {
            var incorrectURL = "https//blabla.com";

            Assert.ThrowsAsync<UriFormatException>(async () => await _readerService.ReadTextFromAsync(incorrectURL));
        }

        [Test]
        public void Reader_ReadTextFromIscorrectStringAsync_ShouldTrue()
        {
            var correctURL = "https://yandex.ru";

            Assert.DoesNotThrowAsync(async () => await _readerService.ReadTextFromAsync(correctURL));
        }

        [Test, TestCase("yandex")]
        public async Task Reader_ReadTextFromStringAsync_ShouldContainWord(string targetWord)
        {
            await Task.Delay(1_000);
            var correctURL = "https://yandex.ru";

            var text = await _readerService.ReadTextFromAsync(correctURL);

            Assert.IsNotNull(text);
            Assert.IsNotEmpty(text);
            Assert.True(text.Contains(targetWord));
        }
    }
}
