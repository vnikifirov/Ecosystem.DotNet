using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;

namespace SentenceComposerTests
{
    public class ComposerServiceTests
    {
        private ComposerService _composerService { get; set; }
        private string[] _baseString = { "Мама", "мыла", "раму" };

        [SetUp]
        public void Setup()
        {
            _composerService = new ComposerService();
        }

        // How pass array in TestCase - https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
        [Test, TestCase(new object[] { "Мама", "мыла", "раму" })]
        public void Composer_ComposeSentence_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWords(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        [Test, TestCase(new object[] { "Вася", "ел", "кашу" })]
        public void Composer_ComposeSentence_ShouldFalse(params string[] targetString)
        {
            var result = _composerService.CheckWords(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test, TestCase(new object[] { "Вася", "чистил", "раму" })]
        public void Composer_ComposeSentenceFromLastWord_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWords(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test, TestCase(new object[] { "Мама", "чистил", "раму" })]
        public void Composer_ComposeSentenceFromFirstAndLastWord_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWords(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }
        
        [Test, TestCase(new object[] { "Мама", "мыла", "стол" })]
        public void Composer_ComposeSentenceFromFirstSecondWord_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWords(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }
    }
}