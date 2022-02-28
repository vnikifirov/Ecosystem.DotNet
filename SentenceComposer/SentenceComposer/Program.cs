using System;
using SentenceComposer.Business.Services.Implementations;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.UI
{
    class Program
    {
        public static IComposerService _composerService { get; set; } = new ComposerService();
        public static ITextReaderService _readerService { get; set; } = new WebSiteTextReaderService();

        // У тебя есть книга и ты из этой книги вырезаешь слова. 
        // На каком-то этапе у тебя набралось 10 тыс слов

        // baseWords - это наши вырезаные слова например из книги
        // targetWords - это какое-то предолжение например "Мама мыла раму". 
        // Цель: мы должны понять можем ли мы составить предложение (которое у нас target) из выреаных слов (baseWords) или нет. 
        // Tip one: По задачке - можно использовать Dictionary<string, int> для обоих наборов данных и получить сложность O(n+m)
        static void Main(string[] args)
        {
            // User input reader
            Console.WriteLine("Please write your source eg web source / web page");
            // URL - you can get data from this web site http://rulyrics.ru/ru/b/belina_sasha/mama_myla_ramu.html 
            var URL = Console.ReadLine();

            // Read text from web page
            var rawText = _readerService.ReadTextFrom(URL);
            var words = rawText.Split(' ');

            // User input reader
            Console.WriteLine("Please write your sentence");
            // You can write eg "Мама мыла раму"
            var rawUserInput = Console.ReadLine();
            var targerWords = rawUserInput.Split(' ');

            // Display 
            var isPossibleCompose = _composerService.CheckWords(words, targerWords);
            Console.WriteLine($"Is it possible to compose the sentence? {isPossibleCompose}");

            Console.ReadKey();
        }
    }
}
