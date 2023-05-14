using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using System.Reflection;

using NUnit.Framework;


using corelib;
namespace test2
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
    public class DynTypingTest
    {
        TupleMetaData _info = new TupleMetaData("test", "test", DateTime.Now, TupleMetaData.StreamAuto);
        EnvConverterFormatter _conv = new EnvConverterFormatter(
            new CartogramPresentationConfig(true, false, true),
            new CartogramPresentationConfig(true, false, true));


        DataCartogramIndexedInt CreateCartogram()
        {
            int[] a = new int[1884];
            for (int i = 0; i < 1884; i++)
                a[i] = i;

            return new DataCartogramIndexedInt(_info, ScaleIndex.Default, _conv.Linear1884, a);
        }

        DataArrayInt CreateArray()
        {
            int[] simple = { 1, 2, 3, 4, 5, 6, 7 };

             return new DataArrayInt(_info, simple);
        }

        [Test]
        public void TestEnum()
        {
            Type[] ty = Component.GetAllTypes(typeof(corelib.DrawCartCell), null);

            Type[] tyd = Component.GetAllTypes(typeof(corelib.ITupleItem), "Data");
        }

        [Test]
        public void TestPlugin()
        {
            Plugins n = new Plugins(typeof(corelib.DrawCartCell), "DrawCart");

            foreach (Type t in n)
            {
                System.Diagnostics.Debug.WriteLine(t.FullName);
            }

            object o = n.Create("Level");
        }

        public static void TestMethod(double[,] pvk_maxes, double[] kgoprp_info, out double i)
        {
            i = 9.892;
        }

        [Test]
        public void TestAttribute()
        {
            AttributeRules c = new AttributeRules(@"average('Av index') is (pvk_maxes as double[native], kgoprp_info as double[]); returns is (i as double)");
            MethodInfo mi = this.GetType().GetMethod("TestMethod");
            ParameterInfo[] info = mi.GetParameters();

            object[] objects = new object[info.Length];

            corelib.DataTuple t1 = new corelib.DataTuple("s", DateTime.Now, CreateCartogram().Rename("pvk_maxes"));
            corelib.DataTuple t2 = new corelib.DataTuple("d", DateTime.Now, CreateArray().Rename("kgoprp_info"));

            MultiDataTuple.CreateParamFromTuple(_conv, objects, info, c.InputRules("average").Rule, null, t1, t2);

            object res = mi.Invoke(null, objects);

            Assert.AreEqual((double)objects[2], 9.892);
        }


        [Test]
        public void TestAttributeMaps()
        {
            AttributeRules c = new AttributeRules(@"average is (pvk_maxes as double[native], kgoprp_info as double[]); returns is (i as double)");
            MethodInfo mi = this.GetType().GetMethod("TestMethod");
            ParameterInfo[] info = mi.GetParameters();

            object[] objects = new object[info.Length];

            corelib.DataTuple t1 = new corelib.DataTuple("s", DateTime.Now, CreateCartogram().Rename("pvk"));
            corelib.DataTuple t2 = new corelib.DataTuple("d", DateTime.Now, CreateArray().Rename("kgoprp"));

            corelib.TupleMaps maps = new corelib.TupleMaps("pvk as pvk_maxes, kgoprp as kgoprp_info");

            MultiDataTuple.CreateParamFromTuple(_conv, objects, info, c.InputRules("average").Rule, maps, t1, t2);

            object res = mi.Invoke(null, objects);

            Assert.AreEqual((double)objects[2], 9.892);
        }


        [Test]
        public void TestAttributeDoubleMaps()
        {
            AttributeRules c = new AttributeRules(@"average is (pvk_maxes as double[native]); min is(kgoprp_info as double[]); returns is (i as double)");
            MethodInfo mi = this.GetType().GetMethod("TestMethod");
            ParameterInfo[] info = mi.GetParameters();

            object[] objects = new object[info.Length];

            corelib.DataTuple t1 = new corelib.DataTuple("s", DateTime.Now, CreateCartogram().Rename("pvk"));
            corelib.DataTuple t2 = new corelib.DataTuple("d", DateTime.Now, CreateArray().Rename("kgoprp"));

            corelib.TupleMaps maps = new corelib.TupleMaps("pvk as pvk_maxes, kgoprp as kgoprp_info");

            MultiDataTuple.CreateParamFromTuple(_conv, objects, info, c.InputRules("average").Rule, maps, t1, t2);
            MultiDataTuple.CreateParamFromTuple(_conv, objects, info, c.InputRules("min").Rule, maps, t1, t2);

            object res = mi.Invoke(null, objects);

            Assert.AreEqual((double)objects[2], 9.892);
        }

        public static void TestMethod2(double[,] pvk_maxes, double[] kgoprp_info, Coords[] pvk_maxes2, out double i)
        {
            i = 9.892;
        }

        [Test]
        public void TestAttributeStorageDoubleMaps()
        {
            AttributeRules c = new AttributeRules(@"average is (pvk_maxes as double[native] to pvk_maxes, pvk_maxes as coords[] to pvk_maxes2); min is(kgoprp_info as double[]); return is (i('Force variable') as ParamTable[micro])");
            MethodInfo mi = this.GetType().GetMethod("TestMethod2");
            ParameterInfo[] info = mi.GetParameters();

            object[] objects = new object[info.Length];

            corelib.DataTuple t1 = new corelib.DataTuple("s", DateTime.Now, CreateCartogram().Rename("pvk"));
            corelib.DataTuple t2 = new corelib.DataTuple("d", DateTime.Now, CreateArray().Rename("kgoprp"));

            corelib.TupleMaps maps = new corelib.TupleMaps("pvk as pvk_maxes, kgoprp as kgoprp_info");

            MultiDataTuple.CreateParamFromTuple(_conv, objects, info, c.InputRules("average").Rule, maps, t1, t2);
            MultiDataTuple.CreateParamFromTuple(_conv, objects, info, c.InputRules("min").Rule, maps, t1, t2);

            object res = mi.Invoke(null, objects);

            Assert.AreEqual((double)objects[3], 9.892);

            IMultiDataTuple res2 = MultiDataTuple.CreateTupleFromParam(_conv, c.OutputRules.Rule, info, objects);

            Assert.IsNotNull(res2);
        }

        public static void TestMethod3(out double m, out float e, out double[,] t, out int[] q)
        {
            m = 98.3;
            e = 2.33f;
            t = new double[48, 48];
            q = new int[5];
        }


        [Test]
        public void TestAttributeReturn()
        {
            AttributeRules c = new AttributeRules(
                @"return is (i('Force variable') as ParamTable[micro, step] to (m,e), 
                             cart('Cartogram of load') as Cart to t,
                             coeff('Coeff of load') as Array to q ) ");

            MethodInfo mi = this.GetType().GetMethod("TestMethod3");
            ParameterInfo[] info = mi.GetParameters();

            object[] objects = new object[info.Length];

            object res = mi.Invoke(null, objects);

            IDataTuple t = MultiDataTuple.CreateTupleFromParam(_conv, c.OutputRules.Rule, info, objects)[0];

            DataParamTable i1 = (DataParamTable) t["i"];
            DataCartogram i2 = (DataCartogram)t["cart"];
            DataArray i3 = (DataArray)t["coeff"];
        }


        public static void TestMethod4(out double[] m, out float[] e, out double[][,] t, out int[][] q)
        {
            m = new double[2] { 98.3, 33.4 };
            e = new float[2] { 2.33f, 33.3f };
            t = new double[2][,] { new double[48, 48], new double[48, 48] };
            q = new int[2][] { new int[5], new int[7] };
        }

        [Test]
        public void TestAttributeMultiReturn()
        {
            AttributeRules c = new AttributeRules(
                @"return[] is (i('Force variable') as ParamTable[micro, step] to (m,e), 
                             cart('Cartogram of load') as Cart to t,
                             coeff('Coeff of load') as Array to q ) ");

            MethodInfo mi = this.GetType().GetMethod("TestMethod4");
            ParameterInfo[] info = mi.GetParameters();

            object[] objects = new object[info.Length];

            object res = mi.Invoke(null, objects);

            IMultiDataTuple t = MultiDataTuple.CreateTupleFromParam(_conv, c.OutputRules.Rule, info, objects);

            DataParamTable i1 = (DataParamTable)t[0]["i"];
            DataCartogram i2 = (DataCartogram)t[0]["cart"];
            DataArray i3 = (DataArray)t[0]["coeff"];

            Assert.AreEqual((double)i1["micro"], 98.3);
            Assert.AreEqual((float)i1["step"], 2.33f);

            DataParamTable ii1 = (DataParamTable)t[1]["i"];
            DataCartogram ii2 = (DataCartogram)t[1]["cart"];
            DataArray ii3 = (DataArray)t[1]["coeff"];

            Assert.AreEqual((double)ii1["micro"], 33.4);
            Assert.AreEqual((float)ii1["step"], 33.3f);

        }
    }
}
