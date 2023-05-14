using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Runtime.InteropServices;
using System.Text;

namespace corelib
{
    public enum ChannelType
    {
        // ВАЖНО! Чтобы нулевой элемент отражал несуществующую ячейку
        EMPTY = 0,

        TVS = 1,
        DP,
        WATER,
        SUZ,
        UNKNOWN = 255
    }

    public struct Sensored : IConvertible
//#if !DOTNET_V11
//        : IComparable<Sensored>
//#endif
    {
        public const byte ArrayStreamCode = KnownType.Sensored_ArrayStreamCode;

        public enum DataState
        {
            /// <summary>
            /// Без ошибок
            /// </summary>
            Ok,
            /// <summary>
            /// Неточность датчика
            /// </summary>
            SensorProhibition = 4,
            /// <summary>
            /// Датчик отсутствует
            /// </summary>
            SensorAbsence
        }

        public short RawValue
        {
            get { return _val; }
        }

		public int Value
		{
			get { return ((0x7fff&_val) >> 6); }
		}

        public bool IsOk
        {
            get { return ((_val & 0x0f) == 0); }
        }

        public DataState State
        {
            get { return (DataState)(_val & 0x0f); }
        }

        public Sensored(short val)
        {
            _val = val;
        }
        public Sensored(short trueval, DataState ds)
        {
            _val = (short)(((ushort)trueval << 6) | ((ushort)ds & 0x0f));
        }

        private short _val;

        public static int Compare(Sensored r1, Sensored r2)
        {
            return r2.Value - r1.Value;
        }

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return IsOk;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)Value;
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Value;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)Value;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Value;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Value;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)Value;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Value;
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)Value;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)Value;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong)Value;
        }

        #endregion
    }

    public struct ScaleIndex
    {
        double min;
        //double max;
        double scale;

        public static readonly ScaleIndex Default = new ScaleIndex(0, 1);

        public double Min
        {
            get { return min; }
        }

        public double ScaleVal
        {
            get { return scale; }
        }            

        public bool IsDefault
        {
            get { return Default == this; }
        }

        public static bool operator !=(ScaleIndex t, ScaleIndex p)
        {
            return (!(t == p));
        }
        public static bool operator ==(ScaleIndex t, ScaleIndex p)
        {
            return ((t.min == p.min) && (p.scale == t.scale));
        }

        public override bool Equals(object o)
        {
            if (typeof(ScaleIndex).IsAssignableFrom(o.GetType()))
            {
                ScaleIndex w = (ScaleIndex)o;
                return (this == w);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return min.GetHashCode() ^ scale.GetHashCode();
        }

        public ScaleIndex(double nmin, double nscale)
        {
            min = nmin;
            scale = nscale;
        }

        public double Scale(int val)
        {
            return (min + (double)val * scale);
        }
        public double Scale(byte val)
        {
            return (min + (double)val * scale);
        }
        public double Scale(short val)
        {
            return (min + (double)val * scale);
        }
        public double Scale(float val)
        {
            return (min + (double)val * scale);
        }
        public double Scale(double val)
        {
            return (min + (double)val * scale);
        }
        public double Scale(Sensored val)
        {
            return (min + (double)val.Value * scale);
        }

        public double Scale(object val)
        {
            if (val.GetType() == typeof(byte))
                return Scale((byte)val);
            else if (val.GetType() == typeof(short))
                return Scale((short)val);
            else if (val.GetType() == typeof(int))
                return Scale((int)val);
            else if (val.GetType() == typeof(float))
                return Scale((float)val);
            else if (val.GetType() == typeof(double))
                return Scale((double)val);
            else if (val.GetType() == typeof(Sensored))
                return Scale((Sensored)val);
            else if ((val as Array) != null)
                return Scale((Sensored)((Array)val).GetValue(0));
            else
                throw new Exception("Cannot cast object for scaling");
        }

        public void Rescale(double scaled, out byte raw)
        {
            raw = (byte)((scaled - min) / scale);
        }
        public void Rescale(double scaled, out short raw)
        {
            raw = (short)((scaled - min) / scale);
        }
        public void Rescale(double scaled, out int raw)
        {
            raw = (int)((scaled - min) / scale);
        }
        public void Rescale(double scaled, out float raw)
        {
            raw = (float)((scaled - min) / scale);
        }
        public void Rescale(double scaled, out double raw)
        {
            raw = (double)((scaled - min) / scale);
        }
        public void Rescale(double scaled, out Sensored raw)
        {
            raw = new Sensored((short)((scaled - min) / scale), Sensored.DataState.Ok);
        }
        public object Rescale(double scaled, Type otype)
        {

            if (otype == typeof(byte))
            {
                byte val;
                Rescale(scaled, out val);
                return val;
            }
            else if (otype == typeof(short))
            {
                short val;
                Rescale(scaled, out val);
                return val;
            }
            else if (otype == typeof(int))
            {
                int val;
                Rescale(scaled, out val);
                return val;
            }
            else if (otype == typeof(float))
            {
                float val;
                Rescale(scaled, out val);
                return val;
            }
            else if (otype == typeof(double))
            {
                double val;
                Rescale(scaled, out val);
                return val;
            }
            else if (otype == typeof(Sensored))
            {
                Sensored val;
                Rescale(scaled, out val);
                return val;
            }
            else
                throw new Exception("Cannot cast object for scaling");
        }
    }

    public struct FiberCoords
    {
        public const byte ArrayStreamCode = KnownType.FiberCoords_ArrayStreamCode;

        public static readonly FiberCoords incorrect = new FiberCoords(-1,-1);

        public const int FiberMax = 16;
        public const int PvkMax = 115;

        private byte _fib;
        private byte _pvk;

        /// <summary>
        /// Номер нитки, начиная с 0
        /// </summary>
        public int Fiber
        {
            get { return (int)_fib - 1; }
        }

        /// <summary>
        /// Номер ПВК, начиная с 0
        /// </summary>
        public int Pvk
        {
            get { return (int)_pvk; }
        }

        /// <summary>
        /// Допустимое ли значение нитки-ПВК
        /// </summary>
        public bool IsValid
        {
            get { return ((_fib >= 1) && (_fib < 17) && (_pvk < 116)); }
        }

        /// <summary>
        /// Создать обект используя значения номера нитки и ПВК с 0ого отсчета
        /// </summary>
        /// <param name="fibindex">Номер нитки, начиная с 0</param>
        /// <param name="pvk">Номер ПВК, начиная с 0</param>
        public FiberCoords(int fibindex, int pvk)
        {
            if ((fibindex < 0) || (pvk < 0) || (fibindex > 15) || (pvk > 116))
            {
                _fib = 0;
                _pvk = 0;
            }
            else
            {
                _fib = (byte)(fibindex + 1);
                _pvk = (byte)pvk;
            }
        }

        public static int Compare(FiberCoords fc1, FiberCoords fc2)
        {
            return (fc2.IsValid ? 1 : 0) * ((fc2.Fiber + 1) * 1000 + fc2.Pvk + 1) -
                (fc1.IsValid ? 1 : 0) * ((fc1.Fiber + 1) * 1000 + fc1.Pvk + 1);
        }
    }

    /// <summary>
    /// Класс по работе с координатами РБМК
    /// </summary>
    public struct Coords
    {
        public const byte ArrayStreamCode = KnownType.Coords_ArrayStreamCode;

        ////////////////////////////////////////////////////////////////////////
        // CONSTRUCTORS
        ////////////////////////////////////////////////////////////////////////

        public Coords(int my, int mx)
        {
            x = (byte)mx;
            y = (byte)my;
        }

        /// <summary>
        /// Создает объект из пары машинных координат (сначала задается Y затем X)
        /// </summary>
        public Coords(byte my, byte mx)
        {
            x = mx;
            y = my;
        }

        /// <summary>
        /// Преобразует строку с координатани Y-X в тип Coords
        /// </summary>
        /// <param name="hy_hx">Строка вида Y-X (например "35-42")</param>
        /// <returns>Координаты в формате Coords</returns>
        public static Coords FromHumane(string hy_hx)
        {
            string[] c = hy_hx.Split('-');
            if (c.Length != 2)
                throw new ArgumentException("Невозможно распознать координаты Y-X");
            return Coords.FromHumane(Convert.ToByte(c[0]), Convert.ToByte(c[1]));
        }

        /// <summary>
        /// Преобразует координаты из принятых к обозначению в машинные
        /// </summary>
        /// <param name="hx">Координата X в стандартном виде</param>
        /// <param name="hy">Координата Y в стандартном виде</param>
        /// <returns></returns>
        public static Coords FromHumane(int hy, int hx)
        {
            Coords m;
            m.x = SingleFromHumane(hx);
            m.y = SingleFromHumane(hy);
            return m;
        }

        ////////////////////////////////////////////////////////////////////////
        // PROPERTIES
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Координата X в общепринятом представлении системы Y-X
        /// </summary>
        public int HumaneX
        {
            get { return SingleToHumane(x); }
        }
        /// <summary>
        /// Координата Y в общепринятом представлении системы Y-X
        /// </summary>
        public int HumaneY
        {
            get { return SingleToHumane(y); }
        }

        /// <summary>
        /// Возвращает координаты в человеко-читаемом виде, по типу 17-27, в системе Y-X
        /// </summary>
        public string HumaneLabelYX
        {
            get { return String.Format("{0}-{1}", HumaneY, HumaneX); }
        }

        /// <summary>
        /// Координата X в машинном представлении
        /// </summary>
        public byte X
        {
            get { return x; }
        }

        /// <summary>
        /// Координата Y в машинном представлении
        /// </summary>
        public byte Y
        {
            get { return y; }
        }

        /// <summary>
        /// Лежит ли координата в стандартном квадрате 48*48
        /// </summary>
        public bool IsOk
        {
            get
            {
                if ((x < 48) && (y < 48))
                    return true;
                return false;
            }
        }

        public override string ToString()
        {
            if (IsOk)
                return HumaneLabelYX;
            return "";
        }

        public override bool Equals(object o)
        {
            if (typeof(Coords).IsAssignableFrom(o.GetType()))
            {
                Coords w = (Coords)o;
                return (this == w);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ((int)x << 8) | (int)y;
        }

        public static bool operator !=(Coords t, Coords p)
        {
            return (!(t == p));
        }

        public static bool operator ==(Coords t, Coords p)
        {
            if ((p.x == t.x) && (p.y == t.y) && (t.IsOk))
                return true;
            return false;
        }

        public static readonly Coords incorrect = new Coords(128,128);

        #region Private
        ////////////////////////////////////////////////////////////////////////
        // PRIVATE
        ////////////////////////////////////////////////////////////////////////

        private byte x;
        private byte y;

        private static byte SingleFromHumane(int hum)
        {
            int i = 0;
            int ret = 0;
            int val = hum - 10; //Starting index of RBMK coordinate is 10 (oct)

            while (val > 0)
            {
                ret |= (val % 10) << (3 * i++);
                val /= 10;
            }

            if (val < 0)
                ret = 128;

            return (byte)ret;
        }

        private static int SingleToHumane(byte machine)
        {
            int ret = 10; //Starting index of RBMK coordinate is 10 (oct)
            int val = machine;
            int mul = 1;

            while (val > 0)
            {
                ret += (val & 0x7) * mul;
                mul *= 10;
                val >>= 3;
            }

            return ret;
        }

        #endregion
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct MultiIntFloat
    {
        public const byte ArrayStreamCode = KnownType.MultiIntFloat_ArrayStreamCode;

        unsafe public MultiIntFloat(byte[] _array, int offset)
        {
            fixed (int* a = &_ival)
                Marshal.Copy(_array, offset, (IntPtr)(a), sizeof(int));
            fixed (float* a = &_fval)
                Marshal.Copy(_array, offset, (IntPtr)(a), sizeof(float));            
        }

        public MultiIntFloat(int val)
        {
            _fval = 0;
            _ival = val;
        }
        public MultiIntFloat(float val)
        {
            _ival = 0;
            _fval = val;
        }

        public float Float
        {
            get { return _fval; }
        }

        public int Int
        {
            get { return _ival; }
        }

        [FieldOffset(0)]
        int _ival;

        [FieldOffset(0)]
        float _fval;
    }


    public struct TupleMetaData
    {
        public string StreamName;
        public string Name;
        public string HumaneName;
        public DateTime Date;

        /// <summary>
        /// Указывает на принадлежность объекта к классу константных значений
        /// </summary>
        public const string StreamConst = "const";
        /// <summary>
        /// Класс автоматически созданных объектов, а также промежуточных объектов
        /// </summary>
        public const string StreamAuto = "auto";

        public TupleMetaData(TupleMetaData d)
        {
            Name = d.Name;
            HumaneName = d.HumaneName;
            Date = d.Date;
            StreamName = d.StreamName;
        }

        public TupleMetaData(string name, string humaneName, DateTime date, string streamName)
        {
            Name = name;
            HumaneName = humaneName;
            Date = date;
            StreamName = streamName;
        }
    }

    internal struct KnownType
    {
        public KnownType(Type t, int id)
        {
            _type = t;
            _id = id;
        }
        private Type _type;
        private int _id;

        public const int empty_ArrayStreamCode = 0;

        public const int byte_ArrayStreamCode = 1;
        public const int short_ArrayStreamCode = 2;
        public const int int_ArrayStreamCode = 3;
        public const int float_ArrayStreamCode = 4;
        public const int double_ArrayStreamCode = 5;
        public const int bool_ArrayStreamCode = 6;
        public const int timestamp_ArrayStreamCode = 7;
        public const int datetime_ArrayStreamCode = 8; //DEPRECATED!!!
        public const int string_ArrayStreamCode = 9;

        public const int Sensored_ArrayStreamCode = 10;
        public const int FiberCoords_ArrayStreamCode = 11;
        public const int Coords_ArrayStreamCode = 12;
        public const int MultiIntFloat_ArrayStreamCode = 13;        

        public const int SensoredScaled_StreamCode = 19;
        public const int SensoredScaledFloat_StreamCode = 20;
        public const int byteScaled_StreamCode = 21;
        public const int shortScaled_StreamCode = 22;
        public const int intScaled_StreamCode = 23;
        public const int floatScaled_StreamCode = 24;
        public const int doubleScaled_ArrayStreamCode = 25;

        //
        public const int DataArray_StreamSerializerId = 30;
        public const int DataCartogramLinear_StreamSerializerId = 31;
        public const int DataCartogramLinearWide_StreamSerializerId = 32;
        public const int DataCartogramNative_StreamSerializerId = 33;
        public const int DataCartogramIndexed_StreamSerializerId = 34;
        public const int DataCartogramPvk_StreamSerializerId = 35;
        public const int DataCartogramIdxMultiValue_StreamSerializerId = 36;

        public const int DataParamTable_StreamSerializerId = 40;

        public const int XmlData_StreamSerializerId = 50;

        public const int serializeDataTupleMulti = 101;
        public const int serializeDataTuple = 100;

        public const int unknown_object = 255;

        public static readonly KnownType[] _knownTypes = {
                new KnownType(typeof(byte),   KnownType.byte_ArrayStreamCode),
                new KnownType(typeof(short),  KnownType.short_ArrayStreamCode),
                new KnownType(typeof(int),    KnownType.int_ArrayStreamCode),
                new KnownType(typeof(float),  KnownType.float_ArrayStreamCode),
                new KnownType(typeof(double), KnownType.double_ArrayStreamCode),

                new KnownType(typeof(Timestamp), KnownType.timestamp_ArrayStreamCode),
                new KnownType(typeof(DateTime), KnownType.datetime_ArrayStreamCode),
                new KnownType(typeof(string),   KnownType.string_ArrayStreamCode),

                // Start index is 10
                new KnownType(typeof(Sensored),      KnownType.Sensored_ArrayStreamCode),
                new KnownType(typeof(FiberCoords),   KnownType.FiberCoords_ArrayStreamCode),
                new KnownType(typeof(Coords),        KnownType.Coords_ArrayStreamCode),
                new KnownType(typeof(MultiIntFloat), KnownType.MultiIntFloat_ArrayStreamCode)
            };

        public static int GetIdFromType(Type type)
        {
            foreach (KnownType t in _knownTypes)
            {
                if (t._type == type)
                    return t._id;
            }
            throw new ArgumentException("Unknown type");
        }

        public static Type GetTypeFromId(int id)
        {
            foreach (KnownType t in _knownTypes)
            {
                if (t._id == id)
                    return t._type;
            }
            throw new ArgumentException("Unknown type");
        }

    }
	
    

   
}

