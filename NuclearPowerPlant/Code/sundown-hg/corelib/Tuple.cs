using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endif
using System.Text;
using System.IO;
using System.Xml;

namespace corelib
{

    public class MultiTupleItem : IMultiTupleItem, IMultiDataTuple
    {
        ITupleItem[] _items;

        private MultiTupleItem()
        {

        }

        public MultiTupleItem(ITupleItem[] items)
        {
            _items = (ITupleItem[])items.Clone();
        }

        public MultiTupleItem(ArrayList items)
        {
            _items = (ITupleItem[])items.ToArray(typeof(ITupleItem));
        }

#if !DOTNET_V11
        public MultiTupleItem(List<ITupleItem> items)
        {
            _items = items.ToArray();
        }
#endif


        public MultiTupleItem(IMultiTupleItem items)
        {
            _items = new ITupleItem[items.Count];
            for (int i = 0; i < _items.Length; i++)
                _items[i] = items[i]; 
        }

        public MultiTupleItem ReStream(string newname)
        {
            ITupleItem[] n = new ITupleItem[_items.Length];
            for (int i = 0; i < n.Length; i++)
                n[i] = _items[i].Rename(newname);
            MultiTupleItem it = new MultiTupleItem();
            it._items = n;
            return it;
        }

        
        #region IMultiTupleItem Members

        public string GetName()
        {
            return _items[0].Name;
        }

        public string Name
        {
            get { return _items[0].Name; }
        }

        public int Count
        {
            get { return _items.Length; }
        }

        public ITupleItem this[int idx]
        {
            get { return _items[idx]; }
        }

        #endregion

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _items[0].Date;
        }

        #endregion

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _items[0].GetStreamName();
        }

        #endregion



        #region IMultiDataTuple Members

        IMultiTupleItem IMultiDataTuple.this[string name]
        {
            get { if (name == Name) return this; throw new ArgumentException(); }
        }

        IMultiTupleItem IMultiDataTuple.GetItem(int idx)
        {
            if (idx == 0) return this; throw new ArgumentException();
        }

        IDataTuple IMultiDataTuple.this[int idx]
        {
            get { if (idx == 0) return new DataTuple(GetStreamName(), GetTimeDate(), _items[0]); throw new ArgumentException(); }
        }

        IMultiDataTuple IMultiDataTuple.ReStream(string newname)
        {
            return ReStream(newname);
        }

        #endregion

        #region IDataTupleInfo Members

        int IDataTupleInfo.ItemsCount
        {
            get { return 1; }
        }

        void IDataTupleInfo.CopyNamesTo(string[] array)
        {
            ((IDataTupleInfo)this).CopyNamesTo(array, 0);
        }

        void IDataTupleInfo.CopyNamesTo(string[] arraym, int idx)
        {
            arraym[idx] = Name;
        }

        #endregion

        /*
        IMultiTupleItem IMultiTupleItem.ReStream(string newname)
        {
            return ReStream(newname);
        }
         */ 
    }

    public class DataTuple : IDataTuple, IMultiDataTuple
#if DOTNET_V11
        , IEnumerable
#else
        , IEnumerable<ITupleItem>
#endif
    {
        private string _streamName;
        private DateTime _time;
        private ITupleItem[] _data;

        public static readonly ITupleItem[] EmptyItems = new ITupleItem[0];

        #region IDataTuple Members

        public ITupleItem GetParam(string name)
        {
            foreach (ITupleItem s in _data)
            {
                if (s.GetName() == name)
                    return s;
            }
            throw new Exception(String.Format("\"{0}\" Not found!", name));
         }

        public ITupleItem GetParamSafe(string name)
        {
            foreach (ITupleItem s in _data)
            {
                if (s.GetName() == name)
                    return s;
            }
            return null;
        }

        public IDataCartogram GetCart(string name)
        {
            return GetParamSafe(name) as IDataCartogram;
        }
        public IDataArray GetArray(string name)
        {
            return GetParamSafe(name) as IDataArray;
        }

        public int ItemsCount
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Возвращает копию массива со всеми элементами в кортеже
        /// </summary>
        /// <returns>Массив эдементов</returns>
        public ITupleItem[] GetData()
        {
            ITupleItem[] newArray = new ITupleItem[ItemsCount];
            _data.CopyTo(newArray, 0);

            return newArray;
        }

        public ITupleItem this[int idx]
        {
            get { return _data[idx]; }
        }

        public ITupleItem this[string name]
        {
            get { return GetParam(name); }
        }

        public IDataTuple CastTo(ISingleTupleRules rules)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _streamName;
        }

        #endregion

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _time;
        }

        #endregion


        public void CopyNamesTo(string[] array)
        {
            CopyNamesTo(array, 0);
        }

        public void CopyNamesTo(string[] array, int idx)
        {            
            foreach (ITupleItem i in _data)
            {
                array[idx++] = i.GetName();
            }
        }

        public struct Enumerator : 
