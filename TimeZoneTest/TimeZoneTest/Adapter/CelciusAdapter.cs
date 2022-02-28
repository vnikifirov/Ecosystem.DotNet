using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest.Adapter
{
    public class CelciusAdapter : IGetTemperature
    {
        private double _myTemperature;

        public CelciusAdapter()
        {
            _myTemperature = new Random(100).NextDouble(); // источник данных
        }

        public CelciusAdapter(double temp)
        {

        }

        public CelciusAdapter(Farenhate f)
        {
            var farengate = f.GetTemp();

            _myTemperature =  Adapt(farengate);
        }

        public double GetTemp()
        {
            return _myTemperature;
        }

        private double Adapt(double farengate)
        {
            // adapt formula
            return (farengate - 32) * 5 / 9;
        }
    }
}
