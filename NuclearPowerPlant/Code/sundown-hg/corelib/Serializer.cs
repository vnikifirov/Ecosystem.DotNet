using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Runtime.InteropServices;

namespace corelib
{
    public class AwfulSerializer
    {
        protected void Ensure(int size)
        {
            if (_data.Length < _dataSize + size)
                Expand((int)((size + _dataSize) * 1.5));
        }
        
        unsafe public void Put(byte val)
        {
            Ensure(1);
            _data[_dataSize++] = val;
        }

        unsafe public void Put(short val)
        {
            Copy(&val, sizeof(short));
        }

        unsafe public void Put(int val)
        {
            Copy(&val, sizeof(int));
        }

        unsafe public void Put(Coords val)
        {
            Copy(&val, sizeof(Coords));
        }

        unsafe public void Put(FiberCoords val)
        {
            Copy(&val, sizeof(FiberCoords));
        }

        unsafe public void Put(Sensored val)
        {
            Copy(&val, sizeof(Sensored));
        }

        unsafe public void Put(MultiIntFloat val)
        {
            Copy(&val, sizeof(MultiIntFloat));
        }

        unsafe public void PutStruct(ValueType obj)
        {
            int itemSize = Marshal.SizeOf(obj.GetType());
            Ensure(itemSize);
            fixed (byte* pSrc = &_data[_dataSize])
            {
                Marshal.StructureToPtr(obj, (IntPtr)pSrc, true);
            }
            _dataSize += itemSize;
        }

        public void Put(string str)
        {
            Put(2 * str.Length);
            Put(str.ToCharArray());
        }

        unsafe public void Put(float val)
        {
            Copy(&val, sizeof(float));
        }

        unsafe public void Put(double val)
        {
            Copy(&val, sizeof(double));
        }

        unsafe void Copy(void* ptr, int size)
        {
            Ensure(size);
            Marshal.Copy((IntPtr)ptr, _data, _dataSize, size);
            _dataSize += size;
        }

        unsafe public void Put(char[] ar) { fixed (char* a = ar) Copy(a, ar.Length * sizeof(char)); }

        unsafe public void Put(int[] ar) { fixed (int* a = ar) Copy(a, ar.Length * sizeof(int)); }
        unsafe public void Put(int[,] ar) { fixed (int* a = ar) Copy(a, ar.Length * sizeof(int)); }
        unsafe public void Put(int[, ,] ar) { fixed (int* a = ar) Copy(a, ar.Length * sizeof(int)); }

        unsafe public void Put(short[] ar) { fixed (short* a = ar) Copy(a, ar.Length * sizeof(short)); }
        unsafe public void Put(short[,] ar) { fixed (short* a = ar) Copy(a, ar.Length * sizeof(short)); }
        unsafe public void Put(short[, ,] ar) { fixed (short* a = ar) Copy(a, ar.Length * sizeof(short)); }

        unsafe public void Put(byte[] ar) { fixed (byte* a = ar) Copy(a, ar.Length * sizeof(byte)); }
        unsafe public void Put(byte[,] ar) { fixed (byte* a = ar) Copy(a, ar.Length * sizeof(byte)); }
        unsafe public void Put(byte[, ,] ar) { fixed (byte* a = ar) Copy(a, ar.Length * sizeof(byte)); }

        unsafe public void Put(float[] ar) { fixed (float* a = ar) Copy(a, ar.Length * sizeof(float)); }
        unsafe public void Put(float[,] ar) { fixed (float* a = ar) Copy(a, ar.Length * sizeof(float)); }
        unsafe public void Put(float[, ,] ar) { fixed (float* a = ar) Copy(a, ar.Length * sizeof(float)); }

        unsafe public void Put(double[] ar) { fixed (double* a = ar) Copy(a, ar.Length * sizeof(double)); }
        unsafe public void Put(double[,] ar) { fixed (double* a = ar) Copy(a, ar.Length * sizeof(double)); }
        unsafe public void Put(double[, ,] ar) { fixed (double* a = ar) Copy(a, ar.Length * sizeof(double)); }


        unsafe public void Put(Coords[] ar) { fixed (Coords* a = ar) Copy(a, ar.Length * sizeof(Coords)); }
        unsafe public void Put(Coords[,] ar) { fixed (Coords* a = ar) Copy(a, ar.Length * sizeof(Coords)); }
        unsafe public void Put(Coords[, ,] ar) { fixed (Coords* a = ar) Copy(a, ar.Length * sizeof(Coords)); }

