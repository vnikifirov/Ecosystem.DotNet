using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Runtime.InteropServices;
using System.Text;

namespace corelib
{
    public class DataArray : AbstactTupleItem, IDataArray, IDataArrayWriter, IEnumerable
    {
#if !DOTNET_V11

        public struct Accessor<T> : IDataArrayAccessor<T>, IDataArrayAccessorReadOnly<T>
        {
            private T[] _array1;
            private T[,] _array2;
            private T[, ,] _array3;
            private DataArray _orig;

            public Accessor(DataArray ar)
            {
                _array1 = null;
                _array2 = null;
                _array3 = null;

                _orig = ar;

                if (ar.Rank == 1)
                    _array1 = ar.GetArray<T>();
                else if (ar.Rank == 2)
                    _array2 = ar.GetArray2<T>();
                else if (ar.Rank == 3)
                    _array3 = ar.GetArray3<T>();
                else
                    throw new ArgumentException();
            }

            public int Rank { get { return _orig.Rank; } }
            public int Length { get { return _orig.Length; } }
            public int DimX { get { return _orig.DimX; } }
            public int DimY { get { return _orig.DimY; } }
            public int DimZ { get { return _orig.DimZ; } }

            public T this[int index]
            {
                get { return _array1[index]; }
                set { _array1[index] = value; }
            }

            public T this[int x, int y]
            {
                get { return _array2[x, y]; }
                set { _array2[x, y] = value; }
            }

            public T this[int x, int y, int z]
            {
                get { return _array3[x, y, z]; }
                set { _array3[x, y, z] = value; }
            }

