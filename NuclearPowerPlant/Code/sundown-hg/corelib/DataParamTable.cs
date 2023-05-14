using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Runtime.InteropServices;
using System.Text;

namespace corelib
{
    public class DataParamTable : AbstactTupleItem, ITupleItem, IDataParamTable
    {

        public object CastTo(corelibnew.IGetCoordsConverter en, ITypeRules rules)
        {
            return CastTo(rules);
        }

        public ITupleItem Rename(TupleMetaData newInfo)
        {
            return new DataParamTable(newInfo.Name, newInfo.HumaneName, newInfo.Date, _table);
        }

        #region IChronoSerializer Members

        public override void Serialize(ISerializeStream stream)
        {
            int count = _table.Count;
            stream.Put(StreamSerializerId);

            stream.Put(count);

#if !DOTNET_V11
            foreach (KeyValuePair<string, object> t in _table)
#else
            foreach (DictionaryEntry t in _table)
#endif
            {
                string idx = (string)t.Key;
                object obj = t.Value;

                stream.Put(idx);
                int type = KnownType.GetIdFromType(obj.GetType());
                stream.Put(type);

                if (type == KnownType.string_ArrayStreamCode)
                {
                    string s = (string)obj;
                    stream.Put(s);
                }
                else
                {
                    stream.PutStruct((ValueType)obj);
                }
            }
        }

        public ISerializeStream Serialize()
        {
            StreamSerializer sr = new StreamSerializer(Name, Date, HumaneName);
            Serialize(sr);
            return sr;
        }

        #endregion


        #region ITupleItem Members

        public ITupleItem CastTo(ITypeRules rules)
        {
            if (rules.GetTypeString() != "ParamTable")
                throw new Exception("Don't know how to cast");

            if (rules.GetCastDetails().Length == 0)
                return Rename(rules.GetName());

            throw new Exception("The method or operation is not implemented.");
        }

        public ITupleItem Clone(string name, string humanName, DateTime date)
        {
            //return new DataParamTable(name, humanName, date, _table.Clone());
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        public DataParamTable(string name, string humanName, DateTime time, string objname1, object obj1)
            : this(new TupleMetaData(name, humanName, time))
        {
            _table.Add(objname1, obj1);
        }
        public DataParamTable(string name, string humanName, DateTime time, string objname1, object obj1, string objname2, object obj2)
            : this(new TupleMetaData(name, humanName, time))
        {
            _table.Add(objname1, obj1);
            _table.Add(objname2, obj2);
        }

        public DataParamTable(string name, string humanName, DateTime time, string objname1, object obj1, string objname2, object obj2, string objname3, object obj3)
            : this(new TupleMetaData(name, humanName, time))
        {
            _table.Add(objname1, obj1);
            _table.Add(objname2, obj2);
            _table.Add(objname3, obj3);
        }

        public DataParamTable(string name, string humanName, DateTime time, string objname1, object obj1, string objname2, object obj2, string objname3, object obj3, string objname4, object obj4)
            : this(new TupleMetaData(name, humanName, time))
        {
            _table.Add(objname1, obj1);
            _table.Add(objname2, obj2);
            _table.Add(objname3, obj3);
            _table.Add(objname4, obj4);
        }
        public DataParamTable(string name, string humanName, DateTime time, Hashtable i)
            : this(new TupleMetaData(name, humanName, time))
        {
            foreach (DictionaryEntry t in i)
            {
                string idx = (string)t.Key;
                object obj = t.Value;
                int type = KnownType.GetIdFromType(obj.GetType());

                _table.Add(idx, obj);
            }
        }

#if !DOTNET_V11

        public DataParamTable(string name, string humanName, DateTime time, Dictionary<string, object> dic)
            : this(new TupleMetaData(name, humanName, time))
        {
            foreach (KeyValuePair<string, object> t in dic)
            {
                string idx = (string)t.Key;
                object obj = t.Value;
                int type = KnownType.GetIdFromType(obj.GetType());

                _table.Add(idx, obj);
            }
        }

        static public DataParamTable Create<T>(string name, string humanName, DateTime time, Dictionary<string, T> dic)
        {
            int type = KnownType.GetIdFromType(typeof(T));

            DataParamTable n = new DataParamTable(new TupleMetaData(name, humanName, time));
            foreach (KeyValuePair<string, T> t in dic)
            {
                string idx = t.Key;
                T obj = t.Value;

                n._table.Add(idx, obj);
            }
            return n;
        }
#endif
        public object this[string idx]
        {
            get { return _table[idx]; }
        }

        public string GetParamString(string param)
        {
            return _table[param].ToString();
        }

        public object GetParam(string param)
        {
            return _table[param];
        }

        public object GetParamSafe(string param)
        {
#if DOTNET_V11
            return _table[param];
#else
            object val;
            if (_table.TryGetValue(param, out val))
                return val;
            return null;
#endif
        }


        static readonly IInfoFormatter defFormatter = new DataStringConverter();
        internal DataParamTable(TupleMetaData info)
            : base (info)
        {
            _formatter = defFormatter;
        }

        public DataParamTable(IDeserializeStream stream)
            : this(stream.Info)
        {
            int testStreamSerializerId;

            stream.Get(out testStreamSerializerId);

            if (testStreamSerializerId != StreamSerializerId)
                throw new Exception("DataArray: incorrect stream");

            int count;
            stream.Get(out count);
            for (int i = 0; i < count; i++)
            {
                string idx;
                stream.Get(out idx);
                int type_idx;
                stream.Get(out type_idx);
                object obj;
                if (type_idx == KnownType.string_ArrayStreamCode)
                {
                    string tmpVal;
                    stream.Get(out tmpVal);
                    obj = tmpVal;
                }
                else
                {
                    obj = stream.GetStruct(KnownType.GetTypeFromId(type_idx));
                }

                _table.Add(idx, obj);
            }
        }

        public String DumpCSV()
        {
            StringBuilder sb = new StringBuilder();

#if DOTNET_V11
            foreach (DictionaryEntry t in _table)
#else
            foreach (KeyValuePair<string, object> t in _table)
#endif
            {
                string idx = (string)t.Key;
                object obj = t.Value;

                sb.AppendFormat("{0};{1}", idx, obj);
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        public static readonly int StreamSerializerId = 40;

        static public bool IsMyStreamSerializerId(int id)
        {
            return (id == StreamSerializerId);
        }

#if DOTNET_V11
        public IEnumerable GetAllNames()
        {
            return _table.Keys;
        }
#else
        public IEnumerable<string> GetAllNames()
        {
            return _table.Keys;
        }
#endif


#if DOTNET_V11
        private Hashtable _table = new Hashtable();
#else
        private Dictionary<string, object> _table = new Dictionary<string, object>();
#endif

        #region ITupleItem Members


        public ITupleItem Rename(string newParamName)
        {
            return new DataParamTable(newParamName, HumaneName, Date, _table);
        }

        #endregion

        public override DataGrid DataTable
        {
            get
            {
                DataGrid d = new DataGrid();

                d.Columns.Add("Параметер", typeof(string));
                d.Columns.Add(new DataGrid.Column("Значение", typeof(object), _formatter));

                d.HeadColumns = 1;

                foreach (string key in _table.Keys)
                {
                    d.Rows.Add(key, _table[key]);
                }

                return d;
            }
        }

        public int Count
        {
            get
            {
                return _table.Count;
            }
        }
    }

}