        unsafe public void Put(Sensored[] ar) { fixed (Sensored* a = ar) Copy(a, ar.Length * sizeof(Sensored)); }
        unsafe public void Put(Sensored[,] ar) { fixed (Sensored* a = ar) Copy(a, ar.Length * sizeof(Sensored)); }
        unsafe public void Put(Sensored[, ,] ar) { fixed (Sensored* a = ar) Copy(a, ar.Length * sizeof(Sensored)); }

        unsafe public void Put(FiberCoords[] ar) { fixed (FiberCoords* a = ar) Copy(a, ar.Length * sizeof(FiberCoords)); }
        unsafe public void Put(FiberCoords[,] ar) { fixed (FiberCoords* a = ar) Copy(a, ar.Length * sizeof(FiberCoords)); }
        unsafe public void Put(FiberCoords[, ,] ar) { fixed (FiberCoords* a = ar) Copy(a, ar.Length * sizeof(FiberCoords)); }

        unsafe public void Put(MultiIntFloat[] ar) { fixed (MultiIntFloat* a = ar) Copy(a, ar.Length * sizeof(MultiIntFloat)); }
        unsafe public void Put(MultiIntFloat[,] ar) { fixed (MultiIntFloat* a = ar) Copy(a, ar.Length * sizeof(MultiIntFloat)); }
        unsafe public void Put(MultiIntFloat[, ,] ar) { fixed (MultiIntFloat* a = ar) Copy(a, ar.Length * sizeof(MultiIntFloat)); }

        unsafe public void Put(DateTime[] ar) { fixed (DateTime* a = ar) Copy(a, ar.Length * sizeof(DateTime)); }
        //unsafe public void Put(DateTime[,] ar) { fixed (DateTime* a = ar) Copy(a, ar.Length * sizeof(DateTime)); }
        //unsafe public void Put(DateTime[, ,] ar) { fixed (DateTime* a = ar) Copy(a, ar.Length * sizeof(DateTime)); }

#if !DOTNET_V11
        public void Put<T>(T[] ar) where T : struct
        {
            if (typeof(T) == typeof(int))
                Put((int[])(Array)ar);
            else if (typeof(T) == typeof(short))
                Put((short[])(Array)ar);
            else if (typeof(T) == typeof(byte))
                Put((byte[])(Array)ar);
            else if (typeof(T) == typeof(float))
                Put((float[])(Array)ar);
            else if (typeof(T) == typeof(double))
                Put((double[])(Array)ar);
            else if (typeof(T) == typeof(Coords))
                Put((Coords[])(Array)ar);
            else if (typeof(T) == typeof(FiberCoords))
                Put((FiberCoords[])(Array)ar);
            else if (typeof(T) == typeof(Sensored))
                Put((Sensored[])(Array)ar);
            else if (typeof(T) == typeof(MultiIntFloat))
                Put((MultiIntFloat[])(Array)ar);
            else
                throw new NotImplementedException("Can't serialize this type");
//                Put((Array)ar);

        }

        public void Put<T>(T[,] ar) where T : struct
        {
            if (typeof(T) == typeof(int))
                Put((int[,])(Array)ar);
            else if (typeof(T) == typeof(short))
                Put((short[,])(Array)ar);
            else if (typeof(T) == typeof(byte))
                Put((byte[,])(Array)ar);
            else if (typeof(T) == typeof(float))
                Put((float[,])(Array)ar);
            else if (typeof(T) == typeof(double))
                Put((double[,])(Array)ar);
            else if (typeof(T) == typeof(Coords))
                Put((Coords[,])(Array)ar);
            else if (typeof(T) == typeof(FiberCoords))
                Put((FiberCoords[,])(Array)ar);
            else if (typeof(T) == typeof(Sensored))
                Put((Sensored[,])(Array)ar);
            else if (typeof(T) == typeof(MultiIntFloat))
                Put((MultiIntFloat[,])(Array)ar);
            else
                throw new NotImplementedException("Can't serialize this type");
                //Put((Array)ar);
        }

