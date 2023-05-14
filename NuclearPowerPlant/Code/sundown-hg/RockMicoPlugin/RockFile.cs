using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

using corelib;


namespace RockPlugin
{
#if !DOTNET_V11
    using DataArrayInt = DataArray<int>;
    using DataArrayFloat = DataArray<float>;

    using DataCartogramIndexedInt = DataCartogramIndexed<int>;
    using DataCartogramIndexedFloat = DataCartogramIndexed<float>;
#endif

    public class RockFile
    {
        public const int Records = 120;
        public const int RecordItmes = 880;
        public const int RecordLen = RecordItmes * 4;
        byte[] _wholeFile;
        Timestamp _date;
        int _blockNum;
        
        public enum NppCode
        {
            Unknown = 0,
            KAES = 1,
            CHAES = 2,
            SAES = 3,
            LAES = 4,
            IAES = 5
        }

        public enum ZagrPrizma
        {
            TVS = 0,
            TVS_DKER = 6,
            DP = 41,
            DPK = 43,
            DPS = 42,
            SV = 43,
            EMPTY = 40,
            BAZ_PP = 1,
            USP = 4,
            RB = 3,
            DKEN = 5,
            AR = 2
        }
        

        NppCode _code;

        static readonly Regex sFilePattern1 =
            new Regex("(\\d)b(\\d\\d)[-_]?(\\d\\d)[-_]?(\\d\\d|\\d\\d\\d\\d)[-\\s_]+(\\d\\d)[-_](\\d\\d)\\.fil$",
		        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //static readonly Regex sFilePattern2 = new Regex("^(\\d)b(\\d\\d)(\\d\\d)(\\d\\d)\\.fil",
        //        RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public RockFile(String filename)
        {
            _wholeFile = new byte[Records * RecordLen];

            using (FileStream fs = File.OpenRead(filename))
            {
                if (fs.Length != Records * RecordLen)
                    throw new ArgumentException("Неправильный файл");

                fs.Read(_wholeFile, 0, Records * RecordLen);             
            }

            int year, month, day;
            int hour = 0;
            int min = 0;

            Match m = sFilePattern1.Match(filename);
            if (m.Success)
            {
                year = Convert.ToInt32(m.Groups[4].Value);
                month = Convert.ToInt32(m.Groups[3].Value);
                day = Convert.ToInt32(m.Groups[2].Value);

                hour = Convert.ToInt32(m.Groups[5].ToString());
                min = Convert.ToInt32(m.Groups[6].ToString());
            }
            else
            {
                year = GetRecInt(4, 572 - 1);
                month = GetRecInt(4, 573 - 1);
                day = GetRecInt(4, 574 - 1);
            }

            if (year < 20)
            {
                year += 2000;
            }
            else if (year < 100)
            {
                year += 1900;
            }

            _date = new DateTime(year, month, day, hour, min, 0);

            _blockNum = GetRecInt(4, 571 - 1);
            _code = (NppCode)GetRecInt(4, 570 - 1);
		}

        public Timestamp Date
        {
            get { return _date; }
        }

        public int BlockNum
        {
            get { return _blockNum; }
        }

        public NppCode Npp
        {
            get { return _code; }
        }

        protected MultiIntFloat GetRecValueUnsafe(int recordNum, int offset)
        {
            return new MultiIntFloat(_wholeFile, recordNum * RecordLen + offset * 4);
        }

        public MultiIntFloat GetRecValue(int recordNum, int offset)
        {
            if ((recordNum >= Records) || (recordNum < 0) || (offset < 0) || (offset >= RecordItmes))
                throw new ArgumentException();

            return GetRecValueUnsafe(recordNum, offset);
        }

        public int GetRecInt(int recNum, int offset)
        {
            return GetRecValue(recNum, offset).Int;
        }

        public float GetRecFloat(int recNum, int offset)
        {
            return GetRecValue(recNum, offset).Float;
        }


        public DataArrayFloat GetArrayFloat(int recordNum, int count, TupleMetaData info)
        {
            if ((recordNum + (count / RecordLen) >= Records) || (recordNum < 0))
                throw new ArgumentException();

            StreamDeserializer ds = new StreamDeserializer(_wholeFile, recordNum * RecordLen);
            return new DataArrayFloat(info, ds, count, 0, 0);
        }

        public DataArrayInt GetArrayInt(int recordNum, int count, TupleMetaData info)
        {
            if ((recordNum + (count / RecordLen) >= Records) || (recordNum < 0))
                throw new ArgumentException();

            StreamDeserializer ds = new StreamDeserializer(_wholeFile, recordNum * RecordLen);
            return new DataArrayInt(info, ds, count, 0, 0);
        }


        public DataCartogramIndexedFloat GetCartogramFloat(int recordNum, bool wide, IGetCoordsConverter conv, TupleMetaData info, ScaleIndex scale)
        {
            CoordsConverter c = conv.GetSpecialConverter(
                wide ? CoordsConverter.SpecialFlag.WideLinear2448 : CoordsConverter.SpecialFlag.Linear1884, info);

            return new DataCartogramIndexedFloat(info, scale, c, 
                GetArrayFloat(recordNum, c.Length, info));
        }

        public DataCartogramIndexedInt GetCartogramInt(int recordNum, bool wide, IGetCoordsConverter conv, TupleMetaData info, ScaleIndex scale)
        {
            CoordsConverter c = conv.GetSpecialConverter(
                wide ? CoordsConverter.SpecialFlag.WideLinear2448 : CoordsConverter.SpecialFlag.Linear1884, info);

            return new DataCartogramIndexedInt(info, scale, c,
                GetArrayInt(recordNum, c.Length, info));
        }

        static public int ZagrPrizmaToInternal(int prizma)
        {
            ChannelType ch = ChannelType.UNKNOWN;

            switch (prizma)
            {
                case (int)ZagrPrizma.TVS:
                case (int)ZagrPrizma.TVS_DKER:
                    ch = ChannelType.TVS; break;

                case (int)ZagrPrizma.DP:
                case (int)ZagrPrizma.DPS:
                    ch = ChannelType.DP; break;

                case (int)ZagrPrizma.SV:
                    ch = ChannelType.WATER; break;

                case (int)ZagrPrizma.EMPTY:
                    ch = ChannelType.EMPTY; break;

                case (int)ZagrPrizma.AR:
                case (int)ZagrPrizma.BAZ_PP:
                case (int)ZagrPrizma.RB:
                    ch = ChannelType.UNKNOWN; break; //For compatibility with old abi

                case (int)ZagrPrizma.DKEN:
                    ch = ChannelType.UNKNOWN; break;
            }

            return (int)ch;
        }

        static public DataCartogramIndexedInt ToDefaultZagr(DataCartogramIndexedInt zagr_prizma)
        {
            int[] res = new int[zagr_prizma.Coords.Length];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = ZagrPrizmaToInternal(zagr_prizma.GetItem(i, 0));
            }

            TupleMetaData info = new TupleMetaData("zagr", "Загрузка во внетреннем представлении",
                zagr_prizma.Date, TupleMetaData.StreamAuto);

            return new DataCartogramIndexedInt(info, ScaleIndex.Default, zagr_prizma.Coords,
                new DataArrayInt(info, res));
        }
    }


