using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest
{
    public static class Collections
    {
        public static List<int> list = new();
        public static int[] array = new int[10];
        public static LinkedList<int> linked = new();
        public static Dictionary<int, int> dict = new();


        public static Dictionary<MyKey, MyValue> myDict = new();

        static Collections()
        {
            list.Add(1);//O(1)
            linked.AddLast(1);//O(1)
            dict.Add(1, 1);//O(1)

            list.Insert(1, 1);//O(n)
            linked.AddAfter(linked.Last, 1);//O(1)
            dict.Add(1, 1);//O(1)?

            list.RemoveAt(1);//O(n)
            linked.RemoveLast();//O(1)
            dict.Remove(1);//O(1)


            list.ElementAt(0);//O(1)
            linked.ElementAt(0);//O(n)
            var d = dict[0];//O(1)

            dict.ContainsKey(0); // O(1) - коллизия O(n)
            var a = dict[1]; // O(1) - коллизия O(n)
            dict.Add(1, 1); // O(1) - коллизия O(n)
        }

        
    }
    public struct MyKey
    {
        public int Id;
        public string Name;


        public override bool Equals(object obj)
        {
            var other = (MyKey)obj;
            return GetHashCode() == other.GetHashCode();
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 1; // O(n^2)
            // return (Id.GetHashCode() + Name.GetHashCode()) % new Random(DateTime.Now.Millisecond).Next(int.MaxValue) ;
            // return Id.GetHashCode();
        }
    }
    public class MyValue
    {
        public object o;
    }
}