            public IEnumerator<T> GetEnumerator()
            {
                switch (Rank)
                {
                    case 1:
                        for (int x = 0; x < _array1.GetLength(0); x++)
                            yield return this[x];
                        break;
                    case 2:
                        for (int x = 0; x < _array2.GetLength(0); x++)
                            for (int y = 0; y < _array2.GetLength(1); y++)
                                yield return this[x, y];
                        break;
                    case 3:
                        for (int x = 0; x < _array3.GetLength(0); x++)
                            for (int y = 0; y < _array3.GetLength(1); y++)
                                for (int z = 0; z < _array3.GetLength(2); z++)
                                    yield return this[x, y, z];
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public AccessorReadOnly<T> GetAccessorReadOnly()
            {
                return new AccessorReadOnly<T>(_orig);
            }

            #region IDataArrayAccessor<T> Members

            IDataArrayAccessorReadOnly<T> IDataArrayAccessor<T>.GetAccessorReadOnly()
            {
                return GetAccessorReadOnly();
            }

            #endregion
        }


        public struct AccessorReadOnly<T> : IDataArrayAccessorReadOnly<T>
        {
            private T[] _array1;
            private T[,] _array2;
            private T[,,] _array3;
            private DataArray _orig;

            public AccessorReadOnly(DataArray ar)
            {
                _array1 = null;
                _array2 = null;
                _array3 = null;

                _orig = ar;

                if (ar.Rank == 1)
                    _array1 = ar.GetArray<T>();
                else if (ar.Rank == 2)
                    _array2 = ar.GetArray2<T>();
                else if (ar.Rank == 3)
                    _array3 = ar.GetArray3<T>();
                else
                    throw new ArgumentException();
            }

            public int Rank { get { return _orig.Rank; } }
            public int Length { get { return _orig.Length; } }
            public int DimX { get { return _orig.DimX; } }
            public int DimY { get { return _orig.DimY; } }
            public int DimZ { get { return _orig.DimZ; } }

            public T this[int index]
            {
                get { return _array1[index]; }
            }

            public T this[int x, int y]
            {
                get { return _array2[x,y]; }
            }

            public T this[int x, int y, int z]
            {
                get { return _array3[x,y,z]; }
            }

            public IEnumerator<T> GetEnumerator()
            {
                switch (Rank)
                {
                    case 1:
                        for (int x = 0; x < _array1.GetLength(0); x++)
                            yield return this[x];
                        break;
                    case 2:
                        for (int x = 0; x < _array2.GetLength(0); x++)
                            for (int y = 0; y < _array2.GetLength(1); y++)
                                yield return this[x, y];
                        break;
                    case 3:
                        for (int x = 0; x < _array3.GetLength(0); x++)
                            for (int y = 0; y < _array3.GetLength(1); y++)
                                for (int z = 0; z < _array3.GetLength(2); z++)
                                    yield return this[x, y, z];
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

#endif

        public static readonly int StreamSerializerId = 30;

        static public bool IsMyStreamSerializerId(int id)
        {
            return (id == StreamSerializerId);
        }


        #region IChronoSerializer Members

        #region ITupleItem Members


        ITupleItem ITupleItem.Clone(string name, string humanName, DateTime date)
        {
            return Clone(name, humanName, date);
        }

        #endregion

        public override void Serialize(ISerializeStream stream)
        {
            stream.Put(StreamSerializerId);
            stream.Put(_x);
            stream.Put(_y);
            stream.Put(_z);
            stream.Put(_elementType);
            stream.Put(_array);
        }

        public ISerializeStream Serialize()
        {
            ISerializeStream s = new StreamSerializer(Name, Date, HumaneName);
            Serialize(s);
            return s;
        }

        #endregion

        public object this[int index]
        {
            get { return _array.GetValue(index); }
            set { _array.SetValue(value, index); }
        }

        public object this[int x, int y]
        {
            get { return _array.GetValue(x, y); }
            set { _array.SetValue(value, x, y); }
        }

        public object this[int x, int y, int z]
        {
            get { return _array.GetValue(x, y, z); }
            set { _array.SetValue(value, x, y, z); }
        }

        #region Get section

        public double[] GetArrayDouble()
        {
            return (double[])_array;
        }
        public double[,] GetArrayDouble2()
        {
            return (double[,])_array;
        }
        public double[, ,] GetArrayDouble3()
        {
            return (double[, ,])_array;
        }
        public float[] GetArrayFloat()
        {
            return (float[])_array;
        }
        public float[,] GetArrayFloat2()
        {
            return (float[,])_array;
        }
        public float[, ,] GetArrayFloat3()
        {
            return (float[, ,])_array;
        }
        public int[] GetArrayInt()
        {
            return (int[])_array;
        }
        public int[,] GetArrayInt2()
        {
            return (int[,])_array;
        }
        public int[, ,] GetArrayInt3()
        {
            return (int[, ,])_array;
        }
        public short[] GetArrayShort()
        {
            return (short[])_array;
        }
        public short[,] GetArrayShort2()
        {
            return (short[,])_array;
        }
        public short[, ,] GetArrayShort3()
        {
            return (short[, ,])_array;
        }
        public byte[] GetArrayByte()
        {
            return (byte[])_array;
        }
        public byte[,] GetArrayByte2()
        {
            return (byte[,])_array;
        }
        public byte[, ,] GetArrayByte3()
        {
            return (byte[, ,])_array;
        }

        public static explicit operator int[](DataArray s)
        {
            return (int[])s._array;
        }
        public static explicit operator int[,](DataArray s)
        {
            return (int[,])s._array;
        }
        public static explicit operator int[, ,](DataArray s)
        {
            return (int[, ,])s._array;
        }
        public static explicit operator double[](DataArray s)
        {
            return (double[])s._array;
        }
        public static explicit operator double[,](DataArray s)
        {
            return (double[,])s._array;
        }
        public static explicit operator double[, ,](DataArray s)
        {
            return (double[, ,])s._array;
        }
        public static explicit operator float[](DataArray s)
        {
            return (float[])s._array;
        }
        public static explicit operator float[,](DataArray s)
        {
            return (float[,])s._array;
        }
        public static explicit operator float[, ,](DataArray s)
        {
            return (float[, ,])s._array;
        }
        public static explicit operator byte[](DataArray s)
        {
            return (byte[])s._array;
        }
        public static explicit operator byte[,](DataArray s)
        {
            return (byte[,])s._array;
        }
        public static explicit operator byte[, ,](DataArray s)
        {
            return (byte[, ,])s._array;
        }
        public static explicit operator short[](DataArray s)
        {
            return (short[])s._array;
        }
        public static explicit operator short[,](DataArray s)
        {
            return (short[,])s._array;
        }
        public static explicit operator short[, ,](DataArray s)
        {
            return (short[, ,])s._array;
        }

        // Special
        public DateTime[] GetArrayDateTime()
        {
            return (DateTime[])_array;
        }
        public static implicit operator DateTime[](DataArray s)
        {
            return (DateTime[])s._array;
        }

        public Coords[] GetArrayCoords()
        {
            return (Coords[])_array;
        }
        public static explicit operator Coords[](DataArray s)
        {
            return (Coords[])s._array;
        }
        public Coords[,] GetArrayCoords2()
        {
            return (Coords[,])_array;
        }
        public static explicit operator Coords[,](DataArray s)
        {
            return (Coords[,])s._array;
        }
        public FiberCoords[] GetArrayFiberCoords()
        {
            return (FiberCoords[])_array;
        }
        public FiberCoords[,] GetArrayFiberCoords2()
        {
            return (FiberCoords[,])_array;
        }
        public Sensored[] GetArraySensored()
        {
            return (Sensored[])_array;
        }
        public Sensored[,] GetArraySensored2()
        {
            return (Sensored[,])_array;
        }
        public Sensored[,,] GetArraySensored3()
        {
            return (Sensored[,,])_array;
        }

        public MultiIntFloat[] GetArrayMultiIntFloat()
        {
            return (MultiIntFloat[])_array;
        }
        #endregion

        public Type ElementType
        {
            get { return _array.GetType().GetElementType(); }
        }

        public int Rank
        {
            get { return _array.Rank; }
        }

        public int Length
        {
            get { return _array.Length; }
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
            get { return _elementType == Sensored.ArrayStreamCode; }
        }
        public bool IsCoords
        {
            get { return _elementType == Coords.ArrayStreamCode; }
        }
        public bool IsFiberCoords
        {
            get { return _elementType == FiberCoords.ArrayStreamCode; }
        }
        public bool IsMultiIntFloat
        {
            get { return _elementType == MultiIntFloat.ArrayStreamCode; }
        }

        public static DataArray operator *(DataArray ar, double val)
        {
            DataArray res = DataArray.CreateDouble("mod_" + ar.Name, ar.HumaneName, ar.GetTimeDate(), ar);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                    res[i] = Convert.ToDouble(ar[i]) * val;
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public static DataArray operator +(DataArray ar, double val)
        {
            DataArray res = DataArray.CreateDouble("mod_" + ar.Name, ar.HumaneName, ar.GetTimeDate(), ar);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                    res[i] = Convert.ToDouble(ar[i]) + val;
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public static DataArray operator -(DataArray ar, double val)
        {
            return ar + (-val);
        }

        public static DataArray operator +(DataArray ar, DataArray ar2)
        {
            DataArray res = DataArray.CreateDouble("add_" + ar.Name, ar.HumaneName, ar.GetTimeDate(), ar);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                    res[i] = Convert.ToDouble(ar[i]) + Convert.ToDouble(ar2[i]);
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public static DataArray operator -(DataArray ar, DataArray ar2)
        {
            DataArray res = DataArray.CreateDouble("sub_" + ar.Name, ar.HumaneName, ar.GetTimeDate(), ar);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                    res[i] = Convert.ToDouble(ar[i]) - Convert.ToDouble(ar2[i]);
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public static DataArray operator *(DataArray ar, DataArray ar2)
        {
            DataArray res = DataArray.CreateDouble("mul_" + ar.Name, ar.HumaneName, ar.GetTimeDate(), ar);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                    res[i] = Convert.ToDouble(ar[i]) * Convert.ToDouble(ar2[i]);
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public static DataArray operator /(DataArray ar, DataArray ar2)
        {
            DataArray res = DataArray.CreateDouble("div_" + ar.Name, ar.HumaneName, ar.GetTimeDate(), ar);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                    res[i] = Convert.ToDouble(ar[i]) / Convert.ToDouble(ar2[i]);
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public DataArray Normalize()
        {
            DataArray res = DataArray.CreateDouble("nor_" + Name, HumaneName, GetTimeDate(), this);

            if (res.Rank == 1)
            {
                for (int i = 0; i < res.DimX; i++)
                {
                    double val = Convert.ToDouble(this[i]);
                    if (Double.IsInfinity(val) || Double.IsNaN(val))
                        val = 0;

                    res[i] = val;
                }
            }
            else
                throw new NotImplementedException();

            return res;
        }

        public double Sum()
        {
            double ret = 0;
            
#if !DOTNET_V11
            if (IsDouble)
            {
                foreach (double o in GetAccessorReadOnly<double>())
                    ret += o;
            }
            else if (IsInt)
            {
                foreach (int o in GetAccessorReadOnly<int>())
                    ret += o;
            }            
#else
            if (IsDouble)
            {
                foreach (double o in this)
                    ret += o;
            }
            else if (IsInt)
            {
                foreach (int o in this)
                    ret += o;
            }            
#endif
            else
            {
                foreach (object o in this)
                {
                    ret += Convert.ToDouble(o);
                }
            }

            return ret;
        }

        public double Mean()
        {
            return Sum() / Length;
        }

        public static DataArray Create(string name, string humanName, DateTime date, DataArray orig)
        {
            Array ar = orig._array;
            Array nar;
            switch (ar.Rank)
            {
                case 1: nar = Array.CreateInstance(ar.GetType().GetElementType(), ar.GetLength(0)); break;
                case 2: nar = Array.CreateInstance(ar.GetType().GetElementType(), ar.GetLength(0), ar.GetLength(1)); break;
                case 3: nar = Array.CreateInstance(ar.GetType().GetElementType(), ar.GetLength(0), ar.GetLength(1), ar.GetLength(2)); break;
                default:
                    throw new ArgumentException();
            }

            DataArray n = new DataArray(name, humanName, date, nar);
            return n;
        }

        public static DataArray CreateDouble(string name, string humanName, DateTime date, DataArray orig)
        {
            Array ar = orig._array;
            Array nar;
            switch (ar.Rank)
            {
                case 1: nar = Array.CreateInstance(typeof(double), ar.GetLength(0)); break;
                case 2: nar = Array.CreateInstance(typeof(double), ar.GetLength(0), ar.GetLength(1)); break;
                case 3: nar = Array.CreateInstance(typeof(double), ar.GetLength(0), ar.GetLength(1), ar.GetLength(2)); break;
                default:
                    throw new ArgumentException();
            }

            DataArray n = new DataArray(name, humanName, date, nar);
            return n;
        }

        public DataArray Clone(string name, string humanName, DateTime date)
        {
            DataArray n = new DataArray(name, humanName, date, (Array)_array.Clone());
            return n;
        }

        public static DataArray Clone(string name, string humanName, DateTime date, Array array)
        {
            DataArray n = new DataArray(name, humanName, date, (Array)array.Clone());
            return n;
        }

        DataArray(string name, string humanName, DateTime date)
            : base(new TupleMetaData(name, humanName, date))
        {
/*            _name = name;
            _humanName = humanName;
            _date = date;*/
        }

        DataArray(string name, string humanName, DateTime date, int elementType)
            : this(name, humanName, date)
        {
            SetConstructorElementType(elementType);
        }

        static readonly IInfoFormatter defNum = new DataNumericConverter();
        static readonly IInfoFormatter defFloat = new DataNumericConverter();
        static readonly IInfoFormatter defStr = new DataNumericConverter();

        void SetConstructorElementType(int et)
        {
            _elementType = et;

            Type t = KnownType.GetTypeFromId(et);
            if ((t == typeof(byte)) || (t == typeof(int)) || (t == typeof(short)) || (t == typeof(DateTime)))
            {
                _formatter = defNum;
            }
            else if ((t == typeof(float)) || (t == typeof(double)))
            {
                _formatter = defFloat;
            }
            else
            {
                _formatter = defStr;
            }
        }

        public DataArray(string name, string humanName, DateTime date, Array ar)
            : this(name, humanName, date, KnownType.GetIdFromType(ar.GetType().GetElementType()))
        {
            switch (ar.Rank)
            {
                case 3:
                    _x = ar.GetLength(0);
                    _y = ar.GetLength(1);
                    _z = ar.GetLength(2);
                    break;
                case 2:
                    _x = ar.GetLength(0);
                    _y = ar.GetLength(1);
                    _z = 0;
                    break;
                case 1:
                    _x = ar.GetLength(0);
                    _y = 0;
                    _z = 0;
                    break;
                default:
                    throw new Exception("DataArray: incorrect array");
            }
            //_array = ar.Clone();
            _array = ar;
        }

        public DataArray(string name, string humanName, DateTime date, Type type, int c1, int c2, int c3)
            : this(name, humanName, date, KnownType.GetIdFromType(type))
        {
            if ((c2 == 0) && (c3 == 0))
                _array = Array.CreateInstance(type, c1);
            else if (c3 == 0)
                _array = Array.CreateInstance(type, c1, c2);
            else
                _array = Array.CreateInstance(type, c1, c2, c3);

            _x = c1;
            _y = c2;
            _z = c3;
        }

        public DataArray(IDeserializeStream stream)
            : this(stream.GetName(), stream.GetHumanName(), stream.GetTimeDate())
        {
            int testStreamSerializerId;
            stream.Get(out testStreamSerializerId);

            if (testStreamSerializerId != StreamSerializerId)
                throw new Exception("DataArray: incorrect stream");

            stream.Get(out _x);
            stream.Get(out _y);
            stream.Get(out _z);
            stream.Get(out _elementType);
            _array = stream.GetArray(KnownType.GetTypeFromId(_elementType), _x, _y, _z);

            SetConstructorElementType(_elementType);
        }

        public String DumpCSV()
        {
            StringBuilder sb = new StringBuilder(Length * 15);
            //Rank
            for (int x = 0; x < _x; x++)
            {
                sb.AppendFormat("{0};", x);
                if (_y == 0)
                {
                    sb.AppendFormat("{0}", this[x]);
                    sb.Append("\r\n");
                }
                else
                    for (int y = 0; y < _y; y++)
                    {
                        sb.AppendFormat("{0};", y);
                        if (_z == 0)
                        {
                            sb.AppendFormat("{0}", this[x, y]);
                            sb.Append("\r\n");
                        }
                        else
                            for (int z = 0; z < _z; z++)
                            {
                                sb.AppendFormat("{0};", z);
                                sb.AppendFormat("{0}", this[x, y, z]);
                                sb.Append("\r\n");
                            }
                    }
            }
            return sb.ToString();
        }

        private int _x;
        private int _y;
        private int _z;

        private int _elementType;
        private Array _array;

        void ArrayToInt(Array a, ref Array b)
        {
            if (a.Rank == 1)
            {
                int[] c = (int[])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    c[k] = Convert.ToInt32(a.GetValue(k));
            }
            else if (a.Rank == 2)
            {
                int[,] c = (int[,])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    for (int j = 0; j < a.GetLength(1); j++)
                        c[k, j] = Convert.ToInt32(a.GetValue(k, j));
            }
            else if (a.Rank == 3)
            {
                int[, ,] c = (int[, ,])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    for (int j = 0; j < a.GetLength(1); j++)
                        for (int i = 0; i < a.GetLength(2); i++)
                            c[k, j, i] = Convert.ToInt32(a.GetValue(k, j, i));
            }
            else
                throw new NotImplementedException();
        }

        void ArrayToDouble(Array a, ref Array b)
        {
            if (a.Rank == 1)
            {
                double[] c = (double[])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    c[k] = Convert.ToDouble(a.GetValue(k));
            }
            else if (a.Rank == 2)
            {
                double[,] c = (double[,])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    for (int j = 0; j < a.GetLength(1); j++)
                        c[k, j] = Convert.ToDouble(a.GetValue(k, j));
            }
            else if (a.Rank == 3)
            {
                double[, ,] c = (double[, ,])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    for (int j = 0; j < a.GetLength(1); j++)
                        for (int i = 0; i < a.GetLength(2); i++)
                            c[k, j, i] = Convert.ToDouble(a.GetValue(k, j, i));
            }
            else
                throw new NotImplementedException();
        }

        void ArrayToFloat(Array a, ref Array b)
        {
            if (a.Rank == 1)
            {
                float[] c = (float[])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    c[k] = Convert.ToSingle(a.GetValue(k));
            }
            else if (a.Rank == 2)
            {
                float[,] c = (float[,])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    for (int j = 0; j < a.GetLength(1); j++)
                        c[k, j] = Convert.ToSingle(a.GetValue(k, j));
            }
            else if (a.Rank == 3)
            {
                float[, ,] c = (float[, ,])b;
                for (int k = 0; k < a.GetLength(0); k++)
                    for (int j = 0; j < a.GetLength(1); j++)
                        for (int i = 0; i < a.GetLength(2); i++)
                            c[k, j, i] = Convert.ToSingle(a.GetValue(k, j, i));
            }
            else
                throw new NotImplementedException();
        }

        public DataArray ConvertTo(Type t, string name, string humanName, DateTime date)
        {
            DataArray n = new DataArray(name, humanName, date, t, _x, _y, _z);
            Array a = n._array;

            if (t == typeof(int))
            {                
                ArrayToInt(_array, ref a);
            }
            else if (t == typeof(double))
            {
                ArrayToDouble(_array, ref a);
            }
            else if (t == typeof(float))
            {
                ArrayToFloat(_array, ref a);
            }
            else
                throw new NotImplementedException();

            return n;
        }

        internal bool ParseDataType(string type, out Type nType)
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

        #region ITupleItem Members

        public ITupleItem CastTo(ITypeRules rules)
        {
            string tp = rules.GetTypeString().ToLower();
            if (tp != "array")
                throw new Exception("Don't know how to convert");

            string[] details = rules.GetCastDetails();
            if (details.Length > 2)
                throw new Exception("Don't know how to convert to type detail");

            if (details.Length == 2)
            {
                int rank = Convert.ToInt32(details[1]);
                if (Rank != rank)
                    throw new Exception("Can't cast to different rank");
            }
            if (details.Length >= 1)
            {
                string type = details[0].ToLower();
                Type t;
                if (!ParseDataType(type, out t))
                    throw new Exception("Can't parse type");

                if (KnownType.GetIdFromType(t) != _elementType)
                    return ConvertTo(t, rules.GetName(), HumaneName, Date);
                else
                    return Clone(rules.GetName(), HumaneName, Date);
            }
            //return Rename;
            return Clone(rules.GetName(), HumaneName, Date);
        }

        #endregion

        #region ITupleItem Members

        public object CastTo(corelibnew.IGetCoordsConverter en, ITypeRules rules)
        {
            return CastTo(rules);
        }

        public ITupleItem Rename(TupleMetaData newInfo)
        {
            return new DataArray(newInfo.Name, newInfo.HumaneName, newInfo.Date, _array);
        }

        public ITupleItem Rename(string newParamName)
        {
            DataArray n = new DataArray(newParamName, HumaneName, Date, _array);
            return n;
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        #endregion

        public override DataGrid DataTable
        {
            get
            {
                DataGrid d = new DataGrid();

                string[] names = { "x", "y", "z" };

                for (int i = 0; i < Rank; i++)
                {
                    d.Columns.Add(names[i], typeof(int));
                }
                d.HeadColumns = Rank;

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
        }

#if !DOTNET_V11

        public AccessorReadOnly<T> GetAccessorReadOnly<T>()
        {
            return new AccessorReadOnly<T>(this);            
        }

        public IDataArrayAccessor<T> GetAccessor<T>()
        {
            return new Accessor<T>(this);
        }

        #region IDataArray Members


        public T Get<T>(int x)
        {
            return ((T[])_array)[x];
        }

        public T Get<T>(int x, int y)
        {
            return ((T[,])_array)[x,y];
        }

        public T Get<T>(int x, int y, int z)
        {
            return ((T[, ,])_array)[x, y, z];
        }

        #endregion

        #region IDataArrayWriter Members


        public T[] GetArray<T>()
        {
            return ((T[])_array);
        }

        public T[,] GetArray2<T>()
        {
            return ((T[,])_array);
        }

        public T[, ,] GetArray3<T>()
        {
            return ((T[, ,])_array);
        }

        #endregion

#endif
    }
}
