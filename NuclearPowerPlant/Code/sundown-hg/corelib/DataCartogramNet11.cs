#if DOTNET_V11
using System;
using System.Collections;
using System.Text;

namespace corelib
{

    public class DataCartogramIndexedDouble : DataCartogramIndexedAbstract
    {
        DataArrayDouble _maint;


        public DataCartogramIndexedDouble(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, double[] array)
            : this(info, scale, conv, new DataArrayDouble(info, array)) { }

        public DataCartogramIndexedDouble(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, double[,] array)
            : this(info, scale, conv, new DataArrayDouble(info, array)) { }

        public DataCartogramIndexedDouble(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, double[, ,] array)
            : this(info, scale, conv, new DataArrayDouble(info, array)) { }

        public DataCartogramIndexedDouble(TupleMetaData info, DataCartogramIndexedDouble cart)
            : this(info, cart._scale, cart._converter, cart._maint) { }

        public DataCartogramIndexedDouble(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArrayDouble cart)
            : base(info, scale, conv, cart) { _maint = cart; }

        public double GetItem(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator, layer];
                else
                    return _maint[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator];
                else
                    return _maint[index];
            }
        }

        public override double GetValue(int index, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(index, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramIndexedDouble(info, this); }

        public new DataArrayDouble GetDataArray() { return _maint; }
    }

    public class DataCartogramIndexedFloat : DataCartogramIndexedAbstract
    {
        DataArrayFloat _maint;


        public DataCartogramIndexedFloat(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, float[] array)
            : this(info, scale, conv, new DataArrayFloat(info, array)) { }

        public DataCartogramIndexedFloat(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, float[,] array)
            : this(info, scale, conv, new DataArrayFloat(info, array)) { }

        public DataCartogramIndexedFloat(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, float[, ,] array)
            : this(info, scale, conv, new DataArrayFloat(info, array)) { }

        public DataCartogramIndexedFloat(TupleMetaData info, DataCartogramIndexedFloat cart)
            : this(info, cart._scale, cart._converter, cart._maint) { }

        public DataCartogramIndexedFloat(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArrayFloat cart)
            : base(info, scale, conv, cart) { _maint = cart; }

        public float GetItem(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator, layer];
                else
                    return _maint[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator];
                else
                    return _maint[index];
            }
        }

        public override double GetValue(int index, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(index, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramIndexedFloat(info, this); }

        public new DataArrayFloat GetDataArray() { return _maint; }
    }

    public class DataCartogramIndexedInt : DataCartogramIndexedAbstract
    {
        DataArrayInt _maint;


        public DataCartogramIndexedInt(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, int[] array)
            : this(info, scale, conv, new DataArrayInt(info, array)) { }

        public DataCartogramIndexedInt(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, int[,] array)
            : this(info, scale, conv, new DataArrayInt(info, array)) { }

        public DataCartogramIndexedInt(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, int[, ,] array)
            : this(info, scale, conv, new DataArrayInt(info, array)) { }

        public DataCartogramIndexedInt(TupleMetaData info, DataCartogramIndexedInt cart)
            : this(info, cart._scale, cart._converter, cart._maint) { }

        public DataCartogramIndexedInt(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArrayInt cart)
            : base(info, scale, conv, cart) { _maint = cart; }

        public int GetItem(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator, layer];
                else
                    return _maint[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator];
                else
                    return _maint[index];
            }
        }

        public override double GetValue(int index, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(index, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramIndexedInt(info, this); }

        public new DataArrayInt GetDataArray() { return _maint; }
    }

    public class DataCartogramIndexedShort : DataCartogramIndexedAbstract
    {
        DataArrayShort _maint;


        public DataCartogramIndexedShort(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, short[] array)
            : this(info, scale, conv, new DataArrayShort(info, array)) { }

        public DataCartogramIndexedShort(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, short[,] array)
            : this(info, scale, conv, new DataArrayShort(info, array)) { }

        public DataCartogramIndexedShort(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, short[, ,] array)
            : this(info, scale, conv, new DataArrayShort(info, array)) { }

        public DataCartogramIndexedShort(TupleMetaData info, DataCartogramIndexedShort cart)
            : this(info, cart._scale, cart._converter, cart._maint) { }

        public DataCartogramIndexedShort(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArrayShort cart)
            : base(info, scale, conv, cart) { _maint = cart; }

        public short GetItem(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator, layer];
                else
                    return _maint[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator];
                else
                    return _maint[index];
            }
        }

        public override double GetValue(int index, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(index, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramIndexedShort(info, this); }

        public new DataArrayShort GetDataArray() { return _maint; }
    }

    public class DataCartogramIndexedByte : DataCartogramIndexedAbstract
    {
        DataArrayByte _maint;


        public DataCartogramIndexedByte(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, byte[] array)
            : this(info, scale, conv, new DataArrayByte(info, array)) { }

        public DataCartogramIndexedByte(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, byte[,] array)
            : this(info, scale, conv, new DataArrayByte(info, array)) { }

        public DataCartogramIndexedByte(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, byte[, ,] array)
            : this(info, scale, conv, new DataArrayByte(info, array)) { }

        public DataCartogramIndexedByte(TupleMetaData info, DataCartogramIndexedByte cart)
            : this(info, cart._scale, cart._converter, cart._maint) { }

        public DataCartogramIndexedByte(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArrayByte cart)
            : base(info, scale, conv, cart) { _maint = cart; }

        public byte GetItem(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator, layer];
                else
                    return _maint[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator];
                else
                    return _maint[index];
            }
        }

        public override double GetValue(int index, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(index, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramIndexedByte(info, this); }

        public new DataArrayByte GetDataArray() { return _maint; }
    }

    public class DataCartogramIndexedSensored : DataCartogramIndexedAbstract
    {
        DataArraySensored _maint;


        public DataCartogramIndexedSensored(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, Sensored[] array)
            : this(info, scale, conv, new DataArraySensored(info, array)) { }

        public DataCartogramIndexedSensored(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, Sensored[,] array)
            : this(info, scale, conv, new DataArraySensored(info, array)) { }

        public DataCartogramIndexedSensored(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, Sensored[, ,] array)
            : this(info, scale, conv, new DataArraySensored(info, array)) { }

        public DataCartogramIndexedSensored(TupleMetaData info, DataCartogramIndexedSensored cart)
            : this(info, cart._scale, cart._converter, cart._maint) { }

        public DataCartogramIndexedSensored(TupleMetaData info, ScaleIndex scale, CoordsConverter conv, DataArraySensored cart)
            : base(info, scale, conv, cart) { _maint = cart; }

        public Sensored GetItem(int index, int layer)
        {
            if (_layers > 1)
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator, layer];
                else
                    return _maint[index, layer];
            }
            else
            {
                if (_subseparator > 0)
                    return _maint[index / _subseparator, index % _subseparator];
                else
                    return _maint[index];
            }
        }

        public override double GetValue(int index, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(index, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramIndexedSensored(info, this); }

        public new DataArraySensored GetDataArray() { return _maint; }
    }
















    public class DataCartogramNativeShort : DataCartogramNativeAbstract
    {
        DataArrayShort _maint;

        public DataCartogramNativeShort(TupleMetaData info, ScaleIndex scale, DataArrayShort cart)
            : base(info, scale, cart) { _maint = cart; }

        public DataCartogramNativeShort(TupleMetaData info, ScaleIndex scale, short[,] array)
            : this(info, scale, new DataArrayShort(info, array)) { }

        public DataCartogramNativeShort(TupleMetaData info, ScaleIndex scale, short[, ,] array)
            : this(info, scale, new DataArrayShort(info, array)) { }

        public DataCartogramNativeShort(TupleMetaData info, DataCartogramNativeShort cart)
            : this(info, cart._scale, cart._maint) { }

        public short GetItem(Coords c, int layer)
        {
            if (_layers > 1)
                return _maint[c.Y, c.X, layer];
            else
                return _maint[c.Y, c.X];
        }

        public override double GetValue(Coords c, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(c, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramNativeShort(info, this); }

        public new DataArrayShort GetDataArray() { return _maint; }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        { return new DataCartogramNativeShort(newInfo, _scale, (DataArrayShort)_main.ConvertTo(newInfo, t)); }

    }

    public class DataCartogramNativeDouble : DataCartogramNativeAbstract
    {
        DataArrayDouble _maint;

        public DataCartogramNativeDouble(TupleMetaData info, ScaleIndex scale, DataArrayDouble cart)
            : base(info, scale, cart) { _maint = cart; }

        public DataCartogramNativeDouble(TupleMetaData info, ScaleIndex scale, Double[,] array)
            : this(info, scale, new DataArrayDouble(info, array)) { }

        public DataCartogramNativeDouble(TupleMetaData info, ScaleIndex scale, Double[, ,] array)
            : this(info, scale, new DataArrayDouble(info, array)) { }

        public DataCartogramNativeDouble(TupleMetaData info, DataCartogramNativeDouble cart)
            : this(info, cart._scale, cart._maint) { }

        public Double GetItem(Coords c, int layer)
        {
            if (_layers > 1)
                return _maint[c.Y, c.X, layer];
            else
                return _maint[c.Y, c.X];
        }

        public override double GetValue(Coords c, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(c, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramNativeDouble(info, this); }

        public new DataArrayDouble GetDataArray() { return _maint; }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        { return new DataCartogramNativeDouble(newInfo, _scale, (DataArrayDouble)_main.ConvertTo(newInfo, t)); }
    }


    public class DataCartogramNativeFloat : DataCartogramNativeAbstract
    {
        DataArrayFloat _maint;

        public DataCartogramNativeFloat(TupleMetaData info, ScaleIndex scale, DataArrayFloat cart)
            : base(info, scale, cart) { _maint = cart; }

        public DataCartogramNativeFloat(TupleMetaData info, ScaleIndex scale, float[,] array)
            : this(info, scale, new DataArrayFloat(info, array)) { }

        public DataCartogramNativeFloat(TupleMetaData info, ScaleIndex scale, float[, ,] array)
            : this(info, scale, new DataArrayFloat(info, array)) { }

        public DataCartogramNativeFloat(TupleMetaData info, DataCartogramNativeFloat cart)
            : this(info, cart._scale, cart._maint) { }

        public float GetItem(Coords c, int layer)
        {
            if (_layers > 1)
                return _maint[c.Y, c.X, layer];
            else
                return _maint[c.Y, c.X];
        }

        public override double GetValue(Coords c, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(c, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramNativeFloat(info, this); }

        public new DataArrayFloat GetDataArray() { return _maint; }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        { return new DataCartogramNativeFloat(newInfo, _scale, (DataArrayFloat)_main.ConvertTo(newInfo, t)); }
    }

    public class DataCartogramNativeByte : DataCartogramNativeAbstract
    {
        DataArrayByte _maint;

        public DataCartogramNativeByte(TupleMetaData info, ScaleIndex scale, DataArrayByte cart)
            : base(info, scale, cart) { _maint = cart; }

        public DataCartogramNativeByte(TupleMetaData info, ScaleIndex scale, byte[,] array)
            : this(info, scale, new DataArrayByte(info, array)) { }

        public DataCartogramNativeByte(TupleMetaData info, ScaleIndex scale, byte[, ,] array)
            : this(info, scale, new DataArrayByte(info, array)) { }

        public DataCartogramNativeByte(TupleMetaData info, DataCartogramNativeByte cart)
            : this(info, cart._scale, cart._maint) { }

        public byte GetItem(Coords c, int layer)
        {
            if (_layers > 1)
                return _maint[c.Y, c.X, layer];
            else
                return _maint[c.Y, c.X];
        }

        public override double GetValue(Coords c, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(c, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramNativeByte(info, this); }

        public new DataArrayByte GetDataArray() { return _maint; }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        { return new DataCartogramNativeByte(newInfo, _scale, (DataArrayByte)_main.ConvertTo(newInfo, t)); }
    }


    public class DataCartogramNativeInt : DataCartogramNativeAbstract
    {
        DataArrayInt _maint;

        public DataCartogramNativeInt(TupleMetaData info, ScaleIndex scale, DataArrayInt cart)
            : base(info, scale, cart) { _maint = cart; }

        public DataCartogramNativeInt(TupleMetaData info, ScaleIndex scale, int[,] array)
            : this(info, scale, new DataArrayInt(info, array)) { }

        public DataCartogramNativeInt(TupleMetaData info, ScaleIndex scale, int[, ,] array)
            : this(info, scale, new DataArrayInt(info, array)) { }

        public DataCartogramNativeInt(TupleMetaData info, DataCartogramNativeInt cart)
            : this(info, cart._scale, cart._maint) { }

        public int GetItem(Coords c, int layer)
        {
            if (_layers > 1)
                return _maint[c.Y, c.X, layer];
            else
                return _maint[c.Y, c.X];
        }

        public override double GetValue(Coords c, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(c, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramNativeInt(info, this); }

        public new DataArrayInt GetDataArray() { return _maint; }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        { return new DataCartogramNativeInt(newInfo, _scale, (DataArrayInt)_main.ConvertTo(newInfo, t)); }
    }

    public class DataCartogramNativeSensored : DataCartogramNativeAbstract
    {
        DataArraySensored _maint;

        public DataCartogramNativeSensored(TupleMetaData info, ScaleIndex scale, DataArraySensored cart)
            : base(info, scale, cart) { _maint = cart; }

        public DataCartogramNativeSensored(TupleMetaData info, ScaleIndex scale, Sensored[,] array)
            : this(info, scale, new DataArraySensored(info, array)) { }

        public DataCartogramNativeSensored(TupleMetaData info, ScaleIndex scale, Sensored[, ,] array)
            : this(info, scale, new DataArraySensored(info, array)) { }

        public DataCartogramNativeSensored(TupleMetaData info, DataCartogramNativeSensored cart)
            : this(info, cart._scale, cart._maint) { }

        public Sensored GetItem(Coords c, int layer)
        {
            if (_layers > 1)
                return _maint[c.Y, c.X, layer];
            else
                return _maint[c.Y, c.X];
        }

        public override double GetValue(Coords c, int layer) { return _scale.Scale(Convert.ToDouble(GetItem(c, layer))); }

        public override DataCartogram Reinfo(TupleMetaData info) { return new DataCartogramNativeSensored(info, this); }

        public new DataArraySensored GetDataArray() { return _maint; }

        public override DataCartogram ConvertToNativeType(TupleMetaData newInfo, Type t)
        { return new DataCartogramNativeSensored(newInfo, _scale, (DataArraySensored)_main.ConvertTo(newInfo, t)); }
    }

}

#endif