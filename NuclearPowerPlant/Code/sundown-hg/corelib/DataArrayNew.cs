using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;


namespace corelib
{

#if !DOTNET_V11
    using DataArrayInt = DataArray<int>;
    using DataArrayShort = DataArray<short>;
    using DataArrayByte = DataArray<byte>;
    using DataArrayFloat = DataArray<float>;
    using DataArrayDouble = DataArray<double>;
    using DataArraySensored = DataArray<Sensored>;
    using DataArrayCoords = DataArray<Coords>;
    using DataArrayFiberCoords = DataArray<FiberCoords>;
    using DataArrayMultiIntFloat = DataArray<MultiIntFloat>;
    using DataArrayTimestamp = DataArray<Timestamp>;

    using OperatorDouble = Operator<double>;
    using ActionDouble = Action<double, double>;
#else
    using ActionDouble = OperatorDouble;
#endif



    public abstract class AbstactTupleItem : ITupleItem, IDataTuple, IMultiTupleItem, IMultiDataTuple
    {
        TupleMetaData _info;

        public AbstactTupleItem(TupleMetaData info)
        {
            _info = info;
        }

        public string HumaneName
        {
            get { return _info.HumaneName; }
        }

        public string Name
        {
            get { return _info.Name; }
        }

        public DateTime Date
        {
            get { return _info.Date; }
        }

        public string Stream
        {
            get { return _info.StreamName; }
        }

        #region ITupleItem Members

        public abstract ITupleItem Rename(TupleMetaData newInfo);

        public abstract IInfoFormatter GetDefForamtter(IGetDataFormatter env);

        public abstract DataGrid CreateDataGrid(IEnviromentEx env);

        public abstract object CastTo(IGetCoordsConverter en, ITypeRules rules);

        #endregion

        #region ITupleItemOld Members

        public abstract void Serialize(int abiver, ISerializeStream stream);
        public abstract void Serialize(ISerializeStream stream);

        public virtual ISerializeStream Serialize()
        {
            ISerializeStream s = new StreamSerializer(_info);
            Serialize(s);
            return s;
        }

        public ITupleItem CastTo(ITypeRules rules)
        {
            return (ITupleItem)CastTo(null, rules);
        }

        public ITupleItem Clone(string name, string humanName, DateTime date)
        {
            return Rename(new TupleMetaData(name, humanName, date, Stream));
        }

        public ITupleItem Rename(string newParamName)
        {
            return Rename(new TupleMetaData(newParamName, HumaneName, Date, Stream));
        }

        public string DumpCSV()
        {
            return DataTable.DumpCSV();
        }

        #endregion

        #region ITupleMetaData Members

        public string GetName()
        {
            return Name;
        }

        public string GetHumanName()
        {
            return HumaneName;
        }

        public TupleMetaData Info
        {
            get { return _info; }
        }

        #endregion

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return Date;
        }

        #endregion

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return Stream;
        }

        #endregion

        #region IFormatterItem Members

        public IInfoFormatter InfoFormatter
        {
            get
            {
                throw new NotSupportedException("Only valid for old API");
            }
            set
            {
                throw new NotSupportedException("Only valid for old API");
            }
        }

        public DataGrid DataTable
        {
            get { return CreateDataGrid(null); }
        }

        #endregion


        public override string ToString()
        {
            return String.Format("{5}:{0,-15} {1,-15}{4} '{2}' {3}", Name, GetType().Name, HumaneName, Date,
#if !DOTNET_V11
                GetType().IsGenericType ? "[" + GetType().GetGenericArguments()[0].Name + "]" : String.Empty,
#else
				String.Empty,
#endif
                Stream);
        }

        #region Single DataTuple Emulation

        #region IDataTuple Members

        int IDataTupleInfo.ItemsCount
        {
            get { return 1; }
        }

        void IDataTupleInfo.CopyNamesTo(string[] array)
        {
            array[0] = _info.Name;
        }

        void IDataTupleInfo.CopyNamesTo(string[] arraym, int idx)
        {
            arraym[idx] = _info.Name;
        }

        ITupleItem IDataTuple.this[int idx]
        {
            get { if (idx == 0) return this; throw new ArgumentException(); }
        }

        ITupleItem IDataTuple.this[string name]
        {
            get { if (name == Name) return this; throw new ArgumentException(); }
        }

        ITupleItem IDataTuple.GetParam(string name)
        {
            if (name == Name) return this; throw new ArgumentException(); 
        }

        ITupleItem IDataTuple.GetParamSafe(string name)
        {
            if (name == Name) return this; return null;
        }

        ITupleItem[] IDataTuple.GetData()
        {
            return new AbstactTupleItem[] { this };
        }

        IDataTuple IDataTuple.CastTo(ISingleTupleRules rules)
        {
            return (IDataTuple)CastTo(null, rules.GetTypeInfo(Name));
        }

        IDataTuple IDataTuple.ReStream(string newname)
        {
            TupleMetaData md = Info;
            md.StreamName = newname;
            return (AbstactTupleItem)Rename(md);
        }

        #endregion

        #region IDuckTuple Members

        string IDuckTuple.GetParamHelp(string name)
        {
            if (name == Name) return HumaneName; throw new ArgumentException(); 
        }

        object IDuckTuple.this[string name]
        {
            get { return new AbstactTupleItem[] { this }; }
        }

        #endregion


        class Enumerator :
#if !DOTNET_V11
            IEnumerator<ITupleItem>
#else
            IEnumerator
