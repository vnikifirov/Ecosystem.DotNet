using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using NUnit.Framework;

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
#endif

    [TestFixture]
    public class DataCartogramTest
    {
        TupleMetaData _info = new TupleMetaData("test", "test", DateTime.Now, TupleMetaData.StreamAuto);
        EnvConverterFormatter _conv = new EnvConverterFormatter(
            new CartogramPresentationConfig(true, false, true),
            new CartogramPresentationConfig(true, false, true));

        [Test]
        public void TestCreateNativeInt()
        {
            int[,] a = new int[48, 48];

            for (int x = 0; x < 48; x++)
                for (int y = 0; y < 48; y++)
                {
                    a[x, y] = x + y + x * (y + 1);
                }

            DataCartogramNativeInt c = new DataCartogramNativeInt(_info, ScaleIndex.Default, a);
            DataCartogramNativeInt c2 = new DataCartogramNativeInt(_info, new ScaleIndex(0, 10), a);
            DataCartogramNativeInt c3 = new DataCartogramNativeInt(_info, new ScaleIndex(0, 0.1), a);
            DataCartogramNativeInt c4 = new DataCartogramNativeInt(_info, new ScaleIndex(1, 1), a);

            for (int x = 0; x < 48; x++)
                for (int y = 0; y < 48; y++)
                {
                    Coords crd = new Coords((byte)x, (byte)y);

                    Assert.AreEqual(a[x, y], c.GetItem(crd, 0));
                    Assert.AreEqual(a[x, y], c2.GetItem(crd, 0));
                    Assert.AreEqual(a[x, y], c3.GetItem(crd, 0));
                    Assert.AreEqual(a[x, y], c4.GetItem(crd, 0));

                    Assert.AreEqual(a[x, y], Convert.ToInt32(c[crd, 0]));
                    Assert.AreEqual(a[x, y], Convert.ToInt32(c2[crd, 0] / 10));
                    Assert.AreEqual(a[x, y], Convert.ToInt32(c4[crd, 0] - 1));
                }
        }

        [Test]
        public void TestCreateIndexedInt()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, ScaleIndex.Default, _conv.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, _info), a);



            for (int i = 0; i < 1884; i++)
            {
                Assert.AreEqual(a[i], c.GetItem(i, 0));
                Assert.AreEqual(a[i], Convert.ToInt32(c[i, 0]));
            }
        }

        [Test]
        public void TestConvertIdxToNInt()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0,10), _conv.Linear1884, a);

            DataCartogramNativeDouble d = c.ScaleToNative(_info);
            DataCartogramIndexedDouble b = c.ScaleToIndexed(_info, c.Coords);
            DataCartogramIndexedShort dc = (DataCartogramIndexedShort)d.ConvertToIndexedType(_info, c.Coords, typeof(short));

            DataCartogramIndexedInt ccc = (DataCartogramIndexedInt)d.ConvertToIndexedType(_info, _conv.WideLinear2448, typeof(int));

            for (int i = 0; i < 1884; i++)
            {
                Coords crd = c.Coords[i];

                Assert.AreEqual(a[i], (int)(c[crd, 0] / 10));
                Assert.AreEqual(a[i], (int)(d[crd, 0] / 10));
                Assert.AreEqual(a[i], (int)(b[i, 0] / 10));

                Assert.AreEqual(a[i], (int)(dc[crd, 0] / 10));

                Assert.AreEqual(a[i], (int)(ccc[crd, 0] / 10));

            }
        }





        const int iterPerf = 3000;
        [Test]
        public void TestScalePerfOp()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);            
            for (int i = 0; i < iterPerf; i++)
            {
                DataCartogramIndexedDouble b = c.Scale(_info);
            }
        }

        [Test]
        public void TestScalePerfConv()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            for (int i = 0; i < iterPerf; i++)
            {
                DataCartogramIndexedDouble b = c.ScaleToIndexed(_info, c.Coords);
            }
        }

        [Test]
        public void TestScalePerfOdinary()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            for (int i = 0; i < iterPerf; i++)
            {
                double[] b = new double[1884];
                for (int j = 0; j < 1884; j++)
                    b[j] = c[j,0];


                DataCartogramIndexedDouble bb = new DataCartogramIndexedDouble(_info, ScaleIndex.Default, _conv.Linear1884, b);
            }
        }

        [Test]
        public void TestScalePerfVoid()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;
            
            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            double[] b = new double[1884];
            for (int j = 0; j < 1884; j++)
                b[j] = c[j, 0];            

            for (int i = 0; i < iterPerf; i++)
            {            
                DataCartogramIndexedDouble bb = new DataCartogramIndexedDouble(_info, ScaleIndex.Default, _conv.Linear1884, b);
            }
        }

        [Test]
        public void TestScalePerfVoidConv()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            double[] b = new double[1884];
            for (int i = 0; i < iterPerf; i++)
            {
                for (int j = 0; j < 1884; j++)
                    b[j] = c[j, 0];

                DataCartogramIndexedDouble bb = new DataCartogramIndexedDouble(_info, ScaleIndex.Default, _conv.Linear1884, b);
            }
        }

        const bool closeExcel = true;
        void CloseExcel(corelib.OleObject o)
        {            
            o.GetProperty("ActiveWorkbook").SetProperty("Saved", true);
            if (closeExcel)
                o.Invoke("Quit");
        }

        [Test]
        public void TestDataGrid()
        {
            Sensored[] a = new Sensored[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = new Sensored((short)i, Sensored.DataState.Ok);

            DataCartogramIndexedSensored c = new DataCartogramIndexedSensored(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            DataGrid d = c.CreateDataGrid(_conv).ExpandAll();
            CloseExcel(d.ExportExcel());
        }


        [Test]
        public void TestCastTo()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            DataCartogramNativeDouble d = (DataCartogramNativeDouble)c.CastTo(null, new AttributeTypeRules("a as Cart(native,double,scale)"));


        }


        [Test]
        public void TestCastToNew()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            DataCartogramIndexedInt c = new DataCartogramIndexedInt(_info, new ScaleIndex(0, 10), _conv.Linear1884, a);

            double[,] de = (double[,])c.CastTo(null, new AttributeTypeRules("a as double[native]"));
        }

        [Test]
        public void TestCastToNew2()
        {
            int[,] a = new int[48, 48];

            for (int x = 0; x < 48; x++)
                for (int y = 0; y < 48; y++)
                {
                    a[x, y] = x + y + x * (y + 1);
                }

            DataCartogramNativeInt c = new DataCartogramNativeInt(_info, ScaleIndex.Default, a);

            double[] d = (double[])c.CastTo(_conv, new AttributeTypeRules("a as double[linear]"));
        }
    }
}
