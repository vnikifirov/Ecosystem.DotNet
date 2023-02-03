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


    using DataCartogramIndexedInt = DataCartogramIndexed<int>;
    using DataCartogramIndexedShort = DataCartogramIndexed<short>;
    using DataCartogramIndexedByte = DataCartogramIndexed<byte>;
    using DataCartogramIndexedFloat = DataCartogramIndexed<float>;
    using DataCartogramIndexedDouble = DataCartogramIndexed<double>;
    using DataCartogramIndexedSensored = DataCartogramIndexed<Sensored>;

    using DataCartogramNativeInt = DataCartogramNative<int>;
    using DataCartogramNativeShort = DataCartogramNative<short>;
    using DataCartogramNativeByte = DataCartogramNative<byte>;
    using DataCartogramNativeFloat = DataCartogramNative<float>;
    using DataCartogramNativeDouble = DataCartogramNative<double>;
    using DataCartogramNativeSensored = DataCartogramNative<Sensored>;

    using GeneratorDouble = Generator1<double>;
    using GeneratorDouble3 = Generator<double>;
#endif

    public interface IDataCartogramIndexed
    {
        CoordsConverter Coords { get; }

        double this[int index, int layer] { get; }

        object GetObject(int index, int layer);

        AnyValue GetAnyValue(int idx, int layer);
    }

    public delegate double OperationCartogram(double p1, double p2);

    public abstract class DataCartogram : AbstactTupleItem, ITupleItem, IDataCartogram
    {
        protected readonly ScaleIndex _scale;
        protected readonly int _layers;

        protected DataCartogram(TupleMetaData info, ScaleIndex scale, int layers)
            : base(info)
        {
            _scale = scale;
            _layers = layers;
        }

        protected static int CalculateLayersCount(DataArray array, bool multiLayer)
        {
            int layers;
            switch (array.Rank)
            {
                case 0:
                    layers = 0;
                    break;
                case 1:
                    layers = 1;
                    if (multiLayer)
                        throw new ArgumentException();
                    break;
                case 2:
                    if (multiLayer)
                        layers = array.DimY;
                    else
                        layers = 1;
                    break;
                case 3:
                    if (multiLayer)
                        layers = array.DimZ;
                    else
                        throw new ArgumentException();
                    break;
                default:
                    throw new ArgumentException();
            }
            return layers;
        }

        protected void ChechCompatabilityConverter(CoordsConverter converter)
        {
            DataArray main = Array;

            switch (converter.Rank)
            {
                case 0:
                    if (_layers > 0)
                        throw new ArgumentException();
                    break;
                case 1:
                    if (_layers > 1)
                    {
                        if ((converter.DimX != main.DimX) || (converter.DimZ != main.DimZ))
                            throw new ArgumentException();
                    }
                    else
                    {
                        if ((converter.DimX != main.DimX) || (converter.DimY != main.DimY) || (converter.DimZ != main.DimZ))
                            throw new ArgumentException();
                    }
                    break;

                case 2:
                    if (_layers > 1)
                    {
                        if ((converter.DimX != main.DimX) || (converter.DimY != 0) || (converter.DimZ != 0))
                            throw new ArgumentException();
                    }
                    else
                    {
                        if ((converter.DimX != main.DimX) || (converter.DimY != main.DimY) || (converter.DimZ != main.DimZ))
                            throw new ArgumentException();
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public ScaleIndex ScaleIndex
        {
            get { return _scale; }
        }

        abstract protected CoordsConverter GetCoords();

        public Type ElementType
        {
            get
            {
                return Array.ElementType;
            }
        }

        abstract protected DataArray Array
        {
            get;
        }

        abstract public bool IsNative
        {
            get;
        }

        public int Layers
        {
            get { return _layers; }
        }


        public bool IsByte
        {
            get { return Array.IsByte; }
        }
        public bool IsShort
        {
            get { return Array.IsShort; }
        }
        public bool IsInt
        {
            get { return Array.IsInt; }
        }
        public bool IsDouble
        {
            get { return Array.IsDouble; }
        }
        public bool IsFloat
        {
            get { return Array.IsFloat; }
        }
        public bool IsSensored
        {
            get { return Array.IsSensored; }
        }
        public bool IsCoords
        {
            get { return Array.IsCoords; }
        }
        public bool IsFiberCoords
        {
            get { return Array.IsFiberCoords; }
        }

        public DataArray GetDataArray()
        {
            return Array;
        }

        abstract public object GetObject(Coords c, int layer);
        abstract public double GetValue(Coords c, int layer);

        public double this[Coords c, int layer]
        {
            get
            {
                return GetValue(c, layer);
            }
        }

        public override ITupleItem Rename(TupleMetaData newInfo)
        {
            return Reinfo(newInfo);
        }

        protected AnyValue GetAnyValue(int idx, int layer)
        {
            float scale = 0;
            if (this.ScaleIndex != ScaleIndex.Default)
            {
                if (ScaleIndex.Min != 0)
                    throw new NotSupportedException();

                scale = (float)this.ScaleIndex.ScaleVal;
            }
            switch (Array.ElementId)
            {
                case KnownType.byte_ArrayStreamCode: return new AnyValue(((DataCartogramIndexedByte)this).GetItem(idx, layer),scale);
                case KnownType.short_ArrayStreamCode: return new AnyValue(((DataCartogramIndexedShort)this).GetItem(idx, layer), scale);
                case KnownType.int_ArrayStreamCode: return new AnyValue(((DataCartogramIndexedInt)this).GetItem(idx, layer), scale);
                case KnownType.float_ArrayStreamCode: return new AnyValue(((DataCartogramIndexedFloat)this).GetItem(idx, layer), scale);
                case KnownType.double_ArrayStreamCode: return new AnyValue(((DataCartogramIndexedDouble)this).GetItem(idx, layer), scale);
                case KnownType.Sensored_ArrayStreamCode: return new AnyValue(((DataCartogramIndexedSensored)this).GetItem(idx, layer), scale);
                default: throw new NotSupportedException();
            }
        }

        public AnyValue GetAnyValue(Coords c, int layer)
        {
            if (IsNative)
            {
                float scale = 0;
                if (this.ScaleIndex != ScaleIndex.Default)
                {
                    if (ScaleIndex.Min != 0)
                        throw new NotSupportedException();

                    scale = (float)this.ScaleIndex.ScaleVal;
                }
                switch (Array.ElementId)
                {
                    case KnownType.byte_ArrayStreamCode: return new AnyValue(((DataCartogramNativeByte)this).GetItem(c, layer), scale);
                    case KnownType.short_ArrayStreamCode: return new AnyValue(((DataCartogramNativeShort)this).GetItem(c, layer), scale);
                    case KnownType.int_ArrayStreamCode: return new AnyValue(((DataCartogramNativeInt)this).GetItem(c, layer), scale);
                    case KnownType.float_ArrayStreamCode: return new AnyValue(((DataCartogramNativeFloat)this).GetItem(c, layer), scale);
                    case KnownType.double_ArrayStreamCode: return new AnyValue(((DataCartogramNativeDouble)this).GetItem(c, layer), scale);
                    case KnownType.Sensored_ArrayStreamCode: return new AnyValue(((DataCartogramNativeSensored)this).GetItem(c, layer), scale);
                    default: throw new NotSupportedException();
                }
            }
            else
            {
                int idx = AllCoords[c];
                if (idx != CoordsConverter.InvalidIndex)
                    return GetAnyValue(idx, layer);
                else
                    return new AnyValue();
            }
        }

        public abstract DataCartogram Reinfo(TupleMetaData info);

        public abstract DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t);

        public abstract DataCartogram ConvertToIndexedType(TupleMetaData newInfo, CoordsConverter coords, Type t);

        public abstract DataCartogramNativeDouble ScaleToNative(TupleMetaData newInfo);

        public abstract DataCartogramIndexedDouble ScaleToIndexed(TupleMetaData newInfo, CoordsConverter coords);

        public abstract bool IsValidCoord(Coords c);


        public abstract double[,] ScaleNativeArray(double defValue);

        public abstract double[, ,] ScaleNativeArrayLayer(double defValue);

        public abstract double[] ScaleToIndexed(CoordsConverter coords, double defValue);
        public abstract double[,] ScaleToIndexedLayer(CoordsConverter coords, double defValue);

        public abstract double[,] ScaleToIndexed2(CoordsConverter coords, double defValue);
        public abstract double[, ,] ScaleToIndexed2Layer(CoordsConverter coords, double defValue);

        public abstract DataArray[] ConvertToPartArray(TupleMetaData info, CoordsConverter coords);

        struct ParsedCartogramRules
        {
            public readonly bool nFormatOk;
            public readonly bool nTypeOk;
            public readonly bool nScOk;

            public readonly bool nNative;
            public readonly CoordsConverter.SpecialFlag nFormat;
            public readonly Type nType;
            public readonly bool nRescale;

            public bool IsNative
            {
                get
                {
                    if (!nFormatOk)
                        throw new ArgumentException("Unknown format: cartogram type");
                    return nNative;
                }
            }

            public CoordsConverter.SpecialFlag CoordsFlag
            {
                get
                {
                    if (!nFormatOk)
                        throw new ArgumentException("Unknown format: cartogram type");
                    return nFormat;
                }
            }
            public Type DataType
            {
                get
                {
                    if (!nTypeOk)
                        throw new ArgumentException("Unknown format: type");
                    return nType;
                }
            }
            public bool Scale
            {
                get
                {
                    if (!nScOk)
                        throw new ArgumentException("Unknown format: scale");
                    return nRescale;
                }
            }

            public bool IsOk
            {
                get { return nScOk && nTypeOk && nFormatOk; }
            }

            static bool ToCartogramType(string format, ref bool native, ref CoordsConverter.SpecialFlag nFormat)
            {
                switch (format)
                {
                    case "pvk":
                        nFormat = CoordsConverter.SpecialFlag.PVK; native = false;
                        break;
                    case "native":
                        nFormat = CoordsConverter.SpecialFlag.Named; native = true;
                        break;
                    case "linear":
                        nFormat = CoordsConverter.SpecialFlag.Linear1884; native = false;
                        break;
                    case "linearwide":
                        nFormat = CoordsConverter.SpecialFlag.WideLinear2448; native = false;
                        break;
                    case "original":
                        break;
                    default:
                        native = true; nFormat = CoordsConverter.SpecialFlag.Named;
                        return false;
                }
                return true;
            }

            static bool ToElementType(string type, ref Type nType)
            {
                switch (type)
                {
                    case "int":
                        nType = typeof(int);
                        break;
                    case "short":
                        nType = typeof(short);
                        break;
                    case "byte":
                        nType = typeof(byte);
                        break;
                    case "float":
                        nType = typeof(float);
                        break;
                    case "double":
                        nType = typeof(double);
                        break;
                    case "sensored":
                        nType = typeof(Sensored);
                        break;
                    case "original":
                        break;
                    default:
                        nType = null;
                        return false;
                }
                return true;
            }

            static bool ToScale(string scale, ref bool nSc)
            {
                switch (scale)
                {
                    case "scale":
                        nSc = true;
                        break;
                    case "original":
                        break;
                    default:
                        nSc = true;
                        return false;
                }
                return true;
            }

            public ParsedCartogramRules(DataCartogram c, ITypeRules rules)
            {
                // Format for details
                // .. Kart(PVK, float, scale)
                // .. Kart(original, float)
                // .. Kart(original, original) == no changes

                nNative = c.IsNative;
                nType = c.ElementType;
                nRescale = false;

                nFormat = (nNative) ? CoordsConverter.SpecialFlag.Named : c.GetCoords().Flag;

                string[] details = rules.GetCastDetails();
                if (details == null)
                {
                    nFormatOk = nTypeOk = nScOk = true;
                }
                else
                {
                    string format = (details.Length > 0) ? details[0].ToLower() : "original";
                    string type = (details.Length > 1) ? details[1].ToLower() : "original";
                    string scale = (details.Length > 2) ? details[2].ToLower() : "original";

                    nFormatOk = ToCartogramType(format, ref nNative, ref nFormat);
                    nTypeOk = ToElementType(type, ref nType);
                    nScOk = ToScale(scale, ref nRescale);
                }
            }
        }

        public Array ConvertToArray(IGetCoordsConverter env, ITypeRules rules)
        {
            switch (rules.GetTypeString())
            {
                case "double":
                    try
                    {
                        string[] details = rules.GetCastDetails();
                        switch (details[0].ToLower())
                        {
                            case "native":
                                if (Layers > 1)
                                    return ScaleNativeArrayLayer(0);
                                else
                                    return ScaleNativeArray(0);

                            case "pvk":
                                if (Layers > 1)
                                    return ScaleToIndexed2Layer(
                                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, Info), 0);
                                else
                                    return ScaleToIndexed2(
                                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, Info), 0);

                            case "linear":
                                if (Layers > 1)
                                    return ScaleToIndexedLayer(
                                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, Info), 0);
                                else
                                    return ScaleToIndexed(
                                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, Info), 0);

                            case "wlinear":
                                if (Layers > 1)
                                    return ScaleToIndexedLayer(
                                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.WideLinear2448, Info), 0);
                                else
                                    return ScaleToIndexed(
                                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.WideLinear2448, Info), 0);
                            default:
                                throw new ArgumentException("don't know how to convert");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new InvalidCastException("Inner error", e);
                    }
                case "coords":
                    try {
                        CoordsConverter c = GetCoords();
                        string[] details = rules.GetCastDetails();
                        switch (c.Rank)
                        {
                            case 0:
                                if (details != null)
                                {
                                    if (details.Length > 0)
                                        throw new ArgumentException("Неверно указан ранк блока коодинат");
                                }
                                Coords[] cret = new Coords[0];
                                c.ToArray(out cret);
                                return cret;
                            case 1:
                                if (details != null)
                                {
                                    if (details.Length > 1)
                                        throw new ArgumentException("Неверно указан ранк блока коодинат");
                                    if (details[0] != null && details[0].Length > 0)
                                    {
                                        int num = Convert.ToInt32(details[0]);
                                        if (num != c.DimX)
                                            throw new ArgumentException("Неверно указан размер блока коодинат");
                                    }
                                }
                                Coords[] ret;
                                c.ToArray(out ret);
                                return ret;
                            case 2:
                                if ((details == null) || (details.Length != 2))
                                    throw new ArgumentException("Неверно указан ранк блока коодинат");

                                if (details[0].Length > 0)
                                {
                                    int num = Convert.ToInt32(details[0]);
                                    if (num != c.DimX)
                                        throw new ArgumentException("Неверно указан размер блока коодинат X");
                                }
                                if (details[1].Length > 0)
                                {
                                    int num = Convert.ToInt32(details[1]);
                                    if (num != c.DimY)
                                        throw new ArgumentException("Неверно указан размер блока коодинат Y");
                                }
                                Coords[,] ret2;
                                c.ToArray(out ret2);
                                return ret2;
                            default:
                                throw new ArgumentException("Неверный ранк исходного преобразования");
                        }
                    }
                    catch(Exception e)
                    {
                        throw new InvalidCastException("Inner error", e);
                    }
                default:
                    throw new InvalidCastException("Unknown type");
            }
        }

        public DataCartogram ConvertTo(IGetCoordsConverter env, ITypeRules rules)
        {
            ParsedCartogramRules cr = new ParsedCartogramRules(this, rules);
            bool native = IsNative;
            Type type = ElementType;
            bool rescale = false;
            CoordsConverter.SpecialFlag format = (native) ? CoordsConverter.SpecialFlag.Named : GetCoords().Flag;

            TupleMetaData newInfo = new TupleMetaData(rules.GetName(), HumaneName, Date, Stream);

            if ((cr.CoordsFlag == format) && (cr.IsNative == native) && (cr.DataType == type) && (cr.Scale == rescale))
            {
                return Reinfo(newInfo);
            }


            if (cr.IsNative)
            {
                if (cr.Scale)
                {
                    if (cr.DataType != typeof(double))
                        throw new InvalidCastException();

                    return ScaleToNative(newInfo);
                }
                else
                {
                    return ConvertToNativeType(newInfo, cr.DataType);
                }
            }
            else
            {
                CoordsConverter coords;

                if ((!native) && (GetCoords().Flag == cr.CoordsFlag))
                {
                    coords = GetCoords();
                }
                else
                {
                    // TODO check meaning of info during conversion
                    coords = env.GetSpecialConverter(cr.CoordsFlag, newInfo);
                }


                if (cr.Scale)
                {
                    if (cr.DataType != typeof(double))
                        throw new InvalidCastException();

                    return ScaleToIndexed(newInfo, coords);
                }
                else
                {
                    return ConvertToIndexedType(newInfo, coords, cr.DataType);
                }
            }
        }

        public DataGrid ExportDataGridCoords(CoordsConverter fiber, CoordsConverter coordsEnumerator, IEnviromentEx env)
        {
            DataGrid d = new DataGrid();
            IInfoFormatter coordsFormatter = (env == null) ? 
                DataGrid.Column.GetDefaultFormatter(typeof(Coords)) : env.GetDefFormatter(FormatterType.Coords);

            d.Columns.Add(new DataGrid.Column("Координата", typeof(Coords), coordsFormatter));
            d.HeadColumns = 1;
            if (fiber != null)
            {
                if (fiber.Flag != CoordsConverter.SpecialFlag.PVK)
                    throw new ArgumentException();

                d.Columns.Add(new DataGrid.Column("Разводка", typeof(FiberCoords), env.GetDefFormatter(FormatterType.FiberCoords)));

                d.HeadColumns = d.HeadColumns + 1;
            }

            if (Layers == 1)
            {
                d.Columns.Add(new DataGrid.Column(HumaneName, ElementType, GetDefForamtter(env)));
            }
            else
            {
                for (int i = 0; i < Layers; i++)
                    d.Columns.Add(new DataGrid.Column(HumaneName + " " + i.ToString(), ElementType, GetDefForamtter(env)));
            }

            foreach (Coords c in coordsEnumerator)
            {
                DataGrid.Row r = d.Rows.Add();
                int i = 0;

                r[i++] = c;
                if (c.IsOk)
                {
                    if (fiber != null)
                        r[i++] = fiber.GetFiberCoords(c);

                    for (int j = 0; j < Layers; j++)
                        r[i++] = GetObject(c, j);
                }
            }
            return d;
        }


        public override DataGrid CreateDataGrid(IEnviromentEx env)
        {
            CoordsConverter pvk = (env == null) ? null : env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, Info);

            if (IsNative)
                return ExportDataGridCoords(pvk, CoordsConverter.sDummyConverter, env);
            else
                return ExportDataGridCoords(pvk, GetCoords(), env);
        }

        public void SerializeABIOld(ISerializeStream stream)
        {
            int streamId;

            if (Layers > 1)
            {
                if ((!IsNative) && (GetCoords().Flag == CoordsConverter.SpecialFlag.Named))
                {
                    streamId = KnownType.DataCartogramIdxMultiValue_StreamSerializerId;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                if (IsNative)
                {
                    streamId = KnownType.DataCartogramNative_StreamSerializerId;
                }
                else
                {
                    switch (GetCoords().Flag)
                    {
                        case CoordsConverter.SpecialFlag.Named:
                            streamId = KnownType.DataCartogramIndexed_StreamSerializerId; break;
                        case CoordsConverter.SpecialFlag.Linear1884:
                            streamId = KnownType.DataCartogramLinear_StreamSerializerId; break;
                        case CoordsConverter.SpecialFlag.WideLinear2448:
                            streamId = KnownType.DataCartogramLinearWide_StreamSerializerId; break;
                        case CoordsConverter.SpecialFlag.PVK:
                            streamId = KnownType.DataCartogramPvk_StreamSerializerId; break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            stream.Put(streamId);
            Array.Serialize(stream);
            stream.PutStruct(_scale);

            if ((streamId == KnownType.DataCartogramIdxMultiValue_StreamSerializerId) ||
                (streamId == KnownType.DataCartogramIndexed_StreamSerializerId))
            {
                GetCoords().SerializeABIOld(stream);
            }
        }

        public static DataCartogram CreateABIOld(IGetCoordsConverter env, IDeserializeStream stream)
        {
            int streamId;
            stream.Get(out streamId);

            if ((streamId != KnownType.DataCartogramLinear_StreamSerializerId) &&
                (streamId != KnownType.DataCartogramLinearWide_StreamSerializerId) &&
                (streamId != KnownType.DataCartogramNative_StreamSerializerId) &&
                (streamId != KnownType.DataCartogramIndexed_StreamSerializerId) &&
                (streamId != KnownType.DataCartogramIdxMultiValue_StreamSerializerId) &&
                (streamId != KnownType.DataCartogramPvk_StreamSerializerId))
            {
                throw new NotSupportedException();
            }

            DataArray main = DataArray.Create(stream);
            ScaleIndex scale = (ScaleIndex)stream.GetStruct(typeof(ScaleIndex));

            if (streamId == KnownType.DataCartogramNative_StreamSerializerId)
            {
                return CreateFromArray(stream.Info, scale, main);
            }
            else
            {
                CoordsConverter coords;
                switch (streamId)
                {
                    case KnownType.DataCartogramLinear_StreamSerializerId:
                        coords = env.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, stream.Info); break;
                    case KnownType.DataCartogramLinearWide_StreamSerializerId:
                        coords = env.GetSpecialConverter(CoordsConverter.SpecialFlag.WideLinear2448, stream.Info); break;
                    case KnownType.DataCartogramPvk_StreamSerializerId:
                        coords = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, stream.Info); break;
                    case KnownType.DataCartogramIndexed_StreamSerializerId:
                    case KnownType.DataCartogramIdxMultiValue_StreamSerializerId:
                        coords = CoordsConverter.CreateABIOld(stream); break;
                    default: throw new NotSupportedException();
                }

                return CreateFromArrayIndexed(stream.Info, scale, coords, main);
            }
        }

        static protected DataCartogram CreateFromArray(TupleMetaData info, ScaleIndex idx, DataArray main)
        {
            Type t = main.ElementType;
            if (t == typeof(double)) return new DataCartogramNativeDouble(info, idx, (DataArrayDouble)main);
            else if (t == typeof(float)) return new DataCartogramNativeFloat(info, idx, (DataArrayFloat)main);
            else if (t == typeof(int)) return new DataCartogramNativeInt(info, idx, (DataArrayInt)main);
            else if (t == typeof(short)) return new DataCartogramNativeShort(info, idx, (DataArrayShort)main);
            else if (t == typeof(byte)) return new DataCartogramNativeByte(info, idx, (DataArrayByte)main);
            else if (t == typeof(Sensored)) return new DataCartogramNativeSensored(info, idx, (DataArraySensored)main);
            else throw new NotSupportedException();
        }

        static protected DataCartogram CreateFromArrayIndexed(TupleMetaData info, ScaleIndex idx, CoordsConverter conv, DataArray main)
        {
            Type t = main.ElementType;
            if (t == typeof(double)) return new DataCartogramIndexedDouble(info, idx, conv, (DataArrayDouble)main);
            else if (t == typeof(float)) return new DataCartogramIndexedFloat(info, idx, conv, (DataArrayFloat)main);
            else if (t == typeof(int)) return new DataCartogramIndexedInt(info, idx, conv, (DataArrayInt)main);
            else if (t == typeof(short)) return new DataCartogramIndexedShort(info, idx, conv, (DataArrayShort)main);
            else if (t == typeof(byte)) return new DataCartogramIndexedByte(info, idx, conv, (DataArrayByte)main);
            else if (t == typeof(Sensored)) return new DataCartogramIndexedSensored(info, idx, conv, (DataArraySensored)main);
            else throw new NotSupportedException();
        }

        #region ITupleItem Members

        public override void Serialize(ISerializeStream stream)
        {
            SerializeABIOld(stream);
        }

        public override void Serialize(int abiver, ISerializeStream stream)
        {
            if (abiver == 0)
                SerializeABIOld(stream);
            else
                throw new NotSupportedException();
        }


        public override object CastTo(IGetCoordsConverter en, ITypeRules rules)
        {
            if (rules.GetTypeString() == "Cart")
                return ConvertTo(en, rules);
            else
                return ConvertToArray(en, rules);
        }

        #endregion

        static readonly corelib.AttributeTypeRules sDummyArrayRule = new corelib.AttributeTypeRules("a", "array");
        public static DataCartogram CastFrom(object a, IGetCoordsConverter en, ITypeRules rules)
        {
            if (rules.GetTypeString() != "Cart")
                return null;

            TupleMetaData info = new TupleMetaData(rules.GetName(), rules.GetHelpName(), DateTime.Now, TupleMetaData.StreamAuto);
            DataArray array = DataArray.CastFrom(a, sDummyArrayRule);
            string[] det = rules.GetCastDetails();
            if (det.Length > 0)
            {
                switch (det[0])
                {
                    case "native":
                        return CreateFromArray(info, ScaleIndex.Default, array);
                    case "pvk":
                        return CreateFromArrayIndexed(info, ScaleIndex.Default,
                            en.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, info), array);
                    case "linear":
                        return CreateFromArrayIndexed(info, ScaleIndex.Default,
                            en.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, info), array);
                    case "wlinear":
                        return CreateFromArrayIndexed(info, ScaleIndex.Default,
                            en.GetSpecialConverter(CoordsConverter.SpecialFlag.WideLinear2448, info), array);
                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                if ((array.DimX == 48) && (array.DimY == 48))
                    return CreateFromArray(info, ScaleIndex.Default, array);
                else if ((array.DimX == 16) && (array.DimY == 115))
                    return CreateFromArrayIndexed(info, ScaleIndex.Default,
                            en.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, info), array);
                else if (array.DimX == 1884)
                    return CreateFromArrayIndexed(info, ScaleIndex.Default,
                            en.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, info), array);
                else if (array.DimX == 2448)
                    return CreateFromArrayIndexed(info, ScaleIndex.Default,
                            en.GetSpecialConverter(CoordsConverter.SpecialFlag.WideLinear2448, info), array);
                else
                    throw new ArgumentException();
            }
        }

        public override IInfoFormatter GetDefForamtter(IGetDataFormatter env)
        {
            if (ElementType == typeof(Sensored))
            {
                IInfoFormatter formatter = (env == null) ?
                    DataGrid.Column.GetDefaultFormatter(typeof(double)) : env.GetDefFormatter(FormatterType.Real);

                return new DataCartogramFormatterSensored(_scale, formatter);
            }
            else
            {
                return new DataCartogramFormatter(_scale, Array.GetDefForamtter(env));
            }
        }


        private static CoordsConverter allNativeCoords = CoordsConverter.sDummyConverter;
        public CoordsConverter AllCoords
        {
            get
            {
                if (!IsNative)
                    return GetCoords();
                else
                    return allNativeCoords;
            }
        }

        public double GetScaledMaxMin(out double min, out double max)
        {
            max = min = 0;
            bool first = true;
            foreach (Coords crd in AllCoords)
            {
                if (crd.IsOk)
                {
                    for (int l = 0; l < Layers; l++)
                    {
                        double diff = this[crd, l];
                        if (first)
                        {
                            min = max = diff;
                            first = false;
                        }
                        else
                        {
                            min = Math.Min(min, diff);
                            max = Math.Max(max, diff);
                        }
                    }
                }
            }
            return Math.Max(Math.Abs(min), Math.Abs(max));
        }


        static double RelativeCalc(double v1, double v2)
        {
            if (v2 != 0)
                return (v1 - v2) / v2;
            else
                return Math.Sign(v1);
        }

        static double SubCalc(double v1, double v2)
        {
            return (v1 - v2);
        }
        static double AddCalc(double v1, double v2)
        {
            return (v1 + v2);
        }
        static double MulCalc(double v1, double v2)
        {
            return (v1 * v2);
        }
        static double DivCalc(double v1, double v2)
        {
            return (v1 / v2);
        }

		static readonly protected OperationCartogram sdRelativeCalc = new OperationCartogram(RelativeCalc);
        static readonly protected OperationCartogram sdSubCalc = new OperationCartogram(SubCalc);
        static readonly protected OperationCartogram sdAddCalc = new OperationCartogram(AddCalc);
        static readonly protected OperationCartogram sdMulCalc = new OperationCartogram(MulCalc);
        static readonly protected OperationCartogram sdDivCalc = new OperationCartogram(DivCalc);

        public static DataCartogram RelativeDiff(DataCartogram result, DataCartogram source)
        {
            return CreateResulting(
                new TupleMetaData("releative_diff", String.Format("Относительная разница {0}", result.GetHumanName()), DateTime.Now, TupleMetaData.StreamAuto),
                result,
                sdRelativeCalc,
                source);
        }

        public static DataCartogram AbsoluteDiff(DataCartogram result, DataCartogram source)
        {
            return CreateResulting(
                new TupleMetaData("absolute_diff", String.Format("Абосютная разница {0}", result.GetHumanName()), DateTime.Now, TupleMetaData.StreamAuto),
                result,
                sdSubCalc,
                source);
        }

        public IDataCartogram RelativeDiff(IDataCartogram source)
        {
            return DataCartogram.RelativeDiff(this, (DataCartogram)source);
        }

        public IDataCartogram AbsoluteDiff(IDataCartogram source)
        {
            return DataCartogram.AbsoluteDiff(this, (DataCartogram)source);
        }


        struct OperationGenerator
        {
            int layers;
            CoordsConverter resulting;

            DataCartogram c1;
            DataCartogram c2;
            OperationCartogram op;
            double cv;

            public OperationGenerator(DataCartogram oc1, OperationCartogram oop, DataCartogram oc2)
            {
                if (oc1.Layers != oc2.Layers)
                    throw new ArgumentException();

                layers = oc1.Layers;
                resulting = CoordsConverter.Intersect(oc1.AllCoords, oc2.AllCoords);
                c1 = oc1;
                op = oop;
                c2 = oc2;
                cv = 0;
            }

            public OperationGenerator(DataCartogram oc1, OperationCartogram oop, double oc2)
            {
                layers = oc1.Layers;
                resulting = oc1.AllCoords;
                c1 = oc1;
                op = oop;
                c2 = null;
                cv = oc2;
            }

            public CoordsConverter Resulting
            {
                get
                {
                    return resulting;
                }
            }

            public double Generate(int x)
            {
                Coords c = resulting[x / layers];
                //if (c1.IsValidCoord(c) && c2.IsValidCoord(c))
                if (c.IsOk)
                    return op(c1[c, x % layers], c2[c, x % layers]);
                return 0.0;
            }

            public double GenerateD(int x)
            {
                Coords c = resulting[x / layers];
                //if (c1.IsValidCoord(c) && c2.IsValidCoord(c))
                if (c.IsOk)
                    return op(c1[c, x % layers], cv);
                return 0.0;
            }
        }

        public static DataCartogram CreateResulting(TupleMetaData info, DataCartogram c1, OperationCartogram op, DataCartogram c2)
        {
            OperationGenerator oper = new OperationGenerator(c1, op, c2);
            DataArrayDouble array;

            if (oper.Resulting == null)
                return null;
            
            if (c1.IsNative && c2.IsNative)
            {
                array = new DataArrayDouble(info,  new GeneratorDouble(oper.Generate), 48, 48, (c1.Layers > 1 ) ? c1.Layers : 0);
                return new DataCartogramNativeDouble(info, ScaleIndex.Default, array);
            }
            else
            {
                int q = c1.Layers > 1 ? c1.Layers : 0;

                if (oper.Resulting.Rank == 1)
                    array = new DataArrayDouble(info, new GeneratorDouble(oper.Generate), oper.Resulting.DimX, q, 0);
                else
                    array = new DataArrayDouble(info, new GeneratorDouble(oper.Generate), oper.Resulting.DimX, oper.Resulting.DimY, q);

                return new DataCartogramIndexedDouble(info, ScaleIndex.Default, oper.Resulting, array);
            }
        }

        public static DataCartogram CreateResulting(TupleMetaData info, DataCartogram c1, OperationCartogram op, double c2)
        {
            OperationGenerator oper = new OperationGenerator(c1, op, c2);
            DataArrayDouble array;

            if (oper.Resulting == null)
                return null;

            if (c1.IsNative)
            {
                array = new DataArrayDouble(info, new GeneratorDouble(oper.GenerateD), 48, 48, (c1.Layers > 1) ? c1.Layers : 0);
                return new DataCartogramNativeDouble(info, ScaleIndex.Default, array);
            }
            else
            {
                int q = c1.Layers > 1 ? c1.Layers : 0;

                if (oper.Resulting.Rank == 1)
                    array = new DataArrayDouble(info, new GeneratorDouble(oper.GenerateD), oper.Resulting.DimX, q, 0);
                else
                    array = new DataArrayDouble(info, new GeneratorDouble(oper.GenerateD), oper.Resulting.DimX, oper.Resulting.DimY, q);

                return new DataCartogramIndexedDouble(info, ScaleIndex.Default, oper.Resulting, array);
            }
        }


        public static DataCartogram operator +(DataCartogram a1, DataCartogram a2)
        {
            return CreateResulting(
                new TupleMetaData("add_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdAddCalc, a2);
        }

        public static DataCartogram operator -(DataCartogram a1, DataCartogram a2)
        {
            return CreateResulting(
                new TupleMetaData("sub_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdSubCalc, a2);
        }
        
        public static DataCartogram operator /(DataCartogram a1, DataCartogram a2)
        {
            return CreateResulting(
                new TupleMetaData("div_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdDivCalc, a2);
        }

        public static DataCartogram operator *(DataCartogram a1, DataCartogram a2)
        {
            return CreateResulting(
                new TupleMetaData("mul_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdMulCalc, a2);
        }

        public static DataCartogram operator *(DataCartogram a1, double a2)
        {
            return CreateResulting(
                new TupleMetaData("smul_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdMulCalc, a2);
        }

        public static DataCartogram operator /(DataCartogram a1, double a2)
        {
            return CreateResulting(
                new TupleMetaData("sdiv_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdDivCalc, a2);
        }

        public static DataCartogram operator -(DataCartogram a1, double a2)
        {
            return CreateResulting(
                new TupleMetaData("ssub_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdSubCalc, a2);
        }

        public static DataCartogram operator +(DataCartogram a1, double a2)
        {
            return CreateResulting(
                new TupleMetaData("sadd_" + a1.Name, a1.HumaneName, a1.Date, TupleMetaData.StreamAuto),
                a1, sdAddCalc, a2);
        }

        public static DataCartogram CreateFromParts(TupleMetaData newInfo, CoordsConverter coords, IMultiTupleItem arrays)
        {
#if !DOTNET_V11
            // Работает только в правильном порядке переданнном массиве
             
            Type t = arrays[0].GetType();
            Array orArr = System.Array.CreateInstance(t, arrays.Count);
            for (int i = 0; i < arrays.Count; i++)
                orArr.SetValue(arrays[i], i);

            t = ((DataArray)arrays[0]).ElementType;
            if (t == typeof(double)) return DataCartogramIndexedDouble.CreateFromParts(newInfo, coords, (DataArrayDouble[])orArr);
            else if (t == typeof(float)) return DataCartogramIndexedFloat.CreateFromParts(newInfo, coords, (DataArrayFloat[])orArr);
            else if (t == typeof(int)) return DataCartogramIndexedInt.CreateFromParts(newInfo, coords, (DataArrayInt[])orArr);
            else if (t == typeof(short)) return DataCartogramIndexedShort.CreateFromParts(newInfo, coords, (DataArrayShort[])orArr);
            else if (t == typeof(byte)) return DataCartogramIndexedByte.CreateFromParts(newInfo, coords, (DataArrayByte[])orArr);
            else if (t == typeof(Sensored)) return DataCartogramIndexedSensored.CreateFromParts(newInfo, coords, (DataArraySensored[])orArr);
            else throw new NotSupportedException();
#else
            throw new NotSupportedException("Необходимо одновить ф-цию для .NET 1.1 конфигурации");
#endif
        }
    }

    

#if !DOTNET_V11

    public abstract class DataCartogram<T> : DataCartogram where T : struct
    {
        protected DataArray<T> _main;
        protected readonly Operator<double> opScale;

        static Converter<T, double> sToDoubleOrig;


        protected DataCartogram(TupleMetaData info, ScaleIndex scale, DataArray<T> array, bool multiLayer)
            : base(info, scale, CalculateLayersCount(array, multiLayer))
        {
            _main = array;
            opScale = (Operator<double>)OperationScale;
        }

        protected DataCartogram(TupleMetaData info, ScaleIndex scale, T[] array)
            : this(info, scale, new DataArray<T>(info, array), false)
        {
        }

        protected DataCartogram(TupleMetaData info, ScaleIndex scale, T[,] array, bool mlayers)
            : this(info, scale, new DataArray<T>(info, array), mlayers)
        {
        }

        protected DataCartogram(TupleMetaData info, ScaleIndex scale, T[, ,] array)
            : this(info, scale, new DataArray<T>(info, array), true)
        {
        }




        protected static Converter<T, double> GetConverterDouble()
        {
            if (sToDoubleOrig == null)
                sToDoubleOrig = DataConverter.GetConverter<T, double>();
            return sToDoubleOrig;
        }        

        protected double OperationScale(double a, double v)
        {
            return _scale.Scale(a);
        }

        public abstract T GetItem(Coords c, int layer);

        protected override DataArray Array
        {
            get { return _main; }
        }

        public override object GetObject(Coords c, int layer)
        {
            return GetItem(c, layer);
        }

        public override double GetValue(Coords c, int layer)
        {
            return _scale.Scale(GetConverterDouble()(GetItem(c, layer)));
        }

        public new DataArray<T> GetDataArray()
        {
            return _main;
        }


        public abstract DataCartogramIndexedDouble ScaleIndexed(TupleMetaData info, CoordsConverter coords, double defValue);
        public abstract DataCartogramNativeDouble ScaleNative(TupleMetaData info, double defValue);

        public abstract DataArray<T> ConvertToPartArray(TupleMetaData info, CoordsConverter coords, int idx, T defValue);

        public abstract DataCartogramIndexed<U> ConvertToIndexed<U>(TupleMetaData info, CoordsConverter coords, U defValue) where U : struct;

        public abstract DataCartogramNative<U> ConvertToNative<U>(TupleMetaData info, U defValue) where U : struct;


        public DataArray<T>[] ConvertToPartArray(TupleMetaData info, CoordsConverter coords, T defValue)
        {
            DataArray<T>[] array = new DataArray<T>[coords.DimX];
            for (int i = 0; i < coords.DimX; i++)
                array[i] = ConvertToPartArray(info, coords, i, defValue);
            return array;
        }

        public override DataArray[] ConvertToPartArray(TupleMetaData info, CoordsConverter coords)
        {
            return ConvertToPartArray(info, coords, new T());
        }        

        public override DataCartogramNativeDouble ScaleToNative(TupleMetaData newInfo)
        {
            return ScaleNative(newInfo, 0);
        }

        public override DataCartogramIndexedDouble ScaleToIndexed(TupleMetaData newInfo, CoordsConverter coords)
        {
            return ScaleIndexed(newInfo, coords, 0);
        }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        {
            if (t == typeof(double)) return ConvertToNative<double>(newInfo, 0);
            else if (t == typeof(float)) return ConvertToNative<float>(newInfo, 0);
            else if (t == typeof(int)) return ConvertToNative<int>(newInfo, 0);
            else if (t == typeof(short)) return ConvertToNative<short>(newInfo, 0);
            else if (t == typeof(byte)) return ConvertToNative<byte>(newInfo, 0);
            else if (t == typeof(Sensored)) return ConvertToNative<Sensored>(newInfo,
                new Sensored(0, Sensored.DataState.SensorAbsence));
            else throw new NotSupportedException();
        }

        public override DataCartogram ConvertToIndexedType(TupleMetaData newInfo, CoordsConverter coords, Type t)
        {
            if (t == typeof(double)) return ConvertToIndexed<double>(newInfo, coords, 0);
            else if (t == typeof(float)) return ConvertToIndexed<float>(newInfo, coords, 0);
            else if (t == typeof(int)) return ConvertToIndexed<int>(newInfo, coords, 0);
            else if (t == typeof(short)) return ConvertToIndexed<short>(newInfo, coords, 0);
            else if (t == typeof(byte)) return ConvertToIndexed<byte>(newInfo, coords, 0);
            else if (t == typeof(Sensored)) return ConvertToIndexed<Sensored>(newInfo, coords,
                new Sensored(0, Sensored.DataState.SensorAbsence));
            else throw new NotSupportedException();
        }
    }


    public class DataCartogramNative<T> : DataCartogram<T> where T : struct
    {
        public const int Xmax = 48;
        public const int Ymax = 48;

        void ChechCompatability()
        {
            if ((_main.DimX != Xmax) || (_main.DimY != Ymax))
                throw new ArgumentException("Массив неподходящего размера");
        }

        public DataCartogramNative(TupleMetaData info, ScaleIndex scale, T[,] array)
            : base(info, scale, array, false)
        {
            ChechCompatability();
        }

        public DataCartogramNative(TupleMetaData info, ScaleIndex scale, T[, ,] array)
            : base(info, scale, array)
        {
            ChechCompatability();
        }

        public DataCartogramNative(TupleMetaData info, DataCartogramNative<T> cart)
            : base(info, cart._scale, cart._main, cart._layers > 1)
        {
        }

        public DataCartogramNative(TupleMetaData info, ScaleIndex scale, Generator<T> gen)
            : base(info, scale, new DataArray<T>(info, gen, Xmax, Ymax, 0), false)
        {
        }

        public DataCartogramNative(TupleMetaData info, ScaleIndex scale, Generator<T> gen, int layer)
            : base(info, scale, new DataArray<T>(info, gen, Xmax, Ymax, layer > 1 ? layer : 0), layer > 1)
        {
        }

        public DataCartogramNative(TupleMetaData info, ScaleIndex scale, Generator1<T> gen, int layer)
            : base(info, scale, new DataArray<T>(info, gen, Xmax, Ymax, layer > 1 ? layer : 0), layer > 1)
        {
        }


        struct DummyLinearConverter
        {
            public T[] array;

            public T Get(int x)
            {
                return array[x];
            }
        };

        static public DataCartogramNative<T> FromLinear(TupleMetaData info, ScaleIndex scale, T[] array)
        {
            DummyLinearConverter tmp;
            tmp.array = array;
            return new DataCartogramNative<T>(info, scale, tmp.Get, 0);
        }



        public DataCartogramNative(TupleMetaData info, ScaleIndex scale, DataArray<T> cart)
            : base(info, scale, cart, cart.DimZ > 1)
        {
            ChechCompatability();
        }

        public override bool IsValidCoord(Coords c)
        {
            return c.IsOk;
        }

        public override T GetItem(Coords c, int layer)
        {
            if (_layers > 1)
            {
                return _main[c.Y, c.X, layer];
            }
            else
            {
                return _main[c.Y, c.X];
            }
        }

        public override DataCartogram Reinfo(TupleMetaData info)
        {
            return new DataCartogramNative<T>(info, this);
        }

        public DataCartogramNative<double> Scale(TupleMetaData info)
        {
            return new DataCartogramNative<double>(info, ScaleIndex.Default,
                DataArray<double>.Operation<T>(info, _main, opScale, 0.0));
        }


        struct ConvertSeparator
        {
            DataCartogramNative<T> _orig;
            T _defValue;
            int _idx;

            CoordsConverter toCoords;

            public ConvertSeparator(DataCartogramNative<T> orig, CoordsConverter tocoords, int idx, T defValue)
            {
                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
                _idx = idx;
            }

            public T GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[_idx, x];
                if (c.IsOk)
                    return _orig.GetItem(c, y);
                else
                    return _defValue;
            }
        }
        struct ConverterTo<U> where U : struct
        {
            DataCartogramNative<T> _orig;
            CoordsConverter toCoords;
            Converter<T, U> conv;
            U _defValue;

            public ConverterTo(DataCartogramNative<T> orig, CoordsConverter tocoords, U defValue)
            {
                conv = DataConverter.GetConverter<T, U>();
                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
            }

            public U GeneratorCross1(int x, int y, int z)
            {
                Coords c = toCoords[x];
                if (c.IsOk)
                    return conv(_orig.GetItem(c, y));
                else
                    return _defValue;
            }

            public U GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                if (c.IsOk)
                    return conv(_orig.GetItem(c, z));
                else
                    return _defValue;
            }
        }


        struct ConverterToDouble
        {
            DataCartogramNative<T> _orig;
            Converter<T, double> conv;
            double _defValue;

            CoordsConverter toCoords;

            public ConverterToDouble(DataCartogramNative<T> orig, CoordsConverter tocoords, double defValue)
            {
                conv = GetConverterDouble();

                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
            }

            public double Generator(int index)
            {
                return _orig._scale.Scale(conv(_orig._main.GetLinear(index)));
            }

            public double GeneratorCross1(int x, int y, int z)
            {
                Coords c = toCoords[x];
                if (c.IsOk)
                    return _orig._scale.Scale(conv(_orig.GetItem(c, y)));
                else
                    return _defValue;
            }

            public double GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                if (c.IsOk)
                    return _orig._scale.Scale(conv(_orig.GetItem(c, z)));
                else
                    return _defValue;
            }

        }

        public override DataCartogramIndexed<U> ConvertToIndexed<U>(TupleMetaData info, CoordsConverter coords, U defValue)
        {
            ConverterTo<U> converter = new DataCartogramNative<T>.ConverterTo<U>(this, coords, defValue);
            switch (coords.Rank)
            {
                case 1: return new DataCartogramIndexed<U>(info, _scale, coords, converter.GeneratorCross1, Layers);
                case 2: return new DataCartogramIndexed<U>(info, _scale, coords, converter.GeneratorCross2, Layers);
                default: throw new NotSupportedException();
            }
        }

        public override DataCartogramNative<U> ConvertToNative<U>(TupleMetaData info, U defValue)
        {
            return new DataCartogramNative<U>(info, _scale, _main.ConvertTo<U>(info));
        }


        public override DataCartogramIndexed<double> ScaleIndexed(TupleMetaData info, CoordsConverter coords, double defValue)
        {
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, coords, defValue);
            switch (coords.Rank)
            {
                case 1: return new DataCartogramIndexed<double>(info, ScaleIndex.Default, coords, converter.GeneratorCross1, Layers);
                case 2: return new DataCartogramIndexed<double>(info, ScaleIndex.Default, coords, converter.GeneratorCross2, Layers);
                default: throw new NotSupportedException();
            }
        }


        public override DataCartogramNative<double> ScaleNative(TupleMetaData info, double defValue)
        {
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, null, defValue);
            return new DataCartogramNative<double>(info, ScaleIndex.Default, converter.Generator, Layers);
        }


        public override double[,] ScaleNativeArray(double defValue)
        {
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, null, defValue);
            return DataArray<double>.CreateArray(converter.Generator, Xmax, Ymax);
        }

        public override double[, ,] ScaleNativeArrayLayer(double defValue)
        {
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, null, defValue);
            return DataArray<double>.CreateArray(converter.Generator, Xmax, Ymax, Layers);
        }

        public override DataArray<T> ConvertToPartArray(TupleMetaData info, CoordsConverter coords, int idx, T defValue)
        {
            ConvertSeparator sp = new ConvertSeparator(this, coords, idx, defValue);
            return new DataArray<T>(info, sp.GeneratorCross2, coords.DimY, Layers > 1 ? Layers : 0, 0);
        }
        public override double[] ScaleToIndexed(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross1, coords.DimX);
        }
        public override double[,] ScaleToIndexedLayer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 3) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross1, coords.DimX, Layers);
        }

        public override double[,] ScaleToIndexed2(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross2, coords.DimX, coords.DimY );
        }
        public override double[, ,] ScaleToIndexed2Layer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 3) || (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNative<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross2, coords.DimX, coords.DimY, Layers);
        }


        public override bool IsNative
        {
            get { return true; }
        }

        protected override CoordsConverter GetCoords()
        {
            throw new NotSupportedException();
        }
    }


    public class DataCartogramIndexed<T> : DataCartogram<T>, IEnumerable<T>, IDataCartogramIndexed where T : struct
    {
        protected readonly CoordsConverter _converter;

        public DataCartogramIndexed(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, T[] array)
            : base(info, scale, array)
        {
            _converter = conv;
            ChechCompatabilityConverter(_converter);
        }

        public DataCartogramIndexed(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, T[,] array)
            : base(info, scale, array, conv.DimY == 0)
        {
            _converter = conv;
            ChechCompatabilityConverter(_converter);
        }

        public DataCartogramIndexed(TupleMetaData info, DataCartogramIndexed<T> cart)
            : this(info, cart._scale, cart._converter, cart._main)
        {
        }

        public DataCartogramIndexed(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, Generator<T> gen)
            : this(info, scale, conv, new DataArray<T>(info, gen, conv.DimX, conv.DimY, 0))
        {
        }

        public DataCartogramIndexed(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, Generator<T> gen, int layer)
            : this(info, scale, conv, new DataArray<T>(info, gen, conv.DimX, (conv.Rank == 2) ? conv.DimY : (layer > 1 ? layer : 0), (conv.Rank == 2) ? (layer > 1 ? layer : 0) : 0))
        {
        }

        public DataCartogramIndexed(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, Generator1<T> gen, int layer)
            : this(info, scale, conv, new DataArray<T>(info, gen, conv.DimX, (conv.Rank == 2) ? conv.DimY : (layer > 1 ? layer : 0), (conv.Rank == 2) ? (layer > 1 ? layer : 0) : 0))
        {
        }

        public DataCartogramIndexed(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArray<T> cart)
            : base(info, scale, cart, cart.Rank > conv.Rank)
        {
            _converter = conv;
            ChechCompatabilityConverter(_converter);
        }

        struct DummyLinearConverter
        {
            public T[] array;

            public T Get(int x)
            {
                return array[x];
            }
        };

        static public DataCartogramIndexed<T> FromLinear(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, T[] array)
        {
            DummyLinearConverter tmp;
            tmp.array = array;
            return new DataCartogramIndexed<T>(info, scale, conv, tmp.Get, 0);
        }

        public override bool IsValidCoord(Coords c)
        {
            return (_converter[c] != CoordsConverter.InvalidIndex);                
        }

        public CoordsConverter Coords
        {
            get
            {
                return _converter;
            }
        }

        new public AnyValue GetAnyValue(int idx, int layer)
        {
            return base.GetAnyValue(idx, layer);
        }

        public double GetValue(int index, int layer)
        {
            return _scale.Scale(GetConverterDouble()(GetItem(index, layer)));
        }

        public double this[int index, int layer]
        {
            get { return GetValue(index, layer); }
        }

        public object GetObject(int index, int layer)
        {
            return GetItem(index, layer);
        }

        public T GetItem(int index, int layer)
        {
            return _main.GetLinear(index * _layers + layer);
        }

        public override T GetItem(Coords c, int layer)
        {
            return GetItem(_converter[c], layer);
        }

        public override DataCartogram Reinfo(TupleMetaData info)
        {
            return new DataCartogramIndexed<T>(info, this);
        }

        public DataCartogramIndexed<double> Scale(TupleMetaData info, double defValue)
        {
            return new DataCartogramIndexed<double>(info, ScaleIndex.Default, _converter,
                DataArray<double>.Operation<T>(info, _main, opScale, defValue));
        }

        public DataCartogramIndexed<double> Scale(TupleMetaData info)
        {
            return Scale(info, 0);
        }

        struct ConvertSeparator
        {
            DataCartogramIndexed<T> _orig;
            T _defValue;
            int _idx;

            CoordsConverter toCoords;

            public ConvertSeparator(DataCartogramIndexed<T> orig, CoordsConverter tocoords, int idx, T defValue)
            {
                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
                _idx = idx;
            }

            public T GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[_idx, x];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig.GetItem(c, y);
                else
                    return _defValue;
            }
        }

        struct ConverterTo<U> where U : struct
        {
            DataCartogramIndexed<T> _orig;
            Converter<T, U> conv;
            U _defValue;

            CoordsConverter toCoords;

            public ConverterTo(DataCartogramIndexed<T> orig, CoordsConverter tocoords, U defValue)
            {
                conv = DataConverter.GetConverter<T, U>();
                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
            }

            public U Generator(int x, int y, int z)
            {
                Coords c = new Coords((byte)x, (byte)y);
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return conv(_orig.GetItem(c, z));
                else
                    return _defValue;
            }

            public U GeneratorCross1(int x, int y, int z)
            {
                Coords c = toCoords[x];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return conv(_orig.GetItem(c, y));
                else
                    return _defValue;
            }

            public U GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return conv(_orig.GetItem(c, z));
                else
                    return _defValue;
            }
        }

        struct ConverterToDouble
        {
            DataCartogramIndexed<T> _orig;
            Converter<T, double> conv;
            double _defValue;

            CoordsConverter toCoords;

            public ConverterToDouble(DataCartogramIndexed<T> orig, CoordsConverter tocoords, double defValue)
            {
                conv = GetConverterDouble();

                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
            }

            public double Generator(int x, int y, int z)
            {
                Coords c = new Coords((byte)x, (byte)y);
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig._scale.Scale(conv(_orig.GetItem(c, z)));
                else
                    return _defValue;
            }

            public double GeneratorCross1(int x, int y, int z)
            {
                Coords c = toCoords[x];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig._scale.Scale(conv(_orig.GetItem(c, y)));
                else
                    return _defValue;
            }

            public double GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig._scale.Scale(conv(_orig.GetItem(c, z)));
                else
                    return _defValue;
            }

            public double GeneratorPassIndexed(int index)
            {
                return _orig._scale.Scale(conv(_orig._main.GetLinear(index)));
            }
        }

        public override DataArray<T> ConvertToPartArray(TupleMetaData info, CoordsConverter coords, int idx, T defValue)
        {
            ConvertSeparator sp = new ConvertSeparator(this, Coords, idx, defValue);
            return new DataArray<T>(info, sp.GeneratorCross2, coords.DimY, Layers > 1 ? Layers : 0, 0);
        }

        public DataCartogramIndexed<U> ConvertToIndexed<U>(TupleMetaData info, U defValue) where U : struct
        {
            return new DataCartogramIndexed<U>(info, _scale, _converter, _main.ConvertTo<U>(info));
        }

        public override DataCartogramIndexed<U> ConvertToIndexed<U>(TupleMetaData info, CoordsConverter coords, U defValue)
        {
            if (coords == _converter)
                return ConvertToIndexed<U>(info, defValue);

            ConverterTo<U> converter = new DataCartogramIndexed<T>.ConverterTo<U>(this, coords, defValue);
            switch (coords.Rank)
            {
                case 1: return new DataCartogramIndexed<U>(info, _scale, coords, converter.GeneratorCross1, Layers);
                case 2: return new DataCartogramIndexed<U>(info, _scale, coords, converter.GeneratorCross2, Layers);
                default: throw new NotSupportedException();
            }
        }

        public override DataCartogramNative<U> ConvertToNative<U>(TupleMetaData info, U defValue)
        {
            ConverterTo<U> converter = new DataCartogramIndexed<T>.ConverterTo<U>(this, null, defValue);
            return new DataCartogramNative<U>(info, _scale, converter.Generator, Layers);
        }



        public override DataCartogramIndexed<double> ScaleIndexed(TupleMetaData info, CoordsConverter coords, double defValue)
        {
            // TODO use Scale() for matching
            if (coords == _converter)
                return ScaleIndexed(info, defValue);

            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, coords, defValue);
            switch (coords.Rank)
            {
                case 1: return new DataCartogramIndexed<double>(info, ScaleIndex.Default, coords, converter.GeneratorCross1, Layers);
                case 2: return new DataCartogramIndexed<double>(info, ScaleIndex.Default, coords, converter.GeneratorCross2, Layers);
                default: throw new NotSupportedException();
            }
        }

        public DataCartogramIndexed<double> ScaleIndexed(TupleMetaData info, double defValue)
        {
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, null, defValue);
            return new DataCartogramIndexed<double>(info, ScaleIndex.Default, _converter, converter.GeneratorPassIndexed, Layers);
        }

        public override DataCartogramNative<double> ScaleNative(TupleMetaData info, double defValue)
        {
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, null, defValue);
            return new DataCartogramNative<double>(info, ScaleIndex.Default, converter.Generator, Layers);
        }

        public override double[] ScaleToIndexed(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 1) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross1, coords.DimX);
        }
        public override double[,] ScaleToIndexedLayer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross1, coords.DimX, Layers);
        }

        public override double[,] ScaleToIndexed2(CoordsConverter coords, double defValue)
        {
            if (/*(_main.Rank != 1) ||*/ (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross2, coords.DimX, coords.DimY);
        }
        public override double[, ,] ScaleToIndexed2Layer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, coords, defValue);
            return DataArray<double>.CreateArray(converter.GeneratorCross2, coords.DimX, coords.DimY, Layers);
        }

        public override double[,] ScaleNativeArray(double defValue)
        {
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, null, defValue);
            return DataArray<double>.CreateArray(converter.Generator, DataCartogramNative<T>.Xmax, DataCartogramNative<T>.Ymax);
        }

        public override double[, ,] ScaleNativeArrayLayer(double defValue)
        {
            ConverterToDouble converter = new DataCartogramIndexed<T>.ConverterToDouble(this, null, defValue);
            return DataArray<double>.CreateArray(converter.Generator, DataCartogramNative<T>.Xmax, DataCartogramNative<T>.Ymax, Layers);
        }

        public override bool IsNative
        {
            get { return false; }
        }

        protected override CoordsConverter GetCoords()
        {
            return _converter;
        }

        DataArray<T>.Enumerator GetEnumerator()
        {
            return _main.GetEnumerator();
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        struct CombinerToSingle
        {
            DataArray<T>[] _parts;
            CoordsConverter toCoords;

            public CombinerToSingle(DataArray<T>[] parts, CoordsConverter tocoords)
            {
                _parts = parts;
                toCoords = tocoords;
            }

            public T Generator1(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                return _parts[x][y];
            }

            public T Generator2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                return _parts[x][y, z];
            }
        }

        // TODO сделать обработку неполных массивов c null
        public static DataCartogramIndexed<T> CreateFromParts(TupleMetaData info, CoordsConverter coords, DataArray<T>[] parts)
        {

            if ((coords.DimX != parts.Length) || (coords.DimY != parts[0].DimX))
                throw new ArgumentException("Неверный вызов при преобразовании");

            CombinerToSingle converter = new DataCartogramIndexed<T>.CombinerToSingle(parts, coords);

            if (parts[0].DimY > 0)
                return new DataCartogramIndexed<T>(info, ScaleIndex.Default, coords, converter.Generator2, parts[0].DimY);
            else
                return new DataCartogramIndexed<T>(info, ScaleIndex.Default, coords, converter.Generator1, 1);
        }
    }


