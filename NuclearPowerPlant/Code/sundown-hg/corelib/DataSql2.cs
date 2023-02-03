#if !DOTNET_V11
#define LZMA
#endif

using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.IO;
using System.IO.Compression;

#if LZMA
using SevenZip.Compression.LZMA;
#endif

namespace corelib
{
    public abstract class AbstractSQLMultiProvider2 : _MultiDataProviderNotifier, IMultiDataProvider
    {
        public struct DataItemInfo
        {
            public string Stream;
            public DateTime StreamDate;
            public string ItemName;
            public DateTime ItemDate;

            public Int64 DataId;
            public Int64 HelpId;

            public int Count;
            public DataItem[] data;
            public HelpItem[] help;
        };

        public struct DataItem
        {
            public Int64 DataId;
            public int idx;

            public int algo;
            public Int64 RefDataId;
            public byte[] data;

            public uint hash;
        };

        public struct HelpItem
        {
            public Int64 HelpId;
            public int idx;

            public string Info;
        };

        public abstract void CreateDatabase(string databaseName);
        public abstract void CreateStructure();
        public abstract void ClearAllData();

        protected abstract string[] GetNamesInAll();
        protected abstract string[] GetNames(string stream);
        protected abstract string[] GetConstNames();


        protected abstract DataItemInfo[,] GetDataItemInfo(DateTime[] dates, string[] names);
        protected abstract DataItemInfo[,] GetConstDataItemInfo(string[] names);

        protected abstract void FillHelpInfo(DataItemInfo[,] data);
        protected abstract void FillDataInfo(DataItemInfo[,] data);


        protected abstract void FindHelp(DataItemInfo[,] data);

        /// <summary>
        /// Поиск блоков данных в базе и замещение на ссылки
        /// </summary>
        /// <param name="data"></param>
        protected abstract void FindData(DataItemInfo[,] data); 

        protected abstract void Store(DataItemInfo[,] data);
        protected abstract void StoreConst(DataItemInfo[,] data);


        public enum Compression : int
        {
            None = 0,
            DeflateRFC1951 = 1,
            LZMA = 2
        };

        protected int _compression = 3;
        protected readonly IEnviromentEx _env;

        protected AbstractSQLMultiProvider2(IEnviromentEx env)
        {
            _env = env;
        }

        
        protected IMultiDataTuple GetData(DateTime dt, string[] names, string streamName)
        {
            DataItemInfo[,] a = GetDataItemInfo(new DateTime[] { dt }, names);
            FillHelpInfo(a);
            FillDataInfo(a);

            return RestoreData(dt, streamName, a, 0);
        }
        /***********************************************************/
        /**************************Роман****************************/
        protected IMultiDataTuple GetConstData(string[] names, string streamName)
        {
            DataItemInfo[,] a = GetConstDataItemInfo(names);
            FillHelpInfo(a);
            FillDataInfo(a);

            return RestoreData(streamName, a, 0);
        }
        /***********************************************************/
        /***********************************************************/
        protected IMultiDataTuple GetConstData(DateTime dt, string[] names, string streamName)
        {
            DataItemInfo[,] a = GetConstDataItemInfo(names);
            FillHelpInfo(a);
            FillDataInfo(a);

            return RestoreData(dt, streamName, a, 0);
        }

        protected byte[] Decompress(ref DataItem item)
        {
            byte[] data = item.data;
            switch ((Compression)item.algo)
            {
#if LZMA
                case Compression.LZMA:
                    data = SevenZipHelper.Decompress(data);
                    uint hh = Crc32.ComputeCrc32(data);
                    if (hh != item.hash)
                        throw new Exception("Данные повреждены");
                    break;
#endif
                case Compression.DeflateRFC1951:
                    MemoryStream ms = new MemoryStream(item.data, 0, item.data.Length);
                    DeflateStream compressedzipStream = new DeflateStream(ms, CompressionMode.Decompress, true);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    data = new byte[20000];
                    int len = compressedzipStream.Read(data, 0, data.Length);
                    compressedzipStream.Close();
                    // Check

                    if (len == data.Length)
                        throw new Exception("Не хватает буфера");

                    uint h = Crc32.ComputeCrc32(data, len);
                    if (h != item.hash)
                        throw new Exception("Данные повреждены");

                    break;
                case Compression.None:
                    break;
                default:
                    throw new Exception("Неизвестный тип сжатия!");
            }

            return data;
        }