        public void Put<T>(T[,,] ar) where T : struct
        {
            if (typeof(T) == typeof(int))
                Put((int[,,])(Array)ar);
            else if (typeof(T) == typeof(short))
                Put((short[,,])(Array)ar);
            else if (typeof(T) == typeof(byte))
                Put((byte[,,])(Array)ar);
            else if (typeof(T) == typeof(float))
                Put((float[,,])(Array)ar);
            else if (typeof(T) == typeof(double))
                Put((double[,,])(Array)ar);
            else if (typeof(T) == typeof(Coords))
                Put((Coords[,,])(Array)ar);
            else if (typeof(T) == typeof(FiberCoords))
                Put((FiberCoords[,,])(Array)ar);
            else if (typeof(T) == typeof(Sensored))
                Put((Sensored[,,])(Array)ar);
            else if (typeof(T) == typeof(MultiIntFloat))
                Put((MultiIntFloat[,,])(Array)ar);
            else
                throw new NotImplementedException("Can't serialize this type");
                //Put((Array)ar);
        }
#endif


        unsafe public void Put(Array ar)
        {
            int len = ar.Length;
            Type t = ar.GetType();
            Type et = t.GetElementType();

            // Check et
            int itemSize = Marshal.SizeOf(et);
            int byteLen = len * itemSize;

            if (_data.Length < byteLen + _dataSize)
                Expand((int)((_dataSize + byteLen) * 1.5));

            // TODO Optimize me!!!
            fixed (byte* pSrc = &_data[_dataSize])
            {
                byte* ptr = pSrc;
                switch (ar.Rank)
                {
                    case 1:
                        for (int i = 0; i < ar.Length; i++, ptr += itemSize)
                            Marshal.StructureToPtr(ar.GetValue(i), (IntPtr)ptr, true);
                        break;
                    case 2:
                        for (int j = 0; j < ar.GetLength(0); j++)
                            for (int i = 0; i < ar.GetLength(1); i++, ptr += itemSize)
                                Marshal.StructureToPtr(ar.GetValue(j, i), (IntPtr)ptr, true);
                        break;
                    case 3:
                        for (int k = 0; k < ar.GetLength(0); k++)
                            for (int j = 0; j < ar.GetLength(1); j++)
                                for (int i = 0; i < ar.GetLength(2); i++, ptr += itemSize)
                                    Marshal.StructureToPtr(ar.GetValue(k, j, i), (IntPtr)ptr, true);
                        break;
                }
            }
            _dataSize += byteLen;
        }


        public byte[] Data
        {
            get { return _data; }
        }
        public int StoredLen
        {
            get { return _dataSize; }
        }

        public void Expand(int newSize)
        {
            if (newSize < _dataSize)
                throw new Exception("Can't shrink data");

            byte[] newData = new byte[newSize];
            _data.CopyTo(newData, 0);

            _data = newData;
        }

        public AwfulSerializer(int preallocatedSize)
        {
            _dataSize = 0;
            _data = new byte[preallocatedSize];
        }

        private int _dataSize;
        private byte[] _data;
    }


    public class AwfulDeserializer
    {
        void Ensure(int size)
        {
            if (_dataSize + size > _dataEnd)
                throw new ArgumentException("End of stream");
        }

        public void Get(out byte val)
        {
            Ensure(1);
            val = _data[_dataSize++];
        }

        unsafe public void Get(out short val)
        {
            fixed (short* v = &val) Copy(v, sizeof(short));
        }

        unsafe public void Get(out int val)
        {
            fixed (int* v = &val) Copy(v, sizeof(int));
        }

        unsafe public void Get(out float val)
        {
            fixed (float* v = &val) Copy(v, sizeof(float));
        }

        unsafe public void Get(out double val)
        {
            fixed (double* v = &val) Copy(v, sizeof(double));
        }

        unsafe public void Get(out Sensored val)
        {
            fixed (Sensored* v = &val) Copy(v, sizeof(Sensored));
        }

        unsafe public void Get(out Coords val)
        {
            fixed (Coords* v = &val) Copy(v, sizeof(Coords));
        }

        unsafe public void Get(out FiberCoords val)
        {
            fixed (FiberCoords* v = &val) Copy(v, sizeof(FiberCoords));
        }

        unsafe public void Get(out MultiIntFloat val)
        {
            fixed (MultiIntFloat* v = &val) Copy(v, sizeof(MultiIntFloat));
        }