#endif
        {
            AbstactTupleItem item;
            bool init;

            public Enumerator(AbstactTupleItem i)
            {
                item = i;
                init = false;
            }

            #region IEnumerator<ITupleItem> Members

            public ITupleItem Current
            {
                get { return item; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return item; }
            }

            public bool MoveNext()
            {
                if (!init)
                {
                    init = true;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                init = false;
            }

            #endregion
        }

#if !DOTNET_V11
        #region IEnumerable<ITupleItem> Members

        IEnumerator<ITupleItem> IEnumerable<ITupleItem>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion
#endif
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion
        #endregion

        #region IMultiTupleItem Members

        int IMultiTupleItem.Count
        {
            get { return 1; }
        }

        ITupleItem IMultiTupleItem.this[int idx]
        {
            get { if (idx == 0) return this; throw new ArgumentException(); }
        }

        IMultiTupleItem IMultiDataTuple.this[string name]
        {
            get { if (name == Name) return this; throw new ArgumentException(); }
        }

        IDataTuple IMultiDataTuple.this[int idx]
        {
            get { if (idx == 0) return this; throw new ArgumentException(); }
        }

        IMultiTupleItem IMultiDataTuple.GetItem(int idx)
        {
            if (idx == 0) return this; throw new ArgumentException();
        }

        int IMultiDataTuple.Count
        {
            get { return 1; }
        }

        IMultiDataTuple IMultiDataTuple.ReStream(string newname)
        {
            TupleMetaData md = Info;
            md.StreamName = newname;
            return (AbstactTupleItem)Rename(md);
        }

        #endregion


        IDataCartogram IDataTuple.GetCart(string name)
        {
            return ((IDataTuple)(this)).GetParam(name) as IDataCartogram;
        }
        IDataArray IDataTuple.GetArray(string name)
        {
            return ((IDataTuple)(this)).GetParam(name) as IDataArray;
        }
    }

    public abstract class DataArray : AbstactTupleItem, ITupleItem, IEnumerable, IDataArray
    {
        readonly protected int _x;
        readonly protected int _y;
        readonly protected int _z;

        readonly protected int _elementType;
        readonly protected int _rank;

        protected DataArray(TupleMetaData info, int x, int y, int z, Type elemnet)
            : base(info)
        {
            _x = x;
            _y = y;
            _z = z;
            _elementType = KnownType.GetIdFromType(elemnet);

            if (_x <= 0)
                //throw new NotSupportedException();
                _rank = 0;
            else if (_y <= 0)
                _rank = 1;
            else if (_z <= 0)
                _rank = 2;
            else
                _rank = 3;

        }

        abstract public Type ElementType
        {
            get;
        }

        public int ElementId
        {
            get { return _elementType; }
        }

        public int Rank
        {
            get { return _rank; }
        }

        abstract public int Length
        {
            get;
        }

        public int DimX
        {
            get { return _x; }
        }
        public int DimY
        {
            get { return _y; }
        }
        public int DimZ
        {
            get { return _z; }
        }

        public bool IsByte
        {
            get { return _elementType == KnownType.byte_ArrayStreamCode; }
        }
        public bool IsShort
        {
            get { return _elementType == KnownType.short_ArrayStreamCode; }
        }
        public bool IsInt
        {
            get { return _elementType == KnownType.int_ArrayStreamCode; }
        }
        public bool IsDouble
        {
            get { return _elementType == KnownType.double_ArrayStreamCode; }
        }
        public bool IsFloat
        {
            get { return _elementType == KnownType.float_ArrayStreamCode; }
        }
        public bool IsSensored
        {
            get { return _elementType == KnownType.Sensored_ArrayStreamCode; }
        }
        public bool IsCoords
        {
            get { return _elementType == KnownType.Coords_ArrayStreamCode; }
        }
        public bool IsFiberCoords
        {
            get { return _elementType == KnownType.FiberCoords_ArrayStreamCode; }
        }
        public bool IsMultiIntFloat
        {
            get { return _elementType == KnownType.MultiIntFloat_ArrayStreamCode; }
        }

        protected int ToLinear(int x, int y)
        {
            return x * _y + y;
        }

        protected int ToLinear(int x, int y, int z)
        {
            return (x * _y + y) * _z + z;
        }

        abstract protected object GetObject(int index);
        abstract protected object GetObject(int x, int y);
        abstract protected object GetObject(int x, int y, int z);

        public object this[int index]
        {
            get { return GetObject(index); }
        }

        public object this[int x, int y]
        {
            get { return GetObject(x, y); }
        }

        public object this[int x, int y, int z]
        {
            get { return GetObject(x, y, z); }
        }

        public AnyValue GetAnyValueLinear(int index)
        {
            switch (_elementType)
            {
                case KnownType.byte_ArrayStreamCode: return ((DataArrayByte)this).GetLinear(index);
                case KnownType.short_ArrayStreamCode: return ((DataArrayShort)this).GetLinear(index);
                case KnownType.int_ArrayStreamCode: return ((DataArrayInt)this).GetLinear(index);
                case KnownType.float_ArrayStreamCode: return ((DataArrayFloat)this).GetLinear(index);
                case KnownType.double_ArrayStreamCode: return ((DataArrayDouble)this).GetLinear(index);
                case KnownType.Sensored_ArrayStreamCode: return ((DataArraySensored)this).GetLinear(index);
                case KnownType.Coords_ArrayStreamCode: return ((DataArrayCoords)this).GetLinear(index);
                case KnownType.FiberCoords_ArrayStreamCode: return ((DataArrayFiberCoords)this).GetLinear(index);
                case KnownType.MultiIntFloat_ArrayStreamCode: return ((DataArrayMultiIntFloat)this).GetLinear(index);
                default:
                    throw new NotSupportedException();
            }
        }

        public AnyValue GetAnyValue(int index)
        {
            if (_rank != 1)
                throw new RankException();
            return GetAnyValueLinear(index);
        }

        public AnyValue GetAnyValue(int x, int y)
        {
            if (_rank != 2)
                throw new RankException();
            else if (y >= _y)
                throw new ArgumentException();

            return GetAnyValueLinear(ToLinear(x, y));
        }

        public AnyValue GetAnyValue(int x, int y, int z)
        {
            if (_rank != 3)
                throw new RankException();
            else if ((y >= _y) || (z >= _z))
                throw new ArgumentException();

            return GetAnyValueLinear(ToLinear(x, y, z));
        }

        public override IInfoFormatter GetDefForamtter(IGetDataFormatter env)
        {
            Type t = ElementType;
            if (env != null)
            {
                
                if (t == typeof(double) || t == typeof(float))
                    return env.GetDefFormatter(FormatterType.Real, this);
                else if (t == typeof(int) || t == typeof(short) || t == typeof(byte))
                    return env.GetDefFormatter(FormatterType.Int, this);
                else
                    return env.GetDefFormatter(FormatterType.String, this);
                 
            }
            else
            {
                return DataGrid.Column.GetDefaultFormatter(t);
            }
        }


        public override DataGrid CreateDataGrid(IEnviromentEx env)
        {
            DataGrid d = new DataGrid();

            string[] names = { "x", "y", "z" };

            for (int i = 0; i < Rank; i++)
            {
                d.Columns.Add(names[i], typeof(int));
            }
            d.HeadColumns = Rank;

            if (env != null)
                d.Columns.Add(new DataGrid.Column("Значение", ElementType, GetDefForamtter(env)));
            else
                d.Columns.Add("Значение", ElementType);

            for (int x = 0; x < _x; x++)
            {
                if (_y == 0)
                    d.Rows.Add(x, this[x]);
                else
                {
                    for (int y = 0; y < _y; y++)
                    {
                        if (_z == 0)
                            d.Rows.Add(x, y, this[x, y]);
                        else
                        {
                            for (int z = 0; z < _z; z++)
                                d.Rows.Add(x, y, z, this[x, y, z]);
                        }
                    }
                }
            }

            return d;
        }

        public static DataArray Create(IDeserializeStream stream)
        {
            int testStreamSerializerId;
            stream.Get(out testStreamSerializerId);

            if (testStreamSerializerId != KnownType.DataArray_StreamSerializerId)
                throw new ArgumentException("DataArray: incorrect stream");

            int x, y, z, elementType;
            DataArray obj;

            stream.Get(out x);
            stream.Get(out y);
            stream.Get(out z);
            stream.Get(out elementType);

            switch (elementType)
            {
                case KnownType.byte_ArrayStreamCode:
                    obj = new DataArrayByte(stream.Info, stream, x, y, z); break;
                case KnownType.short_ArrayStreamCode:
                    obj = new DataArrayShort(stream.Info, stream, x, y, z); break;
                case KnownType.int_ArrayStreamCode:
                    obj = new DataArrayInt(stream.Info, stream, x, y, z); break;
                case KnownType.float_ArrayStreamCode:
                    obj = new DataArrayFloat(stream.Info, stream, x, y, z); break;
                case KnownType.double_ArrayStreamCode:
                    obj = new DataArrayDouble(stream.Info, stream, x, y, z); break;

                //case KnownType.datetime_ArrayStreamCode:
                //case KnownType.string_ArrayStreamCode:

                case KnownType.Sensored_ArrayStreamCode:
                    obj = new DataArraySensored(stream.Info, stream, x, y, z); break;
                case KnownType.FiberCoords_ArrayStreamCode:
                    obj = new DataArrayFiberCoords(stream.Info, stream, x, y, z); break;
                case KnownType.Coords_ArrayStreamCode:
                    obj = new DataArrayCoords(stream.Info, stream, x, y, z); break;
                case KnownType.MultiIntFloat_ArrayStreamCode:
                    obj = new DataArrayMultiIntFloat(stream.Info, stream, x, y, z); break;

                default:
                    throw new ArgumentException("DataArray: Unknown element type!");
            }

            return obj;
        }

        abstract protected void SerializeArray(ISerializeStream stream);



        public override void Serialize(ISerializeStream stream)
        {
            stream.Put(KnownType.DataArray_StreamSerializerId);
            stream.Put(_x);
            stream.Put(_y);
            stream.Put(_z);
            stream.Put(_elementType);

            if (_rank != 0)
                SerializeArray(stream);
        }

        public override void Serialize(int abiver, ISerializeStream stream)
        {
            if (abiver == 0)
                Serialize(stream);
            else
                throw new NotSupportedException();
        }


        public abstract DataArray Reinfo(TupleMetaData newInfo);

        public abstract DataArray ConvertTo(TupleMetaData newInfo, Type t);

        public abstract Array ConvertToArray(Type t);

        public override ITupleItem Rename(TupleMetaData newInfo)
        {
            return Reinfo(newInfo);
        }

        #region Deprecated

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
                    nType = typeof(Coords);
                    return true;

                default:
                    nType = typeof(void);
                    return false;
            }
        }

        public override object CastTo(IGetCoordsConverter en, ITypeRules rules)
        {
            string tp = rules.GetTypeString().ToLower();
            if (tp == "array")
                return CastToDataArray(rules);
            else
                return CastToArray(rules);
        }

        public static DataArray CastFrom(object a, ITypeRules rules)
        {
            Array arr = (Array)a;

            string tp = rules.GetTypeString().ToLower();
            if (tp == "array")
                return Create(new TupleMetaData(rules.GetName(), rules.GetHelpName(), DateTime.Now, TupleMetaData.StreamAuto), arr);
            else
                return null;
        }



        public Array CastToArray(ITypeRules rules)
        {
            Type t;
            string tp = rules.GetTypeString().ToLower();
            if (!ParseDataType(tp, out t))
                throw new InvalidCastException("Don't know how to convert");

            try
            {
                string[] details = rules.GetCastDetails();
                if (details.Length == 0)
                {
                    Array a = ConvertToArray(t);
                    if (a.Rank != 1)
                        throw new RankException();

                    return a;
                }

                Array b = ConvertToArray(t);
                if (b.Rank != details.Length)
                    throw new RankException();

                for (int i = 0; i < b.Rank; i++)
                {
                    if (details[i] != null && details[i].Length > 0)
                    {
                        int sz = Convert.ToInt32(details[i]);
                        if (b.GetLength(i) != sz)
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return b;
            }
            catch (Exception e)
            {
                throw new InvalidCastException("Internal error during conversion", e);
            }
        }


        public DataArray CastToDataArray(ITypeRules rules)
        {
            // Форматы представления:
            // [type,rank]
            // [type]
            // []

            string[] details = rules.GetCastDetails();
            if (details.Length > 2)
                throw new InvalidCastException("Don't know how to convert to type detail");

            TupleMetaData newInfo = new TupleMetaData(rules.GetName(), HumaneName, Date,Stream);

            if (details.Length == 2)
            {
                int rank = Convert.ToInt32(details[1]);
                if (Rank != rank)
                    throw new InvalidCastException("Can't cast to different rank");
            }
            if (details.Length >= 1)
            {
                string type = details[0].ToLower();
                Type t;
                if (!ParseDataType(type, out t))
                    throw new InvalidCastException("Can't parse to type " + type);

                if (KnownType.GetIdFromType(t) != _elementType)
                {
                    try
                    {
                        return ConvertTo(newInfo, t);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidCastException("Internal error during conversion", e);
                    }
                }
                else
                    return Reinfo(newInfo);
            }
            return Reinfo(newInfo);
        }

        #endregion


        protected abstract IEnumerator GetEnumeratorAbstract();

        public IEnumerator GetEnumerator()
        {
            return GetEnumeratorAbstract();
        }

        abstract public DataArrayDouble Normalize();

        #region Operators


        static double opAdd(double a1, double a2)
        {
            return a1 + a2;
        }
        static double opMul(double a1, double a2)
        {
            return a1 * a2;
        }
        static double opSub(double a1, double a2)
        {
            return a1 - a2;
        }
        static double opDiv(double a1, double a2)
        {
            return a1 / a2;
        }
        static double opRSub(double a1, double a2)
        {
            return a2 - a1;
        }
        static double opRDiv(double a1, double a2)
        {
            return a2 / a1;
        }

        static readonly public OperatorDouble sdopAdd = new OperatorDouble(opAdd);
        static readonly public OperatorDouble sdopMul = new OperatorDouble(opMul);
        static readonly public OperatorDouble sdopSub = new OperatorDouble(opSub);
        static readonly public OperatorDouble sdopDiv = new OperatorDouble(opDiv);
        static readonly public OperatorDouble sdopRSub = new OperatorDouble(opRSub);
        static readonly public OperatorDouble sdopRDiv = new OperatorDouble(opRDiv);

		
		public static DataArrayDouble operator +(DataArray a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("add_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopAdd, a2);
        }

        public static DataArrayDouble operator -(DataArray a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("sub_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopSub, a2);
        }

        public static DataArrayDouble operator *(DataArray a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("mul_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopMul, a2);
        }

        public static DataArrayDouble operator /(DataArray a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("div_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopDiv, a2);
        }


        public static DataArrayDouble operator +(DataArray a1, double a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("add_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopAdd, a2);
        }

        public static DataArrayDouble operator -(DataArray a1, double a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("sub_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopSub, a2);
        }

        public static DataArrayDouble operator *(DataArray a1, double a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("mul_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopMul, a2);
        }

        public static DataArrayDouble operator /(DataArray a1, double a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("div_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto), a1, sdopDiv, a2);
        }


        public static DataArrayDouble operator +(double a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("add_" + a2.Name, a2.HumaneName, a2.Date, TupleMetaData.StreamAuto), a2, sdopAdd, a1);
        }

        public static DataArrayDouble operator -(double a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("sub_" + a2.Name, a2.HumaneName, a2.Date, TupleMetaData.StreamAuto), a2, sdopRSub, a1);
        }

        public static DataArrayDouble operator *(double a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("mul_" + a2.Name, a2.HumaneName, a2.Date, TupleMetaData.StreamAuto), a2, sdopMul, a1);
        }

        public static DataArrayDouble operator /(double a1, DataArray a2)
        {
            return DataArrayDouble.Operation(new TupleMetaData("div_" + a2.Name, a2.HumaneName, a2.Date, TupleMetaData.StreamAuto), a2, sdopRDiv, a1);
        }

        static protected ActionDouble sdacAdd = new ActionDouble(opAdd);

        public double Sum()
        {
            return DataArrayDouble.ForEach(this, sdacAdd, 0.0);
        }

        public double Mean()
        {
            return Sum() / Length;
        }

#if !DOTNET_V11
        struct VarCalc
        {
            public double mean;
            public double var;
        }

        VarCalc actionVar(VarCalc prev, double val)
        {
            prev.var += (prev.mean - val) * (prev.mean - val);
            return prev;
        }

        public double Var()
        {
            VarCalc c;
            c.mean = Mean();
            c.var = 0;

            return DataArray<double>.ForEach<VarCalc>(this, actionVar, c).var;
        }
#endif
        #endregion


        static public DataArray Create(TupleMetaData info, Array a)
        {
            Type t = a.GetType().GetElementType();

            switch (a.Rank)
            {
                case 1:
                    if (t == typeof(byte)) return new DataArrayByte(info, (byte[])a);
                    else if (t == typeof(short)) return new DataArrayShort(info, (short[])a);
                    else if (t == typeof(int)) return new DataArrayInt(info, (int[])a);
                    else if (t == typeof(float)) return new DataArrayFloat(info, (float[])a);
                    else if (t == typeof(double)) return new DataArrayDouble(info, (double[])a);
                    else if (t == typeof(byte)) return new DataArraySensored(info, (Sensored[])a);
                    else if (t == typeof(FiberCoords)) return new DataArrayFiberCoords(info, (FiberCoords[])a);
                    else if (t == typeof(Coords)) return new DataArrayCoords(info, (Coords[])a);
                    else if (t == typeof(MultiIntFloat)) return new DataArrayMultiIntFloat(info, (MultiIntFloat[])a);
                    else if (t == typeof(Timestamp)) return new DataArrayTimestamp(info, (Timestamp[])a);
                    else throw new NotSupportedException();
                case 2:
                    if (t == typeof(byte)) return new DataArrayByte(info, (byte[,])a);
                    else if (t == typeof(short)) return new DataArrayShort(info, (short[,])a);
                    else if (t == typeof(int)) return new DataArrayInt(info, (int[,])a);
                    else if (t == typeof(float)) return new DataArrayFloat(info, (float[,])a);
                    else if (t == typeof(double)) return new DataArrayDouble(info, (double[,])a);
                    else if (t == typeof(byte)) return new DataArraySensored(info, (Sensored[,])a);
                    else if (t == typeof(FiberCoords)) return new DataArrayFiberCoords(info, (FiberCoords[,])a);
                    else if (t == typeof(Coords)) return new DataArrayCoords(info, (Coords[,])a);
                    else if (t == typeof(MultiIntFloat)) return new DataArrayMultiIntFloat(info, (MultiIntFloat[,])a);
                    else throw new NotSupportedException();
                case 3:
                    if (t == typeof(byte)) return new DataArrayByte(info, (byte[, ,])a);
                    else if (t == typeof(short)) return new DataArrayShort(info, (short[, ,])a);
                    else if (t == typeof(int)) return new DataArrayInt(info, (int[, ,])a);
                    else if (t == typeof(float)) return new DataArrayFloat(info, (float[, ,])a);
                    else if (t == typeof(double)) return new DataArrayDouble(info, (double[, ,])a);
                    else if (t == typeof(byte)) return new DataArraySensored(info, (Sensored[, ,])a);
                    else if (t == typeof(FiberCoords)) return new DataArrayFiberCoords(info, (FiberCoords[, ,])a);
                    else if (t == typeof(Coords)) return new DataArrayCoords(info, (Coords[, ,])a);
                    else if (t == typeof(MultiIntFloat)) return new DataArrayMultiIntFloat(info, (MultiIntFloat[, ,])a);
                    else throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
        }
    }

#if !DOTNET_V11
    public delegate T Generator1<T>(int x);
    public delegate T Generator2<T>(int x, int y);
    public delegate T Generator3<T>(int x, int y, int z);
    public delegate T Generator<T>(int x, int y, int z);
    public delegate T Operator<T>(T v1, T v2);
    public delegate U Action<U, V>(U subinfo, V value);

    public static class DataConverter
    {
        static double DoubleToDouble(double value) { return (value); }
        static float DoubleToFloat(double value) { return Convert.ToSingle(value); }
        static int DoubleToInt(double value) { return Convert.ToInt32(value); }
        static short DoubleToShort(double value) { return Convert.ToInt16(value); }
        static byte DoubleToByte(double value) { return Convert.ToByte(value); }

        static double FloatToDouble(float value) { return Convert.ToDouble(value); }
        static float FloatToFloat(float value) { return (value); }
        static int FloatToInt(float value) { return Convert.ToInt32(value); }
        static short FloatToShort(float value) { return Convert.ToInt16(value); }
        static byte FloatToByte(float value) { return Convert.ToByte(value); }

        static double IntToDouble(int value) { return Convert.ToDouble(value); }
        static float IntToFloat(int value) { return Convert.ToSingle(value); }
        static int IntToInt(int value) { return (value); }
        static short IntToShort(int value) { return Convert.ToInt16(value); }
        static byte IntToByte(int value) { return Convert.ToByte(value); }

        static double ShortToDouble(short value) { return Convert.ToDouble(value); }
        static float ShortToFloat(short value) { return Convert.ToSingle(value); }
        static int ShortToInt(short value) { return Convert.ToInt32(value); }
        static short ShortToShort(short value) { return (value); }
        static byte ShortToByte(short value) { return Convert.ToByte(value); }

        static double ByteToDouble(byte value) { return Convert.ToDouble(value); }
        static float ByteToFloat(byte value) { return Convert.ToSingle(value); }
        static int ByteToInt(byte value) { return Convert.ToInt32(value); }
        static short ByteToShort(byte value) { return Convert.ToInt16(value); }
        static byte ByteToByte(byte value) { return (value); }

        static double SensoredToDouble(Sensored value) { return Convert.ToDouble(value.Value); }
        static float SensoredToFloat(Sensored value) { return Convert.ToSingle(value.Value); }
        static int SensoredToInt(Sensored value) { return Convert.ToInt32(value.Value); }
        static short SensoredToShort(Sensored value) { return Convert.ToInt16(value.Value); }
        static byte SensoredToByte(Sensored value) { return Convert.ToByte(value.Value); }

        static Sensored SensoredToSensored(Sensored value) { return value; }

        public static object GetConverter(Type t, Type u)
        {
            if (u == typeof(double))
            {
                if (t == typeof(double)) return (Converter<double, double>)DoubleToDouble;
                else if (t == typeof(float)) return (Converter<float, double>)FloatToDouble;
                else if (t == typeof(int)) return (Converter<int, double>)IntToDouble;
                else if (t == typeof(short)) return (Converter<short, double>)ShortToDouble;
                else if (t == typeof(byte)) return (Converter<byte, double>)ByteToDouble;
                else if (t == typeof(Sensored)) return (Converter<Sensored, double>)SensoredToDouble;
                else throw new NotSupportedException();
            }
            else if (u == typeof(float))
            {
                if (t == typeof(double)) return (Converter<double, float>)DoubleToFloat;
                else if (t == typeof(float)) return (Converter<float, float>)FloatToFloat;
                else if (t == typeof(int)) return (Converter<int, float>)IntToFloat;
                else if (t == typeof(short)) return (Converter<short, float>)ShortToFloat;
                else if (t == typeof(byte)) return (Converter<byte, float>)ByteToFloat;
                else if (t == typeof(Sensored)) return (Converter<Sensored, float>)SensoredToFloat;
                else throw new NotSupportedException();
            }
            else if (u == typeof(int))
            {
                if (t == typeof(double)) return (Converter<double, int>)DoubleToInt;
                else if (t == typeof(float)) return (Converter<float, int>)FloatToInt;
                else if (t == typeof(int)) return (Converter<int, int>)IntToInt;
                else if (t == typeof(short)) return (Converter<short, int>)ShortToInt;
                else if (t == typeof(byte)) return (Converter<byte, int>)ByteToInt;
                else if (t == typeof(Sensored)) return (Converter<Sensored, int>)SensoredToInt;
                else throw new NotSupportedException();
            }
            else if (u == typeof(short))
            {
                if (t == typeof(double)) return (Converter<double, short>)DoubleToShort;
                else if (t == typeof(float)) return (Converter<float, short>)FloatToShort;
                else if (t == typeof(int)) return (Converter<int, short>)IntToShort;
                else if (t == typeof(short)) return (Converter<short, short>)ShortToShort;
                else if (t == typeof(byte)) return (Converter<byte, short>)ByteToShort;
                else if (t == typeof(Sensored)) return (Converter<Sensored, short>)SensoredToShort;
                else throw new NotSupportedException();
            }
            else if (u == typeof(byte))
            {
                if (t == typeof(double)) return (Converter<double, byte>)DoubleToByte;
                else if (t == typeof(float)) return (Converter<float, byte>)FloatToByte;
                else if (t == typeof(int)) return (Converter<int, byte>)IntToByte;
                else if (t == typeof(short)) return (Converter<short, byte>)ShortToByte;
                else if (t == typeof(byte)) return (Converter<byte, byte>)ByteToByte;
                else if (t == typeof(Sensored)) return (Converter<Sensored, byte>)SensoredToByte;
                else throw new NotSupportedException();
            }
            else if (u == typeof(Sensored))
            {
                if (t == typeof(Sensored)) return (Converter<Sensored, Sensored>)SensoredToSensored;
                else throw new NotSupportedException();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Converter<T, U> GetConverter<T, U>()
        {
            return (Converter<T, U>)GetConverter(typeof(T), typeof(U));
        }
    }

    public class DataArray<T> : DataArray, IEnumerable<T> where T : struct
    {
        private T[] _main;

        public DataArray(TupleMetaData newInfo, DataArray<T> data)
            : base(newInfo, data.DimX, data.DimY, data.DimZ, typeof(T))
        {
            _main = data._main;
        }

        DataArray(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(T))
        {
            if ((DimZ == 0) && (DimY == 0))
            {
                _main = new T[DimX];
            }
            else if (DimZ == 0)
            {
                _main = new T[DimX * DimY];
            }
            else
            {
                _main = new T[DimX * DimY * DimZ];
            }
        }

        DataArray(TupleMetaData info, int count)
            : this(info, count, 0, 0)
        {
        }

        DataArray(TupleMetaData info, int dimx, int dimy)
            : this(info, dimx, dimy, 0)
        {
        }

        public DataArray(TupleMetaData info, Generator1<T> gen, int dimx, int dimy, int dimz) :
            this (info, dimx, dimy, dimz)
        {
            for (int i = 0; i < Length; i++)
                _main[i] = gen(i);
        }

        public DataArray(TupleMetaData info, Generator<T> gen, int dimx, int dimy, int dimz) :
            this(info, dimx, dimy, dimz)
        {
            switch (_rank)
            {
                case 1:
                    for (int i = 0; i < Length; i++)
                        _main[i] = gen(i, 0, 0);
                    break;
                case 2:
                    for (int x = 0; x < _x; x++)
                        for (int y = 0; y < _y; y++)
                            _main[ToLinear(x, y)] = gen(x, y, 0);
                    break;
                case 3:
                    for (int x = 0; x < _x; x++)
                        for (int y = 0; y < _y; y++)
                            for (int z = 0; z < _z; z++)
                                _main[ToLinear(x, y, z)] = gen(x, y, z);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public static T[] CreateArray(Generator1<T> gen, int dimx)
        {
            T[] res = new T[dimx];
            for (int i = 0; i < dimx; i++)
                res[i] = gen(i);
            return res;
        }

        public static T[] CreateArray(Generator<T> gen, int dimx)
        {
            T[] res = new T[dimx];
            for (int i = 0; i < dimx; i++)
                res[i] = gen(i, 0, 0);
            return res;
        }

        public static T[,] CreateArray(Generator1<T> gen, int dimx, int dimy)
        {
            T[,] res = new T[dimx, dimy];
            for (int x = 0; x < dimx; x++)
                for (int y = 0; y < dimy; y++)
                    res[x, y] = gen(x * dimy + y);
            return res;
        }

        public static T[,] CreateArray(Generator<T> gen, int dimx, int dimy)
        {
            T[,] res = new T[dimx, dimy];
            for (int x = 0; x < dimx; x++)
                for (int y = 0; y < dimy; y++)
                    res[x, y] = gen(x, y, 0);
            return res;
        }

        public static T[, ,] CreateArray(Generator1<T> gen, int dimx, int dimy, int dimz)
        {
            T[, ,] res = new T[dimx, dimy, dimz];
            for (int x = 0; x < dimx; x++)
                for (int y = 0; y < dimy; y++)
                    for (int z = 0; z < dimz; z++)
                        res[x, y, z] = gen((x * dimy + y) * dimz + z);
            return res;
        }

        public static T[, ,] CreateArray(Generator<T> gen, int dimx, int dimy, int dimz)
        {
            T[, ,] res = new T[dimx, dimy, dimz];
            for (int x = 0; x < dimx; x++)
                for (int y = 0; y < dimy; y++)
                    for (int z = 0; z < dimz; z++)
                        res[x, y, z] = gen(x, y, z);
            return res;
        }

        public DataArray(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : this(info, dimx, dimy, dimz)
        {
            source.Get<T>(_main);
        }

        public DataArray(TupleMetaData info, T[] orig)
            : this(info, orig.Length)
        {
            if (_rank == 1)
                orig.CopyTo(_main, 0);
            else if ((_rank == 0) && (orig.Length == 0))
                return;
            else
                throw new ArgumentException();
        }

        public DataArray(TupleMetaData info, T[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1))
        {
            if (_rank == 2)
                for (int x = 0; x < _x; x++)
                    for (int y = 0; y < _y; y++)
                        _main[ToLinear(x, y)] = orig[x, y];
            else
                throw new ArgumentException();
        }

        public DataArray(TupleMetaData info, T[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        {
            if (_rank == 3)
                for (int x = 0; x < _x; x++)
                    for (int y = 0; y < _y; y++)
                        for (int z = 0; z < _z; z++)
                            _main[ToLinear(x, y, z)] = orig[x, y, z];
            else
                throw new ArgumentException();
        }

        public override int Length
        {
            get
            {
                return _main.Length;
            }
        }

        protected override object GetObject(int index)
        {
            return this[index];
        }

        protected override object GetObject(int x, int y)
        {
            return this[x, y];
        }

        protected override object GetObject(int x, int y, int z)
        {
            return this[x, y, z];
        }

        public T First()
        {
            return GetLinear(0);
        }

        new public T this[int index]
        {
            get
            {
                if (_rank != 1)
                    throw new RankException();                
                return _main[index];
            }
        }

        new public T this[int x, int y]
        {
            get
            {
                if (_rank != 2)
                    throw new RankException();
                else if (y >= _y)
                    throw new ArgumentException();

                return _main[ToLinear(x, y)];
            }
        }

        new public T this[int x, int y, int z]
        {
            get
            {
                if (_rank != 3)
                    throw new RankException();
                else if ((y >= _y) || (z >= _z))
                    throw new ArgumentException();

                return _main[ToLinear(x, y, z)];
            }
        }

        public void ToArray(out T[] r)
        {
            if (_rank != 1)
                throw new ArgumentException();

            r = new T[DimX];
            _main.CopyTo(r, 0);
        }

        public void ToArray(out T[,] r)
        {
            if (_rank != 2)
                throw new ArgumentException();

            r = new T[DimX, DimY];
            for (int x = 0; x < _x; x++)
                for (int y = 0; y < _y; y++)
                        r[x, y] = this[x, y];
        }

        public void ToArray(out T[, ,] r)
        {
            if (_rank != 3)
                throw new ArgumentException();

            r = new T[DimX, DimY, DimZ];
            for (int x = 0; x < _x; x++)
                for (int y = 0; y < _y; y++)
                    for (int z = 0; z < _z; z++)
                        r[x, y, z] = this[x, y, z];
        }

        protected override void SerializeArray(ISerializeStream stream)
        {
            stream.Put<T>(_main);
        }

        public override Type ElementType
        {
            get { return typeof(T); }
        }

        public override DataArray Reinfo(TupleMetaData info)
        {
            return new DataArray<T>(info, this);
        }


        #region Convertion

        public DataArray<U> ConvertTo<U>(TupleMetaData newInfo) where U : struct
        {
            return ConvertTo<U>(newInfo, GetConverter<U>());
        }

        public void ConvertToArray<U>(out U[] a) where U : struct
        {
            ConvertToArray<U>(GetConverter<U>(), out a);
        }
        public void ConvertToArray<U>(out U[,] a) where U : struct
        {
            ConvertToArray<U>(GetConverter<U>(), out a);
        }
        public void ConvertToArray<U>(out U[,,] a) where U : struct
        {
            ConvertToArray<U>(GetConverter<U>(), out a);
        }

        public DataArray<U> ConvertTo<U>(TupleMetaData newInfo, Converter<T, U> converter) where U : struct
        {
            DataArray<U> ret = new DataArray<U>(newInfo, DimX, DimY, DimZ);

            for (int k = 0; k < Length; k++)
                ret._main[k] = converter(_main[k]);

            return ret;
        }

        public void ConvertToArray<U>(Converter<T, U> converter, out U[] r) where U : struct
        {
            if (_rank != 1)
                throw new ArgumentException();

            r = new U[DimX];
            for (int k = 0; k < Length; k++)
                r[k] = converter(_main[k]);
        }

        public void ConvertToArray<U>(Converter<T, U> converter, out U[,] r) where U : struct
        {
            if (_rank != 2)
                throw new ArgumentException();

            r = new U[DimX, DimY];
            for (int x = 0; x < _x; x++)
                for (int y = 0; y < _y; y++)
                    r[x, y] = converter(_main[ToLinear(x, y)]);
        }

        public void ConvertToArray<U>(Converter<T, U> converter, out U[,,] r) where U : struct
        {
            if (_rank != 3)
                throw new ArgumentException();

            r = new U[DimX, DimY, DimZ];
            for (int x = 0; x < _x; x++)
                for (int y = 0; y < _y; y++)
                    for (int z = 0; z < _z; z++)
                        r[x, y, z] = converter(_main[ToLinear(x, y, z)]);
        }

        static public void Operation<T1>(out T[] ret, Converter<T1, T> c1, DataArray<T1> a1, Operator<T> op, T val)
    where T1 : struct
        {
            if (a1._rank != 1)
                throw new ArgumentException();

            ret = new T[a1.DimX];
            for (int k = 0; k < a1.Length; k++)
                ret[k] = op(c1(a1._main[k]), val);
        }

        static public void Operation<T1>(out T[,] ret, Converter<T1, T> c1, DataArray<T1> a1, Operator<T> op, T val)
    where T1 : struct
        {
            if (a1._rank != 2)
                throw new ArgumentException();

            ret = new T[a1._x, a1._y];
            for (int x = 0; x < a1._x; x++)
                for (int y = 0; y < a1._y; y++)
                    ret[x, y] = op(c1(a1._main[a1.ToLinear(x, y)]),val);
        }

        static public void Operation<T1>(out T[, ,] ret, Converter<T1, T> c1, DataArray<T1> a1, Operator<T> op, T val)
where T1 : struct
        {
            if (a1._rank != 3)
                throw new ArgumentException();

            ret = new T[a1._x, a1._y, a1._z];
            for (int x = 0; x < a1._x; x++)
                for (int y = 0; y < a1._y; y++)
                    for (int z = 0; z < a1._z; z++)
                        ret[x, y, z] = op(c1(a1._main[a1.ToLinear(x, y, z)]), val);
        }

        public T GetLinear(int index)
        {
            return _main[index];
        }

        #endregion

        public struct Enumerator : IEnumerator<T>
        {
            int _index;
            DataArray<T> _orig;   

            public Enumerator(DataArray<T> orig)
            {
                _index = -1;
                _orig = orig;
            }

            public T Current
            {
                get 
                {
                    return _orig.GetLinear(_index);
                }
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
                _index++;
                if (_index < _orig.Length)
                    return true;

                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        new public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override IEnumerator GetEnumeratorAbstract()
        {
            return GetEnumerator();
        }

        public override DataArrayDouble Normalize()
        {
            return ConvertTo<double>(new TupleMetaData("nor_" + Name, HumaneName, Date, Stream), sToDoubleNorm);
        }

        #region Operations

        static public DataArray<T> Operation<T1, T2>(TupleMetaData info, Converter<T1, T> c1, DataArray<T1> a1, Operator<T> op, Converter<T2, T> c2, DataArray<T2> a2)
            where T1 : struct
            where T2 : struct
        {
            if (a1.Rank != a2.Rank)
                throw new RankException();
            if ((a1.DimX != a2.DimX) || (a1.DimY != a2.DimY) || (a1.DimZ != a2.DimZ))
                throw new ArgumentException();

            DataArray<T> ret = new DataArray<T>(info, a1.DimX, a1.DimY, a1.DimZ);

            for (int k = 0; k < ret.Length; k++)
                ret._main[k] = op(c1(a1[k]), c2(a2[k]));

            return ret;
        }

        static public DataArray<T> Operation<T1>(TupleMetaData info, Converter<T1, T> c1, DataArray<T1> a1, Operator<T> op, T val)
            where T1 : struct
        {
            DataArray<T> ret = new DataArray<T>(info, a1.DimX, a1.DimY, a1.DimZ);

            for (int k = 0; k < ret.Length; k++)
                ret._main[k] = op(c1(a1[k]), val);

            return ret;
        }

        static public DataArray<T> Operation<T1, T2>(TupleMetaData info, DataArray<T1> a1, Operator<T> op, DataArray<T2> a2)
            where T1 : struct
            where T2 : struct
        {
            return Operation<T1, T2>(info, DataArray<T1>.GetConverter<T>(), a1, op, DataArray<T2>.GetConverter<T>(), a2);
        }

        static public DataArray<T> Operation<T1>(TupleMetaData info, DataArray<T1> a1, Operator<T> op, T val)
            where T1 : struct
        {
            return Operation<T1>(info, DataArray<T1>.GetConverter<T>(), a1, op, val);
        }

        #endregion

        public U ForEach<U, V>(Converter<T, V> conv, Action<U, V> action, U start)
            where U : struct
            where V : struct
        {
            foreach (T val in this)
            {
                start = action(start, conv(val));
            }
            return start;
        }

        public U ForEach<U, V>(Action<U, V> action, U start)
            where U : struct
            where V : struct
        {
            return ForEach<U, V>(GetConverter<V>(), action, start);
        }
        

        static readonly Converter<T, double> sToDoubleNorm = new Converter<T, double>(ToDoubleNormalize);
        static Converter<T, double> sToDoubleOrig;

        static double ToDoubleNormalize(T value)
        {
            if (sToDoubleOrig == null)
                sToDoubleOrig = GetConverter<double>();

            double val = sToDoubleOrig(value);
            if (Double.IsInfinity(val) || Double.IsNaN(val))
                val = 0;

            return val;
        }
        
        static Converter<T, U> GetConverter<U>()
        {
            return DataConverter.GetConverter<T, U>();
        }
        
        #region Instatatings

        public override Array ConvertToArray(Type t)
        {
            switch (Rank)
            {
                case 1:
                    if (t == typeof(double)) { double[] a; ConvertToArray<double>(out a); return a; }
                    else if (t == typeof(float)) { float[] a; ConvertToArray<float>(out a); return a; }
                    else if (t == typeof(int)) { int[] a; ConvertToArray<int>(out a); return a; }
                    else if (t == typeof(short)) { short[] a; ConvertToArray<short>(out a); return a; }
                    else if (t == typeof(byte)) { byte[] a; ConvertToArray<byte>(out a); return a; }
                    else throw new NotSupportedException();
                case 2:
                    if (t == typeof(double)) { double[,] a; ConvertToArray<double>(out a); return a; }
                    else if (t == typeof(float)) { float[,] a; ConvertToArray<float>(out a); return a; }
                    else if (t == typeof(int)) { int[,] a; ConvertToArray<int>(out a); return a; }
                    else if (t == typeof(short)) { short[,] a; ConvertToArray<short>(out a); return a; }
                    else if (t == typeof(byte)) { byte[,] a; ConvertToArray<byte>(out a); return a; }
                    else throw new NotSupportedException();
                case 3:
                    if (t == typeof(double)) { double[,,] a; ConvertToArray<double>(out a); return a; }
                    else if (t == typeof(float)) { float[,,] a; ConvertToArray<float>(out a); return a; }
                    else if (t == typeof(int)) { int[,,] a; ConvertToArray<int>(out a); return a; }
                    else if (t == typeof(short)) { short[,,] a; ConvertToArray<short>(out a); return a; }
                    else if (t == typeof(byte)) { byte[,,] a; ConvertToArray<byte>(out a); return a; }
                    else throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
        }

        public override DataArray ConvertTo(TupleMetaData newInfo, Type t)
        {
            if (t == typeof(double)) return ConvertTo<double>(newInfo);
            else if (t == typeof(float)) return ConvertTo<float>(newInfo);
            else if (t == typeof(int)) return ConvertTo<int>(newInfo);
            else if (t == typeof(short)) return ConvertTo<short>(newInfo);
            else if (t == typeof(byte)) return ConvertTo<byte>(newInfo);
            else throw new NotSupportedException();
        }

        static public DataArray<T> Operation<T1>(TupleMetaData info, DataArray<T1> a1, Operator<T> op, DataArray a2)
            where T1 : struct
        {
            if (a2.IsByte) return Operation<T1, byte>(info, a1, op, (DataArray<byte>)a2);
            else if (a2.IsShort) return Operation<T1, short>(info, a1, op, (DataArray<short>)a2);
            else if (a2.IsInt) return Operation<T1, int>(info, a1, op, (DataArray<int>)a2);
            else if (a2.IsFloat) return Operation<T1, float>(info, a1, op, (DataArray<float>)a2);
            else if (a2.IsDouble) return Operation<T1, double>(info, a1, op, (DataArray<double>)a2);
            else throw new NotSupportedException();
        }

        static public DataArray<T> Operation(TupleMetaData info, DataArray a1, Operator<T> op, DataArray a2)
        {
            if (a1.IsByte) return Operation<byte>(info, (DataArray<byte>)a1, op, a2);
            else if (a1.IsShort) return Operation<short>(info, (DataArray<short>)a1, op, a2);
            else if (a1.IsInt) return Operation<int>(info, (DataArray<int>)a1, op, a2);
            else if (a1.IsFloat) return Operation<float>(info, (DataArray<float>)a1, op, a2);
            else if (a1.IsDouble) return Operation<double>(info, (DataArray<double>)a1, op, a2);
            else throw new NotSupportedException();
        }

        static public DataArray<T> Operation(TupleMetaData info, DataArray a1, Operator<T> op, T val)
        {
            if (a1.IsByte) return Operation<byte>(info, (DataArray<byte>)a1, op, val);
            else if (a1.IsShort) return Operation<short>(info, (DataArray<short>)a1, op, val);
            else if (a1.IsInt) return Operation<int>(info, (DataArray<int>)a1, op, val);
            else if (a1.IsFloat) return Operation<float>(info, (DataArray<float>)a1, op, val);
            else if (a1.IsDouble) return Operation<double>(info, (DataArray<double>)a1, op, val);
            else throw new NotSupportedException();
        }

        static public U ForEach<U>(DataArray a, Action<U, T> action, U start) where U:struct
        {
            if (a.IsByte) return ((DataArray<byte>)a).ForEach<U,T>(action, start);
            else if (a.IsShort) return ((DataArray<short>)a).ForEach<U, T>(action, start);
            else if (a.IsInt) return ((DataArray<int>)a).ForEach<U, T>(action, start);
            else if (a.IsFloat) return ((DataArray<float>)a).ForEach<U, T>(action, start);
            else if (a.IsDouble) return ((DataArray<double>)a).ForEach<U, T>(action, start);
            else throw new NotSupportedException();            
        }  
        #endregion

        static public T ForEach(DataArray a, Action<T, T> action, T start)
        {
            return ForEach<T>(a, action, start);
        }
    }

#else

    public abstract class DataArrayAbstract : DataArray, IEnumerable
    {
        protected Array _array;

        protected DataArrayAbstract(TupleMetaData info, int dimx, int dimy, int dimz, Type type)
            : base(info, dimx, dimy, dimz, type)
        {
            if ((dimz == 0) && (dimy == 0))
            {
                _array = Array.CreateInstance(type, dimx);
            }
            else if (dimz == 0)
            {
                _array = Array.CreateInstance(type, dimx, dimy);
            }
            else
            {
                _array = Array.CreateInstance(type, dimx, dimy, dimz);
            }
        }

        protected DataArrayAbstract(TupleMetaData info, Array a)
            : base(info, a.GetLength(0), a.Rank > 1 ? a.GetLength(1) : 0, a.Rank > 2 ? a.GetLength(2) : 0, a.GetType().GetElementType())
        {
            _array = a;
        }


        public override Array ConvertToArray(Type t)
        {
            Array ret;
            switch (_rank)
            {
                case 1:
                    ret = Array.CreateInstance(t, _x);
                    for (int k = 0; k < Length; k++)
                        ret.SetValue(AnyValue.FromBoxedValue(_array.GetValue(k)).ToType(t, null), k);
                    break;
                case 2:
                    ret = Array.CreateInstance(t, _x, _y);
                    for (int x = 0; x < _x; x++)
                        for (int y = 0; y < _y; y++)
                            ret.SetValue(AnyValue.FromBoxedValue(_array.GetValue(x, y)).ToType(t, null), x, y);
                    break;
                case 3:
                    ret = Array.CreateInstance(t, _x, _y, _z);
                    for (int x = 0; x < _x; x++)
                        for (int y = 0; y < _y; y++)
                            for (int z = 0; z < _z; z++)
                                ret.SetValue(AnyValue.FromBoxedValue(_array.GetValue(x, y, z)).ToType(t, null), x, y, z);
                    break;
                default:
                    throw new RankException();
            }
            return ret;
        }

        static new public DataArrayAbstract Create(TupleMetaData info, Array a)
        {
            return (DataArrayAbstract)DataArray.Create(info, a);
        }

        public object GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array.GetValue(index);
                case 2: return _array.GetValue(index / _y, index % _y);
                case 3: return _array.GetValue(index / (_y * _z), ((index / _z) % (_y)), index % _z);
                default: throw new NotSupportedException();
            }
        }

        public override Type ElementType
        {
            get { return _array.GetType().GetElementType(); }
        }

        public override int Length
        {
            get { return _array.Length; }
        }

        protected override object GetObject(int index)
        {
            return _array.GetValue(index);
        }

        protected override object GetObject(int x, int y)
        {
            return _array.GetValue(x, y);
        }

        protected override object GetObject(int x, int y, int z)
        {
            return _array.GetValue(x, y, z);
        }

        protected override void SerializeArray(ISerializeStream stream)
        {
            stream.Put(_array);
        }


        #region Conversion

        public override DataArray ConvertTo(TupleMetaData newInfo, Type t)
        {
            if (t == typeof(int))
                return ArrayToInt(new DataArrayInt(newInfo, DimX, DimY, DimZ));
            else if (t == typeof(short))
                return ArrayToShort(new DataArrayShort(newInfo, DimX, DimY, DimZ));
            else if (t == typeof(byte))
                return ArrayToByte(new DataArrayByte(newInfo, DimX, DimY, DimZ));
            else if (t == typeof(float))
                return ArrayToFloat(new DataArrayFloat(newInfo, DimX, DimY, DimZ));
            else if (t == typeof(double))
                return ArrayToDouble(new DataArrayDouble(newInfo, DimX, DimY, DimZ));
            else
                throw new NotSupportedException();
        }

        DataArrayDouble ArrayToDouble(DataArrayDouble b)
        {
            if (Rank == 1)
            {
                double[] c = (double[])b._array;
                for (int k = 0; k < DimX; k++)
                    c[k] = GetAnyValue(k).ToDouble();
            }
            else if (Rank == 2)
            {
                double[,] c = (double[,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        c[k, j] = GetAnyValue(k, j).ToDouble();
            }
            else if (Rank == 3)
            {
                double[, ,] c = (double[, ,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            c[k, j, i] = GetAnyValue(k, j, i).ToDouble();
            }
            else
                throw new NotSupportedException();

            return b;
        }

        DataArrayFloat ArrayToFloat(DataArrayFloat b)
        {
            if (Rank == 1)
            {
                float[] c = (float[])b._array;
                for (int k = 0; k < DimX; k++)
                    c[k] = GetAnyValue(k).ToSingle();
            }
            else if (Rank == 2)
            {
                float[,] c = (float[,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        c[k, j] = GetAnyValue(k, j).ToSingle();
            }
            else if (Rank == 3)
            {
                float[, ,] c = (float[, ,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            c[k, j, i] = GetAnyValue(k, j, i).ToSingle();
            }
            else
                throw new NotSupportedException();


            return b;
        }

        DataArrayInt ArrayToInt(DataArrayInt b)
        {
            if (Rank == 1)
            {
                int[] c = (int[])b._array;
                for (int k = 0; k < DimX; k++)
                    c[k] = GetAnyValue(k).ToInt32();
            }
            else if (Rank == 2)
            {
                int[,] c = (int[,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        c[k, j] = GetAnyValue(k, j).ToInt32();
            }
            else if (Rank == 3)
            {
                int[, ,] c = (int[, ,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            c[k, j, i] = GetAnyValue(k, j, i).ToInt32();
            }
            else
                throw new NotSupportedException();
            return b;
        }

        DataArrayShort ArrayToShort(DataArrayShort b)
        {
            if (Rank == 1)
            {
                short[] c = (short[])b._array;
                for (int k = 0; k < DimX; k++)
                    c[k] = GetAnyValue(k).ToInt16();
            }
            else if (Rank == 2)
            {
                short[,] c = (short[,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        c[k, j] = GetAnyValue(k, j).ToInt16();
            }
            else if (Rank == 3)
            {
                short[, ,] c = (short[, ,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            c[k, j, i] = GetAnyValue(k, j, i).ToInt16();
            }
            else
                throw new NotSupportedException();

            return b;
        }

        DataArrayByte ArrayToByte(DataArrayByte b)
        {
            if (Rank == 1)
            {
                byte[] c = (byte[])b._array;
                for (int k = 0; k < DimX; k++)
                    c[k] = GetAnyValue(k).ToByte();
            }
            else if (Rank == 2)
            {
                byte[,] c = (byte[,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        c[k, j] = GetAnyValue(k, j).ToByte();
            }
            else if (Rank == 3)
            {
                byte[, ,] c = (byte[, ,])b._array;
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            c[k, j, i] = GetAnyValue(k, j, i).ToByte();
            }
            else
                throw new NotSupportedException();

            return b;
        }

        #endregion


        new public IEnumerator GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        protected override IEnumerator GetEnumeratorAbstract()
        {
            return _array.GetEnumerator();
        }

        double NormalizeGenerator(int idx)
        {
            double val = GetAnyValueLinear(idx).ToDouble();
            if (Double.IsInfinity(val) || Double.IsNaN(val))
                val = 0;
            return val;
        }

        public override DataArrayDouble Normalize()
        {
            return new DataArrayDouble(new TupleMetaData("nor_" + Name, HumaneName, Date, TupleMetaData.StreamAuto), new GeneratorDouble(NormalizeGenerator), DimX, DimY, DimZ);
        }
    }

#endif

}