        protected void Compress(byte[] data, ref DataItem item)
        {
            item.algo = (int)Compression.None;
            item.data = data;

#if LZMA
            if (_compression == 3)
            {
                byte[] compressed = SevenZipHelper.Compress(data);

                if (compressed.Length < data.Length)
                {
                    item.data = compressed;
                    item.algo = (int)Compression.LZMA;
                }
            }
            else
            if (_compression == 2)
            {
                byte[] compressed = SevenZipHelper.Compress(data);

                if (compressed.Length < (data.Length * 9) / 10)
                {
                    item.data = compressed;
                    item.algo = (int)Compression.LZMA;
                }
            } else
#endif
            if (_compression == 1)
            {
                MemoryStream ms = new MemoryStream();
                DeflateStream compressedzipStream = new DeflateStream(ms, CompressionMode.Compress, true);
                compressedzipStream.Write(data, 0, data.Length);
                compressedzipStream.Close();

                byte[] compressed = ms.ToArray();

                if (compressed.Length < (data.Length * 4) / 5)
                {
                    item.data = compressed;
                    item.algo = (int)Compression.DeflateRFC1951;
                }
            }
        }
        /***********************************************************/
        /**************************Роман****************************/
        protected IMultiDataTuple RestoreData(string streamName, DataItemInfo[,] a, int i)
        {
            RawTupleItem[] t = new RawTupleItem[a.GetLength(1)];
            DataTuple[] tupels = null;
            DateTime dt = new DateTime();

            int cnt = 0;
            for (int k = 0; ; k++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (cnt == 0)
                        cnt = a[i, j].Count;
                    else if (cnt != a[i, j].Count)
                        throw new Exception("Несоответсвие количества элементов в грумме данных при построении кортежа");

                    byte[] data = Decompress(ref a[i, j].data[k]);

                    t[j] = new RawTupleItem(
                        new TupleMetaData(a[i, j].ItemName, a[i, j].help[k].Info, a[i, j].ItemDate, a[i, j].Stream), data);

                    dt = a[i, j].ItemDate;
                }

                DataTuple tup = new RawTuple(streamName, dt, t).Restore(_env);
                if (cnt == 1)
                    return tup;
                if (tupels == null)
                    tupels = new DataTuple[cnt];

                tupels[k] = tup;

                if (k + 1 == cnt)
                    break;
            }

            return new MultiDataTuple(tupels);
        }
        /***********************************************************/
        /***********************************************************/
        protected IMultiDataTuple RestoreData(DateTime dt, string streamName, DataItemInfo[,] a, int i)
        {
            RawTupleItem[] t = new RawTupleItem[a.GetLength(1)];
            DataTuple[] tupels = null;

            int cnt = 0;
            for (int k = 0; ; k++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (cnt == 0)
                        cnt = a[i, j].Count;
                    else if (cnt != a[i, j].Count)
                        throw new Exception("Несоответсвие количества элементов в грумме данных при построении кортежа");

                    byte[] data = Decompress(ref a[i, j].data[k]);

                    t[j] = new RawTupleItem(
                        new TupleMetaData(a[i, j].ItemName, a[i, j].help[k].Info, a[i, j].ItemDate, a[i, j].Stream), data);

                }

                DataTuple tup = new RawTuple(streamName, dt, t).Restore(_env);
                if (cnt == 1)
                    return tup;
                if (tupels == null)
                    tupels = new DataTuple[cnt];

                tupels[k] = tup;

                if (k + 1 == cnt)
                    break;
            }