    [AttributeDataComponent("Чтение среза состояния СКАЛА", ComponentFileFilter = "Файл FIL СКАЛА *.fil|*.fil", SourceNames = RockSingleProvider.sourceNames, ComponentFileNameArgument = "filepath")]
    public class RockSingleProvider : RockFile, ISingleDataProvider
    {
        public const string sourceNames = "zagr_dkr,zagr_fil,power,flow,energovir,fizras,rbmk_params,ustavki";
        public static readonly string[] splitedSourceNames = sourceNames.Split(',');

        public const string defaultStreamName = "rock";

        IEnviromentEx _env;

        public RockSingleProvider(IEnviromentEx enviromentObject, string filepath)
            : base(filepath)
        {
            _env = enviromentObject;
        }

        protected DataParamTable CreateInfoTable()
        {
            return new DataParamTable(new TupleMetaData("rbmk_params", "Параметры систем", Date, TupleMetaData.StreamAuto),
                new DataParamTableItem("totalHeatPower", GetRecFloat(4, 489 - 1)),
                new DataParamTableItem("pLeftNK", new AnyValue(GetRecFloat(4, 176 - 1), 0.1f)),
                new DataParamTableItem("pRightNK", new AnyValue(GetRecFloat(4, 177 - 1), 0.1f)),

                new DataParamTableItem("pLeftBS1", new AnyValue(GetRecFloat(4, 174 - 1), 0.1f)),
                new DataParamTableItem("pLeftBS2", new AnyValue(GetRecFloat(4, 174 - 1), 0.1f)),
                new DataParamTableItem("pRightBS1", new AnyValue(GetRecFloat(4, 175 - 1), 0.1f)),
                new DataParamTableItem("pRightBS2", new AnyValue(GetRecFloat(4, 175 - 1), 0.1f)),

                new DataParamTableItem("tLeftVK1", new AnyValue(GetRecFloat(4, 178 - 1))),
                new DataParamTableItem("tLeftVK2", new AnyValue(GetRecFloat(4, 178 - 1))),
                new DataParamTableItem("tRightVK1", new AnyValue(GetRecFloat(4, 179 - 1))),
                new DataParamTableItem("tRightVK2", new AnyValue(GetRecFloat(4, 179 - 1))));
        }

