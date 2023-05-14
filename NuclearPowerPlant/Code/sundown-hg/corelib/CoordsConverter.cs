using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

namespace corelib
{

#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
#endif


    public class CoordsConverter : DataArrayCoords
    {
        public enum SpecialFlag
        {
            Named,
            Linear1884,
            WideLinear2448,
            PVK
        }

        public const int InvalidIndex = -1;

        readonly SpecialFlag _flag;
        int[,] _reverse;
        readonly int _id;
        readonly int _separator;

        public static readonly CoordsConverter sDummyConverter = CreateDummyConverter();

        public CoordsConverter(TupleMetaData info, SpecialFlag flag, DataArrayCoords array)
            : base(info, array)
        {
            InitReverse48();

            _flag = flag;
            _separator = DimY;

            _id = 0;

        }

        protected CoordsConverter(TupleMetaData info, SpecialFlag flag, Coords[] table, int id)
            : base(info, table)
        {
            InitReverse48();

            _flag = flag;
            _separator = 0;

            _id = id;
        }

        public CoordsConverter(TupleMetaData info, SpecialFlag flag, Coords[] table)
            : this(info, flag, table, 0)
        {
            if ((flag == SpecialFlag.Linear1884) || (flag == SpecialFlag.WideLinear2448) || (flag == SpecialFlag.PVK))
                throw new ArgumentException("Bad flag for this call");
        }

        public CoordsConverter(TupleMetaData info, SpecialFlag flag, Coords[,] table)
            : base(info, table)
        {
            if ((flag == SpecialFlag.Linear1884) || (flag == SpecialFlag.WideLinear2448))
                throw new ArgumentException("Bad flag for this call");

            InitReverse48();

            _flag = flag;
            _separator = DimY;
        }

        static CoordsConverter CreateDummyConverter()
        {
            Coords[,] c = new Coords[48, 48];
            for (int x = 0; x < 48; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    c[y, x] = new Coords((byte)y, (byte)x);
                }
            }
            TupleMetaData info = new TupleMetaData("convdummy", "convdummy", DateTime.Now, TupleMetaData.StreamConst);
            return new CoordsConverter(info, SpecialFlag.Named, new DataArrayCoords(info, c));
        }

        static public CoordsConverter CreateABIOld(IDeserializeStream stream)
        {
            DataArrayCoords coords = (DataArrayCoords)DataArray.Create(stream);
            return new CoordsConverter(coords.Info, SpecialFlag.Named, coords);
        }

        public SpecialFlag Flag
        {
            get
            {
                return _flag;
            }
        }

