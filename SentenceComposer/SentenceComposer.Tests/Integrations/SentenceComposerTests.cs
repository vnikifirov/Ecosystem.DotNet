using Moq;
using System.Threading.Tasks;
using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Tests.Integration
{
    public class SentenceComposerTests
    {
        private IComposerService _composerService { get; set; }
        private ITextReaderService _readerService { get; set; }

        public string URL { get; set; } = "http://rulyrics.ru/ru/b/belina_sasha/mama_myla_ramu.html";
        private readonly char[] Delimiters = new char[] { ' ', '\r', '\n', ',', '-', '!', '.', ';', '\'', '\"', '`' };

        [SetUp]
        public void Setup()
        {
            _composerService = new ComposerService();
            _readerService = new HtmlAgilityPackTextReaderService();
        }
        
        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(new object[] { "Мама", "мыла", "раму" })]
        public async Task SentenceComposer_ComposeSentence_ShouldTrue(params string[] targetString)
        {
            var baseString = await _readerService.ReadTextFromAsync(URL);
            var result = _composerService.CheckWordsFast(baseString.Split(Delimiters), targetString);

            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(new object[] { "Мама", "мыла", "раму" })]
        public async Task SentenceComposer_ComposeSentenceWithoutComposerService_ShouldTrue(params string[] targetString)
        {
            await Task.Delay(1_000);
            var baseString = await _readerService.ReadTextFromAsync(URL);
            var composerService = GetComposerService();
            var result = composerService.CheckWordsFast(baseString.Split(Delimiters), targetString);

            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(new object[] { "Мама", "мыла", "раму" })]
        public async Task SentenceComposer_ComposeSentenceWithoutTextReaderService_ShouldTrue(params string[] targetString)
        {
            var readerService = GetTextReaderService();
            var baseString = await readerService.ReadTextFromAsync(URL);
            var result = _composerService.CheckWordsFast(baseString.Split(Delimiters), targetString);

            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        private ITextReaderService GetTextReaderService()
        {
            var mock = new Mock<ITextReaderService>();
            mock.Setup(x => x.ReadTextFromAsync(It.IsAny<string>()))
                .ReturnsAsync("Мама мыла раму");
            return mock.Object;
        }

        private IComposerService GetComposerService()
        {
            var mock = new Mock<IComposerService>();
            mock.Setup(x => x.CheckWordsFast(It.IsAny<string[]>(), It.IsAny<string[]>()))
                .Returns(true);
            return mock.Object;
        }
    }
}
