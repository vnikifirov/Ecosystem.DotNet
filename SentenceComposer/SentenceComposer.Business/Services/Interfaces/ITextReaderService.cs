using System.Threading.Tasks;

namespace SentenceComposer.Business.Services.Interfaces
{
    /// <summary>
    /// TextReaderService
    /// </summary>
    public interface ITextReaderService
    {
        /// <summary>
        /// Read any text from specific source (file path, URL or etc)
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Any text</returns>
        public string ReadTextFrom(string source);

        /// <summary>
        /// Asynchronosly read any text from specific source (file path, URL or etc)
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Any text</returns>
        public Task<string> ReadTextFromAsync(string source);
    }
}
