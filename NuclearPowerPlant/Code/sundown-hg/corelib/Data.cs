using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

namespace corelib
{
    public delegate void MultiDataProviderGetDataEventHandler(IMultiDataProvider sender, MultiDataProviderGetDataEventArgs args);
    public delegate void MultiDataProviderDataEventHandler(IMultiDataProvider sender, MultiDataProviderDataTupleEventArgs args);

    public class MultiDataProviderDataTupleEventArgs : EventArgs
    {
        DateTime _dateTime;
        IMultiDataTuple _origTuple;

        public IMultiDataTuple Original
        {
            get { return _origTuple; }
        }

        public DateTime DateTime
        {
            get { return _dateTime; }
        }

        public MultiDataProviderDataTupleEventArgs(IMultiDataTuple tup, DateTime date, string stream)
        {
            _origTuple = tup;
            _dateTime = date;
        }

        public MultiDataProviderDataTupleEventArgs(IMultiDataTuple tup, DateTime date, string[] names)
        {
            _origTuple = tup;
            _dateTime = date;
        }
    }

    public class MultiDataProviderGetDataEventArgs : MultiDataProviderDataTupleEventArgs
    {
        IMultiDataTuple _modifiedTuple;

        public bool IsModified
        {
            get { return _modifiedTuple != null; }
        }

        public IMultiDataTuple Modified
        {
            get { return _modifiedTuple; }
            set
            {
                if (_modifiedTuple == null)
                    _modifiedTuple = value;
                else
                    throw new ArgumentException("Модифицированный поток уже задан");
            }
        }

        public MultiDataProviderGetDataEventArgs(IMultiDataTuple tup, DateTime date, string stream)
            : base(tup, date, stream)
        {

        }
        public MultiDataProviderGetDataEventArgs(IMultiDataTuple tup, DateTime date, string[] names)
            : base(tup, date, names)
        {

        }
    }


    public struct RawTupleItem : ITupleMetaData
    {
        #region IRawChronoSerializer Members

        public byte[] GetData()
        {
            return _rawData;
        }

        #endregion

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _metaData.Date;
        }

        #endregion

        #region INameData Members

        public string GetName()
        {
            return _metaData.Name;
        }

        #endregion

        #region IHumanInfo Members

        public string GetHumanName()
        {
            return _metaData.HumaneName;
        }

        #endregion

        public string HumaneName
        {
            get { return GetHumanName(); }
        }

        public string Name
        {
            get { return GetName(); }
        }

        public DateTime Date
        {
            get { return GetTimeDate(); }
        }

        public static AbstactTupleItem Restore(IEnviromentEx env, IDeserializeStream d)
        {
            int streamIdx;
            d.Get(out streamIdx);
            d.RestoryOriginal();


            switch (streamIdx)
            {
                case KnownType.DataCartogramIdxMultiValue_StreamSerializerId:
                case KnownType.DataCartogramIndexed_StreamSerializerId:
                case KnownType.DataCartogramLinear_StreamSerializerId:
                case KnownType.DataCartogramLinearWide_StreamSerializerId:
                case KnownType.DataCartogramNative_StreamSerializerId:
                case KnownType.DataCartogramPvk_StreamSerializerId:
                    return DataCartogram.CreateABIOld(env, d);
                case KnownType.DataArray_StreamSerializerId:
                    return DataArray.Create(d);
                case KnownType.DataParamTable_StreamSerializerId:
                    return DataParamTable.Create(d);
                default:
                    throw new Exception("Don't know how to recovery");
            }
        }

        public AbstactTupleItem Restore(IEnviromentEx env)
        {
            StreamDeserializer d = new StreamDeserializer(_rawData, _metaData);
            return Restore(env, d);
        }

        public RawTupleItem(TupleMetaData info, byte[] rawData)
        {
            _metaData = info;
            _rawData = rawData;
        }


        private TupleMetaData _metaData;
        private byte[] _rawData;

