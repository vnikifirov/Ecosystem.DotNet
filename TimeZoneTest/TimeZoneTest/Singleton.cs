using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest
{
    public sealed class Singleton1
    {
        private static volatile Singleton1 _instance;

        //locker
        private static readonly object Loker = new();

        public static Singleton1 Instance
        {
            get
            {
                //first check
                if (_instance == null)
                {
                    // another Thread may create instance
                    lock (Loker) // very expensive, because - lock = try + Monitor.Enter() + finally + Monitor.Release()
                    {
                        //second check
                        if (_instance == null)
                        {
                            _instance = new Singleton1();
                        }
                    }
                }

                return _instance;
            }
        }
    }

    //-------------------
    public sealed class Singleton2
    {
        // one time in static constructor
        private static readonly Singleton2 instance = new();

        // static constructor
        static Singleton2() { }

        // get instance 
        public static Singleton2 Instance
        {
            get { return instance; }
        }
    }
}
