using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace WebScrappingService.Tests.Units
{
    public class HtmlAgilityPackTextReaderServiceTests
    {
        private ITextReaderService _readerService { get; set; }

        [SetUp]
        public void Setup() => _readerService = new HtmlAgilityPackTextReaderService(null);

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

        [Test, TestCase("https//blabla.com")]
        public void Reader_ReadTextFromIncorrectString_ShouldTrue(string URL) => Assert.Throws<UriFormatException>(() => _readerService.ReadTextFrom(URL));

        [Test, TestCase("https//blabla.com")]
        public void Reader_ReadTextFromIncorrectStringAsync_ShouldTrue(string URL) => Assert.ThrowsAsync<UriFormatException>(async () => await _readerService.ReadTextFromAsync(URL));


        [Test, TestCase("https://coinmarketcap.com/currencies/solana/")]
        public void Reader_ReadTextFromIscorrectStringAsync_ShouldTrue(string URL) => Assert.DoesNotThrowAsync(async () => await _readerService.ReadTextFromAsync(URL));

        [Test, TestCase("https://coinmarketcap.com/currencies/solana/")]
        public async Task Reader_ReadTextFromStringAsync_ShouldBeNotEmpty(string URL)
        {
            await Task.Delay(1_000);
            var text = await _readerService.ReadTextFromAsync(URL);

            Assert.IsNotNull(text);
            Assert.IsNotEmpty(text);
            Assert.True(text.Length > 0);
        }
    }
}