            return new MultiDataTuple(tupels);
        }


        #region IMultiDataProvider Members


        public override TupleMetaData GetTupleItemInfo(DateTime date, string name)
        {
            DataItemInfo[,] a = GetDataItemInfo(new DateTime[] { date }, new string[] { name });
            FillHelpInfo(a);

            return new TupleMetaData(a[0, 0].ItemName, a[0, 0].help[0].Info, a[0, 0].ItemDate, a[0, 0].Stream);
        }


        public override TupleMetaData[,] GetTupleItemInfo(DateTime[] dates, string[] names)
        {
            DataItemInfo[,] a = GetDataItemInfo(dates, names);
            FillHelpInfo(a);

            TupleMetaData[,] odata = new TupleMetaData[dates.Length, names.Length];

            for (int n = 0; n < names.Length; n++)
            {
                for (int i = 0; i < dates.Length; i++)
                {
                    odata[i, n] = new TupleMetaData(a[i, n].ItemName, a[i, n].help[0].Info, a[i, n].ItemDate, a[i, n].Stream);
                }
            }
            return odata;
        }

        /***********************************************************/
        /**************************Роман****************************/
        protected override IMultiDataTuple UncheckedGetData(string streamName)
        {
            string[] names;
            switch (streamName)
            {
                case TupleMetaData.StreamConst:
                    names = GetConstNames();
                    break;
                default:
                    names = GetNames(streamName);
                    break;
            }

            return GetConstData(names, streamName);
        }
        /***********************************************************/
        /***********************************************************/
        
        protected override IMultiDataTuple UncheckedGetData(DateTime date, string streamName)
        {
            string[] names;
            switch (streamName)
            {
                case TupleMetaData.StreamConst:
                    names = GetConstNames();
                    break;
                default:
                    names = GetNames(streamName);
                    break;
            }

            return GetData(date, names, streamName);
        }

        protected override IMultiDataTuple UncheckedGetData(DateTime date, string[] names)
        {
            return GetData(date, names, TupleMetaData.StreamAuto);
        }


        protected void PreparePush(DataItemInfo[,] a, int i, IMultiDataTuple data)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                a[i, j].HelpId = 0;
                a[i, j].DataId = 0;
                a[i, j].Stream = data.GetStreamName();
                a[i, j].StreamDate = data.GetTimeDate();
                IMultiTupleItem item = data.GetItem(j);
                a[i, j].ItemName = item.Name;
                a[i, j].ItemDate = item.GetTimeDate();

                a[i, j].help = new HelpItem[item.Count];
                a[i, j].data = new DataItem[item.Count];
                a[i, j].Count = item.Count;

                for (int k = 0; k < item.Count; k++)
                {
                    a[i, j].help[k].idx = k;
                    a[i, j].help[k].Info = item[k].HumaneName;

                    a[i, j].data[k].idx = k;
                    a[i, j].data[k].RefDataId = 0;

                    byte[] bindata = item[k].Serialize().GetData();
                    a[i, j].data[k].hash = Crc32.ComputeCrc32(bindata);

                    Compress(bindata, ref a[i, j].data[k]);
                }
            }
        }

        protected override void UncheckedPushData(IMultiDataTuple data)
        {
            DataItemInfo[,] a = new DataItemInfo[1, data.ItemsCount];

            PreparePush(a, 0, data);

            FindHelp(a);
            FindData(a);

            if (data.GetStreamName() == TupleMetaData.StreamConst)
                StoreConst(a);
            else
                Store(a);
        }

        protected override void UncheckedPushData(IMultiDataTuple[] datas)
        {
            int icnt = datas[0].ItemsCount;
            bool cstream = datas[0].GetStreamName() == TupleMetaData.StreamConst;

            DataItemInfo[,] a = new DataItemInfo[datas.Length, datas[0].ItemsCount];
            for (int i = 0; i < datas.Length; i++ )
            {
                if ((datas[i].ItemsCount != icnt) || (cstream != (datas[i].GetStreamName() == TupleMetaData.StreamConst)))
                    throw new Exception("Кортежи разной структуры!");

                PreparePush(a, i, datas[i]);
            }

            FindHelp(a);
            FindData(a);

            if (cstream)
                StoreConst(a);
            else
                Store(a);
        }

        public override string[] GetAllDataNames(string stream)
        {
            return GetNames(stream);
        }

        public override string[] GetExistNames()
        {
            return GetNamesInAll();
        }

        public override int GetMultiTuplesCount(DateTime date, string name)
        {
            DataItemInfo[,] a = GetDataItemInfo(new DateTime[] { date }, new string[] { name });
            return a[0, 0].Count;
        }

        #endregion


    }

}
