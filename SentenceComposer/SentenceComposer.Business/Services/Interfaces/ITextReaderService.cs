using System;
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
    }
}