        void InitReverse48()
        {
            _reverse = new int[48, 48];
            for (int x = 0; x < 48; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    _reverse[x, y] = InvalidIndex;
                }
            }
            for (int i = 0; i < Length; i++)
            {
                Coords c = this[i];
                if (c.IsOk)
                    //_reverse[c.X, c.Y] = i;
                    _reverse[c.Y, c.X] = i;
            }
        }

        public Coords this[FiberCoords i]
        {
            get
            {
                if (_flag != SpecialFlag.PVK)
                    throw new ArgumentException();
                if (i.IsValid)
                    return this[i.Fiber, i.Pvk];

                return Coords.incorrect;
            }
        }

        public FiberCoords GetFiberCoords(Coords c)
        {
            if (_flag != SpecialFlag.PVK)
                throw new ArgumentException();
            int idx = this[c];

            if (idx == InvalidIndex)
            {
                return FiberCoords.incorrect;
            }
            else
            {
                return new FiberCoords(idx / FiberCoords.PvkMax, idx % FiberCoords.PvkMax);
            }
        }

        public new Coords this[int i]
        {
            get
            {
                return GetLinear(i);
            }
        }

        public int this[Coords c]
        {
            get
            {
                if (c.IsOk)
                {
                    //return _reverse[c.X, c.Y];
                    return _reverse[c.Y, c.X];
                }
                //throw new ArgumentException();
                return InvalidIndex;
            }
        }

        public void SerializeABIOld(ISerializeStream stream)
        {
            base.Serialize(stream);
        }

        public static CoordsConverter LoadLinear1884(bool revertX, bool revertY, bool switchXY)
        {
            Coords[] linear = new Coords[1884];

            for (int i = 0; i < linear.Length; i++)
            {
                linear[i] = ComputeFrom1884(i, revertX, revertY, switchXY);
            }

            int id = (revertX ? 1 : 0) | (revertY ? 2 : 0) | (switchXY ? 4 : 0);
            return new CoordsConverter(
                new TupleMetaData("convlinear1884", "Converter 1884", DateTime.Now, TupleMetaData.StreamConst),
                SpecialFlag.Linear1884,
                linear,
                id);
        }

        public static CoordsConverter LoadWideLinear2448(bool revertX, bool revertY, bool switchXY)
        {
            Coords[] linear = new Coords[2448];

            for (int i = 0; i < linear.Length; i++)
            {
                linear[i] = ComputeFrom2448(i, revertX, revertY, switchXY);
            }

            int id = (revertX ? 1 : 0) | (revertY ? 2 : 0) | (switchXY ? 4 : 0);
            return new CoordsConverter(
                new TupleMetaData("convlinear2448", "Converter 2448", DateTime.Now, TupleMetaData.StreamConst),
                SpecialFlag.WideLinear2448,
                linear,
                id);
        }

        static public CoordsConverter Intersect(CoordsConverter c1, CoordsConverter c2)
        {
            if (c1 == c2)
                return c1;

            if ((c1._flag == SpecialFlag.Linear1884) && (c2._flag == SpecialFlag.WideLinear2448))
                return c1;
            if (/*(c1._flag != SpecialFlag.Named) &&*/ (c2 == sDummyConverter))
                return c1;

            if ((c2._flag == SpecialFlag.Linear1884) && (c1._flag == SpecialFlag.WideLinear2448))
                return c2;
            if (/*(c2._flag != SpecialFlag.Named) &&*/ (c1 == sDummyConverter))
                return c2;
#if !DOTNET_V11
            List<Coords> l = new List<Coords>();
#else
            ArrayList l = new ArrayList();
#endif

            for (int i = 0; i < c1.Length; i++)
            {
                Coords c = c1[i];
                if (!c.IsOk)
                    continue;

                for (int j = 0; j < c2.Length; j++)
                {
                    if (c2[j] == c)
                    {
                        l.Add(c);
                        goto next;
                    }
                }
            next: ;
            }
            
            if (l.Count == 0)
                return null;

            Coords[] inter = new Coords[l.Count];
            l.CopyTo(inter);
            return new CoordsConverter(new TupleMetaData("intersect", "intersect", DateTime.Now, TupleMetaData.StreamAuto), SpecialFlag.Named, inter);
        }


        static private Coords ComputeFrom1884(int index, bool revertX, bool revertY, bool switchXY)
        {
            int i;
            int prev = 0;
            int next = width_of_zone[0];

            for (i = 1; i < width_of_zone.Length; i++)
            {
                if ((index >= prev) && (index < (prev + next)))
                {
                    goto index_found;
                }
                prev += next;
                next = width_of_zone[i];
            }
            for (i = width_of_zone.Length; i < 2 * width_of_zone.Length; i++)
            {
                if ((index >= prev) && (index < (prev + next)))
                {
                    goto index_found;
                }
                prev += next;
                next = width_of_zone[2 * width_of_zone.Length - i - 1];
            }
            if ((index >= prev) && (index < (prev + next)))
            {
                goto index_found;
            }

            //Bad index!
            //m.x = m.y = 255;
            return Coords.incorrect;

        index_found:
            int x, y;
            //m.x
            x = (2 * width_of_zone.Length - i);
            if (i <= width_of_zone.Length)
            {
                //m.y
                y = ((index - prev) + (width_of_zone.Length -
                    (width_of_zone[i - 1] >> 1)));
            }
            else
            {
                //m.y
                y = ((index - prev) + (width_of_zone.Length -
                    (width_of_zone[2 * width_of_zone.Length - i] >> 1)));
            }

            if (revertY)
                y = (47 - y);

            if (revertX)
                x = (47 - x);

            if (!switchXY)
                return new Coords((byte)y, (byte)x);
            else
                return new Coords((byte)x, (byte)y);
        }

        static private Coords ComputeFrom2448(int idx, bool revertX, bool revertY, bool switchXY)
        {
            int i;
            int prev = 0;
            int next = width_of_zone[0];
            idx -= skip_wide_zone[0];
            for (i = 1; i < width_of_zone.Length; i++)
            {
                if ((idx >= prev) && (idx < (prev + next)))
                {
                    goto index_found;
                }
                prev += next;
                next = width_of_zone[i];
                idx -= skip_wide_zone[i];
            }
            for (i = width_of_zone.Length; i < 2 * width_of_zone.Length; i++)
            {
                if ((idx >= prev) && (idx < (prev + next)))
                {
                    goto index_found;
                }
                prev += next;
                next = width_of_zone[2 * width_of_zone.Length - i - 1];
                idx -= skip_wide_zone[i];
            }
            if ((idx >= prev) && (idx < (prev + next)))
            {
                goto index_found;
            }

            //Bad index!
            //m.x = m.y = 255;
            return Coords.incorrect;

        index_found:
            int x, y;
            //m.x
            x = (byte)(2 * width_of_zone.Length - i);
            if (i <= width_of_zone.Length)
            {
                //m.y
                y = (byte)((idx - prev) + (width_of_zone.Length -
                    (width_of_zone[i - 1] >> 1)));
            }
            else
            {
                //m.y
                y = (byte)((idx - prev) + (width_of_zone.Length -
                    (width_of_zone[2 * width_of_zone.Length - i] >> 1)));
            }


            if (revertY)
                y = (47 - y);

            if (revertX)
                x = (47 - x);

            if (!switchXY)
                return new Coords((byte)y, (byte)x);
            else
                return new Coords((byte)x, (byte)y);
        }

        private static int[] width_of_zone = 
	         { 14, 20, 24, 28, 30, 32, 34, 36, 38, 40, 42, 42, 
               44, 44, 46, 46, 46, 48, 48, 48, 48, 48, 48, 48};
        private static int[] skip_wide_zone =
              { 91, 16, 13, 11,     10, 10, 10, 10,
                10, 9, 8, 8,    8, 8, 8, 8,
                8, 7, 6, 7,     8, 8, 8, 8,
                8,
	            8, 8, 8, 8,     7, 6, 7, 8,
	            8, 8, 8, 8,     8, 8, 9, 10,
	            10, 10, 10, 10, 11, 13, 16, 0};
    }
}