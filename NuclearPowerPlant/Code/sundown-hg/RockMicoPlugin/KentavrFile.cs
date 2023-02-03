using System;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using System.Runtime.InteropServices;
using System.IO;

using corelib;

namespace KentavrPlugin
{
#if !DOTNET_V11
    using DataArrayInt = DataArray<int>;
    using DataArrayFloat = DataArray<float>;
#endif

    [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Ansi)]
    struct KentavrNitkaInfo
    {
        byte n_len;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        string name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x2d)]
        byte[] unknown;

        public string FullName
        {
            get { return name; }
        }
        public int Fiber
        {
            get
            {
                string numbers = name.Substring(0, 2);
                return Convert.ToInt32(numbers) - 1;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Ansi)]
    struct KentavrHeaderDat
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] unk1; //Signature ???

        float time_discret;
        uint diskrets;

        byte descr_len;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
        string descr;

        byte info_len;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
        string info;

        byte unk2; //Zero

        byte npp_block;

        byte npp_len;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        string npp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        byte[] unk3;

        double ole_time;
        double ole_date;

        byte unk4;
        byte prog_len;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        char[] prog;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x69)]
        byte[] unk5;


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        KentavrNitkaInfo[] ni;


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x46)]
        byte[] unk6;

        public KentavrNitkaInfo this[int i]
        {
            get { return ni[i]; }
        }

        public string Description
        {
            get { return descr.Substring(0, descr_len); }
        }

        public string Info
        {
            get { return info.Substring(0, info_len); }
        }

        public string Npp
        {
            get { return npp.Substring(0, npp_len); }
        }

        public string ProgramName
        {
            get { return new string(prog, 0, prog_len); }
        }

        public int BlockNum
        {
            get { return npp_block; }
        }

        public DateTime Date
        {
            get { return DateTime.FromOADate(ole_date + ole_time); }
        }

        public int Records
        {
            get { return (int)diskrets; }
        }

        public float Clock
        {
            get { return time_discret; }
        }

    }


    unsafe public class KentavrFile
    {
        KentavrHeaderDat header;
        protected DataArrayFloat[] _records;

        public KentavrFile(String filename)
        {
            byte[] _wholeFile;
            using (FileStream fs = File.OpenRead(filename))
            {
                _wholeFile = new byte[fs.Length];

                fs.Read(_wholeFile, 0, _wholeFile.Length);
            }

            fixed (byte* ptr = &_wholeFile[0])
                header = (KentavrHeaderDat)Marshal.PtrToStructure((IntPtr)ptr, typeof(KentavrHeaderDat));

            int offset = Marshal.SizeOf(typeof(KentavrHeaderDat));

            int end = offset + 17 * sizeof(float) * header.Records;
            if (end > _wholeFile.Length)                
                throw new ArgumentException("Ошибка в файле!");
            else if (end + 4 < _wholeFile.Length)
                throw new ArgumentException("Ошибка в файле!");
                

            float[][] records;
            records = new float[16][];
            int[] indexes = new int[16];
            _records = new DataArrayFloat[16];

            for (int i = 0; i < 16; i++)
            {
                records[i] = new float[header.Records];
                indexes[i] = header[i].Fiber;
            }

            AwfulDeserializer ds = new AwfulDeserializer(_wholeFile, offset);
            for (int rec = 0; rec < header.Records; rec++)
            {
                int dummy;
                ds.Get(out dummy);

                for (int i = 0; i < 16; i++)
                {
                    ds.Get(out records[indexes[i]][rec]);
                }
            }

            for (int i = 0; i < 16; i++)
            {
                _records[i] = new DataArrayFloat(
                    new TupleMetaData("kgoprp_azot", String.Format("Азотная активность (Н.{0})", i + 1), header.Date, TupleMetaData.StreamAuto), records[i]);
            }
        }
        public DateTime Date
        {
            get { return header.Date; }
        }
        public int Fibers
        {
            get { return 16; }
        }

        public DataParamTable GetInfo(int num)
        {
            return new DataParamTable(
                new TupleMetaData("kgoprp_info", String.Format("Информация по азотной прописке (Н.{0})", num + 1), header.Date, TupleMetaData.StreamAuto),
                new DataParamTableItem[] {
                    new DataParamTableItem("fiberNum", num),
                    new DataParamTableItem("prpDate", header.Date),
                    new DataParamTableItem("specClock", header.Clock),
                    new DataParamTableItem("npp", header.Npp),
                    new DataParamTableItem("info", header.Info),
                    new DataParamTableItem("description", header.Description),
                    new DataParamTableItem("program", header.ProgramName),
                    new DataParamTableItem("blockNum", header.BlockNum)
                }
                );
        }
    }


    [AttributeDataComponent("Чтение записи ситемы КЕНТАВР", ComponentFileFilter = "Данные прописки КЕНТАВР *.dat|*.dat", SourceNames = KentavrSingleProvider.sourceNames, MultiTuple = true, ComponentFileNameArgument = "kentavrFilepath")]
    public class KentavrSingleProvider : KentavrFile, ISingleDataProvider
    {
        public const string sourceNames = "kgoprp_azot,kgoprp_info";
        public static readonly string[] splitedSourceNames = sourceNames.Split(',');

        public const string defaultStreamName = "kentavr";

        public KentavrSingleProvider(string kentavrFilepath)
            : base(kentavrFilepath)
        {
        }

        #region ISingleDataProvider Members

        public IMultiDataTuple GetData()
        {
            DataTuple[] tupels = new DataTuple[Fibers];

            for (int i = 0; i < Fibers; i++)
                tupels[i] = new DataTuple(defaultStreamName, Date,
                     _records[i], GetInfo(i));

            return new MultiDataTuple(tupels);
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
