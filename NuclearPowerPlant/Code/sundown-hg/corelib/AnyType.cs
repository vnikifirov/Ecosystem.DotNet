using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace corelib
{

    public struct Timestamp
    {
        private double _oledate;

        public Timestamp(double aodate)
        {
            _oledate = aodate;
        }

        public Timestamp(DateTime date)
        {
            _oledate = date.ToOADate();
        }

        public DateTime DateTime
        {
            get
            {
                return System.DateTime.FromOADate(_oledate);
            }
        }

        public double OADate
        {
            get
            {
                return _oledate;
            }
        }

        static public implicit operator DateTime(Timestamp s)
        {
            return s.DateTime;
        }

        static public implicit operator double(Timestamp s)
        {
            return s._oledate;
        }

        static public implicit operator Timestamp(double a)
        {
            return new Timestamp(a);
        }

        static public implicit operator Timestamp(DateTime a)
        {
            return new Timestamp(a);
        }

        public override string ToString()
        {
            //return String.Format("{0} [{1}]", DateTime, OADate);
            return DateTime.ToString();
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 5)]
    public struct SensoredMas
    {
        [FieldOffset(0)]
        private short _min;

        [FieldOffset(2)]
        private short _max;

        [FieldOffset(4)]
        private byte _ras;

        public SensoredMas(short min, short max, byte ras)
        {
            _min = min;
            _max = max;
            _ras = ras;
        }

        public ScaleIndex ToScaleIndex(short range)
        {
            return new ScaleIndex((double)_min,
            ((double)_max - (double)_min) / range / (double)Math.Pow(10, _ras));
        }

        public short Min
        {
            get { return _min; }
        }

        public short Max
        {
            get { return _max; }
        }

        public byte Ras
        {
            get { return _ras; }
        }

        public override string ToString()
        {
            return String.Format("[{0} -- {1}]*10^{2}*Range", _min, _max, _ras);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct AnyValue : IConvertible
    {
        [FieldOffset(0)]
        private byte _byte;

        [FieldOffset(0)]
        private short _short;

        [FieldOffset(0)]
        private int _int;

        [FieldOffset(0)]
        private float _float;

        [FieldOffset(0)]
        private double _double;

        [FieldOffset(0)]
        private Sensored _sensored;

        [FieldOffset(0)]
        private Coords _coords;

        [FieldOffset(0)]
        private FiberCoords _fcoords;

        [FieldOffset(0)]
        private MultiIntFloat _multi;

        [FieldOffset(8)]
        private float _mp;

        [FieldOffset(8)]
        private short _srange;

        [FieldOffset(10)]
        private SensoredMas _smas;

        [FieldOffset(15)]
        private byte _type;

        [FieldOffset(16)]
        public object _object;

        static public AnyValue DropScale(AnyValue v)
        {
            switch (v._type)
            {
                case KnownType.byte_ArrayStreamCode:
                case KnownType.short_ArrayStreamCode:
                case KnownType.int_ArrayStreamCode:
                case KnownType.float_ArrayStreamCode:
                case KnownType.double_ArrayStreamCode:

                case KnownType.timestamp_ArrayStreamCode:
                case KnownType.Sensored_ArrayStreamCode:
                case KnownType.Coords_ArrayStreamCode:
                case KnownType.FiberCoords_ArrayStreamCode:
                case KnownType.MultiIntFloat_ArrayStreamCode:
                    return v;

                case KnownType.SensoredScaled_StreamCode: return new AnyValue(v._sensored);
                case KnownType.byteScaled_StreamCode: return new AnyValue(v._byte);
                case KnownType.shortScaled_StreamCode: return new AnyValue(v._short);
                case KnownType.intScaled_StreamCode: return new AnyValue(v._int);
                case KnownType.floatScaled_StreamCode: return new AnyValue(v._float);
                case KnownType.doubleScaled_ArrayStreamCode: return new AnyValue(v._double);

                default: throw new InvalidCastException();
            }
        }

		public interface IDummy
		{

		}

		 public AnyValue(IDummy d)
		 {
			_byte = 0;
			_short = 0;
			_int = 0;
			_float = 0;
			_double = 0;
			_sensored = new Sensored();
			_coords = new Coords();
			_fcoords = new FiberCoords();
			_multi = new MultiIntFloat();
			_mp = 0;
			_srange = 0;
			_smas = new SensoredMas();
			_type = 0;
			_object = null;
		 }

        public AnyValue(Sensored s, SensoredMas mas, short range)
            : this((IDummy)null)
        {
            _type = KnownType.SensoredScaled_StreamCode;
            _smas = mas;
            _srange = range;
            _sensored = s;
        }

        public AnyValue(byte b)
            : this(b, 0)
        {
        }

        public AnyValue(byte b, float mp)
            : this((IDummy)null)
        {
            _mp = mp;
            if (mp != 0)
                _type = KnownType.byteScaled_StreamCode;
            else
                _type = KnownType.byte_ArrayStreamCode;

            _byte = b;
        }

        public AnyValue(short s)
            : this(s, 0)
        {

        }
        public AnyValue(short s, float mp)
            : this((IDummy)null)
        {
            _mp = mp;
            if (mp != 0)
                _type = KnownType.shortScaled_StreamCode;
            else
                _type = KnownType.short_ArrayStreamCode;
            _short = s;
        }

        public AnyValue(int i)
            : this(i, 0)
        {

        }
        public AnyValue(int i, float mp)
            : this((IDummy)null)
        {
            _mp = mp;
            if (mp != 0)
                _type = KnownType.intScaled_StreamCode;
            else
                _type = KnownType.int_ArrayStreamCode;
            _int = i;
        }

        public AnyValue(bool i)
            : this((IDummy)null)
        {
            _type = KnownType.bool_ArrayStreamCode;
            _int = i ? 1 : 0;
        }

        public AnyValue(float f)
            : this(f, 0)
        {

        }

        public AnyValue(float f, float mp)
            : this((IDummy)null)
        {
            _mp = mp;
            if (mp != 0)
                _type = KnownType.floatScaled_StreamCode;
            else
                _type = KnownType.float_ArrayStreamCode;
            _float = f;
        }

        public AnyValue(Timestamp date)
            : this((IDummy)null)
        {
            _type = KnownType.timestamp_ArrayStreamCode;
            _double = date;
        }

        public AnyValue(double d)
            : this(d, 0)
        { }

        public AnyValue(double d, float mp)
            : this((IDummy)null)
        {
            _mp = mp;
            if (mp != 0)
                _type = KnownType.doubleScaled_ArrayStreamCode;
            else
                _type = KnownType.double_ArrayStreamCode;
            _double = d;
        }

        public AnyValue(Sensored d)
            : this(d, 0)
        { }

        public AnyValue(Sensored s, float mp)
            : this((IDummy)null)
        {
            _mp = mp;
            if (mp != 0)
                _type = KnownType.SensoredScaledFloat_StreamCode;
            else
                _type = KnownType.Sensored_ArrayStreamCode;
            _sensored = s;
        }

        public AnyValue(Coords c)
            : this((IDummy)null)
        {
            _type = KnownType.Coords_ArrayStreamCode;
            _coords = c;
        }

        public AnyValue(FiberCoords c)
            : this((IDummy)null)
        {
            _type = KnownType.FiberCoords_ArrayStreamCode;
            _fcoords = c;
        }

        public AnyValue(MultiIntFloat m)
            : this((IDummy)null)
        {
            _type = KnownType.MultiIntFloat_ArrayStreamCode;
            _multi = m;
        }

        public static AnyValue Empty()
        {
            AnyValue v = new AnyValue((IDummy)null);
            v._type = KnownType.empty_ArrayStreamCode;
            return v;
        }

        private AnyValue(int type, object obj)
            : this((IDummy)null)
        {
            _type = (byte)type;
            _object = obj;
        }

        public AnyValue(string str)
            : this(KnownType.string_ArrayStreamCode, str)
        {
        }

        public AnyValue(DataParamTable table)
            : this (KnownType.DataParamTable_StreamSerializerId, table)
        {

        }

        static public AnyValue FromXmlString(string s)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(s);
            XmlElement element = (XmlElement)doc.FirstChild;

            return new AnyValue(KnownType.XmlData_StreamSerializerId, element);
        }

        static public AnyValue FromBoxedValue(object o)
        {
            if (o is double) return (double)o;
            else if (o is float) return (float)o;
            else if (o is int) return (int)o;
            else if (o is short) return (short)o;
            else if (o is byte) return (byte)o;
            else if (o is Coords) return (Coords)o;
            else if (o is DateTime) return (DateTime)o;
            else if (o is Sensored) return (Sensored)o;
            else if (o is FiberCoords) return (FiberCoords)o;
            else if (o is MultiIntFloat) return (MultiIntFloat)o;
            else if (o is AnyValue) return (AnyValue)o;
            else if (o is String) return (String)o;
            else if (o is bool) return (bool)o;
            else if (o == null) return new AnyValue();
            else

                return new AnyValue(KnownType.unknown_object, o);
        }

        public static implicit operator AnyValue(int i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(short i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(byte i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(float i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(double i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(DateTime i)
        {
            return new AnyValue(new Timestamp(i));
        }

        public static implicit operator AnyValue(Timestamp i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(Sensored i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(Coords i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(FiberCoords i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(MultiIntFloat i)
        {
            return new AnyValue(i);
        }

        public static implicit operator AnyValue(string s)
        {
            return new AnyValue(s);
        }

        public static implicit operator AnyValue(bool b)
        {
            return new AnyValue(b);
        }

        public static implicit operator AnyValue(DataParamTable s)
        {
            return new AnyValue(s);
        }


        public static explicit operator int(AnyValue v)
        {
            return v.ValueInt;
        }

        public static explicit operator short(AnyValue v)
        {
            return v.ValueShort;
        }

        public static explicit operator byte(AnyValue v)
        {
            return v.ValueByte;
        }

        public static explicit operator float(AnyValue v)
        {
            return v.ValueFloat;
        }

        public static explicit operator double(AnyValue v)
        {
            return v.ValueDouble;
        }

        public static explicit operator Coords(AnyValue v)
        {
            return v.ValueCoords;
        }

        public static explicit operator FiberCoords(AnyValue v)
        {
            return v.ValueFiberCoords;
        }

        public static explicit operator MultiIntFloat(AnyValue v)
        {
            return v.ValueMulti;
        }

        public static explicit operator Sensored(AnyValue v)
        {
            return v.ValueSensored;
        }

        public static explicit operator string(AnyValue v)
        {
            return v.ValueString;
        }

        public byte ValueByte
        {
            get
            {
                if (_type == KnownType.byte_ArrayStreamCode) return _byte;
                throw new InvalidCastException();
            }
        }

        public short ValueShort
        {
            get
            {
                if (_type == KnownType.short_ArrayStreamCode) return _short;
                throw new InvalidCastException();
            }
        }

        public int ValueInt
        {
            get
            {
                if (_type == KnownType.int_ArrayStreamCode) return _int;
                throw new InvalidCastException();
            }
        }

        public float ValueFloat
        {
            get
            {
                if (_type == KnownType.float_ArrayStreamCode) return _float;
                throw new InvalidCastException();
            }
        }

        public double ValueDouble
        {
            get
            {
                if (_type == KnownType.double_ArrayStreamCode) return _double;
                throw new InvalidCastException();
            }
        }

        public Sensored ValueSensored
        {
            get
            {
                if (_type == KnownType.Sensored_ArrayStreamCode) return _sensored;
                throw new InvalidCastException();
            }
        }

        public Coords ValueCoords
        {
            get
            {
                if (_type == KnownType.Coords_ArrayStreamCode) return _coords;
                throw new InvalidCastException();
            }
        }

        public FiberCoords ValueFiberCoords
        {
            get
            {
                if (_type == KnownType.FiberCoords_ArrayStreamCode) return _fcoords;
                throw new InvalidCastException();
            }
        }

        public MultiIntFloat ValueMulti
        {
            get
            {
                if (_type == KnownType.MultiIntFloat_ArrayStreamCode) return _multi;
                throw new InvalidCastException();
            }
        }

        public Timestamp ValueTimestamp
        {
            get
            {
                if (_type == KnownType.timestamp_ArrayStreamCode) return _double;
                throw new InvalidCastException();
            }
        }

        public double ValueScaled
        {
            get
            {
                return ToDouble(null);
            }
        }

        public DataParamTable ValueDataParamTable
        {
            get {
                if (_type == KnownType.DataParamTable_StreamSerializerId) return (DataParamTable)_object;
                throw new InvalidCastException();
            }
        }

        public string ValueString
        {
            get
            {
                if (_type == KnownType.empty_ArrayStreamCode) return null;
                else if (_type == KnownType.string_ArrayStreamCode) return (string)_object;
                throw new InvalidCastException();
            }
        }

        public XmlElement ValueXml
        {
            get
            {
                if (_type == KnownType.XmlData_StreamSerializerId) return (XmlElement)_object;
                throw new InvalidCastException();
            }
        }

        public object Value
        {
            get
            {
                switch (_type)
                {
                    case KnownType.empty_ArrayStreamCode: return null;
                    case KnownType.byte_ArrayStreamCode: return _byte;
                    case KnownType.short_ArrayStreamCode: return _short;
                    case KnownType.int_ArrayStreamCode: return _int;
                    case KnownType.float_ArrayStreamCode: return _float;
                    case KnownType.double_ArrayStreamCode: return _double;

                    case KnownType.Sensored_ArrayStreamCode: return _sensored;
                    case KnownType.Coords_ArrayStreamCode: return _coords;
                    case KnownType.FiberCoords_ArrayStreamCode: return _fcoords;
                    case KnownType.MultiIntFloat_ArrayStreamCode: return _multi;
                    case KnownType.timestamp_ArrayStreamCode: return new Timestamp(_double).DateTime;
                    case KnownType.bool_ArrayStreamCode: return _int != 0;

                    case KnownType.SensoredScaled_StreamCode:
                    case KnownType.byteScaled_StreamCode:
                    case KnownType.shortScaled_StreamCode:
                    case KnownType.intScaled_StreamCode:
                    case KnownType.floatScaled_StreamCode:
                    case KnownType.doubleScaled_ArrayStreamCode:
                    case KnownType.SensoredScaledFloat_StreamCode:
                        return ValueScaled;

                    case KnownType.string_ArrayStreamCode:
                    case KnownType.DataParamTable_StreamSerializerId:
                    case KnownType.XmlData_StreamSerializerId:
                    case KnownType.unknown_object:
                        return _object;

                    default: throw new InvalidCastException();
                }
            }
        }

        public int Type
        {
            get { return _type; }
        }

        public bool IsNull
        {
            get { return _type == KnownType.empty_ArrayStreamCode; }
        }

        public bool IsNotNull
        {
            get { return _type != KnownType.empty_ArrayStreamCode; }
        }

        public bool IsByte
        {
            get { return _type == KnownType.byte_ArrayStreamCode; }
        }
        public bool IsShort
        {
            get { return _type == KnownType.short_ArrayStreamCode; }
        }
        public bool IsInt
        {
            get { return _type == KnownType.int_ArrayStreamCode; }
        }
        public bool IsDouble
        {
            get { return _type == KnownType.double_ArrayStreamCode; }
        }
        public bool IsFloat
        {
            get { return _type == KnownType.float_ArrayStreamCode; }
        }
        public bool IsSensored
        {
            get { return _type == KnownType.Sensored_ArrayStreamCode; }
        }
        public bool IsCoords
        {
            get { return _type == KnownType.Coords_ArrayStreamCode; }
        }
        public bool IsFiberCoords
        {
            get { return _type == KnownType.FiberCoords_ArrayStreamCode; }
        }
        public bool IsMultiIntFloat
        {
            get { return _type == KnownType.MultiIntFloat_ArrayStreamCode; }
        }
        public bool IsTimestamp
        {
            get { return _type == KnownType.timestamp_ArrayStreamCode; }
        }


        public bool IsString
        {
            get { return _type == KnownType.string_ArrayStreamCode; }
        }

        public bool IsDataParamTable
        {
            get { return _type == KnownType.DataParamTable_StreamSerializerId; }
        }

        public bool IsObject
        {
            get { return _type == KnownType.unknown_object; }
        }

        public bool IsXml
        {
            get { return _type == KnownType.XmlData_StreamSerializerId; }
        }

        public ScaleIndex ScaleIndex
        {
            get
            {
                switch (_type)
                {
                    case KnownType.SensoredScaled_StreamCode:
                        return _smas.ToScaleIndex(_srange);

                    case KnownType.byteScaled_StreamCode:
                    case KnownType.shortScaled_StreamCode:
                    case KnownType.intScaled_StreamCode:
                    case KnownType.floatScaled_StreamCode:
                    case KnownType.doubleScaled_ArrayStreamCode:
                    case KnownType.SensoredScaledFloat_StreamCode:
                        return new ScaleIndex(0, _mp);

                    default:
                        throw new InvalidCastException();
                }
            }
        }

        public bool IsScaled
        {
            get
            {
                switch (_type)
                {
                    case KnownType.byteScaled_StreamCode:
                    case KnownType.shortScaled_StreamCode:
                    case KnownType.intScaled_StreamCode:
                    case KnownType.floatScaled_StreamCode:
                    case KnownType.doubleScaled_ArrayStreamCode:
                    case KnownType.SensoredScaled_StreamCode:
                    case KnownType.SensoredScaledFloat_StreamCode:
                        return true;
                    default:
                        return false;
                }
            }
        }

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            switch (_type)
            {
                case KnownType.empty_ArrayStreamCode: return TypeCode.Empty;
                case KnownType.byte_ArrayStreamCode: return TypeCode.Byte;
                case KnownType.short_ArrayStreamCode: return TypeCode.Int16;
                case KnownType.int_ArrayStreamCode: return TypeCode.Int32;
                case KnownType.float_ArrayStreamCode: return TypeCode.Single;
                case KnownType.double_ArrayStreamCode: return TypeCode.Double;

                case KnownType.Sensored_ArrayStreamCode: return TypeCode.Object;
                case KnownType.Coords_ArrayStreamCode: return TypeCode.Object;
                case KnownType.FiberCoords_ArrayStreamCode: return TypeCode.Object;
                case KnownType.MultiIntFloat_ArrayStreamCode: return TypeCode.Object;

                case KnownType.byteScaled_StreamCode:
                case KnownType.shortScaled_StreamCode:
                case KnownType.intScaled_StreamCode:
                case KnownType.floatScaled_StreamCode:
                case KnownType.doubleScaled_ArrayStreamCode:
                case KnownType.SensoredScaled_StreamCode:
                case KnownType.SensoredScaledFloat_StreamCode:
                    return TypeCode.Object;

                case KnownType.string_ArrayStreamCode:
                    return TypeCode.String;

                default: return TypeCode.Empty;
            }
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.bool_ArrayStreamCode:
                    return (_int == 0) ? false : true;
                case KnownType.string_ArrayStreamCode:
                    return Convert.ToBoolean((string)_object);
                default: throw new InvalidCastException();
            }
        }

        public byte ToByte(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return (byte)_byte;
                case KnownType.short_ArrayStreamCode: return (byte)_short;
                case KnownType.int_ArrayStreamCode: return (byte)_int;
                case KnownType.float_ArrayStreamCode: return (byte)_float;
                case KnownType.double_ArrayStreamCode: return (byte)_double;
                case KnownType.Sensored_ArrayStreamCode: return (byte)_sensored.Value;

                case KnownType.byteScaled_StreamCode: return (byte)(_byte * _mp);
                case KnownType.shortScaled_StreamCode: return (byte)(_short * _mp);
                case KnownType.intScaled_StreamCode: return (byte)(_int * _mp);
                case KnownType.floatScaled_StreamCode: return (byte)(_float * _mp);
                case KnownType.doubleScaled_ArrayStreamCode: return (byte)(_double * _mp);
                case KnownType.SensoredScaledFloat_StreamCode: return (byte)(_sensored.Value * _mp);
                case KnownType.SensoredScaled_StreamCode:
                    return (byte)(_smas.ToScaleIndex(_srange).Scale(_sensored));

                case KnownType.string_ArrayStreamCode:
                    return Convert.ToByte((string)_object);

                default: throw new InvalidCastException();
            }
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            if ((_type == KnownType.double_ArrayStreamCode) ||
             (_type == KnownType.timestamp_ArrayStreamCode))
                return new Timestamp(_double).DateTime;

            else if (_type == KnownType.string_ArrayStreamCode)
                return Convert.ToDateTime((string)_object);

            throw new InvalidCastException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return (double)_byte;
                case KnownType.short_ArrayStreamCode: return (double)_short;
                case KnownType.int_ArrayStreamCode: return (double)_int;
                case KnownType.float_ArrayStreamCode: return (double)_float;
                case KnownType.double_ArrayStreamCode: return (double)_double;
                case KnownType.Sensored_ArrayStreamCode: return (double)_sensored.Value;

                case KnownType.byteScaled_StreamCode: return ((double)_byte * _mp);
                case KnownType.shortScaled_StreamCode: return ((double)_short * _mp);
                case KnownType.intScaled_StreamCode: return ((double)_int * _mp);
                case KnownType.floatScaled_StreamCode: return ((double)_float * _mp);
                case KnownType.doubleScaled_ArrayStreamCode: return (double)(_double * _mp);
                case KnownType.SensoredScaledFloat_StreamCode: return (double)(_sensored.Value * _mp);
                case KnownType.SensoredScaled_StreamCode:
                    return (double)(_smas.ToScaleIndex(_srange).Scale(_sensored));

                case KnownType.string_ArrayStreamCode:
                    return Convert.ToDouble((string)_object);

                default: throw new InvalidCastException();
            }
        }

        public short ToInt16(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return (short)_byte;
                case KnownType.short_ArrayStreamCode: return (short)_short;
                case KnownType.int_ArrayStreamCode: return (short)_int;
                case KnownType.float_ArrayStreamCode: return (short)_float;
                case KnownType.double_ArrayStreamCode: return (short)_double;
                case KnownType.Sensored_ArrayStreamCode: return (short)_sensored.Value;

                case KnownType.byteScaled_StreamCode: return (short)(_byte * _mp);
                case KnownType.shortScaled_StreamCode: return (short)(_short * _mp);
                case KnownType.intScaled_StreamCode: return (short)(_int * _mp);
                case KnownType.floatScaled_StreamCode: return (short)(_float * _mp);
                case KnownType.doubleScaled_ArrayStreamCode: return (short)(_double * _mp);
                case KnownType.SensoredScaledFloat_StreamCode: return (short)(_sensored.Value * _mp);
                case KnownType.SensoredScaled_StreamCode:
                    return (short)(_smas.ToScaleIndex(_srange).Scale(_sensored));

                case KnownType.string_ArrayStreamCode:
                    return Convert.ToInt16((string)_object);

                default: throw new InvalidCastException();
            }
        }

        public int ToInt32(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return (int)_byte;
                case KnownType.short_ArrayStreamCode: return (int)_short;
                case KnownType.int_ArrayStreamCode: return (int)_int;
                case KnownType.float_ArrayStreamCode: return (int)_float;
                case KnownType.double_ArrayStreamCode: return (int)_double;
                case KnownType.Sensored_ArrayStreamCode: return (int)_sensored.Value;

                case KnownType.byteScaled_StreamCode: return (int)(_byte * _mp);
                case KnownType.shortScaled_StreamCode: return (int)(_short * _mp);
                case KnownType.intScaled_StreamCode: return (int)(_int * _mp);
                case KnownType.floatScaled_StreamCode: return (int)(_float * _mp);
                case KnownType.doubleScaled_ArrayStreamCode: return (int)(_double * _mp);
                case KnownType.SensoredScaledFloat_StreamCode: return (int)(_sensored.Value * _mp);
                case KnownType.SensoredScaled_StreamCode:
                    return (int)(_smas.ToScaleIndex(_srange).Scale(_sensored));

                case KnownType.string_ArrayStreamCode:
                    return Convert.ToInt32((string)_object);

                default: throw new InvalidCastException();
            }
        }

        public long ToInt64(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return (long)_byte;
                case KnownType.short_ArrayStreamCode: return (long)_short;
                case KnownType.int_ArrayStreamCode: return (long)_int;
                case KnownType.float_ArrayStreamCode: return (long)_float;
                case KnownType.double_ArrayStreamCode: return (long)_double;
                case KnownType.Sensored_ArrayStreamCode: return (long)_sensored.Value;

                case KnownType.byteScaled_StreamCode: return (long)(_byte * _mp);
                case KnownType.shortScaled_StreamCode: return (long)(_short * _mp);
                case KnownType.intScaled_StreamCode: return (long)(_int * _mp);
                case KnownType.floatScaled_StreamCode: return (long)(_float * _mp);
                case KnownType.doubleScaled_ArrayStreamCode: return (long)(_double * _mp);
                case KnownType.SensoredScaledFloat_StreamCode: return (long)(_sensored.Value * _mp);
                case KnownType.SensoredScaled_StreamCode:
                    return (long)(_smas.ToScaleIndex(_srange).Scale(_sensored));

                case KnownType.string_ArrayStreamCode:
                    return Convert.ToInt64((string)_object);

                default: throw new InvalidCastException();
            }
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return (float)_byte;
                case KnownType.short_ArrayStreamCode: return (float)_short;
                case KnownType.int_ArrayStreamCode: return (float)_int;
                case KnownType.float_ArrayStreamCode: return (float)_float;
                case KnownType.double_ArrayStreamCode: return (float)_double;
                case KnownType.Sensored_ArrayStreamCode: return (float)_sensored.Value;

                case KnownType.byteScaled_StreamCode: return (float)(_byte * _mp);
                case KnownType.shortScaled_StreamCode: return (float)(_short * _mp);
                case KnownType.intScaled_StreamCode: return (float)(_int * _mp);
                case KnownType.floatScaled_StreamCode: return (float)(_float * _mp);
                case KnownType.doubleScaled_ArrayStreamCode: return (float)(_double * _mp);
                case KnownType.SensoredScaledFloat_StreamCode: return (float)(_sensored.Value * _mp);
                case KnownType.SensoredScaled_StreamCode:
                    return (float)(_smas.ToScaleIndex(_srange).Scale(_sensored));

                default: throw new InvalidCastException();
            }
        }

        public string ToString(IFormatProvider provider)
        {
            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: return _byte.ToString(provider);
                case KnownType.short_ArrayStreamCode: return _short.ToString(provider);
                case KnownType.int_ArrayStreamCode: return _int.ToString(provider);
                case KnownType.float_ArrayStreamCode: return _float.ToString(provider);
                case KnownType.double_ArrayStreamCode: return _double.ToString(provider);
                case KnownType.Sensored_ArrayStreamCode: return _sensored.ToString(provider);

                case KnownType.byteScaled_StreamCode: return String.Format(provider, "{0} [{1}]", _byte * _mp, _byte);
                case KnownType.shortScaled_StreamCode: return String.Format(provider, "{0} [{1}]", _short * _mp, _short);
                case KnownType.intScaled_StreamCode: return String.Format(provider, "{0} [{1}]", _int * _mp, _int);
                case KnownType.floatScaled_StreamCode: return String.Format(provider, "{0} [{1}]", _float * _mp, _float);
                case KnownType.doubleScaled_ArrayStreamCode: return String.Format(provider, "{0} [{1}]", _double * _mp, _double);
                case KnownType.SensoredScaled_StreamCode:
                    return String.Format(provider, "{0} [{1}]", _smas.ToScaleIndex(_srange).Scale(_sensored), _sensored);

                case KnownType.timestamp_ArrayStreamCode:
                    return ValueTimestamp.DateTime.ToString(provider);

                case KnownType.string_ArrayStreamCode:
                case KnownType.DataParamTable_StreamSerializerId:
                    return _object.ToString();                

                case KnownType.empty_ArrayStreamCode:
                    return null;

                default: throw new InvalidCastException();
            }
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(double))
                return ToDouble(provider);
            else if (conversionType == typeof(int))
                return ToInt32(provider);
            else if (conversionType == typeof(short))
                return ToInt16(provider);
            else if (conversionType == typeof(byte))
                return ToByte(provider);
            else if (conversionType == typeof(float))
                return ToSingle(provider);
            else if (conversionType == typeof(DateTime))
                return ToDateTime(provider);
            else
                throw new InvalidCastException();
        }


        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        #endregion

        public double ToDouble()
        {
            return ToDouble(null);
        }

        public float ToSingle()
        {
            return ToSingle(null);
        }

        public int ToInt32()
        {
            return ToInt32(null);
        }

        public short ToInt16()
        {
            return ToInt16(null);
        }

        public byte ToByte()
        {
            return ToByte(null);
        }

        public void Serialize(ISerializeStream s, int ver)
        {
            if ((ver == 0) && (IsScaled))
                s.Put((int)KnownType.double_ArrayStreamCode);
            else
                s.Put((int)_type);

            switch (_type)
            {
                case KnownType.byte_ArrayStreamCode: s.Put(_byte); return;
                case KnownType.short_ArrayStreamCode: s.Put(_short); return;
                case KnownType.int_ArrayStreamCode: s.Put(_int); return;
                case KnownType.float_ArrayStreamCode: s.Put(_float); return;
                case KnownType.double_ArrayStreamCode: s.Put(_double); return;

                case KnownType.Sensored_ArrayStreamCode: s.Put(_sensored); return;
                case KnownType.Coords_ArrayStreamCode: s.Put(_coords); return;
                case KnownType.FiberCoords_ArrayStreamCode: s.Put(_fcoords); return;
                case KnownType.MultiIntFloat_ArrayStreamCode: s.Put(_multi); return;

                case KnownType.bool_ArrayStreamCode: s.Put(_int); return;
                case KnownType.string_ArrayStreamCode: s.Put(ValueString); return;
                case KnownType.XmlData_StreamSerializerId: s.Put(ValueXml.ParentNode.InnerXml); return;
                case KnownType.empty_ArrayStreamCode: return;
                case KnownType.timestamp_ArrayStreamCode: s.Put(_double); return;
            }

            if (ver == 0)
            {
                switch (_type)
                {
                    case KnownType.byteScaled_StreamCode:
                    case KnownType.shortScaled_StreamCode:
                    case KnownType.intScaled_StreamCode:
                    case KnownType.floatScaled_StreamCode:
                    case KnownType.double_ArrayStreamCode:
                    case KnownType.SensoredScaled_StreamCode:
                        s.Put(ValueScaled); return;

                    default: throw new InvalidCastException();
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }


        static public AnyValue Deserialize(IDeserializeStream s)
        {
            int type;
            s.Get(out type);

            switch (type)
            {
                case KnownType.byte_ArrayStreamCode: { byte _byte; s.Get(out _byte); return _byte; }
                case KnownType.short_ArrayStreamCode: { short _short; s.Get(out _short); return _short; }
                case KnownType.int_ArrayStreamCode: { int _int; s.Get(out _int); return _int; }
                case KnownType.float_ArrayStreamCode: { float _float; s.Get(out _float); return _float; }
                case KnownType.double_ArrayStreamCode: { double _double; s.Get(out _double); return _double; }

                case KnownType.Sensored_ArrayStreamCode: { Sensored _sensored; s.Get(out _sensored); return _sensored; }
                case KnownType.Coords_ArrayStreamCode: { Coords _coords; s.Get(out _coords); return _coords; }
                case KnownType.FiberCoords_ArrayStreamCode: { FiberCoords _fcoords; s.Get(out _fcoords); return _fcoords; }
                case KnownType.MultiIntFloat_ArrayStreamCode: { MultiIntFloat _multi; s.Get(out _multi); return _multi; }

                case KnownType.bool_ArrayStreamCode: { int i; s.Get(out i); return i != 0 ? true : false; }
                case KnownType.string_ArrayStreamCode: { string str; s.Get(out str); return str; }
                case KnownType.XmlData_StreamSerializerId: { string str; s.Get(out str); return AnyValue.FromXmlString(str); }
                case KnownType.empty_ArrayStreamCode: return new AnyValue();
                case KnownType.timestamp_ArrayStreamCode: { double a; s.Get(out a); return new Timestamp(a); }

                default: throw new InvalidCastException();
            }
        }

        public override string ToString()
        {
            return ToString(null);
        }
    }
}
