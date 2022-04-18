using System.Threading.Tasks;

namespace SentenceComposer.Business.Services.Interfaces
{
    public interface ISentenceComposerService
    {
        /// <summary>
        /// Check from random user input can we compose sentence from our words which we have received from external WebService or not?
        /// </summary>
        public Task<bool> ComposeSentenceAsync(string url, string userInput);
    }
}
