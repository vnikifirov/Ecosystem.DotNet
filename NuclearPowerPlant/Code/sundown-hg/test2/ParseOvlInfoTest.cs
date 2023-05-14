using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using NUnit.Framework;

using corelib;
using Algorithms;

namespace Algorithms
{
#if !DOTNET_V11
    using DataCartogramNativeDouble = DataCartogramNative<double>;
    using DataArrayCoords = DataArray<Coords>;
    using DataCartogramIndexedCoords = DataCartogramIndexed<Coords>;
    using DataCartogramIndexedFloat = DataCartogramIndexed<float>;
#endif

    [TestFixture]
    public class ParseOvlInfoTest
    {
        [Test]
        public void Open()
        {
            ParseOvlInfo oi = new ParseOvlInfo("1_280509.txt");

        }

    }
}
