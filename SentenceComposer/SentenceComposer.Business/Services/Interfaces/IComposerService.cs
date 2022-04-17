using System;
namespace SentenceComposer.Business.Services.Interfaces
{

    /// <summary>
    /// Sentence composer 
    /// </summary>
    public interface IComposerService
    {
        /// <summary>
        /// Check can we compose sentence from our words or not?
        /// </summary>
        /// <param name="baseWords">это наши вырезаные слова например из книги, памяти, файла, с web page или any service</param>
        /// <param name="target">это какое-то предолжение например "Мама мыла раму"</param>
        /// <returns></returns>
        public bool CheckWordsLINQ(string[] baseWords, string[] target);

        /// <summary>
        /// Check can we compose sentence from our words or not?
        /// </summary>
        /// <param name="baseWords">это наши вырезаные слова например из книги, памяти, файла, с web page или any service</param>
        /// <param name="target">это какое-то предолжение например "Мама мыла раму"</param>
        /// <returns>O(N + M)</returns>
        public bool CheckWordsFast(string[] baseWords, string[] target);
    }
}
