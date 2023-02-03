using System;
using System.Reflection;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.IO;
using System.Text;
using System.Data;

using NUnit.Framework;

using corelib;

using SqliteStorage;

namespace Sql
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
    public class TestDataSql2
    {
        EnvConverterFormatter _conv = new EnvConverterFormatter(
            new CartogramPresentationConfig(true, false, true),
            new CartogramPresentationConfig(true, false, true));

        [Test]
        public void CreateDb()
        {
            if (File.Exists("test_test.dq3"))
                File.Delete("test_test.dq3");

            using (SqliteProvider2 p = new SqliteProvider2(_conv, "test_test.dq3", false, 0))
            {
                string[] streams = p.GetStreamNames();
                DateTime[] dates = p.GetDates();

                Assert.AreEqual(streams.Length, 0);
                Assert.AreEqual(dates.Length, 0);
            }
        }

        static void Compare(TupleMetaData orig, TupleMetaData e)
        {
            Assert.AreEqual(orig.Date, e.Date);
            Assert.AreEqual(orig.HumaneName, e.HumaneName);
            Assert.AreEqual(orig.Name, e.Name);
        }

        [Test]
        public void PushData1DT2()
        {
            using  (SqliteProvider2 p = new SqliteProvider2(_conv, "test_test.dq3", false, 0))
            {
                TupleMetaData info = new TupleMetaData("test", "test", DateTime.Now, TupleMetaData.StreamAuto);
                TupleMetaData info2 = new TupleMetaData("test2", "test2", DateTime.Now, TupleMetaData.StreamAuto);

                int[,] a = new int[48, 48];
                DataCartogramNativeInt c = new DataCartogramNativeInt(info, ScaleIndex.Default, a);
                DataCartogramNativeInt c2 = new DataCartogramNativeInt(info2, new ScaleIndex(0, 10), a);

                DataTuple pdata = new DataTuple("p", DateTime.Now, c, c2);

                p.PushData(pdata);


                string[] streams = p.GetStreamNames();
                DateTime[] dates = p.GetDates();

                TupleMetaData db1 = p.GetTupleItemInfo(pdata.GetTimeDate(), info.Name);
                TupleMetaData db2 = p.GetTupleItemInfo(pdata.GetTimeDate(), info2.Name);


                Compare(info, db1);
                Compare(info2, db2);

                DataTuple rdata = (DataTuple)p.GetData(pdata.GetTimeDate(), pdata.GetStreamName());
            }
        }
    }
}