#if DOTNET_V11
            IEnumerator
#else
            IEnumerator<ITupleItem>
#endif
        {
            int _index;
            ITupleItem[] _items;

            public Enumerator(DataTuple t)
            {
                _index = -1;
                _items = t._data;
            }


            public ITupleItem Current
            {
                get { return _items[_index]; }
            }


            public void Dispose()
            {

            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (++_index < _items.Length)
                    return true;
                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);

        }
#if !DOTNET_V11
        IEnumerator<ITupleItem> IEnumerable<ITupleItem>.GetEnumerator()
        {
            return new Enumerator(this);
        }
#endif

        public string StreamName
        {
            get
            {
                return _streamName;
            }
        }

      

        public DataTuple(IDataTuple t, string newStreamName, DateTime newDate)
        {
            _streamName = newStreamName;
            _time = newDate;
            _data = t.GetData();
        }
        public DataTuple(IDataTuple t, string newStreamName)
        {
            _streamName = newStreamName;
            _time = t.GetTimeDate();
            _data = t.GetData();
        }

        public DataTuple(IDataTuple t)
        {
            _streamName = t.GetStreamName();
            _time = t.GetTimeDate();
            _data = t.GetData();
        }

        public DataTuple(DataTuple t, string newStreamName, DateTime newDate)
        {
            _streamName = newStreamName;
            _time = newDate;
            _data = t._data;
        }
        public DataTuple(DataTuple t, string newStreamName)
        {
            _streamName = newStreamName;
            _time = t.GetTimeDate();
            _data = t._data;
        }

        public DataTuple(string streamName, DateTime time, ITupleItem singleObj)
        {
            _streamName = streamName;
            _time = time;
            _data = new ITupleItem[] { singleObj };
        }

        public DataTuple(string streamName, DateTime time, params ITupleItem[] objs)
        {
            _streamName = streamName;
            _time = time;
            _data = (ITupleItem[])objs.Clone();
        }

        /// <summary>
        /// Создает пустой кортеж
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="dt"></param>
        public DataTuple(string streamName, DateTime dt)
        {
            _streamName = streamName;
            _time = dt;
            _data = EmptyItems;
        }

      
        private DataTuple()
        {
        }


        public DataTuple(ISingleTupleRules rules, ITupleMaps maps, params IDataTuple[] objs)
        {
            string[] names = rules.GetDesiredNames();
            _streamName = rules.GetTupleName();

            _data = new ITupleItem[names.Length];
            _time = new DateTime(0);

            for (int i = 0; i < names.Length; i++)
            {
                ITupleItem found = null;
                if (objs != null)
                foreach (DataTuple d in objs)
                {
                    ITupleItem tmp = null;
                    foreach (ITupleItem n in d)
                    {
                        string newStream;
                        string newName;
                        if (maps != null && maps.IsMapped(d.GetStreamName(), n.GetName(), out newStream, out newName))
                        {
                            if ((newStream != null) && (newStream.Length > 0) && (newStream != _streamName))
                                throw new Exception("Unable to handle this map");

                            if (newName == names[i])
                            {
                                tmp = n;
                                break;
                            }
                        }
                        else
                        {
                            if (n.GetName() == names[i])
                            {
                                tmp = n;
                                break;
                            }
                        }
                    }
                    if (tmp != null)
                    {
                        if (found != null)
                            throw new Exception("Don't know how to handle multiply names");
                    
                        found = tmp;
                    }
                }

                if (found == null)
                {
                    throw new Exception(String.Format("Parameter \"{0}\" didn't find", names[i]));
                }
                if (found.GetTimeDate() > _time)
                    _time = found.GetTimeDate();

                if (rules.IsTypeInfoFor(names[i]))
                {
                    if (found.GetName() != names[i])
                        found = found.Rename(names[i]);

                    _data[i] = found.CastTo(rules.GetTypeInfo(names[i]));
                }
                else
                    _data[i] = found.Rename(names[i]);
            }
        }

        /// <summary>
        /// Используя правила rules получить необходимые потоки из objs и привести их к нужному формату
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="objs"></param>
        public DataTuple(ISingleTupleRules rules, params IDataTuple[] objs)
            : this(rules, null, objs)
        {
            
        }

        public DataTuple(ISingleTupleRules rules, IDataTuple singleObj)
            : this(rules, null, new IDataTuple[] { singleObj })
        {

        }

        
        public static DataTuple Combine(string[] newNames, params IDataTuple[] tupels)
        {
            int totalLen = 0;

            for (int i = 0; i < tupels.Length; i++)
                totalLen += tupels[i].ItemsCount;

            DataTuple dt = new DataTuple();
            dt._streamName = tupels[0].GetStreamName();
            dt._time = tupels[0].GetTimeDate();

            dt._data = new ITupleItem[totalLen];
            int k = 0;

            Hashtable h = new Hashtable();

            for (int i = 0; i < tupels.Length; i++)
                for (int j = 0; j < tupels[i].ItemsCount; j++, k++)
                {
                    if (newNames != null)
                        dt._data[k] = tupels[i][j].Rename(newNames[k]);
                    else
                        dt._data[k] = tupels[i][j];
                    h.Add(dt._data[k].GetName(), dt._data[k]);
                }
           
            return dt;
        }



        public static DataTuple LoadFromFile(IEnviromentEx env, string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] data = new byte[(int)fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            fs.Close();

            StreamDeserializer s = new StreamDeserializer(data, 0);
            return new DataTuple(env, s);
        }

        public static DataTuple combine(params IDataTuple[] tups)
        {
            int len = 0;
            for (int i = 0; i < tups.Length; i++)
                len += tups[i].ItemsCount;

            ITupleItem[] items = new ITupleItem[len];
            int l = 0;
            for (int i = 0; i < tups.Length; i++)
            {
                for (int j = 0; j < tups[i].ItemsCount; j++, l++)
                {
                    items[l] = tups[i][j];
                }
            }

            return new DataTuple(tups[0].GetStreamName(), tups[0].GetTimeDate(), items);
        }

        public DataTuple ReNameItems(string add)
        {
            ITupleItem[] items = new ITupleItem[ItemsCount];
            for (int j = 0; j < ItemsCount; j++)
            {
                items[j] = this[j].Rename(new TupleMetaData(
                    this[j].Name + add, this[j].HumaneName + add, this[j].Info.Date, this[j].Info.StreamName));
            }
            return new DataTuple(StreamName, GetTimeDate(), items);
        }

        public DataTuple ReStream(string newname)
        {
            DataTuple t = new DataTuple();
            t._streamName = newname;
            t._time = _time;
            t._data = new ITupleItem[_data.Length];

            for (int i = 0; i < _data.Length; i++)
            {
                TupleMetaData md = _data[i].Info;
                md.StreamName = newname;
                t._data[i] = _data[i].Rename(md);
            }

            return t;
        }

        public DataTuple ReDate(DateTime newDate)
        {
            DataTuple t = new DataTuple();
            t._streamName = _streamName;
            t._time = newDate;
            t._data = new ITupleItem[_data.Length];

            for (int i = 0; i < _data.Length; i++)
            {
                TupleMetaData md = _data[i].Info;
                md.Date = newDate;
                t._data[i] = _data[i].Rename(md);
            }

            return t;
        }

        public void SaveToFile(string path)
        {
            FileStream fs = File.Create(path);
            byte[] data = Serialize().GetData();
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        public ISerializeStream Serialize()
        {
            StreamSerializer sr = new StreamSerializer(new TupleMetaData());
            Serialize(0, sr);
            return sr;
        }
        public void Serialize(int abiver, ISerializeStream stream)
        {

            if (abiver != 0)
                throw new ArgumentException();

            stream.Put(KnownType.serializeDataTuple);
            stream.Put(_streamName);
            stream.Put(_time.ToOADate());
            stream.Put(_data.Length);

            for (int i = 0; i < _data.Length; i++)
            {
                ISerializeStream s = new StreamSerializer(_data[i].Info);
                _data[i].Serialize(abiver, s);
                stream.Put(s.GetName());
                stream.Put(s.GetHumanName());
                stream.Put(s.GetTimeDate().ToOADate());
                stream.Put((int)s.GetData().Length);

                stream.Put(s.GetData());
            }
        }
        public DataTuple(IEnviromentEx env, IDeserializeStream stream)
        {
            int id;
            stream.Get(out id);

            if (id != KnownType.serializeDataTuple)
                throw new ArgumentException("Данный поток не относится к данному типу");

            stream.Get(out _streamName);
            double time;
            stream.Get(out time);
            _time = DateTime.FromOADate(time);

            int tuples;
            stream.Get(out tuples);

            _data = new ITupleItem[tuples];
            for (int i = 0; i < tuples; i++)
            {
                string tname;
                string thelp;
                double ttime;
                int length;
                byte[] data;

                stream.Get(out tname);
                stream.Get(out thelp);
                stream.Get(out ttime);
                stream.Get(out length);
                data = (byte[])stream.GetArray(typeof(byte), length, 0, 0);

                StreamDeserializer d = new StreamDeserializer(data, 
                    new TupleMetaData(tname, thelp, DateTime.FromOADate(ttime), _streamName));
                _data[i] = RawTupleItem.Restore(env, d);
            }
        }


        #region ITuple Members


        public string GetParamHelp(string name)
        {
            foreach (ITupleItem s in _data)
            {
                if (s.GetName() == name)
                    return s.GetHumanName();
            }
            throw new Exception("Help string has not found!"); 
        }

        #endregion


        #region IDuckTuple Members

        object IDuckTuple.this[string name]
        {
            get { return this[name]; }
        }

        #endregion

        #region IMultiDataTuple Members

        IDataTuple IMultiDataTuple.this[int idx]
        {
            get { if (idx == 0) return this; throw new ArgumentException(); }
        }

        IMultiTupleItem IMultiDataTuple.this[string name]
        {
            get { return new MultiTupleItem(new ITupleItem[] { this[name] }); }
        }

        IMultiTupleItem IMultiDataTuple.GetItem(int idx)
        {
            return new MultiTupleItem(new ITupleItem[] { this[idx] });
        }

        IMultiDataTuple IMultiDataTuple.ReStream(string newname)
        {
            return this.ReStream(newname);
        }

        #endregion

        #region IDataTupleInfo Members

        int IMultiDataTuple.Count
        {
            get { return 1; }
        }

        #endregion

        IDataTuple IDataTuple.ReStream(string newname)
        {
            return this.ReStream(newname);
        }
    }


    public class MultiDataTuple : IMultiDataTuple
    {
        DataTuple[] _tups;

        protected MultiDataTuple(DataTuple[] tups)
        {
            _tups = tups;
        }

        public MultiDataTuple(IDataTuple[] tups)
        {
            _tups = new DataTuple[tups.Length];
            for (int i = 0; i < tups.Length; i++)
                _tups[i] = new DataTuple(tups[i]);
        }

        public MultiDataTuple(ArrayList tups)
        {
            _tups = new DataTuple[tups.Count];
            for (int i = 0; i < tups.Count; i++)
                _tups[i] = new DataTuple((IDataTuple)tups[i]);
        }
  
        public MultiDataTuple(IMultiDataTuple tups)
        {
            _tups = new DataTuple[tups.Count];
            for (int i = 0; i < tups.Count; i++)
                _tups[i] = new DataTuple(tups[i]);
        }

        public MultiDataTuple(string stream, DataTuple[] tups)
        {
            _tups = new DataTuple[tups.Length];
            for (int i = 0; i < tups.Length; i++)
                _tups[i] = new DataTuple(tups[i], stream);
        }
        public MultiDataTuple(string stream, DateTime dt, DataTuple[] tups)
        {
            _tups = new DataTuple[tups.Length];
            for (int i = 0; i < tups.Length; i++)
                _tups[i] = new DataTuple(tups[i], stream, dt);
        }

        public MultiDataTuple(string stream, DateTime dt, params IMultiTupleItem[] items)
        {
            _tups = new DataTuple[items[0].Count];
            ITupleItem[] itms = new ITupleItem[items.Length];
            for (int i = 0; i < _tups.Length; i++)
            {
                for (int j = 0; j < items.Length; j++ )
                    itms[j] = items[j][i];

                _tups[i] = new DataTuple(stream, dt, itms);
            }
        }

        public MultiDataTuple NewStream(string stream)
        {
            return new MultiDataTuple(stream, _tups);
        }
        public MultiDataTuple NewDate(DateTime d)
        {
            return new MultiDataTuple(GetStreamName(), d, _tups);
        }
        public MultiDataTuple NewStreamDate(string stream, DateTime d)
        {
            return new MultiDataTuple(stream, d, _tups);
        }

        public MultiDataTuple ReStream(string newname)
        {
            DataTuple[] n = new DataTuple[_tups.Length];
            for (int i = 0; i < n.Length; i++)
            {
                n[i] = _tups[i].ReStream(newname);
            }
            return new MultiDataTuple(n);
        }

        public static DataTuple[] MultiDeserialize(IEnviromentEx env, IDeserializeStream stream)
        {
            int id;
            stream.Get(out id);

            if (id != KnownType.serializeDataTupleMulti)
                throw new Exception("can't parse");

            int length;
            stream.Get(out length);

            DataTuple[] items = new DataTuple[length];

            for (int i = 0; i < length; i++)
            {
                items[i] = new DataTuple(env, stream);
            }
            return items;
        }

        public static void MultiSerialize(IMultiDataTuple items, int abiver, ISerializeStream stream)
        {
            stream.Put(KnownType.serializeDataTupleMulti);
            stream.Put((int)items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                DataTuple t = new DataTuple(items[i]);
                t.Serialize(abiver, stream);
            }
        }

        public MultiDataTuple(IEnviromentEx env, string path)
        {
            byte[] data;
            using (FileStream fs = File.OpenRead(path))
            {
                data = new byte[(int)fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }

            StreamDeserializer s = new StreamDeserializer(data, 0);
            _tups = MultiDeserialize(env, s);
        }

        public MultiDataTuple(IEnviromentEx env, StreamDeserializer s)
        {
            _tups = MultiDeserialize(env, s);
        }

        public static void SaveToFile(IMultiDataTuple items, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                StreamSerializer sr = new StreamSerializer(new TupleMetaData());
                MultiSerialize(items, 0, sr);
                byte[] data = sr.GetData();
                fs.Write(data, 0, data.Length);
            }
        }

        public static MultiDataTuple Combine(string[] names, params IMultiDataTuple[] tup)
        {
            DataTuple[] tups = new DataTuple[tup[0].Count];
            IDataTuple[] ids = new IDataTuple[tup.Length];
            for (int i = 0; i < tups.Length; i++)
            {
                for (int j = 0; j < tup.Length; j++)
                    ids[j] = tup[j][i];
                tups[i] = DataTuple.Combine(names, ids);
            }
            return new MultiDataTuple(tups);
        }

        public void SaveToFile(string path)
        {
            SaveToFile(this, path);   
        }

        public static IMultiDataTuple Create(IEnviromentEx env, ISingleTupleRules rules, ITupleMaps maps, params IMultiDataTuple[] objs)
        {
            return CreateParamFromTuple(env, null, null, rules, maps, objs);
        }

        public static IMultiDataTuple CreateParamFromTuple(IEnviromentEx env, object[] storParams, System.Reflection.ParameterInfo[] methodParam, ISingleTupleRules rules, ITupleMaps maps, params IMultiDataTuple[] objs)
        {
            DataTuple[] tuples = null;

            string streamName = rules.GetTupleName();

            string[] names = rules.GetDesiredNames(); //Имена которые будут содержаться в выходном потоке
            string[] names_as = (string[])rules.GetDesiredNames().Clone(); //Имена, которые нужно искать в параметрах

            int parameters = objs.Length;     // Количество переданных параметров
            int itemsCount = names.Length;    // Количество элементов в выходном картеже

            // Предполагаем что размерность массива у всех картежей одинакова
            int tuplesCount = objs[0].Count; // Размерность массива элементов  ВЫЧЕСЛЯЕТСЯ ОШИБОЧНО !! НУЖЕН ПЕРЕБОР!!!!
            for (int i = 1; i < objs.Length; i++)
            {
                if (objs[i].Count != tuplesCount)
                {
                    if (tuplesCount == 1 && objs[i].Count > 1)
                        tuplesCount = objs[i].Count;
                    else
                        throw new ArgumentException("Несбалансированное количество элементов во входном потоке");
                }
            }

            bool doMulti = tuplesCount > 1;
            string[] storages;
            if (!rules.IsStorageSet())
                storages = names;
            else
                storages = rules.GetStorageNames();

            int[] indexResultNameInCalling = null;

            if (methodParam != null)
            {
                indexResultNameInCalling = new int[itemsCount];
                for (int i = 0; i < names.Length; i++)
                {
                    for (int j = 0; j < methodParam.Length; j++)
                    {
                        if (methodParam[j].Name == storages[i])
                        {
                            indexResultNameInCalling[i] = j;
                            
                            //if (methodParam[j].ParameterType.GetElementType() == null)
                            //    throw new ArgumentException("Неверная привязка к параметру вызова, должен быть массив");

                            if (methodParam[j].ParameterType.GetElementType() != null)
                                storParams[j] = Array.CreateInstance(methodParam[j].ParameterType.GetElementType(), tuplesCount);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (tuplesCount > 1)
                    tuples = new DataTuple[tuplesCount];
            }

            int[] index = new int[itemsCount]; // Показывает в каком элементе *objs* содержится объект соответствующем именем для *rules*
            bool init = false;
            for (int i = 0; i < tuplesCount; i++)
            {
                //// FIXME SPEED ISSUE
                ITupleItem[] items = null;
                if (methodParam == null)
                    items = new ITupleItem[itemsCount];

                for (int j = 0; j < names.Length; j++) // Собираем выходной кортеж
                {
                    if (init == false)
                    {
                        bool found = false;
                        for (int m = 0; (m < parameters) && (found == false); m++) //Ищем интересующий параметр во всех параметрах
                        {
                            foreach (ITupleItem item in objs[m][i])
                            {
                                string name = item.GetName();
                                string newStream, newName;
                                if (maps != null && (maps.IsMapped(objs[m][i].GetStreamName(), name, out newStream, out newName)))
                                {
                                    if ((newStream != null) && (newStream.Length > 0) && (newStream != streamName))
                                        throw new Exception("Unable to handle this map");

                                    if (newName == names[j])
                                    {
                                        index[j] = m;
                                        found = true;

                                        names_as[j] = name;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (name == names[j])
                                    {
                                        index[j] = m;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (found == false)
                            throw new Exception(String.Format("Can't find name \"{0}\" in multi item", names[j]));
                    }

                    object itemResulting = null;
                    if (rules.IsTypeInfoFor(names[j]))
                    {
                        if (i == 0 || objs[index[j]].Count > 1)
                            itemResulting = objs[index[j]][i].GetParam(names_as[j]).CastTo(env, rules.GetTypeInfo(j));
                    }
                    else
                    {
                        if (i == 0 || objs[index[j]].Count > 1)
                            itemResulting = objs[index[j]][i].GetParam(names_as[j]).Rename(names[j]);
                    }

                    if (methodParam != null)
                    {
                        if (objs[index[j]].Count > 1)
                            ((Array)storParams[indexResultNameInCalling[j]]).SetValue(itemResulting, i);
                        else if (i == 0)
                            storParams[indexResultNameInCalling[j]] = itemResulting;
                    }
                    else
                    {
                        if (i == 0 || objs[index[j]].Count > 1)
                            items[j] = (ITupleItem)itemResulting;
                        else
                            items[j] = tuples[0][j];
                    }

                }

                if (methodParam == null)
                {
                    DataTuple dataTup = new DataTuple(streamName, objs[0][i].GetTimeDate(), items);
                    if (doMulti)
                        tuples[i] = dataTup;
                    else
                        return dataTup;
                }

                init = true;
            }

            return new MultiDataTuple(tuples);
        }


        static object GetParametrObject(int index, int layer, object[] inObjects)
        {
            if (layer == -1)
                return inObjects[index];

            Array i = (Array)inObjects[index];
            return i.GetValue(layer);
        }

        public static IMultiDataTuple CreateTupleFromParam(IEnviromentEx env, ISingleTupleRules rules, System.Reflection.ParameterInfo[] paramsInfo, object[] inObjects)
        {
            string[] names = rules.GetDesiredNames();
            string[] storage;

            if (rules.IsStorageSet())
                storage = rules.GetStorageNames();
            else
                storage = names;

            int layerCount = 1;
            if (rules.IsTupleArray())
            {
                layerCount = -1; //Если сопоставляемое имя не было найде то не удасться создать массив, тоесть вызовет исплючение
                string testStorage = storage[0];
                if (testStorage == null)
                    testStorage = rules.GetTypeInfo(0).GetStorages()[0];

                if (testStorage == null)
                    throw new ArgumentException("Невозможно определить размерность выходных данных");

                for (int j = 0; j < paramsInfo.Length; j++)
                    if (paramsInfo[j].Name == testStorage)
                    {
                        Array q = (Array)inObjects[j];
                        layerCount = q.Length;
                        break;
                    }
            }

            DataTuple[] all = rules.IsTupleArray() ? new DataTuple[layerCount] : null;
            for (int l = 0; l < layerCount; l++)
            {
                ITupleItem[] items = new ITupleItem[names.Length];
                int layer = -1;
                if (rules.IsTupleArray())
                    layer = l;

                for (int i = 0; i < items.Length; i++)
                {
                    // Определяем соответствие индекса в массиве параметра вызова для данного имени
                    int indexResultNameInCalling = -1;
                    for (int j = 0; j < paramsInfo.Length; j++)
                        if (paramsInfo[j].Name == storage[i])
                        {
                            indexResultNameInCalling = j;
                            break;
                        }

                    ITypeRules tr = rules.GetTypeInfo(i);
                    ITupleItem item = null;

                    switch (tr.GetTypeString().ToLower())
                    {
                        case "array":
                            item = DataArray.CastFrom(GetParametrObject(indexResultNameInCalling, layer, inObjects), tr);
                            break;
                        case "cart":
                            item = DataCartogram.CastFrom(GetParametrObject(indexResultNameInCalling, layer, inObjects), env, tr);
                            break;
                        case "paramtable":
                            {
                                string[] details = tr.GetCastDetails();
                                string[] vars = tr.GetStorages();
                                if (vars == null)
                                    vars = new string[1] { tr.GetStorage() == null ? tr.GetName() : tr.GetStorage() };
                                if (details.Length != vars.Length)
                                    throw new ArgumentException();

                                DataParamTableItem[] v = new DataParamTableItem[details.Length];
                                for (int j = 0; j < paramsInfo.Length; j++)
                                    for (int k = 0; k < details.Length; k++)
                                    {
                                        if (paramsInfo[j].Name == vars[k])
                                        {
                                            v[k] = new DataParamTableItem(details[k], AnyValue.FromBoxedValue(
                                                GetParametrObject(j, layer, inObjects)));
                                            break;
                                        }
                                    }

                                item = new DataParamTable(new TupleMetaData(tr.GetName(), tr.GetHelpName(), DateTime.Now, TupleMetaData.StreamAuto), v);
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }

                    items[i] = item;
                }

                DataTuple tup = new DataTuple(rules.GetTupleName(), DateTime.Now, items);
                if (rules.IsTupleArray())
                    all[l] = tup;
                else
                    return tup;
            }

            return new MultiDataTuple(all);
        }

        public struct MultiItem : IMultiTupleItem
        {
            int _num;
            MultiDataTuple _items;

            public MultiItem(MultiDataTuple items, int num)
            {
                _items = items;
                _num = num;
            }

            public MultiItem(MultiDataTuple items, string name)
            {
                _items = items;

                for (int i = 0; i < _items._tups[0].ItemsCount; i++)
                {
                    if (_items._tups[0][i].Name == name)
                    {
                        _num = i;
                        return;
                    }
                }

                throw new ArgumentException();
            }




            #region IMultiTupleItem Members

            public string GetName()
            {
                return _items._tups[0][_num].Name;
            }

            public string Name
            {
                get { return _items._tups[0][_num].Name; }
            }

            public int Count
            {
                get { return _items.Count; }
            }

            public ITupleItem this[int idx]
            {
                get { return _items._tups[idx][_num]; }
            }

            #endregion

            #region ITimeData Members

            public DateTime GetTimeDate()
            {
                return _items._tups[0][_num].Date;
            }

            #endregion

            #region IStreamInfo Members

            public string GetStreamName()
            {
                return _items._tups[0][_num].GetStreamName();
            }

            #endregion
        }

        

        public MultiItem this[string name]
        {
            get { return new MultiItem(this, name); }
        }

        public MultiItem GetItem(int idx)
        {
            return new MultiItem(this, idx);
        }

        public DataTuple this[int idx]
        {
            get { return _tups[idx]; }
        }

        #region IMultiDataTuple Members

        IDataTuple IMultiDataTuple.this[int idx]
        {
            get { return this[idx]; }
        }

        IMultiTupleItem IMultiDataTuple.this[string name]
        {
            get { return this[name]; }
        }

        IMultiTupleItem IMultiDataTuple.GetItem(int idx)
        {
            return GetItem(idx);
        }

        IMultiDataTuple IMultiDataTuple.ReStream(string newname)
        {
            return this.ReStream(newname);
        }

        #endregion

        #region IDataTupleInfo Members

        public int Count
        {
            get { return _tups.Length; }
        }

        public void CopyNamesTo(string[] array)
        {
            CopyNamesTo(array, 0);
        }

        public void CopyNamesTo(string[] arraym, int idx)
        {
            for (int i = 0; i < _tups[0].ItemsCount; i++)
            {
                arraym[idx + i] = _tups[0][i].Name;
            }
        }

        #endregion

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _tups[0].GetTimeDate();
        }

        #endregion

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _tups[0].StreamName;
        }

        #endregion


        #region IMultiDataTuple Members

        int IMultiDataTuple.Count
        {
            get { return _tups.Length; }
        }

        #endregion

        #region IDataTupleInfo Members

        int IDataTupleInfo.ItemsCount
        {
            get { return _tups[0].ItemsCount; }
        }

        #endregion
    }
 


    [AttributeDataComponent("Чтение среза данных", ComponentFileFilter = "Данные среза *.tup|*.tup", ComponentFileNameArgument = "dataTupleSingleFilePath")]
    public class DataTupleProvider : ISingleDataProvider
    {
        IEnviromentEx _env;
        string _filename;
        public DataTupleProvider(IEnviromentEx enviromentObject, string dataTupleSingleFilePath)
        {
            _env = enviromentObject;
            _filename = dataTupleSingleFilePath;
        }

        #region ISingleDataProvider Members

        public IMultiDataTuple GetData()
        {
            string sss = Directory.GetCurrentDirectory();
            byte[] data;
            using (FileStream fs = File.OpenRead(_filename))
            {
                data = new byte[(int)fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }

            int id;
            StreamDeserializer s = new StreamDeserializer(data, 0);
            s.Get(out id);
            s.RestoryOriginal();

            if (id == KnownType.serializeDataTuple)
                return new DataTuple(_env, s);
            else
                return new MultiDataTuple(_env, s);
        }

        public void PushData(IMultiDataTuple data)
        {
            if (data.Count > 1)
                MultiDataTuple.SaveToFile(data, _filename);
            else
            {
                DataTuple d = new DataTuple(data[0]);
                d.SaveToFile(_filename);
            }
        }

        public string[] GetExtraNames()
        {
            return null;
        }

        #endregion

        #region IDataProvider Members

        public string[] GetPossibleNames()
        {
            return new string[0];
        }

        public string[] GetExistNames()
        {
            return new string[0];
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
