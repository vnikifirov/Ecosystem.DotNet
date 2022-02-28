using System.Collections.Generic;
using Business.Services.Interfaces;
using System.Linq;
using System;

namespace Business.Services.Implementations
{
    /// <inheritdoc/>
    public class WordsCounterService : IWordsCounterService
    {
        /// <summary>
        /// Delimiters are to split text by them
        /// </summary>
        public char[] Delimiters { get; set; } = new char[] { ' ', '\r', '\n', ',', '-', '!', '.' };

        /// <inheritdoc/>
        public Dictionary<string, int> CountAllWordsIn(string text, bool isNoRegisterSensitive = true)
        {
            if (Delimiters.Length <= 0)
                return null;

            if (string.IsNullOrWhiteSpace(text))
                return null;

            /*if (!isNoRegisterSensitive)
                text = text.ToLower();*/

            var split_text = text.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

            var words = split_text
                .AsQueryable()
                .GroupBy(key => key)
                .Select(word => new KeyValuePair<string, int>(word.Key, word.Count()))
                .ToDictionary(key => key.Key, count => count.Value);

            return words;
        }
    }
}
