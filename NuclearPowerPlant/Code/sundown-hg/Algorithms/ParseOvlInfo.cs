using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

using corelib;

namespace Algorithms
{
#if !DOTNET_V11
    using DataCartogramNativeDouble = DataCartogramNative<double>;
    using DataArrayCoords = DataArray<Coords>;
    using DataCartogramIndexedCoords = DataCartogramIndexed<Coords>;
    using DataCartogramIndexedFloat = DataCartogramIndexed<float>;
#endif

    public class ParseOvlInfo
    {
        Regex r = new Regex("\\s+");
        CultureInfo ci = CultureInfo.InvariantCulture;

        public ParseOvlInfo(string filename)
        {
            double[,] data = new double[48, 48];
            string[] h = null;

            using (StreamReader sr = new StreamReader(filename))
            {
                string head = sr.ReadLine();
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sp = r.Split(line);
                    Coords c = Coords.FromHumane(sp[0].Substring(0, 2) + "-" + sp[0].Substring(2,2));
                    double len = Convert.ToDouble(sp[4], ci.NumberFormat);// / 1000.0;
                    
                    data[c.Y, c.X] = len;
                }

                h = r.Split(head);
            }
            int year = Convert.ToInt32(h[4]);
            if (year<50)
                year+=2000;
            else if (year < 100)
                year+=1900;

            DateTime d = new DateTime(year, Convert.ToInt32(h[3]), Convert.ToInt32(h[2]));

            _cart = new DataCartogramNativeDouble(
                new TupleMetaData("zrk", "Лимб открытия ЗРК", d /*File.GetLastWriteTime(filename)*/, TupleMetaData.StreamConst),
                new ScaleIndex(0, 1), data);
        }

        public DataCartogramNativeDouble Zrk
        {
            get { return _cart; }
        }

        public static implicit operator DataCartogramNativeDouble(ParseOvlInfo s)
        {
            return s._cart;
        }

        DataCartogramNativeDouble _cart;
    }


    [AttributeDataComponent("Файл записи ЗРК", ComponentFileFilter = "Данные по ЗРК (OVL) *.txt|*.txt", ComponentFileNameArgument = "ovlFilePath")]
    public class OvlInfoProvider : ParseOvlInfo, ISingleDataProvider
    {

        #region ISingleDataProvider Members

        public IMultiDataTuple GetData()
        {
            return new DataTuple("const", Zrk.GetTimeDate(), Zrk);
        }

        public void PushData(IMultiDataTuple data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDataProvider Members

        public string[] GetPossibleNames()
        {
            return new string[] { "zrk" };
        }

        public string[] GetExistNames()
        {
            return new string[] { "zrk" };
        }

        public void Close()
        {

        }

        #endregion


        public OvlInfoProvider(string ovlFilePath)
            : base(ovlFilePath)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}