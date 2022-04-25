using System;
using SentenceComposer.Business.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SentenceComposer.Business.Services.Implementations
{
    public class SentenceComposerService: ISentenceComposerService
    {
        private IComposerService _composerService { get; set; }
        private ITextReaderService _readerService { get; set; }

        /// <summary>
        /// Delimiters are to split text by them
        /// </summary>
        private readonly char[] Delimiters = new char[] { ' ', '\r', '\n', ',', '-', '!', '.', ';', '\'', '\"', '`' };


        /// <summary>
        /// Create a new instance of sentence composer
        /// </summary>
        /// <param name="composerService">Takes Composer Service</param>
        /// <param name="readerService">Takes Reader Service</param>
        public SentenceComposerService()
        {
            _composerService = DIContainer.ServiceProvider.GetService<IComposerService>();
            _readerService = DIContainer.ServiceProvider.GetService<ITextReaderService>();
        }

        /// TODO: Rename method ComposeSentence 
        /// <inheritdoc/>
        public async Task<bool> ComposeSentenceAsync(string url, string userInput)
        {
            // Read text from web page
            // TODO: Make cache + Decorator with better Microsoft or Redis
            var rawText = await _readerService.ReadTextFromAsync(url);

            var words = rawText.Split(Delimiters);
            var targerWords = userInput.Split(Delimiters);

            return _composerService.CheckWordsFast(words, targerWords);
        } 
    }
}