        unsafe public void Get(out string str)
        {
            int byteLen;
            Get(out byteLen);
            Ensure(byteLen);

            fixed (byte* b = &_data[_dataSize])
                str = new String((char*)b, 0, byteLen / 2);

            _dataSize += byteLen;
        }

        unsafe protected void Copy(void* ptr, int size)
        {
            if (size > 0)
            {
                Ensure(size);
                Marshal.Copy(_data, _dataSize, (IntPtr)ptr, size);
                _dataSize += size;
            }
        }

        unsafe public void Get(char[] ar) { fixed (char* a = ar) Copy(a, ar.Length * sizeof(char)); }

        unsafe public void Get(int[] ar) { fixed (int* a = ar) Copy(a, ar.Length * sizeof(int)); }
        unsafe public void Get(int[,] ar) { fixed (int* a = ar) Copy(a, ar.Length * sizeof(int)); }
        unsafe public void Get(int[, ,] ar) { fixed (int* a = ar) Copy(a, ar.Length * sizeof(int)); }

        unsafe public void Get(short[] ar) { fixed (short* a = ar) Copy(a, ar.Length * sizeof(short)); }
        unsafe public void Get(short[,] ar) { fixed (short* a = ar) Copy(a, ar.Length * sizeof(short)); }
        unsafe public void Get(short[, ,] ar) { fixed (short* a = ar) Copy(a, ar.Length * sizeof(short)); }

        unsafe public void Get(byte[] ar) { fixed (byte* a = ar) Copy(a, ar.Length * sizeof(byte)); }
        unsafe public void Get(byte[,] ar) { fixed (byte* a = ar) Copy(a, ar.Length * sizeof(byte)); }
        unsafe public void Get(byte[, ,] ar) { fixed (byte* a = ar) Copy(a, ar.Length * sizeof(byte)); }

        unsafe public void Get(float[] ar) { fixed (float* a = ar) Copy(a, ar.Length * sizeof(float)); }
        unsafe public void Get(float[,] ar) { fixed (float* a = ar) Copy(a, ar.Length * sizeof(float)); }
        unsafe public void Get(float[, ,] ar) { fixed (float* a = ar) Copy(a, ar.Length * sizeof(float)); }

        unsafe public void Get(double[] ar) { fixed (double* a = ar) Copy(a, ar.Length * sizeof(double)); }
        unsafe public void Get(double[,] ar) { fixed (double* a = ar) Copy(a, ar.Length * sizeof(double)); }
        unsafe public void Get(double[, ,] ar) { fixed (double* a = ar) Copy(a, ar.Length * sizeof(double)); }


        unsafe public void Get(Coords[] ar) { fixed (Coords* a = ar) Copy(a, ar.Length * sizeof(Coords)); }
        unsafe public void Get(Coords[,] ar) { fixed (Coords* a = ar) Copy(a, ar.Length * sizeof(Coords)); }
        unsafe public void Get(Coords[, ,] ar) { fixed (Coords* a = ar) Copy(a, ar.Length * sizeof(Coords)); }

        unsafe public void Get(Sensored[] ar) { fixed (Sensored* a = ar) Copy(a, ar.Length * sizeof(Sensored)); }
        unsafe public void Get(Sensored[,] ar) { fixed (Sensored* a = ar) Copy(a, ar.Length * sizeof(Sensored)); }
        unsafe public void Get(Sensored[, ,] ar) { fixed (Sensored* a = ar) Copy(a, ar.Length * sizeof(Sensored)); }

        unsafe public void Get(FiberCoords[] ar) { fixed (FiberCoords* a = ar) Copy(a, ar.Length * sizeof(FiberCoords)); }
        unsafe public void Get(FiberCoords[,] ar) { fixed (FiberCoords* a = ar) Copy(a, ar.Length * sizeof(FiberCoords)); }
        unsafe public void Get(FiberCoords[, ,] ar) { fixed (FiberCoords* a = ar) Copy(a, ar.Length * sizeof(FiberCoords)); }

