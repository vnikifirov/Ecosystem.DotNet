using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest.Adapter
{
    public class Farenhate : IGetTemperature
    {
        private double _myTemperature;

        public Farenhate()
        {
            _myTemperature = new Random(100).NextDouble();// какой-то источник
        }
        public double GetTemp()
        {
            return _myTemperature;
        }
    }
}
