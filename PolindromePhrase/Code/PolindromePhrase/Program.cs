using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace PolindromePhrase
{
    class Program
    {
        static void Main(string[] args)
        {
            string example = "A man, a plan, a canal; Panama";
            
            var isPolindromeByDelimiters = example.IsPolindromeByDelimiters(); // Time complicity O(n + (n / 2))
            var isPolindomeWithouMemory = example.IsPolindromeWithoutMemory(); // Time complicity O(N), Space complicity O(1)
            var isPolindromeByRegex = example.IsPolindromeByRegex(); // Time complicity O(4N)

            Console.WriteLine($"Is the string a polindrome or not by delimiters? - {isPolindromeByDelimiters}");
            Console.WriteLine($"Is the string a polindrome or not by regex? - {isPolindromeByRegex}");
            Console.WriteLine($"Is the string a polindrome or not without memory? - {isPolindomeWithouMemory}");

            Console.ReadKey();
        }
    }

    public static class Polindrome
    {
        public static bool IsPolindromeWithoutMemory(this string input)
        {
            // Time complicity O(N), Space complicity O(1)
            for (int i = 0, j = (input.Length - 1); i < j; i++, j--)
            {
                while (!char.IsLetterOrDigit(input[i]))
                {
                    i++;
                }

                while (!char.IsLetterOrDigit(input[j]))
                {
                    j--;
                }

                if (char.ToLower(input[i]) != char.ToLower(input[j]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPolindromeByDelimiters(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var text = input
                //.ToLower() // Time complicity O(N)
                .ToAlphaNumericOnlyByDelimiters(); // Time complicity O(N)

            // string reversed = text.Reverse(); // 
            //string reversed = text.Reverse<string>();
            //string reversed = string.Concat(text.Reverse()); // O(N)

            //return string.Equals(text, reversed); ; // O(N)

            // Time complicity - O(N / 2)
            for (int i = 0; i < (text.Length / 2); i++)
            {
                if (text[i] != text[text.Length - i - 1])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPolindromeByRegex(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var text = input
                .ToLower() // Time complicity O(N)
                .ToAlphaNumericOnlyRegex(); // Time complicity O(N)

            string reversed = string.Concat(text.Reverse()); // Time complicity Reverse - O(N), Contact create a new string

            return string.Equals(text, reversed); // O(N)
        }

        private static string ToAlphaNumericOnlyByDelimiters(this string input)
        {
            // Delimiters are to split text by them
            //var delimiters = new char[] { ' ', '\r', '\n', ',', '-', '!', '.', ':' };
            //return string.Concat(input.Split(delimiters)); // Memory O(N + N) or O(2N)
            return string.Concat(input
                .Where(c => char.IsLetterOrDigit(c))
                .Select( c => char.ToLower(c))); // Memory N
        }

        private static string ToAlphaNumericOnlyRegex(this string input)
        {
            var rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, "");
        }

        /*private static string Reverse(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            char[] array = text.ToCharArray();
            Array.Reverse(array);

            return new String(array);
        }*/
    }

}
