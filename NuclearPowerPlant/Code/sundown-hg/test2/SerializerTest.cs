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
    public class SerializerTest
    {
        void Check(AwfulSerializer a, byte[] orig)
        {
            for (int i = 0; i < orig.Length; i++)
            {
                Assert.AreEqual(a.Data[i], orig[i]);
            }
        }

        [Test]
        public void ArrayInt1()                
        {
            AwfulSerializer sr = new AwfulSerializer(0);

            int[] testAr = { 0, 1, 2, 3, 4 };
            byte[] shouldBe = { 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0 };
            sr.Put(testAr);

            Check(sr, shouldBe);
        }

        [Test]
        public void ArrayShort1()
        {
            AwfulSerializer sr = new AwfulSerializer(0);

            short[] testAr = { 0, 1, 2, 3, 4 };
            byte[] shouldBe = { 0, 0, 1, 0, 2, 0, 3, 0, 4, 0};
            sr.Put(testAr);

            Check(sr, shouldBe);
        }

        [Test]
        public void ArrayByte1()
        {
            AwfulSerializer sr = new AwfulSerializer(0);

            byte[] testAr = { 0, 1, 2, 3, 4 };
            byte[] shouldBe = { 0, 1, 2, 3, 4 };
            sr.Put(testAr);

            Check(sr, shouldBe);
        }


        [Test]
        public void ArrayInt2()
        {
            AwfulSerializer sr = new AwfulSerializer(0);

            int[,] testAr = { { 0, 1 }, { 2, 3 }, { 4, 5 } };
            byte[] shouldBe = { 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0 };
            sr.Put(testAr);

            Check(sr, shouldBe);
        }

        [Test]
        public void ArrayInt3()
        {
            AwfulSerializer sr = new AwfulSerializer(0);

            int[, ,] testAr = { { { 0, 1 }, { 2, 3 } }, { { 4, 5 }, { 6, 7 } } };
            byte[] shouldBe = { 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 6, 0, 0, 0, 7, 0, 0, 0 };
            sr.Put(testAr);

            Check(sr, shouldBe);
        }


        [Test]
        public void String()
        {
            AwfulSerializer sr = new AwfulSerializer(0);

            byte[] shoudBe = { 10, 0, 0, 0, 72, 0, 101, 0, 108, 0, 108, 0, 111, 0 };
            string testh = "Hello";
            sr.Put(testh);

            Check(sr, shoudBe);

            DateTime[] d = { DateTime.Now };
            sr.Put(d);
            double[] c = { d[0].ToOADate() };
            sr.Put(c);

        }
    }
}
