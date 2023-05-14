#if DOTNET_V11
using System;
using System.Collections;
using System.Text;


namespace corelib
{
    public delegate double GeneratorDouble(int idx);
    public delegate double GeneratorDouble3(int x, int y, int z);

    #region Helpers
    public class DataArrayInt : DataArrayAbstract
    {
        private int[] _array1;
        private int[,] _array2;
        private int[, ,] _array3;

        void Init()
        {
            _array1 = _array as int[];
            _array2 = _array as int[,];
            _array3 = _array as int[, ,];
        }

        internal DataArrayInt(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(int)) { Init(); }

        public DataArrayInt(TupleMetaData newInfo, DataArrayInt data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayInt(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(int), dimx, dimy, dimz)) { Init(); }

        public DataArrayInt(TupleMetaData info, int[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayInt(TupleMetaData info, int[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayInt(TupleMetaData info, int[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public int this[int index] { get { return _array1[index]; } }
        new public int this[int x, int y] { get { return _array2[x, y]; } }
        new public int this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public int GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out int[] r) { r = (int[])_array.Clone(); }
        public void ToArray(out int[,] r) { r = (int[,])_array.Clone(); }
        public void ToArray(out int[, ,] r) { r = (int[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayInt(info, this); }
    }

    public class DataArrayShort : DataArrayAbstract
    {
        private short[] _array1;
        private short[,] _array2;
        private short[, ,] _array3;

        void Init()
        {
            _array1 = _array as short[];
            _array2 = _array as short[,];
            _array3 = _array as short[, ,];
        }

        internal DataArrayShort(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(short)) { Init(); }

        public DataArrayShort(TupleMetaData newInfo, DataArrayShort data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayShort(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(short), dimx, dimy, dimz)) { Init(); }

        public DataArrayShort(TupleMetaData info, short[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayShort(TupleMetaData info, short[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayShort(TupleMetaData info, short[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public short this[int index] { get { return _array1[index]; } }
        new public short this[int x, int y] { get { return _array2[x, y]; } }
        new public short this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public short GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out short[] r) { r = (short[])_array.Clone(); }
        public void ToArray(out short[,] r) { r = (short[,])_array.Clone(); }
        public void ToArray(out short[, ,] r) { r = (short[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayShort(info, this); }
    }

    public class DataArrayByte : DataArrayAbstract
    {
        private byte[] _array1;
        private byte[,] _array2;
        private byte[, ,] _array3;

        void Init()
        {
            _array1 = _array as byte[];
            _array2 = _array as byte[,];
            _array3 = _array as byte[, ,];
        }

        internal DataArrayByte(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(byte)) { Init(); }

        public DataArrayByte(TupleMetaData newInfo, DataArrayByte data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayByte(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(byte), dimx, dimy, dimz)) { Init(); }

        public DataArrayByte(TupleMetaData info, byte[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayByte(TupleMetaData info, byte[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayByte(TupleMetaData info, byte[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public byte this[int index] { get { return _array1[index]; } }
        new public byte this[int x, int y] { get { return _array2[x, y]; } }
        new public byte this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public byte GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out byte[] r) { r = (byte[])_array.Clone(); }
        public void ToArray(out byte[,] r) { r = (byte[,])_array.Clone(); }
        public void ToArray(out byte[, ,] r) { r = (byte[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayByte(info, this); }
    }

    public class DataArrayFloat : DataArrayAbstract
    {
        private float[] _array1;
        private float[,] _array2;
        private float[, ,] _array3;

        void Init()
        {
            _array1 = _array as float[];
            _array2 = _array as float[,];
            _array3 = _array as float[, ,];
        }

        internal DataArrayFloat(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(float)) { Init(); }

        public DataArrayFloat(TupleMetaData newInfo, DataArrayFloat data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayFloat(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(float), dimx, dimy, dimz)) { Init(); }

        public DataArrayFloat(TupleMetaData info, float[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayFloat(TupleMetaData info, float[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayFloat(TupleMetaData info, float[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public float this[int index] { get { return _array1[index]; } }
        new public float this[int x, int y] { get { return _array2[x, y]; } }
        new public float this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public float GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out float[] r) { r = (float[])_array.Clone(); }
        public void ToArray(out float[,] r) { r = (float[,])_array.Clone(); }
        public void ToArray(out float[, ,] r) { r = (float[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayFloat(info, this); }
    }

    public delegate double OperatorDouble(double v1, double v2);

    public class DataArrayDouble : DataArrayAbstract
    {
        private double[] _array1;
        private double[,] _array2;
        private double[, ,] _array3;

        void Init()
        {
            _array1 = _array as double[];
            _array2 = _array as double[,];
            _array3 = _array as double[, ,];
        }

        public DataArrayDouble(TupleMetaData info, GeneratorDouble gen, int dimx, int dimy, int dimz)
            : this(info, dimx, dimy, dimz)
        {
            if (Rank == 1)
            {
                for (int k = 0; k < DimX; k++)
                    _array1[k] = gen(k);
            }
            else if (Rank == 2)
            {
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        _array2[k, j] = gen(ToLinear(k, j));
            }
            else if (Rank == 3)
            {
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            _array3[k, j, i] = gen(ToLinear(k, j, i));
            }
        }

        public DataArrayDouble(TupleMetaData info, GeneratorDouble3 gen, int dimx, int dimy, int dimz)
            : this(info, dimx, dimy, dimz)
        {
            if (Rank == 1)
            {
                for (int k = 0; k < DimX; k++)
                    _array1[k] = gen(k,0,0);
            }
            else if (Rank == 2)
            {
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        _array2[k, j] = gen(k,j,0);
            }
            else if (Rank == 3)
            {
                for (int k = 0; k < DimX; k++)
                    for (int j = 0; j < DimY; j++)
                        for (int i = 0; i < DimZ; i++)
                            _array3[k, j, i] = gen(k, j, i);
            }
        }

        static public double[] CreateArray(GeneratorDouble3 gen, int dimx)
        {
            double[] r = new double[dimx];
            for (int k = 0; k < dimx; k++)
                r[k] = gen(k, 0, 0);
            return r;
        }
        static public double[,] CreateArray(GeneratorDouble3 gen, int dimx, int dimy)
        {
            double[,] r = new double[dimx,dimy];
            for (int k = 0; k < dimy; k++)
                for (int j = 0; j < dimy; j++)
                r[k,j] = gen(k, j, 0);
            return r;
        }
        static public double[,] CreateArray(GeneratorDouble gen, int dimx, int dimy)
        {
            double[,] r = new double[dimx, dimy];
            for (int k = 0; k < dimy; k++)
                for (int j = 0; j < dimy; j++)
                    r[k, j] = gen(k * dimy + j);
            return r;
        }

        static public double[,,] CreateArray(GeneratorDouble3 gen, int dimx, int dimy, int dimz)
        {
            double[,,] r = new double[dimx,dimy,dimz];
            for (int k = 0; k < dimx; k++)
                for (int j = 0; j < dimy; j++)
                    for (int i = 0; i < dimz; i++)
                r[k,j,i] = gen(k, j, i);
            return r;
        }
        static public double[, ,] CreateArray(GeneratorDouble gen, int dimx, int dimy, int dimz)
        {
            double[, ,] r = new double[dimx, dimy, dimz];
            for (int k = 0; k < dimx; k++)
                for (int j = 0; j < dimy; j++)
                    for (int i = 0; i < dimz; i++)
                        r[k, j, i] = gen((k * dimy + j) * dimz + i);
            return r;
        }

        static public double ForEach(DataArray a, OperatorDouble action, double start)
        {
            for (int i = 0; i < a.Length; i++ )
            {
                start = action(start, a.GetAnyValueLinear(i).ToDouble());
            }
            return start;
        }

        static public DataArrayDouble Operation(TupleMetaData info, DataArray a1, OperatorDouble op, DataArray a2)
        {
            if (a1.Rank != a2.Rank)
                throw new RankException();
            if ((a1.DimX != a2.DimX) || (a1.DimY != a2.DimY) || (a1.DimZ != a2.DimZ))
                throw new ArgumentException();

            DataArrayDouble ret = new DataArrayDouble(info, a1.DimX, a1.DimY, a1.DimZ);

            if (a1.Rank == 1)
            {
                for (int k = 0; k < a1.DimX; k++)
                    ret._array1[k] = op(a1.GetAnyValueLinear(k).ToDouble(), a2.GetAnyValueLinear(k).ToDouble());
            }
            else if (a1.Rank == 2)
            {
                for (int k = 0; k < a1.DimX; k++)
                    for (int j = 0; j < a1.DimY; j++)
                        ret._array2[k, j] = op(a1.GetAnyValue(k, j).ToDouble(), a2.GetAnyValue(k, j).ToDouble());
            }
            else if (a1.Rank == 3)
            {
                for (int k = 0; k < a1.DimX; k++)
                    for (int j = 0; j < a1.DimY; j++)
                        for (int i = 0; i < a1.DimZ; i++)
                            ret._array3[k, j, i] = op(a1.GetAnyValue(k, j, i).ToDouble(), a2.GetAnyValue(k, j, i).ToDouble());
            }
            return ret;
        }

        static public DataArrayDouble Operation(TupleMetaData info, DataArray a1, OperatorDouble op, double a2)
        {
            DataArrayDouble ret = new DataArrayDouble(info, a1.DimX, a1.DimY, a1.DimZ);

            if (a1.Rank == 1)
            {
                for (int k = 0; k < a1.DimX; k++)
                    ret._array1[k] = op(a1.GetAnyValueLinear(k).ToDouble(), a2);
            }
            else if (a1.Rank == 2)
            {
                for (int k = 0; k < a1.DimX; k++)
                    for (int j = 0; j < a1.DimY; j++)
                        ret._array2[k, j] = op(a1.GetAnyValue(k, j).ToDouble(), a2);
            }
            else if (a1.Rank == 3)
            {
                for (int k = 0; k < a1.DimX; k++)
                    for (int j = 0; j < a1.DimY; j++)
                        for (int i = 0; i < a1.DimZ; i++)
                            ret._array3[k, j, i] = op(a1.GetAnyValue(k, j, i).ToDouble(), a2);
            }
            return ret;
        }


        internal DataArrayDouble(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(double)) { Init(); }

        public DataArrayDouble(TupleMetaData newInfo, DataArrayDouble data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayDouble(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(double), dimx, dimy, dimz)) { Init(); }

        public DataArrayDouble(TupleMetaData info, double[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayDouble(TupleMetaData info, double[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayDouble(TupleMetaData info, double[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public double this[int index] { get { return _array1[index]; } }
        new public double this[int x, int y] { get { return _array2[x, y]; } }
        new public double this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public double GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out double[] r) { r = (double[])_array.Clone(); }
        public void ToArray(out double[,] r) { r = (double[,])_array.Clone(); }
        public void ToArray(out double[, ,] r) { r = (double[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayDouble(info, this); }
    }

    public class DataArrayCoords : DataArrayAbstract
    {
        private Coords[] _array1;
        private Coords[,] _array2;
        private Coords[, ,] _array3;

        void Init()
        {
            _array1 = _array as Coords[];
            _array2 = _array as Coords[,];
            _array3 = _array as Coords[, ,];
        }

        internal DataArrayCoords(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(Coords)) { Init(); }

        public DataArrayCoords(TupleMetaData newInfo, DataArrayCoords data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayCoords(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(Coords), dimx, dimy, dimz)) { Init(); }

        public DataArrayCoords(TupleMetaData info, Coords[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayCoords(TupleMetaData info, Coords[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayCoords(TupleMetaData info, Coords[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public Coords this[int index] { get { return _array1[index]; } }
        new public Coords this[int x, int y] { get { return _array2[x, y]; } }
        new public Coords this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public Coords GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out Coords[] r) { r = (Coords[])_array.Clone(); }
        public void ToArray(out Coords[,] r) { r = (Coords[,])_array.Clone(); }
        public void ToArray(out Coords[, ,] r) { r = (Coords[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayCoords(info, this); }
    }

    public class DataArraySensored : DataArrayAbstract
    {
        private Sensored[] _array1;
        private Sensored[,] _array2;
        private Sensored[, ,] _array3;

        void Init()
        {
            _array1 = _array as Sensored[];
            _array2 = _array as Sensored[,];
            _array3 = _array as Sensored[, ,];
        }

        internal DataArraySensored(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(Sensored)) { Init(); }

        public DataArraySensored(TupleMetaData newInfo, DataArraySensored data)
            : base(newInfo, data._array) { Init(); }

        public DataArraySensored(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(Sensored), dimx, dimy, dimz)) { Init(); }

        public DataArraySensored(TupleMetaData info, Sensored[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArraySensored(TupleMetaData info, Sensored[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArraySensored(TupleMetaData info, Sensored[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public Sensored this[int index] { get { return _array1[index]; } }
        new public Sensored this[int x, int y] { get { return _array2[x, y]; } }
        new public Sensored this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public Sensored GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out Sensored[] r) { r = (Sensored[])_array.Clone(); }
        public void ToArray(out Sensored[,] r) { r = (Sensored[,])_array.Clone(); }
        public void ToArray(out Sensored[, ,] r) { r = (Sensored[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArraySensored(info, this); }
    }

    public class DataArrayFiberCoords : DataArrayAbstract
    {
        private FiberCoords[] _array1;
        private FiberCoords[,] _array2;
        private FiberCoords[, ,] _array3;

        void Init()
        {
            _array1 = _array as FiberCoords[];
            _array2 = _array as FiberCoords[,];
            _array3 = _array as FiberCoords[, ,];
        }

        internal DataArrayFiberCoords(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(FiberCoords)) { Init(); }

        public DataArrayFiberCoords(TupleMetaData newInfo, DataArrayFiberCoords data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayFiberCoords(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(FiberCoords), dimx, dimy, dimz)) { Init(); }

        public DataArrayFiberCoords(TupleMetaData info, FiberCoords[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayFiberCoords(TupleMetaData info, FiberCoords[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayFiberCoords(TupleMetaData info, FiberCoords[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public FiberCoords GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        new public FiberCoords this[int index] { get { return _array1[index]; } }
        new public FiberCoords this[int x, int y] { get { return _array2[x, y]; } }
        new public FiberCoords this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        public void ToArray(out FiberCoords[] r) { r = (FiberCoords[])_array.Clone(); }
        public void ToArray(out FiberCoords[,] r) { r = (FiberCoords[,])_array.Clone(); }
        public void ToArray(out FiberCoords[, ,] r) { r = (FiberCoords[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayFiberCoords(info, this); }
    }

    public class DataArrayMultiIntFloat : DataArrayAbstract
    {
        private MultiIntFloat[] _array1;
        private MultiIntFloat[,] _array2;
        private MultiIntFloat[, ,] _array3;

        void Init()
        {
            _array1 = _array as MultiIntFloat[];
            _array2 = _array as MultiIntFloat[,];
            _array3 = _array as MultiIntFloat[, ,];
        }

        internal DataArrayMultiIntFloat(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(MultiIntFloat)) { Init(); }

        public DataArrayMultiIntFloat(TupleMetaData newInfo, DataArrayMultiIntFloat data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayMultiIntFloat(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(MultiIntFloat), dimx, dimy, dimz)) { Init(); }

        public DataArrayMultiIntFloat(TupleMetaData info, MultiIntFloat[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayMultiIntFloat(TupleMetaData info, MultiIntFloat[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayMultiIntFloat(TupleMetaData info, MultiIntFloat[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public MultiIntFloat this[int index] { get { return _array1[index]; } }
        new public MultiIntFloat this[int x, int y] { get { return _array2[x, y]; } }
        new public MultiIntFloat this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public MultiIntFloat GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out MultiIntFloat[] r) { r = (MultiIntFloat[])_array.Clone(); }
        public void ToArray(out MultiIntFloat[,] r) { r = (MultiIntFloat[,])_array.Clone(); }
        public void ToArray(out MultiIntFloat[, ,] r) { r = (MultiIntFloat[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayMultiIntFloat(info, this); }
    }

    

    public class DataArrayTimestamp : DataArrayAbstract
    {
        private Timestamp[] _array1;
        private Timestamp[,] _array2;
        private Timestamp[, ,] _array3;

        void Init()
        {
            _array1 = _array as Timestamp[];
            _array2 = _array as Timestamp[,];
            _array3 = _array as Timestamp[, ,];
        }

        internal DataArrayTimestamp(TupleMetaData info, int dimx, int dimy, int dimz)
            : base(info, dimx, dimy, dimz, typeof(Timestamp)) { Init(); }

        public DataArrayTimestamp(TupleMetaData newInfo, DataArrayTimestamp data)
            : base(newInfo, data._array) { Init(); }

        public DataArrayTimestamp(TupleMetaData info, IDeserializeStream source, int dimx, int dimy, int dimz)
            : base(info, source.GetArray(typeof(Timestamp), dimx, dimy, dimz)) { Init(); }

        public DataArrayTimestamp(TupleMetaData info, Timestamp[] orig)
            : this(info, orig.Length, 0, 0)
        { orig.CopyTo(_array1, 0); }

        public DataArrayTimestamp(TupleMetaData info, Timestamp[,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), 0)
        { Array.Copy(orig, _array2, orig.Length); }

        public DataArrayTimestamp(TupleMetaData info, Timestamp[, ,] orig)
            : this(info, orig.GetLength(0), orig.GetLength(1), orig.GetLength(2))
        { Array.Copy(orig, _array3, orig.Length); }

        new public Timestamp this[int index] { get { return _array1[index]; } }
        new public Timestamp this[int x, int y] { get { return _array2[x, y]; } }
        new public Timestamp this[int x, int y, int z] { get { return _array3[x, y, z]; } }

        new public Timestamp GetLinear(int index)
        {
            switch (_rank)
            {
                case 1: return _array1[index];
                case 2: return _array2[index / _y, index % _y];
                case 3: return _array3[index / (_y * _z), ((index / _z) % (_y)), index % _z];
                default: throw new NotSupportedException();
            }
        }

        public void ToArray(out Timestamp[] r) { r = (Timestamp[])_array.Clone(); }
        public void ToArray(out Timestamp[,] r) { r = (Timestamp[,])_array.Clone(); }
        public void ToArray(out Timestamp[, ,] r) { r = (Timestamp[, ,])_array.Clone(); }

        public override DataArray Reinfo(TupleMetaData info)
        { return new DataArrayTimestamp(info, this); }
    }

    #endregion
}
#endif


