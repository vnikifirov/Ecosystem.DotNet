using System.Threading.Tasks;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

using static System.Console;

namespace SentenceComposer.Console
{
    class Program
    {
        private static ISentenceComposerService _sentenceComposerService = new SentenceComposerService();

        // У тебя есть книга и ты из этой книги вырезаешь слова. 
        // На каком-то этапе у тебя набралось 10 тыс слов

        // baseWords - это наши вырезаные слова например из книги
        // targetWords - это какое-то предолжение например "Мама мыла раму". 
        // Цель: мы должны понять можем ли мы составить предложение (которое у нас target) из выреаных слов (baseWords) или нет. 
        // Tip one: По задачке - можно использовать Dictionary<string, int> для обоих наборов данных и получить сложность O(n+m)
        static async Task Main(string[] args)
        {
            // User input reader
            WriteLine("Please write your source eg web source / web page");
            // URL - you can get data from this web site http://rulyrics.ru/ru/b/belina_sasha/mama_myla_ramu.html 
            var URL = ReadLine();

            // User input reader
            WriteLine("Please write your sentence");
            // You can write eg "Мама мыла раму"
            var userInput = ReadLine();

            // Display 
            var isPossibleCompose = await _sentenceComposerService?.ComposeSentenceAsync(URL, userInput);
            WriteLine($"Is it possible to compose the sentence? {isPossibleCompose}");
            ReadKey();
        }
    }
}
