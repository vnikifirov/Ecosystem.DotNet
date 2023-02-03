using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Xml;
using System.IO;

namespace corelib
{

    public struct DataParamTableItem    
    {
        private string _idx;
        private AnyValue _value;

        public DataParamTableItem(string name, AnyValue value)
        {
            _idx = name;
            _value = value;
        }

        public string Index
        {
            get { return _idx; }
        }

        public AnyValue Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return String.Format("{0}=>{1}", _idx, _value);
        }
    }

    public class DataParamTable : AbstactTupleItem, IDataParamTable
#if !DOTNET_V11
        , IEnumerable<DataParamTableItem>
#else
        , IEnumerable
#endif
    {

        public const string RulesName = "ParamTable";

#if DOTNET_V11
        private Hashtable _table = new Hashtable();

        public struct Enumarator : IEnumerator
        {
            IDictionaryEnumerator _enum;

            public Enumarator(DataParamTable parent)
            {
                _enum = parent._table.GetEnumerator();
            }

            public DataParamTableItem Current
            {
                get
                {
                    return new DataParamTableItem(
                        (string)_enum.Key,
                        (AnyValue)_enum.Value);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return _enum.MoveNext();
            }

            public void Reset()
            {
                _enum.Reset();
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool GetParam(string name, out AnyValue val)
        {
            object o = _table[name];
            if (o != null)
            {
                val = (AnyValue)o;
                return true;
            }
            else
            {
                val = new AnyValue();
                return false;
            }
        }
#else
        private Dictionary<string, AnyValue> _table = new Dictionary<string, AnyValue>();

        public struct Enumarator : IEnumerator<DataParamTableItem>
        {
            Dictionary<string, AnyValue>.Enumerator _enum;

            public Enumarator(DataParamTable parent)
            {
                _enum = parent._table.GetEnumerator();
            }

            public DataParamTableItem Current
            {
                get { return new DataParamTableItem(_enum.Current.Key, _enum.Current.Value); }
            }

            public void Dispose()
            {
                _enum.Dispose();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return _enum.MoveNext();
            }

            public void Reset()
            {
                ((IEnumerator)_enum).Reset();
            }
        }

        IEnumerator<DataParamTableItem> IEnumerable<DataParamTableItem>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool GetParam(string name, out AnyValue val)
        {
            return _table.TryGetValue(name, out val);
        }
#endif

        public int Count
        {
            get
            {
                return _table.Count;
            }
        }

        public AnyValue this[string index]
        {
            get { return GetParam(index); }
        }

        public AnyValue GetParam(string name)
        {
            return (AnyValue)_table[name];
        }

        public AnyValue GetParamSafe(string name)
        {
            AnyValue v;
            if (_table.TryGetValue(name, out v))
                return v;

            return AnyValue.Empty();
        }

        public Enumarator GetEnumerator()
        {
            return new Enumarator(this);
        }

        protected DataParamTable(TupleMetaData info)
            : base(info)
        {
        }

        protected DataParamTable(TupleMetaData info, DataParamTable orig)
            : base(info)
        {
            _table = orig._table;
        }

        public DataParamTable(TupleMetaData info, params DataParamTableItem[] items)
            : base(info)
        {
            foreach (DataParamTableItem item in items)
                _table.Add(item.Index, item.Value);
        }

        public DataParamTable(TupleMetaData info, IEnumerable<DataParamTableItem> items)
            : base(info)
        {
            foreach (DataParamTableItem item in items)
                _table.Add(item.Index, item.Value);
        }

        public DataParamTable(TupleMetaData info, DataParamTable orig, IEnumerable<DataParamTableItem> items)
            : base(info)
        {
            foreach (KeyValuePair<string,AnyValue> it in orig._table)
                _table.Add(it.Key, it.Value);

            foreach (DataParamTableItem item in items)
                _table.Add(item.Index, item.Value);
        }

        public DataParamTable(TupleMetaData info, DataParamTable orig, DataParamTableItem i1)
            : base(info)
        {
            foreach (KeyValuePair<string, AnyValue> it in orig._table)
                _table.Add(it.Key, it.Value);

            _table.Add(i1.Index, i1.Value);
        }

        public DataParamTable(TupleMetaData info, DataParamTable orig, DataParamTableItem i1, DataParamTableItem i2)
            : base(info)
        {
            foreach (KeyValuePair<string, AnyValue> it in orig._table)
                _table.Add(it.Key, it.Value);

            _table.Add(i1.Index, i1.Value);
            _table.Add(i2.Index, i2.Value);
        }

        public DataParamTable(TupleMetaData info, DataParamTableItem i1)
            : base(info)
        {
            _table.Add(i1.Index, i1.Value);
        }

        public DataParamTable(TupleMetaData info, DataParamTableItem i1, DataParamTableItem i2)
            : base(info)
        {
            _table.Add(i1.Index, i1.Value);
            _table.Add(i2.Index, i2.Value);
        }

        public DataParamTable(TupleMetaData info, DataParamTableItem i1, DataParamTableItem i2, DataParamTableItem i3)
            : base(info)
        {
            _table.Add(i1.Index, i1.Value);
            _table.Add(i2.Index, i2.Value);
            _table.Add(i3.Index, i3.Value);
        }

        public override IInfoFormatter GetDefForamtter(IGetDataFormatter env)
        {
            return env.GetDefFormatter(FormatterType.String);
        }

        public override DataGrid CreateDataGrid(IEnviromentEx env)
        {
            DataGrid d = new DataGrid();

            d.Columns.Add("Параметер", typeof(string));
            if (env != null)
                d.Columns.Add(new DataGrid.Column("Значение", typeof(object), GetDefForamtter(env)));
            else
                d.Columns.Add(new DataGrid.Column("Значение", typeof(object)));

            d.HeadColumns = 1;

            foreach (DataParamTableItem i in this)
            {
                d.Rows.Add(i.Index, i.Value);
            }

            return d;
        }


        bool ParseDataType(string type, out Type nType)
        {
            switch (type)
            {
                case "int":
                    nType = typeof(int);
                    return true;
                case "short":
                    nType = typeof(short);
                    return true;
                case "byte":
                    nType = typeof(byte);
                    return true;
                case "float":
                    nType = typeof(float);
                    return true;
                case "double":
                    nType = typeof(double);
                    return true;

                case "coords":
                    nType = typeof(corelib.Coords);
                    return true;

                default:
                    nType = typeof(void);
                    return false;
            }
        }

        public object CastToObject(IGetCoordsConverter en, ITypeRules rules)
        {
            Type t;
            if (ParseDataType(rules.GetTypeString(), out t))
            {
                if ((rules.GetCastDetails() == null) || rules.GetCastDetails().Length != 1)
                    throw new InvalidCastException("Need precise spec");
                string param = rules.GetCastDetails()[0];

                try
                {
                    AnyValue v = GetParam(param);
                    return v.ToType(t, null);
                }
                catch (Exception e)
                {
                    throw new InvalidCastException("Inner error", e);
                }
            }
            throw new InvalidCastException("Unknown type");
        }

        public override object CastTo(IGetCoordsConverter en, ITypeRules rules)
        {
            if (rules.GetTypeString() == RulesName)
            {
                if (rules.GetCastDetails().Length == 0)
                    return Rename(new TupleMetaData(rules.GetName(), HumaneName, Date, Stream));

                //TODO: сделать проверку запрашиваешмых элементов
                throw new InvalidCastException();
            }
            else
                return CastToObject(en, rules);
        }

        public override void Serialize(ISerializeStream stream)
        {
            Serialize(0, stream);
        }

        public override void Serialize(int abiver, ISerializeStream stream)
        {
            if ((abiver == 0) /*||  (abiver == 1)*/)
            {
                int count = _table.Count;
                stream.Put(KnownType.DataParamTable_StreamSerializerId);

                stream.Put(count);

                foreach (DataParamTableItem t in this)
                {
                    string idx = (string)t.Index;
                    AnyValue obj = (AnyValue)t.Value;

                    stream.Put(idx);
                    if (obj.IsDataParamTable)
                    {
                        obj.ValueDataParamTable.Serialize(abiver, stream);
                    }
                    else
                    {
                        obj.Serialize(stream, abiver);
                    }
                }
            }
            else
                throw new NotSupportedException();
        }

        public override ITupleItem Rename(TupleMetaData newInfo)
        {
            return new DataParamTable(newInfo, this);
        }

        static public DataParamTable Create(IDeserializeStream stream)
        {
            int testStreamSerializerId;
            stream.Get(out testStreamSerializerId);

            if (testStreamSerializerId != KnownType.DataParamTable_StreamSerializerId)
                throw new ArgumentException("DataArray: incorrect stream");

            DataParamTable res = new DataParamTable(stream.Info);

            int count;
            stream.Get(out count);
            for (int i = 0; i < count; i++)
            {
                string idx;
                stream.Get(out idx);
                int type;
                stream.Get(out type);
                stream.Rewind(4);

                AnyValue value;

                if (type == KnownType.DataParamTable_StreamSerializerId)
                {
                    int dx = stream.GetPos();
                    TupleMetaData md = new TupleMetaData(idx, idx, stream.Date, idx);
                    StreamDeserializer ds = new StreamDeserializer((StreamDeserializer)stream, stream.GetPos(), md);

                    value = DataParamTable.Create(ds);

                    dx -= ds.GetPos();
                    stream.Rewind(dx);
                }
                else
                {
                    value = AnyValue.Deserialize(stream);
                }

                res._table.Add(idx, value);
            }

            return res;
        }


        public static DataParamTable LoadFromXML(string file)
        {
            XmlDocument doc = new XmlDocument();
            using (StreamReader streamReader = new StreamReader(file))
            {
                doc.Load(streamReader);
            }
            string stream = doc.GetElementsByTagName("ParamTuple")[0].Attributes["stream"].Value;

            TupleMetaData m;
            m.Date = File.GetLastWriteTime(file);
            m.HumaneName = stream;
            m.Name = stream;
            m.StreamName = stream;

            return LoadFromXML(doc.GetElementsByTagName("ParamTuple")[0], m);
        }

        private static DataParamTable LoadFromXML(XmlNode nc, TupleMetaData info)
        {
            List<DataParamTableItem> lst = new List<DataParamTableItem>();

            foreach (XmlNode n in nc.ChildNodes)
            {
                if (n.Name == "#comment")
                    continue;

                string name = n.Attributes["name"].Value;
                string help = (n.Attributes["help"] == null) ? "" : n.Attributes["help"].Value;
                string nstream = (n.Attributes["stream"] == null) ? "" : n.Attributes["stream"].Value;

                if (n.Name == "i")
                {
                    string data = n.InnerText;
                    lst.Add(new DataParamTableItem(name, data));
                    //p.Add(name, help, data);
                }
                else if (n.Name == "xml")
                {
                    //p.Add(name, help, n.FirstChild);
                    lst.Add(new DataParamTableItem(name, AnyValue.FromXmlString(n.InnerXml)));
                }
                else if (n.Name == "p")
                {
                    string nfo = n.Attributes["value"].Value;
                    TupleMetaData m = info;
                    m.Name = nfo;
                    m.HumaneName = nfo;
                    DataParamTable pt = LoadFromXML(n, m);
                    //p.Add(name, help, pt);
                    lst.Add(new DataParamTableItem(name, pt));
                }
                else
                    throw new Exception("Incorrect XML config file");
            }
            return new DataParamTable(info, lst);
        }

        public XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elem = doc.CreateElement("ParamTuple");
            XmlAttribute attr = doc.CreateAttribute("stream");            
            attr.Value = Stream;
            elem.Attributes.Append(attr);

            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");

            ToXml(elem);
            doc.AppendChild(dec);
            doc.AppendChild(elem);

            return doc;
        }

        private void ToXml(XmlNode parent)
        {
            string name;

            foreach (DataParamTableItem it in this)
            {
                AnyValue v = it.Value;
                if (v.IsDataParamTable)
                {
                    name = "p";
                }
                else if (v.IsObject)
                {
                    continue;
                }
                else if (v.IsXml)
                {
                    name = "xml";
                }
                else
                {
                    name = "i";
                }

                XmlElement element = parent.OwnerDocument.CreateElement(name);
                XmlAttribute attr = parent.OwnerDocument.CreateAttribute("name");

                if (v.IsDataParamTable)
                {
                    attr.Value = Info.Name;
                    element.Attributes.Append(attr);
                    v.ValueDataParamTable.ToXml(element);
                }
                else if (v.IsXml)
                {
                    attr.Value = Info.Name;
                    element.Attributes.Append(attr);


                    element.InnerXml = v.ValueXml.ParentNode.InnerXml;
                }
                else
                {
                    attr.Value = it.Index;
                    XmlAttribute val = parent.OwnerDocument.CreateAttribute("value");
                    val.Value = v.ToString();

                    element.Attributes.Append(attr);
                    element.Attributes.Append(val);
                }

                parent.AppendChild(element);
            }
        }
    }
}