        #region ITupleMetaData Members


        public TupleMetaData Info
        {
            get { return _metaData; }
        }

        #endregion

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _metaData.StreamName;
        }

        #endregion
    }

    public struct RawTuple : ITimeData
    {
        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _streamName;
        }

        #endregion

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _date;
        }

        #endregion


        public DataTuple Restore(IEnviromentEx env)
        {
            AbstactTupleItem[] items = new AbstactTupleItem[_data.Length];
            for (int i = 0; i < _data.Length; i++)
                items[i] = _data[i].Restore(env);

            return new DataTuple(_streamName, _date, items);
        }

        /// <summary>
        /// Конструирование временной обертки
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="date"></param>
        /// <param name="data">Массив данных для десериализации (Примечание: данный массив используется как есть без дополнительного копирования, поэтому он не должен изменяться во вне)</param>
        public RawTuple(string streamName, DateTime date, RawTupleItem[] data)
        {
            _streamName = streamName;
            _date = date;
            _data = data;
        }

        private string _streamName;
        private DateTime _date;
        private RawTupleItem[] _data;
    }



    public abstract class _MultiDataProviderNotifier : IMultiDataProvider
    {
        public abstract void Dispose();
        /***********************************************************/
        /**************************Роман****************************/
        protected abstract IMultiDataTuple UncheckedGetData(string stream);
        /***********************************************************/
        /***********************************************************/
        protected abstract IMultiDataTuple UncheckedGetData(DateTime date, string stream);
        protected abstract IMultiDataTuple UncheckedGetData(DateTime date, string[] names);

        protected abstract void UncheckedPushData(IMultiDataTuple data);

        protected virtual void UncheckedPushData(IMultiDataTuple[] datas)
        {
            foreach (IMultiDataTuple d in datas)
            {
                UncheckedPushData(d);
            }
        }
        /***********************************************************/
        /**************************Роман****************************/
        public IMultiDataTuple GetConstData(string stream)
        {
            IMultiDataTuple dt = UncheckedGetData(stream);
            
            return dt;
        }
        /***********************************************************/
        /***********************************************************/
        public IMultiDataTuple GetData(DateTime date, string[] names)
        {
            IMultiDataTuple dt = UncheckedGetData(date, names);
            if (OnGetData != null)
            {
                MultiDataProviderGetDataEventArgs e = new
                                MultiDataProviderGetDataEventArgs(dt, date, names);

                OnGetData(this, e);

                if (e.IsModified)
                    return e.Modified;
            }
            return dt;
        }

        public IMultiDataTuple GetData(DateTime date, string stream)
        {
            IMultiDataTuple dt = UncheckedGetData(date, stream);
            if (OnGetData != null)
            {
                MultiDataProviderGetDataEventArgs e = new
                                MultiDataProviderGetDataEventArgs(dt, date, stream);

                OnGetData(this, e);

                if (e.IsModified)
                    return e.Modified;
            }
            return dt;
        }

        public void PushData(IMultiDataTuple data)
        {
            IMultiDataTuple mod = data;
            if (OnPushingData != null)
            {
                MultiDataProviderGetDataEventArgs e = new
                                MultiDataProviderGetDataEventArgs(mod, mod.GetTimeDate(), mod.GetStreamName());
                OnPushingData(this, e);

                if (e.IsModified)
                    mod = e.Modified;
            }

            UncheckedPushData(data);

            if (OnPushedData != null)
            {
                MultiDataProviderDataTupleEventArgs e = new
                    MultiDataProviderDataTupleEventArgs(mod, mod.GetTimeDate(), mod.GetStreamName());

                OnPushedData(this, e);
            }
        }

#if !DOTNET_V11
        public void PushData(IList<IMultiDataTuple> datas)
        {
            int i = 0;
            IMultiDataTuple[] ar = new IMultiDataTuple[datas.Count];
            foreach (IMultiDataTuple mod in datas)
            {
                ar[i] = mod;
                if (OnPushingData != null)
                {
                    MultiDataProviderGetDataEventArgs e = new
                                    MultiDataProviderGetDataEventArgs(mod, mod.GetTimeDate(), mod.GetStreamName());
                    OnPushingData(this, e);

                    if (e.IsModified)
                        ar[i] = e.Modified;
                }
                i++;
            }

            UncheckedPushData(ar);

            if (OnPushedData != null)
            {
                foreach (IMultiDataTuple mod in ar)
                {
                    MultiDataProviderDataTupleEventArgs e = new
                        MultiDataProviderDataTupleEventArgs(mod, mod.GetTimeDate(), mod.GetStreamName());

                    OnPushedData(this, e);
                }
            }
        }
#endif

        public virtual TupleMetaData[,] GetTupleItemInfo(DateTime[] dates, string[] names)
        {
            TupleMetaData[,] odata = new TupleMetaData[dates.Length, names.Length];
            for (int d = 0; d < dates.Length; d++)
            {
                for (int n = 0; n < names.Length; n++)
                {
                    odata[d, n] = GetTupleItemInfo(dates[d], names[n]);
                }
            }
            return odata;
        }

        public virtual void GetMultiTuplesCount(DateTime[] dates, string name, out int[] count)
        {
            count = new int[dates.Length];
            for (int i = 0; i < dates.Length; i++)
                count[i] = GetMultiTuplesCount(dates[i], name);
        }

        public abstract string[] GetAllDataNames(string stream);

        public abstract string[] GetDataNames(DateTime date, string stream);

        public abstract DateTime[] GetDates();

        public abstract DateTime[] GetDates(string stream);

        public abstract string[] GetStreamNames();

        public event MultiDataProviderGetDataEventHandler OnGetData;

        public event MultiDataProviderGetDataEventHandler OnPushingData;

        public event MultiDataProviderDataEventHandler OnPushedData;

        public abstract string[] GetExistNames();

        public abstract TupleMetaData GetTupleItemInfo(DateTime date, string name);

        public abstract int GetMultiTuplesCount(DateTime date, string name);
    }


    public class MultiDataProviderOverSingle : _MultiDataProviderNotifier, IMultiDataProvider
    {
        ISingleDataProvider _prov;
        public MultiDataProviderOverSingle(ISingleDataProvider prov)
        {
            _prov = prov;
        }

        public ISingleDataProvider ISingleDataProvider
        {
            get
            {
                return _prov;
            }
        }

        public override void Dispose()
        {
            _prov.Dispose();
        }
        /***********************************************************/
        /**************************Роман****************************/
        protected override IMultiDataTuple UncheckedGetData(string stream)
        {
            throw new NotImplementedException();
        }
        /***********************************************************/
        /***********************************************************/
        protected override IMultiDataTuple UncheckedGetData(DateTime date, string stream)
        {
            // TODO Add checking
            return _prov.GetData();
        }

        protected override IMultiDataTuple UncheckedGetData(DateTime date, string[] names)
        {
            IMultiTupleItem[] items = new IMultiTupleItem[names.Length];
            IMultiDataTuple t = _prov.GetData();
            for (int i = 0; i < names.Length; i++)
                items[i] = t.GetItem(i);
            return new MultiDataTuple(TupleMetaData.StreamAuto, t.GetTimeDate(), items);
        }

        protected override void UncheckedPushData(IMultiDataTuple data)
        {
            _prov.PushData(data);
        }

        public override string[] GetDataNames(DateTime date, string stream)
        {
            return GetAllDataNames(stream);
        }

        public override DateTime[] GetDates()
        {
            return new DateTime[] { _prov.GetData().GetTimeDate() };
        }

        public override DateTime[] GetDates(string stream)
        {
            return GetDates();
        }

        public override string[] GetStreamNames()
        {
            return new string[] { _prov.GetData().GetStreamName() };
        }

        public override string[] GetExistNames()
        {
            return _prov.GetExistNames();
        }

        public override TupleMetaData GetTupleItemInfo(DateTime date, string name)
        {
            return _prov.GetData()[name][0].Info;
        }

        public override int GetMultiTuplesCount(DateTime date, string name)
        {
            return _prov.GetData()[name].Count;
        }

        public override string[] GetAllDataNames(string stream)
        {
            IMultiDataTuple t = _prov.GetData();
            string[] names = new string[t.ItemsCount];
            t.CopyNamesTo(names);
            return names;
        }
    }


    public class ListMultiDataProvider : _MultiDataProviderNotifier, IMultiDataProvider
    {
        struct CahcedNamesStruct
        {
            public string streamName;
            public bool isStreamMulti;

            public CahcedNamesStruct(string stream, bool imstream)
            {
                streamName = stream;
                isStreamMulti = imstream;
            }
        }
#if !DOTNET_V11
        Dictionary<string, Dictionary<DateTime, IMultiDataTuple>> _data = new Dictionary<string, Dictionary<DateTime, IMultiDataTuple>>();

        Dictionary<string, CahcedNamesStruct> _cachedNames;
        Dictionary<string, CahcedNamesStruct> CachedNames
        {
#else
        Hashtable _data = new Hashtable();

        Hashtable _cachedNames;
        Hashtable CachedNames
        {

#endif
            get
            {
                if (_cachedNames == null)
                {
                    UpdateCacheNames();
                }

                return _cachedNames;
            }
        }

        void UpdateCacheNames()
        {
#if !DOTNET_V11
            _cachedNames = new Dictionary<string, CahcedNamesStruct>();
#else
            _cachedNames = new Hashtable();
#endif

            foreach (string s in _data.Keys)
            {
#if !DOTNET_V11
                List<string> names = new List<string>();
                foreach (IMultiDataTuple t in _data[s].Values)
#else
                ArrayList names = new ArrayList();
                foreach (IMultiDataTuple t in ((Hashtable)_data[s]).Values)
#endif
                {
                    //foreach (ITupleItem n in t)
                    for (int i = 0; i < t.ItemsCount; i++)
                    {
                        IMultiTupleItem n = t.GetItem(i);

                        if (!names.Contains(n.GetName()))
                        {
                            names.Add(n.GetName());
                        }
                    }
                }
                foreach (string n in names)
                {
                    _cachedNames.Add(n, new CahcedNamesStruct(s, false));
                }
            }

        }


        public ListMultiDataProvider()
        {

        }


#if !DOTNET_V11
        public ListMultiDataProvider(IEnumerable<IMultiDataTuple> items)        
#else
        public ListMultiDataProvider(IEnumerable items)       
#endif
        {
            foreach (IMultiDataTuple item in items)
            {
                PushData(item);
            }
        }

        public override void Dispose()
        {
            
        }
        /***********************************************************/
        /**************************Роман****************************/
        //функция приведена лишь потому, что она должна быть реальизована в этом классе
        //(как производном от абстрактного, к котором эта функция только определена)
        //аналогично с MultiDataProviderOverSingle
        protected override IMultiDataTuple UncheckedGetData(string stream)
        {
            throw new NotImplementedException();
        }
        /***********************************************************/
        /***********************************************************/
        protected override IMultiDataTuple UncheckedGetData(DateTime date, string stream)
        {
#if !DOTNET_V11
            return _data[stream][date];
#else
            return (IMultiDataTuple)((Hashtable)_data[stream])[date];
#endif
        }

        protected override IMultiDataTuple UncheckedGetData(DateTime date, string[] names)
        {
#if !DOTNET_V11
            List<IMultiTupleItem> items = new List<IMultiTupleItem>();
#else
            ArrayList items = new ArrayList();
#endif
            foreach (string n in names)
            {
                CahcedNamesStruct c = (CahcedNamesStruct)CachedNames[n];
                if (c.isStreamMulti)
                    throw new ArgumentException(n);

#if !DOTNET_V11
                items.Add(_data[c.streamName][date][n]);
#else
                items.Add(((IMultiDataTuple)((Hashtable)_data[c.streamName])[date])[n]);
#endif
            }
            return new MultiDataTuple(TupleMetaData.StreamAuto, date, 
#if !DOTNET_V11
                 items.ToArray());
#else
                (IMultiTupleItem[])items.ToArray(typeof(IMultiTupleItem)));
#endif
        }

        protected override void UncheckedPushData(IMultiDataTuple data)
        {
            _cachedNames = null;
#if !DOTNET_V11
            Dictionary<DateTime, IMultiDataTuple> dic;
            if (!_data.TryGetValue(data.GetStreamName(), out dic))
            {
                dic = new Dictionary<DateTime, IMultiDataTuple>();
#else
            Hashtable dic = (Hashtable)_data[data.GetStreamName()];
            if (dic == null)
            {
                dic = new Hashtable();
#endif
                _data.Add(data.GetStreamName(), dic);
            }

            dic.Add(data.GetTimeDate(), data);
        }

        public override string[] GetDataNames(DateTime date, string stream)
        {
#if !DOTNET_V11
            Dictionary<DateTime, IMultiDataTuple> dic;
            if (_data.TryGetValue(stream, out dic))
            {
                IMultiDataTuple dataTuple;
                if (dic.TryGetValue(date, out dataTuple))
                {
                    string[] res = new string[dataTuple.ItemsCount];
                    dataTuple.CopyNamesTo(res);
                    return res;
                }
            }
#else
            Hashtable dic = (Hashtable)_data[stream];
            if (dic != null)
            {
                IMultiDataTuple dataTuple = (IMultiDataTuple)dic[date];
                if (dataTuple != null)
                {
                    string[] res = new string[dataTuple.Count];
                    dataTuple.CopyNamesTo(res);
                    return res;
                }
            }
#endif
            return null;
        }

        protected static readonly DateTime[] sdtNull = new DateTime[0];

        public override DateTime[] GetDates()
        {
            if (_data.Count == 1)
                foreach (string stream in _data.Keys)
                    return GetDates(stream);

            throw new NotImplementedException();
        }

        public override DateTime[] GetDates(string stream)
        {
#if !DOTNET_V11
            Dictionary<DateTime, IMultiDataTuple> dic;
            if (_data.TryGetValue(stream, out dic))
            {
                List<DateTime> dates = new List<DateTime>(dic.Keys);
                dates.Sort();
                
                // TODO Add caching
                return dates.ToArray();
            }
#else
            Hashtable dic = (Hashtable)_data[stream];
            ArrayList dates = new ArrayList();
            if (dic != null)
            {
                foreach (DateTime s in dic.Keys)
                    dates.Add(s);

                dates.Sort();
                return (DateTime[])dates.ToArray(typeof(DateTime));
            }
#endif

            return sdtNull;
        }

        public override string[] GetStreamNames()
        {
            string[] streams = new string[_data.Keys.Count];
            _data.Keys.CopyTo(streams, 0);
            return streams;
        }

        public override string[] GetExistNames()
        {
#if !DOTNET_V11
            List<string> names = new List<string>(CachedNames.Keys);
            return names.ToArray();
#else
            ArrayList res = new ArrayList();
            foreach (string s in CachedNames.Keys)
                res.Add(s);

            return (string[])res.ToArray(typeof(string));
#endif
        }

        public override TupleMetaData GetTupleItemInfo(DateTime date, string name)
        {
            return UncheckedGetData(date, new string[] { name })[0][name].Info;
        }

        public override int GetMultiTuplesCount(DateTime date, string name)
        {
            return UncheckedGetData(date, new string[] { name }).Count;
        }

        public override string[] GetAllDataNames(string stream)
        {
            return null;
        }
    }
}
