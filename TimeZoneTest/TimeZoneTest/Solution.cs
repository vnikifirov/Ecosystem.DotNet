using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest
{
    public class Solution
    {
        public static IList<IList<string>> GroupAnagrams(string[] strs)
        {
            return strs.GroupBy(str => string.Concat(str.OrderBy(c => c.ToString())), (key, ienum) => (IList<string>)ienum.ToList()).ToList(); // n^2
        }

        public static IList<IList<string>> GroupAnagrams2(string[] strs)
        {
            var dict = new Dictionary<string, List<string>>();
            for (var i = 0; i < strs.Count(); i++) // n
            {
                var sort = string.Concat(strs[i].OrderBy(s => s)); // m * log m
                if (!dict.ContainsKey(sort)) // 1
                    dict.Add(sort, new List<string> { strs[i] }); // 1
                else
                    dict[sort].Add(strs[i]); // 1
            }
            var result = dict.Select(pair => (IList<string>)pair.Value).ToList(); // n
            return result; // n * m * log m 
        }

        public static IList<IList<string>> GroupAnagrams3(string[] strs)
        {
            var keyGen = new DictKeyGen();

            var dict = new Dictionary<int, List<string>>();
            
            for (var i = 0; i < strs.Count(); i++) // n
            {
                var key = keyGen.GetKey(strs[i]); // m
                if (!dict.ContainsKey(key)) // 1
                    dict.Add(key, new List<string> { strs[i] }); // 1
                else
                    dict[key].Add(strs[i]); // 1
            }

            var result = dict.Select(pair => (IList<string>)pair.Value).ToList(); // n

            return result; // n * m
        }
    }

    public sealed class DictKeyGen
    {
        const int hash = int.MaxValue;
        static Dictionary<char, int> keyer = new();

        public DictKeyGen()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            
            for (var i = (int)'a'; i <= 'z'; i++)
            {
                keyer.Add((char)i, rand.Next(hash));
            }
        }

        public int GetKey(string s)
        {
            int key = 0;
            if (s == "") return key;
            foreach (var c in s) // m
            {
                key = (key + keyer[c]) % hash;
            }
            return key;
        }
    }
}