#else

    public abstract class DataCartogramAbstract : DataCartogram
    {
        protected DataArrayAbstract _main;

        protected DataCartogramAbstract(TupleMetaData info, ScaleIndex scale, DataArrayAbstract array, bool multiLayer)
            : base(info, scale, CalculateLayersCount(array, multiLayer))
        {
            _main = array;
            //opScale = (Operator<double>)OperationScale;
        }

        protected override DataArray Array
        {
            get { return _main; }
        }


        public abstract DataCartogramIndexedDouble ScaleIndexed(TupleMetaData info, CoordsConverter coords, double defValue);
        public abstract DataCartogramNativeDouble ScaleNative(TupleMetaData info, double defValue);


        public override DataCartogramNativeDouble ScaleToNative(TupleMetaData newInfo)
        {
            return ScaleNative(newInfo, 0);
        }

        public override DataCartogramIndexedDouble ScaleToIndexed(TupleMetaData newInfo, CoordsConverter coords)
        {
            return ScaleIndexed(newInfo, coords, 0);
        }


        static protected DataCartogramIndexedAbstract Create(TupleMetaData newInfo, ScaleIndex scale, CoordsConverter converter, DataArray a)
        {
            Type t = a.ElementType;

            if (t == typeof(int))
                return new DataCartogramIndexedInt(newInfo, scale, converter, (DataArrayInt)a);
            else if (t == typeof(short))
                return new DataCartogramIndexedShort(newInfo, scale, converter, (DataArrayShort)a);
            else if (t == typeof(byte))
                return new DataCartogramIndexedByte(newInfo, scale, converter, (DataArrayByte)a);
            else if (t == typeof(double))
                return new DataCartogramIndexedDouble(newInfo, scale, converter, (DataArrayDouble)a);
            else if (t == typeof(float))
                return new DataCartogramIndexedFloat(newInfo, scale, converter, (DataArrayFloat)a);
            else if (t == typeof(Sensored))
                return new DataCartogramIndexedSensored(newInfo, scale, converter, (DataArraySensored)a);
            else
                throw new NotSupportedException();
        }

        static protected DataCartogramNativeAbstract Create(TupleMetaData newInfo, ScaleIndex scale, DataArray a)
        {
            Type t = a.ElementType;

            if (t == typeof(int))
                return new DataCartogramNativeInt(newInfo, scale, (DataArrayInt)a);
            else if (t == typeof(short))
                return new DataCartogramNativeShort(newInfo, scale, (DataArrayShort)a);
            else if (t == typeof(byte))
                return new DataCartogramNativeByte(newInfo, scale, (DataArrayByte)a);
            else if (t == typeof(double))
                return new DataCartogramNativeDouble(newInfo, scale, (DataArrayDouble)a);
            else if (t == typeof(float))
                return new DataCartogramNativeFloat(newInfo, scale, (DataArrayFloat)a);
            else if (t == typeof(Sensored))
                return new DataCartogramNativeSensored(newInfo, scale, (DataArraySensored)a);
            else
                throw new NotSupportedException();
        }




        public abstract DataArray ConvertToPartArray(TupleMetaData info, CoordsConverter coords, int idx);

        
        public override DataArray[] ConvertToPartArray(TupleMetaData info, CoordsConverter coords)
        {
            DataArray[] array = new DataArray[coords.DimX];
            for (int i = 0; i < coords.DimX; i++)
                array[i] = ConvertToPartArray(info, coords, i);
            return array;
        }
    }

    public abstract class DataCartogramNativeAbstract : DataCartogramAbstract
    {
        public const int Xmax = 48;
        public const int Ymax = 48;

        void ChechCompatability()
        {
            if ((_main.DimX != Xmax) || (_main.DimY != Ymax))
                throw new ArgumentException("Массиз неподходящего размера");
        }


        public DataCartogramNativeAbstract(TupleMetaData info, ScaleIndex scale, DataArrayAbstract cart)
            : base(info, scale, cart, cart.DimZ > 1)
        {
            ChechCompatability();
        }


        public override object GetObject(Coords c, int layer)
        {
            if (_layers > 1)
                return _main[c.Y, c.X, layer];
            else
                return _main[c.Y, c.X];
        }

        public override bool IsValidCoord(Coords c)
        {
            return c.IsOk;
        }

        public override DataCartogramIndexedDouble ScaleIndexed(TupleMetaData info, CoordsConverter coords, double defValue)
        {
            if (Layers > 1)
            {
                if (coords.DimY > 0)
                {
                    double[, ,] o = new double[coords.DimX, coords.DimY, Layers];
                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];

                        if (c.IsOk)
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i / sep, i % sep, l] = GetValue(c, l);
                        }
                        else
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i / sep, i % sep, l] = defValue;
                        }
                    }

                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
                else
                {
                    double[,] o = new double[coords.DimX, Layers];

                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];

                        if (c.IsOk)
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i, l] = GetValue(c, l);
                        }
                        else
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i, l] = defValue;
                        }
                    }
                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
            }
            else
            {
                if (coords.DimY > 0)
                {
                    double[,] o = new double[coords.DimX, coords.DimY];

                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        if (c.IsOk)
                            o[i / sep, i % sep] = GetValue(c, 0);
                        else
                            o[i / sep, i % sep] = defValue;
                    }
                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
                else
                {
                    double[] o = new double[coords.DimX];
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        if (c.IsOk)
                            o[i] = GetValue(c, 0);
                        else
                            o[i] = defValue;

                    }
                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
            }
        }

        public override DataCartogramNativeDouble ScaleNative(TupleMetaData info, double defValue)
        {
            if (Layers > 1)
            {
                double[, ,] o = new double[Xmax, Ymax, Layers];
                for (int x = 0; x < Xmax; x++)
                    for (int y = 0; y < Ymax; y++)
                    {
                        Coords c = new Coords((byte)y, (byte)x);
                        for (int l = 0; l < Layers; l++)
                        {
                            o[y, x, l] = GetValue(c, l);
                        }
                    }

                return new DataCartogramNativeDouble(info, ScaleIndex.Default, o);
            }
            else
            {
                double[,] o = new double[Xmax, Ymax];
                for (int x = 0; x < Xmax; x++)
                    for (int y = 0; y < Ymax; y++)
                    {
                        Coords c = new Coords((byte)y, (byte)x);
                        o[y, x] = GetValue(c, 0);
                    }

                return new DataCartogramNativeDouble(info, ScaleIndex.Default, o);
            }
        }

        protected override CoordsConverter GetCoords()
        {
            throw new NotSupportedException();
        }

        public override bool IsNative
        {
            get { return true; }
        }

        public override DataCartogram ConvertToIndexedType(TupleMetaData info, CoordsConverter coords, Type t)
        {
            DataCartogramNativeAbstract tarray = Create(info, ScaleIndex.Default, _main.ConvertTo(info, t));

            Array na;
            if (Layers > 1)
            {
                if (coords.DimY > 0)
                {
                    na = System.Array.CreateInstance(t, coords.DimX, coords.DimY, Layers);

                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        if (c.IsOk)
                        {
                            for (int l = 0; l < Layers; l++)
                                na.SetValue(tarray.GetObject(c, l), i / sep, i % sep, l);
                        }
                    }
                }
                else
                {
                    na = System.Array.CreateInstance(t, coords.DimX, Layers);

                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        if (c.IsOk)
                        {
                            for (int l = 0; l < Layers; l++)
                                na.SetValue(tarray.GetObject(c, l), i, l);
                        }
                    }
                }
            }
            else
            {
                if (coords.DimY > 0)
                {
                    na = System.Array.CreateInstance(t, coords.DimX, coords.DimY);

                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        if (c.IsOk)
                            na.SetValue(tarray.GetObject(c, 0), i / sep, i % sep);
                    }
                }
                else
                {
                    na = System.Array.CreateInstance(t, coords.DimX);

                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        if (c.IsOk)
                            na.SetValue(tarray.GetObject(c, 0), i);
                    }
                }
            }

            return Create(info, ScaleIndex.Default, coords, DataArrayAbstract.Create(info, na));
        }




        struct ConverterToDouble
        {
            DataCartogramNativeAbstract _orig;
            double _defValue;

            CoordsConverter toCoords;

            public ConverterToDouble(DataCartogramNativeAbstract orig, CoordsConverter tocoords, double defValue)
            {
                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
            }

            public double Generator(int index)
            {
                return _orig._scale.Scale((_orig._main.GetAnyValueLinear(index).ToDouble()));
            }

            public double GeneratorCross1(int x, int y, int z)
            {
                Coords c = toCoords[x];
                if (c.IsOk)
                    return _orig._scale.Scale((_orig.GetAnyValue(c, y).ToDouble()));
                else
                    return _defValue;
            }

            public double GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                if (c.IsOk)
                    return _orig._scale.Scale((_orig.GetAnyValue(c, z).ToDouble()));
                else
                    return _defValue;
            }

        }

        public override double[,] ScaleNativeArray(double defValue)
        {
            ConverterToDouble converter = new DataCartogramNativeAbstract.ConverterToDouble(this, null, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble(converter.Generator), Xmax, Ymax);
        }

        public override double[, ,] ScaleNativeArrayLayer(double defValue)
        {
            ConverterToDouble converter = new DataCartogramNativeAbstract.ConverterToDouble(this, null, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble(converter.Generator), Xmax, Ymax, Layers);
        }

        public override double[] ScaleToIndexed(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNativeAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross1), coords.DimX);
        }
        public override double[,] ScaleToIndexedLayer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 3) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNativeAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross1), coords.DimX, Layers);
        }

        public override double[,] ScaleToIndexed2(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNativeAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross2), coords.DimX, coords.DimY);
        }
        public override double[, ,] ScaleToIndexed2Layer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 3) || (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramNativeAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross2), coords.DimX, coords.DimY, Layers);
        }


        struct ConvertSeparator
        {
            DataCartogramNativeAbstract _orig;
            int _idx;

            CoordsConverter toCoords;

            public ConvertSeparator(DataCartogramNativeAbstract orig, CoordsConverter tocoords, int idx)
            {
                _orig = orig;
                toCoords = tocoords;
                _idx = idx;
            }

            public double GeneratorCross2Double(int x, int y, int z)
            {
                Coords c = toCoords[_idx, x];
                if (c.IsOk)
                    return _orig.GetAnyValue(c, y).ToDouble();
                else
                    return 0;
            }
        }

        
        public override DataArray ConvertToPartArray(TupleMetaData info, CoordsConverter coords, int idx)
        {
            ConvertSeparator sp = new ConvertSeparator(this, coords, idx);

            if (ElementType == typeof(double))
            {
                return new DataArrayDouble(info, new GeneratorDouble3(sp.GeneratorCross2Double), coords.DimY, Layers > 1 ? Layers : 0, 0);
            }

            throw new NotSupportedException();
        }
        
    }


    public abstract class DataCartogramIndexedAbstract : DataCartogramAbstract, IDataCartogramIndexed
    {
        protected readonly int _subseparator;
        protected readonly CoordsConverter _converter;       


        public DataCartogramIndexedAbstract(TupleMetaData info, DataCartogramIndexedAbstract cart)
            : this(info, cart._scale, cart._converter, cart._main)
        {
        }

        public DataCartogramIndexedAbstract(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArrayAbstract cart)
            : base(info, scale, cart, cart.Rank > conv.Rank)
        {
            _converter = conv;

            _subseparator = conv.DimY;

            ChechCompatabilityConverter(_converter);
        }

        new public AnyValue GetAnyValue(int idx, int layer)
        {
            return base.GetAnyValue(idx, layer);
        }

        public CoordsConverter Coords
        {
            get
            {
                return _converter;
            }
        }

        public object GetObject(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _main[index / _subseparator, index % _subseparator, layer];
                else
                    return _main[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _main[index / _subseparator, index % _subseparator];
                else
                    return _main[index];
            }
        }

        public override object GetObject(Coords c, int layer)
        {
            return GetObject(_converter[c], layer);        
        }

        public override double GetValue(Coords c, int layer)
        {
            return GetValue(_converter[c], layer);        
        }

        public double this[int index, int layer]
        {
            get { return GetValue(index, layer); }
        }

        public abstract double GetValue(int index, int layer);

        public DataCartogramIndexedDouble Scale(TupleMetaData info)
        {
            return ScaleIndexed(info, _converter, 0.0);
        }

        public override bool IsValidCoord(Coords c)
        {
            return (_converter[c] != CoordsConverter.InvalidIndex);
        }

        public override DataCartogramIndexedDouble ScaleIndexed(TupleMetaData info, CoordsConverter coords, double defValue)
        {
            if (Layers > 1)
            {
                if (coords.DimY > 0)
                {
                    double[, ,] o = new double[coords.DimX, coords.DimY, Layers];
                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i / sep, i % sep, l] = GetValue(idx, l);
                        }
                        else
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i / sep, i % sep, l] = defValue;
                        }
                    }

                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
                else
                {
                    double[,] o = new double[coords.DimX, Layers];

                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i, l] = GetValue(idx, l);
                        }
                        else
                        {
                            for (int l = 0; l < Layers; l++)
                                o[i, l] = defValue;
                        }
                    }
                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
            }
            else
            {
                if (coords.DimY > 0)
                {
                    double[,] o = new double[coords.DimX, coords.DimY];

                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                        {
                            o[i / sep, i % sep] = GetValue(idx, 0);
                        }
                        else
                        {
                            o[i / sep, i % sep] = defValue;
                        }
                    }
                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
                else
                {
                    double[] o = new double[coords.DimX];
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                        {
                            o[i] = GetValue(idx, 0);
                        }
                        else
                        {
                            o[i] = defValue;
                        }
                    }
                    return new DataCartogramIndexedDouble(info, ScaleIndex.Default, coords, o);
                }
            }
        }

        public override DataCartogramNativeDouble ScaleNative(TupleMetaData info, double defValue)
        {
            if (Layers > 1)
            {
                double[, ,] o = new double[DataCartogramNativeAbstract.Xmax, DataCartogramNativeAbstract.Ymax, Layers];
                for (int x = 0; x < DataCartogramNativeAbstract.Xmax; x++)
                    for (int y = 0; y < DataCartogramNativeAbstract.Ymax; y++)
                    {
                        Coords c = new Coords((byte)y, (byte)x);
                        int idx = _converter[c];
                        if (idx != CoordsConverter.InvalidIndex)
                            for (int l = 0; l < Layers; l++)
                            {
                                o[y, x, l] = GetValue(idx, l);
                            }
                        else
                        {
                            for (int l = 0; l < Layers; l++)
                            {
                                o[y, x, l] = defValue;
                            }
                        }
                    }

                return new DataCartogramNativeDouble(info, ScaleIndex.Default, o);
            }
            else
            {
                double[,] o = new double[DataCartogramNativeAbstract.Xmax, DataCartogramNativeAbstract.Ymax];
                for (int x = 0; x < DataCartogramNativeAbstract.Xmax; x++)
                    for (int y = 0; y < DataCartogramNativeAbstract.Ymax; y++)
                    {
                        Coords c = new Coords((byte)y, (byte)x);
                        int idx = _converter[c];
                        if (idx != CoordsConverter.InvalidIndex)
                            o[y, x] = GetValue(idx, 0);
                        else
                            o[y, x] = defValue;
                    }

                return new DataCartogramNativeDouble(info, ScaleIndex.Default, o);
            }
        }

        protected override CoordsConverter GetCoords()
        {
            return _converter;
        }

        public override bool IsNative
        {
            get { return false; }
        }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        {
            DataCartogramIndexedAbstract tarray = Create(newInfo, ScaleIndex.Default, _converter, _main.ConvertTo(newInfo, t));

            if (Layers > 1)
            {
                Array na = System.Array.CreateInstance(t, DataCartogramNativeAbstract.Xmax, DataCartogramNativeAbstract.Ymax, Layers);
                for (int x = 0; x < DataCartogramNativeAbstract.Xmax; x++)
                    for (int y = 0; y < DataCartogramNativeAbstract.Ymax; y++)
                    {
                        Coords c = new Coords((byte)y, (byte)x);
                        int idx = _converter[c];
                        if (idx != CoordsConverter.InvalidIndex)
                            for (int l = 0; l < Layers; l++)
                            {
                                na.SetValue(tarray.GetObject(idx, l), x, y, l);
                            }
                    }

                return Create(newInfo, _scale, DataArrayAbstract.Create(newInfo, na));
            }
            else
            {
                Array na = System.Array.CreateInstance(t, DataCartogramNativeAbstract.Xmax, DataCartogramNativeAbstract.Ymax, Layers);
                for (int x = 0; x < DataCartogramNativeAbstract.Xmax; x++)
                    for (int y = 0; y < DataCartogramNativeAbstract.Ymax; y++)
                    {
                        Coords c = new Coords((byte)y, (byte)x);
                        int idx = _converter[c];
                        if (idx != CoordsConverter.InvalidIndex)
                            na.SetValue(tarray.GetObject(idx, 0), x, y);
                    }
                return Create(newInfo, _scale, DataArrayAbstract.Create(newInfo, na));
            }
        }

        public override DataCartogram ConvertToIndexedType(TupleMetaData info, CoordsConverter coords, Type t)
        {
            if (_converter == coords)
            {
                if (t == ElementType)
                    return Reinfo(info);
                else
                    return Create(info, _scale, coords, _main.ConvertTo(info, t));
            }

            DataCartogramIndexedAbstract tarray = Create(info, ScaleIndex.Default, _converter, _main.ConvertTo(info, t));

            Array na;
            if (Layers > 1)
            {
                if (coords.DimY > 0)
                {
                    na = System.Array.CreateInstance(t, coords.DimX, coords.DimY, Layers);

                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                        {
                            for (int l = 0; l < Layers; l++)
                                na.SetValue(tarray.GetObject(idx, l), i / sep, i % sep, l);
                        }
                    }
                }
                else
                {
                    na = System.Array.CreateInstance(t, coords.DimX, Layers);

                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                        {
                            for (int l = 0; l < Layers; l++)
                                na.SetValue(tarray.GetObject(idx, l), i, l);
                        }
                    }
                }
            }
            else
            {
                if (coords.DimY > 0)
                {
                    na = System.Array.CreateInstance(t, coords.DimX, coords.DimY);

                    int sep = coords.DimX;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                            na.SetValue(tarray.GetObject(idx, 0), i / sep, i % sep);
                    }
                }
                else
                {
                    na = System.Array.CreateInstance(t, coords.DimX);

                    for (int i = 0; i < coords.Length; i++)
                    {
                        Coords c = coords[i];
                        int idx = _converter[c];
                        if (c.IsOk && idx != CoordsConverter.InvalidIndex)
                            na.SetValue(tarray.GetObject(idx, 0), i);
                    }

                }
            }

            return Create(info, ScaleIndex.Default, coords, DataArrayAbstract.Create(info, na));
        }



        struct ConverterToDouble
        {
            DataCartogramIndexedAbstract _orig;
            double _defValue;

            CoordsConverter toCoords;

            public ConverterToDouble(DataCartogramIndexedAbstract orig, CoordsConverter tocoords, double defValue)
            {
                _orig = orig;
                _defValue = defValue;
                toCoords = tocoords;
            }

            public double Generator(int x, int y, int z)
            {
                Coords c = new Coords((byte)x, (byte)y);
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig._scale.Scale((_orig.GetAnyValue(c, z)).ToDouble());
                else
                    return _defValue;
            }

            public double GeneratorCross1(int x, int y, int z)
            {
                Coords c = toCoords[x];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig._scale.Scale((_orig.GetAnyValue(c, y)).ToDouble());
                else
                    return _defValue;
            }

            public double GeneratorCross2(int x, int y, int z)
            {
                Coords c = toCoords[x, y];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig._scale.Scale((_orig.GetAnyValue(c, z)).ToDouble());
                else
                    return _defValue;
            }

            public double GeneratorPassIndexed(int index)
            {
                return _orig._scale.Scale((_orig._main.GetAnyValueLinear(index).ToDouble()));
            }
        }

        public override double[] ScaleToIndexed(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 1) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexedAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross1), coords.DimX);
        }
        public override double[,] ScaleToIndexedLayer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 1))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexedAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross1), coords.DimX, Layers);
        }

        public override double[,] ScaleToIndexed2(CoordsConverter coords, double defValue)
        {
            if (/*(_main.Rank != 1) ||*/ (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexedAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross2), coords.DimX, coords.DimY);
        }
        public override double[, ,] ScaleToIndexed2Layer(CoordsConverter coords, double defValue)
        {
            if ((_main.Rank != 2) || (coords.Rank != 2))
                throw new ArgumentException("Неверный вызов при преобразовании");
            ConverterToDouble converter = new DataCartogramIndexedAbstract.ConverterToDouble(this, coords, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.GeneratorCross2), coords.DimX, coords.DimY, Layers);
        }

        public override double[,] ScaleNativeArray(double defValue)
        {
            ConverterToDouble converter = new DataCartogramIndexedAbstract.ConverterToDouble(this, null, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.Generator), DataCartogramNativeAbstract.Xmax, DataCartogramNativeAbstract.Ymax);
        }

        public override double[, ,] ScaleNativeArrayLayer(double defValue)
        {
            ConverterToDouble converter = new DataCartogramIndexedAbstract.ConverterToDouble(this, null, defValue);
            return DataArrayDouble.CreateArray(new GeneratorDouble3(converter.Generator), DataCartogramNativeAbstract.Xmax, DataCartogramNativeAbstract.Ymax, Layers);
        }


        struct ConvertSeparator
        {
            DataCartogramIndexedAbstract _orig;
            int _idx;

            CoordsConverter toCoords;

            public ConvertSeparator(DataCartogramIndexedAbstract orig, CoordsConverter tocoords, int idx)
            {
                _orig = orig;
                toCoords = tocoords;
                _idx = idx;
            }

            public double GeneratorCross2double(int x, int y, int z)
            {
                Coords c = toCoords[_idx, x];
                int idx = _orig._converter[c];

                if (idx != CoordsConverter.InvalidIndex)
                    return _orig.GetAnyValue(c, y).ToDouble();
                else
                    return 0;
            }
        }

        public override DataArray ConvertToPartArray(TupleMetaData info, CoordsConverter coords, int idx)
        {
            ConvertSeparator sp = new ConvertSeparator(this, Coords, idx);
            if (ElementType == typeof(double))
                return new DataArrayDouble(info, new GeneratorDouble3(sp.GeneratorCross2double), coords.DimY, Layers > 1 ? Layers : 0, 0);

            throw new NotImplementedException();
        }

    }

#endif

}
