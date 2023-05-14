using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.IO;

using corelib;

namespace Algorithms
{
#if !DOTNET_V11
    using DataCartogramNativeDouble = DataCartogramNative<double>;
    using DataArrayCoords = DataArray<Coords>;
    using DataCartogramIndexedCoords = DataCartogramIndexed<Coords>;
    using DataCartogramIndexedFloat = DataCartogramIndexed<float>;
#endif

    public class ParseLenPvk
    {
        /// <summary>
        /// Длина ПВК расчет
        /// </summary>
        /// <param name="filename">Полное имя файла со значениями длин ПВК</param>

        public ParseLenPvk(string filename)
        {
            double[,] data = new double[48, 48];

            using (StreamReader sr = new StreamReader(filename))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sp = line.Split(';');
                    Coords c = Coords.FromHumane(sp[0]);
                    double len = 0;
                    if (sp[1].Length != 0)
                        len = Convert.ToDouble(sp[1]);// / 1000.0;
                    
                    data[c.Y, c.X] = len;
                }
            }

            _cart = new DataCartogramNativeDouble(
                new TupleMetaData("pvk_length", "Длины коммуникаций ПВК", File.GetLastWriteTime(filename), TupleMetaData.StreamConst),
                new ScaleIndex(0, 1 / 1000.0), data);
        }

        public DataCartogramNativeDouble PvkLength
        {
            get { return _cart; }
        }

        public static implicit operator DataCartogramNativeDouble(ParseLenPvk s)
        {
            return s._cart;
        }

        DataCartogramNativeDouble _cart;
    }


    public class ParseLenPvkAndScheme
    {
        public readonly CoordsConverter Pvk;
        public readonly DataCartogramIndexedFloat PvkLen;

        public ParseLenPvkAndScheme(string filename)
        {
            Coords[,] scheme = new Coords[16, 115];
            float[,] lenpvk = new float[16, 115];


            using (FileStream fs = File.OpenRead(filename))
            using (BinaryReader br = new BinaryReader(fs))
            {
                if (fs.Length != 16 * 115 * 8)
                    throw new ArgumentException("Ошибка чтения файла");

                for (int f = 0; f < 16; f++)
                {
                    for (int p = 0; p < 115; p++)
                    {
                        int pr, x, y, w;
                        pr = br.ReadByte();
                        x = br.ReadByte();
                        y = br.ReadByte();
                        w = br.ReadByte();

                        Coords c = (pr > 0) ? Coords.FromHumane(y, x) : Coords.incorrect;
                        scheme[f, p] = c;
                        lenpvk[f, p] = br.ReadSingle();
                    }
                }
            }

            Pvk = new CoordsConverter(
                new TupleMetaData("pvk_scheme", "Разводка АЗ по ниткам", File.GetLastWriteTime(filename), TupleMetaData.StreamConst),
                CoordsConverter.SpecialFlag.PVK,
                scheme);

            PvkLen = new DataCartogramIndexedFloat(
                new TupleMetaData("pvk_length", "Длины коммуникаций ПВК", File.GetLastWriteTime(filename), TupleMetaData.StreamConst),
                ScaleIndex.Default,
                Pvk,
                lenpvk);
        }
    }

    [AttributeDataComponent("Файл длин ниток ПВК", ComponentFileFilter = "Данные длин ниток ПВК *.csv|*.csv", ComponentFileNameArgument = "csvPvkLengthFilePath")]
    public class CSVPvkLengthProvider : ParseLenPvk, ISingleDataProvider
    {

        #region ISingleDataProvider Members

        public IMultiDataTuple GetData()
        {
            return new DataTuple("const", PvkLength.GetTimeDate(), PvkLength);
        }

        public void PushData(IMultiDataTuple data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDataProvider Members

        public string[] GetPossibleNames()
        {
            return new string[] { "pvk_length" };
        }

        public string[] GetExistNames()
        {
            return new string[] { "pvk_length" };
        }

        public void Close()
        {
            
        }

        #endregion


        public CSVPvkLengthProvider(string csvPvkLengthFilePath)
            : base(csvPvkLengthFilePath)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }




    [AttributeDataComponent("Файл длин ниток и разводки ПВК", ComponentFileFilter = "Данные длин ниток и разводки ПВК *.pvk_bin|*.pvk_bin", ComponentFileNameArgument = "pvkSchemePvkLengthFilePath")]
    public class PvkSchemePvkLengthProvider : ParseLenPvkAndScheme, ISingleDataProvider
    {
        public const string sourceNames = "pvk_length,pvk_scheme";
        public static readonly string[] splitedSourceNames = sourceNames.Split(',');

        public const string defaultStreamName = "const";

        public PvkSchemePvkLengthProvider(string pvkSchemePvkLengthFilePath)
            : base(pvkSchemePvkLengthFilePath)
        {

        }

        #region ISingleDataProvider Members

        public IMultiDataTuple GetData()
        {
            return new DataTuple(defaultStreamName, Pvk.Date, Pvk, PvkLen);
        }

        public void PushData(IMultiDataTuple data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDataProvider Members

        public string[] GetPossibleNames()
        {
            return splitedSourceNames;
        }

        public string[] GetExistNames()
        {
            return splitedSourceNames;
        }

        public void Close()
        {
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
