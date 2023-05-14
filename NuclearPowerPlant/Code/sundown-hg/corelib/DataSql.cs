using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

namespace corelib
{
    public abstract class AbstractSQLMultiProvider : _MultiDataProviderNotifier, IMultiDataProvider
    {
        public struct DataItemInfo
        {
            public string Stream;
            public DateTime StreamDate;
            public string ItemName;
            public DateTime ItemDate;
            public byte[] Data;
        };

        public abstract void CreateDatabase(string databaseName);
        public abstract void CreateStructure();
        public abstract void ClearAllData();

        protected abstract string[] GetNamesInAll();
        protected abstract string[] GetNames(string stream);
        

        protected abstract string GetHelpString(string param);

        protected abstract void SetHelpString(string param, string value);


        protected abstract bool GetInfoItem(DateTime[] dates, string name, int idx, out string stream, out DateTime[] sdate);

        protected abstract DataItemInfo[,] GetDataItem(DateTime date, string[] names, int maxIdx);

        protected abstract void StoreData(DataItemInfo[,] data);

        protected abstract int MaxIdx(DateTime date, string name);

        protected abstract void MaxIdx(DateTime[] dates, string name, out int[] maxs);



        protected abstract string[] GetConstNames();

        protected abstract byte[] GetConstData(string name);

        protected abstract void StoreConstData(string name, byte[] item);


        protected AbstractSQLMultiProvider(IEnviromentEx env)
        {
            _env = env;
        }

        private string GetHelp(string name)
        {
            //TODO REWRITE ME FOR FAST .NET 2
            if (_table.ContainsKey(name))
                return (string)_table[name];
            else
            {
                string str = GetHelpString(name);
                if (str != null)
                    _table.Add(name, str);
                else
                    str = name;
                return str;
            }
        }
        /***********************************************************/
        /**************************Роман****************************/
       protected IMultiDataTuple GetConstData(string[] names, string streamName)
        {
            //int maxIdx = MaxIdx(date, names[0]);
            //DataItemInfo[,] dataInfo = GetDataItem(date, names, maxIdx);

            RawTupleItem[] t = new RawTupleItem[names.Length];
            DataTuple[] tupels = null;

            //for (int j = 0; j <= maxIdx; j++)
            //{
                for (int i = 0; i < names.Length; i++)
                {
                    //t[i] = new RawTupleItem(
                      //  new TupleMetaData(names[i], GetHelp(names[i]), date, dataInfo[i, j].Stream), dataInfo[i, j].Data);
                    t[i] = new RawTupleItem();
                }

                //DataTuple tup = new RawTuple(streamName, date, t).Restore(_env);
                DataTuple tup = new RawTuple().Restore(_env);
                tupels = new DataTuple[1];
                tupels[0] = tup;

                
            //}

            return new MultiDataTuple(tupels);
        }
       /***********************************************************/
       /***********************************************************/


        protected IMultiDataTuple GetData(DateTime date, string[] names, string streamName)
        {
            int maxIdx = MaxIdx(date, names[0]);
            DataItemInfo[,] dataInfo = GetDataItem(date, names, maxIdx);

            RawTupleItem[] t = new RawTupleItem[names.Length];
            DataTuple[] tupels = null;

            for (int j = 0; j <= maxIdx; j++)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    t[i] = new RawTupleItem(
                        new TupleMetaData(names[i], GetHelp(names[i]), date, dataInfo[i, j].Stream), dataInfo[i, j].Data);
                }

                DataTuple tup = new RawTuple(streamName, date, t).Restore(_env);
                if (maxIdx == 0)
                    return tup;
                if (tupels == null)
                    tupels = new DataTuple[maxIdx + 1];

                tupels[j] = tup;
            }

