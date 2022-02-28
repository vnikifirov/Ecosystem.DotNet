using System.Collections.Generic;
using System.Linq;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    /// <inheritdoc/>
    public class ComposerService : IComposerService
    {
        /// <inheritdoc/>
        public bool CheckWords(string[] baseWords, string[] target)
        {
            // Return False if we haven't recived any words from eg book, web page or web sevice or haven't any target sentence 
            if (!baseWords.Any() && !target.Any())
                return false;

            // Extra information
            // About join on stackoverflow - https://stackoverflow.com/questions/11139326/how-to-join-two-tables-in-linq-using-wcf-data-service
            // About LINQ and Join or Inner Join operator - https://www.tutorialsteacher.com/linq/linq-joining-operator-join
            // About counting words occurrences eg in string array - https://stackoverflow.com/questions/13373359/how-to-count-word-occurrences-in-an-array-of-strings-using-c
            // About converting string arr to Dictionary - https://stackoverflow.com/questions/1385421/most-elegant-way-to-convert-string-array-into-a-dictionary-of-strings
            /*var innerJoin = baseWords
                .Join(target,
                      word => word,
                      target => target,
                      (word, target) => word)
                .GroupBy(w => w)
                .Select(g => new KeyValuePair<string, int>(g.Key, g.Count())) // Key is our word and Count is word occurrences in string array
                .ToDictionary(key => key.Key, count => count.Value);
            */

            var countBaseWords = baseWords
                .GroupBy(key => key)
                .Select(word => new KeyValuePair<string, int>(word.Key, word.Count())) // Key is our word and Count is word occurrences in string array eg Мама => 5 раз
                .ToDictionary(key => key.Key, count => count.Value);

            var countTargetWords = target
                .GroupBy(key => key)
                .Select(word => new KeyValuePair<string, int>(word.Key, word.Count())) // Key is our word and Count is word occurrences in string array eg Мама => 1 раз
                .ToDictionary(key => key.Key, count => count.Value);

            /* // from table2 
            // inner join table on ...
            // A { мама1, мама2, мама3 } - B { мама2 } = C { мама1, мама3 } where мама1 ... мама3 = Мама 
            var innerJoin = countBaseWords
                .GroupJoin(countTargetWords, // Not A intercet with B => Left A except B
                    baseWord => baseWord.Key,
                    targetWord => targetWord.Key,
                    (baseWords, targetWords) => baseWords.Value - targetWords.) // For example => if exist 5 - 1 = 4, if not exist 5 - 0 = 5, if complex target eg 5 - 6 = -1, if both are equal => 5 - 5 = 0
                // target is 6, base is 3
                // B set is in A
            */

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
    }
}
