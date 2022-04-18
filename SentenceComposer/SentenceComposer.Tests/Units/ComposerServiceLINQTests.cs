using System;
using NUnit.Framework;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Tests.Unit
{
    public class ComposerServiceLINQTests
    {
        private IComposerService _composerService { get; set; }
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
            var result = _composerService.CheckWordsLINQ(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        [Test, TestCase(new object[] { "Вася", "ел", "кашу" })]
        public void Composer_ComposeSentenceIsnotIncluded_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWordsLINQ(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test, TestCase(new object[] { "Вася", "чистил", "раму" })]
        public void Composer_ComposeSentenceFromLastWord_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWordsLINQ(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test, TestCase(new object[] { "Мама", "чистил", "раму" })]
        public void Composer_ComposeSentenceFromFirstAndLastWord_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWordsLINQ(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }
        
        [Test, TestCase(new object[] { "Мама", "мыла", "стол" })]
        public void Composer_ComposeSentenceFromFirstSecondWord_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWordsLINQ(_baseString, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Composer_ComposeSentenceNullArgTargetWords_ShouldTrue()
        {
            var result = _composerService.CheckWordsLINQ(_baseString, null);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Composer_ComposeSentenceEmptyArgTargetWords_ShouldTrue()
        {
            var result = _composerService.CheckWordsLINQ(_baseString, Array.Empty<string>());

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test, TestCase(new object[] { "Мама", "мыла", "раму" })]
        public void Composer_ComposeSentenceEmptyArgBaseWords_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWordsLINQ(Array.Empty<string>(), targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test, TestCase(new object[] { "Мама", "мыла", "раму" })]
        public void Composer_ComposeSentenceNullArgBaseWords_ShouldTrue(params string[] targetString)
        {
            var result = _composerService.CheckWordsLINQ(null, targetString);

            Assert.NotNull(result);
            Assert.IsFalse(result);
        }
    }
}