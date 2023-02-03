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
#endif

    [TestFixture]
    public class  CoordsConverterTest
    {

        [Test]
        public void TestLinear1884()
        {
            CoordsConverter c0 = CoordsConverter.LoadLinear1884(false, false, false);
            CoordsConverter c1 = CoordsConverter.LoadLinear1884(false, false, true);
            CoordsConverter c2 = CoordsConverter.LoadLinear1884(false, true, false);
            CoordsConverter c3 = CoordsConverter.LoadLinear1884(false, true, true);

            CoordsConverter c4 = CoordsConverter.LoadLinear1884(true, false, false);
            CoordsConverter c5 = CoordsConverter.LoadLinear1884(true, false, true);
            CoordsConverter c6 = CoordsConverter.LoadLinear1884(true, true, false);
            CoordsConverter c7 = CoordsConverter.LoadLinear1884(true, true, true);
            
            Coords crd0 = c0[0];
            Coords crd1 = c1[0];
            Coords crd2 = c2[0];
            Coords crd3 = c3[0];
            Coords crd4 = c4[0];
            Coords crd5 = c5[0];
            Coords crd6 = c6[0];
            Coords crd7 = c7[0];

            Assert.IsTrue(crd0.IsOk);
            Assert.IsTrue(crd1.IsOk);
            Assert.IsTrue(crd2.IsOk);
            Assert.IsTrue(crd3.IsOk);
            Assert.IsTrue(crd4.IsOk);
            Assert.IsTrue(crd5.IsOk);
            Assert.IsTrue(crd6.IsOk);
            Assert.IsTrue(crd7.IsOk);

            Assert.AreEqual(c0[crd0], 0);
            Assert.AreEqual(c1[crd1], 0);
            Assert.AreEqual(c2[crd2], 0);
            Assert.AreEqual(c3[crd3], 0);
            Assert.AreEqual(c4[crd4], 0);
            Assert.AreEqual(c5[crd5], 0);
            Assert.AreEqual(c6[crd6], 0);
            Assert.AreEqual(c7[crd7], 0);
        }

        [Test]
        public void TestWideLinear2448()
        {
            CoordsConverter c0 = CoordsConverter.LoadWideLinear2448(false, false, false);
            CoordsConverter c1 = CoordsConverter.LoadWideLinear2448(false, false, true);
            CoordsConverter c2 = CoordsConverter.LoadWideLinear2448(false, true, false);
            CoordsConverter c3 = CoordsConverter.LoadWideLinear2448(false, true, true);

            CoordsConverter c4 = CoordsConverter.LoadWideLinear2448(true, false, false);
            CoordsConverter c5 = CoordsConverter.LoadWideLinear2448(true, false, true);
            CoordsConverter c6 = CoordsConverter.LoadWideLinear2448(true, true, false);
            CoordsConverter c7 = CoordsConverter.LoadWideLinear2448(true, true, true);

            Coords crd0 = c0[100];
            Coords crd1 = c1[100];
            Coords crd2 = c2[100];
            Coords crd3 = c3[100];
            Coords crd4 = c4[100];
            Coords crd5 = c5[100];
            Coords crd6 = c6[100];
            Coords crd7 = c7[100];

            Assert.IsTrue(crd0.IsOk);
            Assert.IsTrue(crd1.IsOk);
            Assert.IsTrue(crd2.IsOk);
            Assert.IsTrue(crd3.IsOk);
            Assert.IsTrue(crd4.IsOk);
            Assert.IsTrue(crd5.IsOk);
            Assert.IsTrue(crd6.IsOk);
            Assert.IsTrue(crd7.IsOk);

            Assert.AreEqual(c0[crd0], 100);
            Assert.AreEqual(c1[crd1], 100);
            Assert.AreEqual(c2[crd2], 100);
            Assert.AreEqual(c3[crd3], 100);
            Assert.AreEqual(c4[crd4], 100);
            Assert.AreEqual(c5[crd5], 100);
            Assert.AreEqual(c6[crd6], 100);
            Assert.AreEqual(c7[crd7], 100);
        }

    }
}