        public DataTuple GetData()
        {
            DataCartogramIndexedInt zagr_dkr = GetCartogramInt(0, false, _env, new TupleMetaData("zagr_dkr", "Загрузка по типам с учетом ДКЭР", Date, TupleMetaData.StreamAuto), ScaleIndex.Default);

            ITupleItem flow = GetCartogramFloat(6, false, _env, new TupleMetaData("flow", "Расходы поканальные", Date, TupleMetaData.StreamAuto), new ScaleIndex(0, 0.1));

            ITupleItem fizras = GetCartogramFloat(9, false, _env, new TupleMetaData("fizras", "Мощности физрасчета", Date, TupleMetaData.StreamAuto), new ScaleIndex(0, 0.01));

            ITupleItem energovir = GetCartogramFloat(12, true, _env, new TupleMetaData("energovir", "Энерговыработки поканальные", Date, TupleMetaData.StreamAuto), ScaleIndex.Default);

            ITupleItem power = GetCartogramFloat(15, true, _env, new TupleMetaData("power", "Мощности поканальные", Date, TupleMetaData.StreamAuto), new ScaleIndex(0, 0.01));

            ITupleItem zagr_fil = GetCartogramInt(22, true, _env, new TupleMetaData("zagr_fil", "Загрузка по типам", Date, TupleMetaData.StreamAuto), ScaleIndex.Default);

            ITupleItem fizras_opt = GetCartogramFloat(25, false, _env, new TupleMetaData("fizras_opt", "Мощности по физрасчету ОПТИМА", Date, TupleMetaData.StreamAuto), new ScaleIndex(0, 0.01));

            ITupleItem rbmk_params = CreateInfoTable();


            return new DataTuple(defaultStreamName, Date,
                zagr_dkr, flow, fizras, energovir, power, zagr_fil, fizras_opt, rbmk_params/*,
                RockFile.ToDefaultZagr(zagr_dkr)*/);
        }

        IMultiDataTuple ISingleDataProvider.GetData()
        {
            return GetData();
        }

        public void PushData(IMultiDataTuple data)
        {
            throw new NotSupportedException();
        }

        public string[] GetExistNames()
        {
            return splitedSourceNames;
        }

        public void Dispose()
        {
        }

    }


    public class RockAlgo
    {
        [AttributeAlgoComponent("Преобразование загрезки")]
        [AttributeRules("zagr is (zagr_dkr as double[native]) ;" +
                 " return is (zagr('Загрузка во внетреннем представлении') as Cart(native) to zagr_dkr )")]
        public static void zagrRockConvertToInternal(double[,] zagr_dkr)
        {
            for (int y = 0; y < 48; y++)
                for (int x = 0; x < 48; x++)
                {
                    zagr_dkr[y, x] = RockFile.ZagrPrizmaToInternal((int)zagr_dkr[y, x]);
                }
        }
    }
}