            return new MultiDataTuple(tupels);
        }

        public override TupleMetaData GetTupleItemInfo(DateTime date, string name)
        {
            string sname;
            DateTime[] sdate;

            bool ret = GetInfoItem(new DateTime[] {date}, name, 0, out sname, out sdate);
            // probably search in const
            if (!ret)
                throw new NotSupportedException();

            return new TupleMetaData(name, GetHelp(name), sdate[0], sname);
        }


        public override TupleMetaData[,] GetTupleItemInfo(DateTime[] dates, string[] names)
        {
            TupleMetaData[,] odata = new TupleMetaData[dates.Length, names.Length];
            for (int n = 0; n < names.Length; n++)
            {
                DateTime[] sdates;
                string sname;

                bool ret = GetInfoItem(dates, names[n], 0, out sname, out sdates);

                for (int i = 0; i < dates.Length; i++)
                {
                    odata[i, n] = new TupleMetaData(names[n], GetHelp(names[n]), sdates[i], sname);
                }
            }
            return odata;
        }

        #region IMultiDataProvider Members

        /***********************************************************/
        /**************************Роман****************************/
        protected override IMultiDataTuple UncheckedGetData(string streamName)
        {
            if (streamName != TupleMetaData.StreamConst)
            {
                string[] names = GetConstNames();
                return GetConstData(names, streamName);
            }
            else
            {
                string[] names = GetConstNames();
                RawTupleItem[] t = new RawTupleItem[names.Length];

                for (int i = 0; i < names.Length; i++)
                {
                    byte[] bt = GetConstData(names[i]);
                    t[i] = new RawTupleItem(
                        new TupleMetaData(names[i], GetHelp(names[i]), DateTime.Now, streamName), bt);
                }
                return new RawTuple(streamName, t[0].GetTimeDate(), t).Restore(_env);
            }
        }

        /***********************************************************/
        /***********************************************************/

        protected override IMultiDataTuple UncheckedGetData(DateTime date, string streamName)
        {
            if (streamName != TupleMetaData.StreamConst)
            {
                string[] names = GetDataNames(date, streamName);
                return GetData(date, names, streamName);
            }
            else
            {
                string[] names = GetConstNames();
                RawTupleItem[] t = new RawTupleItem[names.Length];

                for (int i = 0; i < names.Length; i++)
                {
                    byte[] bt = GetConstData(names[i]);
                    t[i] = new RawTupleItem(
                        new TupleMetaData(names[i], GetHelp(names[i]), DateTime.Now, streamName), bt);
                }
                return new RawTuple(streamName, t[0].GetTimeDate(), t).Restore(_env);
            }
        }

        protected override IMultiDataTuple UncheckedGetData(DateTime date, string[] names)
        {
            return GetData(date, names, TupleMetaData.StreamAuto);
        }


        protected override void UncheckedPushData(IMultiDataTuple data)
        {
            if (data.GetStreamName() == "const")
            {
                if (data.Count > 1)
                    throw new NotSupportedException();

                for (int i = 0; i < data.ItemsCount; i++)
                {
                    ITupleItem r = data[0][i];

                    StoreConstData(r.GetName(), r.Serialize().GetData());
                    if (!_table.ContainsKey(r.GetName()))
                    {
                        SetHelpString(r.GetName(), r.GetHumanName());
                    }
                }
            }
            else
            {
                DataItemInfo[,] dataInfo = new DataItemInfo[data.ItemsCount, data.Count];

                for (int i = 0; i < data.ItemsCount; i++)
                {
                    for (int j = 0; j < data.Count; j++)
                    {
                        ITupleItem r = data[j][i];

                        dataInfo[i, j].Stream = data.GetStreamName();
                        dataInfo[i, j].StreamDate = data.GetTimeDate();
                        dataInfo[i, j].ItemName = r.Name;
                        dataInfo[i, j].ItemDate = r.Date;
                        dataInfo[i, j].Data = r.Serialize().GetData();
                        
                        if (!_table.ContainsKey(r.GetName()))
                        {
                            SetHelpString(r.GetName(), r.GetHumanName());
                        }
                    }
                }

                StoreData(dataInfo);
            }
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
            return MaxIdx(date, name) + 1;
        }

        #endregion

        protected readonly IEnviromentEx _env;
#if DOTNET_V11
        private Hashtable _table = new Hashtable();
#else
        private Dictionary<string, string> _table = new Dictionary<string, string>();
#endif
    }

}