        unsafe public void Get(MultiIntFloat[] ar) { fixed (MultiIntFloat* a = ar) Copy(a, ar.Length * sizeof(MultiIntFloat)); }
        unsafe public void Get(MultiIntFloat[,] ar) { fixed (MultiIntFloat* a = ar) Copy(a, ar.Length * sizeof(MultiIntFloat)); }
        unsafe public void Get(MultiIntFloat[, ,] ar) { fixed (MultiIntFloat* a = ar) Copy(a, ar.Length * sizeof(MultiIntFloat)); }


#if !DOTNET_V11
        public void Get<T>(T[] ar) where T : struct
        {
            if (typeof(T) == typeof(int))
                Get((int[])(Array)ar);
            else if (typeof(T) == typeof(short))
                Get((short[])(Array)ar);
            else if (typeof(T) == typeof(byte))
                Get((byte[])(Array)ar);
            else if (typeof(T) == typeof(float))
                Get((float[])(Array)ar);
            else if (typeof(T) == typeof(double))
                Get((double[])(Array)ar);
            else if (typeof(T) == typeof(Coords))
                Get((Coords[])(Array)ar);
            else if (typeof(T) == typeof(FiberCoords))
                Get((FiberCoords[])(Array)ar);
            else if (typeof(T) == typeof(Sensored))
                Get((Sensored[])(Array)ar);
            else if (typeof(T) == typeof(MultiIntFloat))
                Get((MultiIntFloat[])(Array)ar);
            else
                throw new NotImplementedException("Can't deserialize this type");
        }

        public void Get<T>(T[,] ar) where T : struct
        {
            if (typeof(T) == typeof(int))
                Get((int[,])(Array)ar);
            else if (typeof(T) == typeof(short))
                Get((short[,])(Array)ar);
            else if (typeof(T) == typeof(byte))
                Get((byte[,])(Array)ar);
            else if (typeof(T) == typeof(float))
                Get((float[,])(Array)ar);
            else if (typeof(T) == typeof(double))
                Get((double[,])(Array)ar);
            else if (typeof(T) == typeof(Coords))
                Get((Coords[,])(Array)ar);
            else if (typeof(T) == typeof(FiberCoords))
                Get((FiberCoords[,])(Array)ar);
            else if (typeof(T) == typeof(Sensored))
                Get((Sensored[,])(Array)ar);
            else if (typeof(T) == typeof(MultiIntFloat))
                Get((MultiIntFloat[,])(Array)ar);
            else
                throw new NotImplementedException("Can't deserialize this type");
        }

        public void Get<T>(T[, ,] ar) where T : struct
        {
            if (typeof(T) == typeof(int))
                Get((int[, ,])(Array)ar);
            else if (typeof(T) == typeof(short))
                Get((short[, ,])(Array)ar);
            else if (typeof(T) == typeof(byte))
                Get((byte[, ,])(Array)ar);
            else if (typeof(T) == typeof(float))
                Get((float[, ,])(Array)ar);
            else if (typeof(T) == typeof(double))
                Get((double[, ,])(Array)ar);
            else if (typeof(T) == typeof(Coords))
                Get((Coords[, ,])(Array)ar);
            else if (typeof(T) == typeof(FiberCoords))
                Get((FiberCoords[, ,])(Array)ar);
            else if (typeof(T) == typeof(Sensored))
                Get((Sensored[, ,])(Array)ar);
            else if (typeof(T) == typeof(MultiIntFloat))
                Get((MultiIntFloat[, ,])(Array)ar);
            else
                throw new NotImplementedException("Can't deserialize this type");
        }
#endif

        unsafe public object GetStruct(Type objType)
        {
            int itemSize = Marshal.SizeOf(objType);
            Ensure(itemSize);

            object obj;
            fixed (byte* pSrc = &_data[_dataSize])
            {
                obj = Marshal.PtrToStructure((IntPtr)pSrc, objType);
            }

            _dataSize += itemSize;
            return obj;
        }

