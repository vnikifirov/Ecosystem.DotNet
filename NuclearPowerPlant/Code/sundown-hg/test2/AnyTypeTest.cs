using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using NUnit.Framework;

namespace corelib
{
    [TestFixture]
    public class AnyTypeTest
    {
        [Test]
        public void TestCreate()
        {
            int i = 6;
            AnyValue ai = i;

            short j = 7;
            AnyValue aj = j;

            byte b = 79;
            AnyValue ab = b;

            float f = 78.2f;
            AnyValue af = f;

            double d = 21.2;
            AnyValue ad = d;

            Sensored s = new Sensored(78, Sensored.DataState.Ok);
            AnyValue ase = s;


            Assert.AreEqual(i, (int)ai);
            Assert.AreEqual(j, (short)aj);
            Assert.AreEqual(b, (byte)ab);
            Assert.AreEqual(f, (float)af);
            Assert.AreEqual(d, (double)ad);
            Assert.AreEqual(s, (Sensored)ase);


            Assert.IsTrue(ai.IsInt);
            Assert.IsTrue(aj.IsShort);
            Assert.IsTrue(ab.IsByte);
            Assert.IsTrue(af.IsFloat);
            Assert.IsTrue(ad.IsDouble);
            Assert.IsTrue(ase.IsSensored);
        }

        [Test]
        public void TestScale()
        {
            float scale = 10;

            double oi = 60;
            int i = 6;
            AnyValue ai = new AnyValue(i,scale);

            double oj = 70;
            short j = 7;
            AnyValue aj = new AnyValue(j,scale);

            double ob = 790;
            byte b = 79;
            AnyValue ab = new AnyValue(b,scale);

            double of = 782.5;
            float f = 78.25f;
            AnyValue af = new AnyValue(f,scale);

            double od = 212;
            double d = 21.2;
            AnyValue ad = new AnyValue(d, scale);

            Assert.AreEqual(oi, ai.ValueScaled);
            Assert.AreEqual(oj, aj.ValueScaled);
            Assert.AreEqual(ob, ab.ValueScaled);
            Assert.AreEqual(of, af.ValueScaled);
            Assert.AreEqual(od, ad.ValueScaled);

            Assert.IsFalse(ai.IsInt);
            Assert.IsFalse(aj.IsShort);
            Assert.IsFalse(ab.IsByte);
            Assert.IsFalse(af.IsFloat);
            Assert.IsFalse(ad.IsDouble);   
        }

    }
}
