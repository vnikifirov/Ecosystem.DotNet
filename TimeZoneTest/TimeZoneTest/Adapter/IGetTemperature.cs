using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneTest.Adapter
{
    // Celcius <-> Farenhate
    public interface IGetTemperature
    {
        double GetTemp();
    }
}