        public Array GetArray(Type element, int x, int y, int z)
        {
            Array res;

            if ((y == 0) && (z == 0))
                res = Array.CreateInstance(element, x);
            else if (z == 0)
                res = Array.CreateInstance(element, x, y);
            else
                res = Array.CreateInstance(element, x, y, z);

            int itemSize = Marshal.SizeOf(element);
            int byteLen = res.Length * itemSize;

            Ensure(byteLen);


            // TODO Optimize me!!!
            unsafe
            {
                fixed (byte* pSrc = &_data[_dataSize])
                {
                    byte* ptr = pSrc;
                    switch (res.Rank)
                    {
                        case 1:
                            for (int i = 0; i < res.Length; i++, ptr += itemSize)
                                //Marshal.StructureToPtr(ar.GetValue(i), (IntPtr)ptr, true);
                                res.SetValue(Marshal.PtrToStructure((IntPtr)ptr, element), i);
                            break;
                        case 2:
                            for (int j = 0; j < res.GetLength(0); j++)
                                for (int i = 0; i < res.GetLength(1); i++, ptr += itemSize)
                                    //Marshal.StructureToPtr(ar.GetValue(j, i), (IntPtr)ptr, true);
                                    res.SetValue(Marshal.PtrToStructure((IntPtr)ptr, element), j, i);
                            break;
                        case 3:
                            for (int k = 0; k < res.GetLength(0); k++)
                                for (int j = 0; j < res.GetLength(1); j++)
                                    for (int i = 0; i < res.GetLength(2); i++, ptr += itemSize)
                                        //Marshal.StructureToPtr(ar.GetValue(k, j, i), (IntPtr)ptr, true);
                                        res.SetValue(Marshal.PtrToStructure((IntPtr)ptr, element), k, j, i);
                            break;
                    }
                }
            }

            _dataSize += byteLen;
            return res;
        }

        public void RestoryOriginal()
        {
            _dataSize = _start;
        }

        public void Rewind(int count)
        {
            _dataSize -= count;
            if (_dataSize < 0)
                throw new ArgumentException();
            else if (_dataSize >= _dataEnd)
                throw new ArgumentException();
        }

        public int GetPos()
        {
            return _dataSize;
        }

        public AwfulDeserializer(byte[] data, int start)
        {
            _start = start;
            _data = data;
            _dataSize = start;
            _dataEnd = data.Length;
        }

        public AwfulDeserializer(AwfulDeserializer data, int start)
        {
            _start = start;
            _data = data._data;
            _dataSize = start;
            _dataEnd = data._data.Length;
        }

        private int _start;
        private int _dataSize;
        private int _dataEnd;
        private byte[] _data;
    }


    public class StreamSerializer : AwfulSerializer, ISerializeStream
    {
        private TupleMetaData _info;


        public StreamSerializer(TupleMetaData info)
            : base(8000)
        {
            _info = info;
        }

        public byte[] GetData(out int actualLen)
        {
            actualLen = StoredLen;
            return Data;
        }

        #region IRawChronoSerializer Members

        public string GetName()
        {
            return _info.Name;
        }

        #endregion 

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _info.Date;
        }

        #endregion

        #region IHumanInfo Members

        public string GetHumanName()
        {
            return _info.HumaneName;
        }

        #endregion

        public string HumaneName
        {
            get { return GetHumanName(); }
        }

        public string Name
        {
            get { return GetName(); }
        }

        public DateTime Date
        {
            get { return GetTimeDate(); }
        }

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _info.StreamName;
        }

        #endregion

        #region IRawChronoSerializer Members

        public byte[] GetData()
        {
            byte[] adata = new byte[StoredLen];
            for (int i = 0; i < StoredLen; i++)
                adata[i] = Data[i];

            return adata;
        }

        #endregion

        #region ITupleMetaData Members

        public TupleMetaData Info
        {
            get { return _info; }
        }

        #endregion

    }

    public class StreamDeserializer : AwfulDeserializer, IDeserializeStream
    {
        TupleMetaData _info;

        #region ITimeData Members

        public DateTime GetTimeDate()
        {
            return _info.Date;
        }

        #endregion

        #region IHumanInfo Members

        public string GetHumanName()
        {
            return _info.HumaneName;
        }

        #endregion

        public string HumaneName
        {
            get { return GetHumanName(); }
        }

        public string Name
        {
            get { return GetName(); }
        }

        public DateTime Date
        {
            get { return GetTimeDate(); }
        }

        #region INameData Members

        public string GetName()
        {
            return _info.Name;
        }

        #endregion

        public StreamDeserializer(StreamDeserializer s, int idx, TupleMetaData info)
            : base(s, idx)
        {
            _info = info;
        }

        public StreamDeserializer(byte[] data, TupleMetaData info)
            : base(data, 0)
        {
            _info = info;
        }
        public StreamDeserializer(byte[] data, int startByte)
            : base(data, startByte)
        {
        }

        #region ITupleMetaData Members

        public TupleMetaData Info
        {
            get { return _info; }
        }

        #endregion

        #region IStreamInfo Members

        public string GetStreamName()
        {
            return _info.StreamName;
        }

        #endregion
    }
}
