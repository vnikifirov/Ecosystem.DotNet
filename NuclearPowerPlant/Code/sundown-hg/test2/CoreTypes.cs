using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Globalization;

using NUnit.Framework;
using corelib;

namespace test2
{
    [TestFixture]
    public class CoreTypes
    {
        [Test]
        public void CoordsHumane()
        {
            Coords c = Coords.FromHumane("12-45");

            Assert.AreEqual(c.IsOk, true);
            //Assert.AreEqual(c.HumaneX, 12);
            //Assert.AreEqual(c.HumaneY, 45);
        }

        /*
#if !DOTNET_V11
        [Test]
        public void DataArrayCreate()
        {
            int[] e = new int[10];
            e[0] = 10;

            DataArray r = new DataArray("dd", "dd", DateTime.Now, e);

            IDataArrayWriter wr = r;

            object i = wr[0];
            wr[2] = i;

            IDataArrayAccessorReadOnly<int> a = r.GetAccessorReadOnly<int>();
            int c = a[2];

            double cc = r.Sum();

            DataArray.AccessorReadOnly<int> b = r.GetAccessorReadOnly<int>();
            int tt = b[3];

        }
#endif
        */
    }
}
