using System.Collections.Generic;
using System.Linq;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    /// <inheritdoc/>
    public class ComposerService : IComposerService
    {
        private string[] _baseWords { get; set; }
        private string[] _target { get; set; }

        public ComposerService() { }

        /// <summary>
        /// Create a new instance of ComposerService
        /// </summary>
        /// <param name="baseWords">это наши вырезаные слова например из книги, памяти, файла, с web page или any service</param>
        /// <param name="target">это какое-то предолжение например "Мама мыла раму"</param>
        public ComposerService(string[] baseWords, string[] target)
        {
            _baseWords = baseWords;
            _target = target;
        }

        /// <inheritdoc/>
        public bool CheckWordsFast(string[] baseWords, string[] target)
        {
            // Return False if we haven't received any words from eg book, web page or web sevice or haven't any target sentence 
            if (IsNullOrEmpty(baseWords) || IsNullOrEmpty(target))
                return false;

            var dTarget = new Dictionary<string, int>();

            foreach (var word in target) // O(N)
            {
                if (dTarget.ContainsKey(word))
                    dTarget[word]++;
                else
                    dTarget.Add(word, 1);
                
            }

            foreach (var baseword in baseWords) // O(M)
            {
                var trimed = baseword.Trim();
                if (dTarget.ContainsKey(trimed))
                {
                    dTarget[trimed]--;
                    if (dTarget[trimed] == 0)
                        dTarget.Remove(trimed);
                }
            }

            return dTarget.Count == 0;
        }

        /// <inheritdoc/>
        public bool CheckWordsLINQ(string[] baseWords, string[] target)
        {
            // Return False if we haven't recived any words from eg book, web page or web sevice or haven't any target sentence 
            if (IsNullOrEmpty(baseWords) || IsNullOrEmpty(target))
                return false;

            // Extra information
            // About join on stackoverflow - https://stackoverflow.com/questions/11139326/how-to-join-two-tables-in-linq-using-wcf-data-service
            // About LINQ and Join or Inner Join operator - https://www.tutorialsteacher.com/linq/linq-joining-operator-join
            // About counting words occurrences eg in string array - https://stackoverflow.com/questions/13373359/how-to-count-word-occurrences-in-an-array-of-strings-using-c
            // About converting string arr to Dictionary - https://stackoverflow.com/questions/1385421/most-elegant-way-to-convert-string-array-into-a-dictionary-of-strings
            var countBaseWords = baseWords
                .GroupBy(key => key)
                .Select(word => new KeyValuePair<string, int>(word.Key, word.Count())) // Key is our word and Count is word occurrences in string array eg Мама => 5 раз
                .ToDictionary(key => key.Key, count => count.Value);

            var countTargetWords = target
                .GroupBy(key => key)
                .Select(word => new KeyValuePair<string, int>(word.Key, word.Count())) // Key is our word and Count is word occurrences in string array eg Мама => 1 раз
                .ToDictionary(key => key.Key, count => count.Value);

            var leftJoin = from targetWord
                            in countTargetWords
                            join baseWord
                            in countBaseWords
                            on targetWord.Key equals baseWord.Key
                            into groupJoin from subjoin in groupJoin.DefaultIfEmpty() // Если в наших baseWords нет targetWord он возьмет default
                            select new KeyValuePair<string, int>(targetWord.Key, subjoin.Value - targetWord.Value); 

            return leftJoin
                .ToDictionary(key => key.Key, count => count.Value)
                .All(word => word.Value >= 0); // Return False if we can't compose any words from eg book, webserivce and our target sentence otherwise true
        }

        private bool IsNullOrEmpty(string[] words) => words is null || !words.Any();
    }
}
