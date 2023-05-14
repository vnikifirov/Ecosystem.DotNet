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
    public class DataArrayTest
    {
        TupleMetaData _info = new TupleMetaData("test", "test", DateTime.Now, TupleMetaData.StreamAuto);

        [Test]
        public void TestCreate1()
        {
            int[] simple = { 1, 2, 3, 4, 5, 6, 7 };

            DataArrayInt a = new DataArrayInt(_info, simple);

            Assert.AreEqual(simple[0], a[0]);
            Assert.AreEqual(simple[4], a[4]);

            Assert.AreEqual(simple.Rank, a.Rank);
            Assert.AreEqual(simple.Length, a.Length);

            simple[0] = 1000;

            Assert.AreNotEqual(simple[0], a[0]);
        }

        [Test]
        public void TestCreate2()
        {
            int[,] simple = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            DataArrayInt a = new DataArrayInt(_info, simple);

            Assert.AreEqual(simple[0, 0], a[0, 0]);
            Assert.AreEqual(simple[3, 1], a[3, 1]);

            Assert.AreEqual(simple.Rank, a.Rank);
            Assert.AreEqual(simple.Length, a.Length);

            simple[0, 0] = 1000;

            Assert.AreNotEqual(simple[0, 0], a[0, 0]);
        }

        [Test]
        public void TestCreate3()
        {
            int[, ,] simple = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };

            DataArrayInt a = new DataArrayInt(_info, simple);

            Assert.AreEqual(simple[0, 0, 0], a[0, 0, 0]);
            Assert.AreEqual(simple[1, 1, 0], a[1, 1, 0]);

            Assert.AreEqual(simple.Rank, a.Rank);
            Assert.AreEqual(simple.Length, a.Length);

            simple[0, 0, 0] = 1000;

            Assert.AreNotEqual(simple[0, 0, 0], a[0, 0, 0]);
        }


        [Test]
        public void TestConvert1()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            double[] simpled = { 1, 2, 3, 4, 5, 6, 7, 8 };
            float[] simplef = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] simpleb = { 1, 2, 3, 4, 5, 6, 7, 8 };
            short[] simples = { 1, 2, 3, 4, 5, 6, 7, 8 };
            Sensored[] simplesn = { new Sensored(1, Sensored.DataState.Ok),
                                    new Sensored(2, Sensored.DataState.Ok),
                                    new Sensored(3, Sensored.DataState.Ok),
                                    new Sensored(4, Sensored.DataState.Ok),
                                    new Sensored(5, Sensored.DataState.Ok),
                                    new Sensored(6, Sensored.DataState.Ok),
                                    new Sensored(7, Sensored.DataState.Ok),
                                    new Sensored(8, Sensored.DataState.Ok)};

            DataArray[] origs = {
                                    new DataArrayInt(_info, simplei),
                                    new DataArrayDouble(_info, simpled),
                                    new DataArrayFloat(_info, simplef),
                                    new DataArrayByte(_info, simpleb),
                                    new DataArrayShort(_info, simples),
                                    new DataArraySensored(_info, simplesn)
                                };
            foreach (DataArray orig in origs)
            {
                DataArrayInt a = (DataArrayInt)orig.ConvertTo(_info, typeof(int));
                DataArrayDouble b = (DataArrayDouble)orig.ConvertTo(_info, typeof(double));
                DataArrayFloat c = (DataArrayFloat)orig.ConvertTo(_info, typeof(float));
                DataArrayByte d = (DataArrayByte)orig.ConvertTo(_info, typeof(byte));
                DataArrayShort e = (DataArrayShort)orig.ConvertTo(_info, typeof(short));

                DataArray[] mods = { a, b, c, d, e };
                foreach (DataArray mod in mods)
                {
                    Assert.AreEqual(mod.Rank, orig.Rank);
                    Assert.AreEqual(mod.Length, orig.Length);

                    Assert.AreEqual(mod.DimX, orig.DimX);
                    Assert.AreEqual(mod.DimY, orig.DimY);
                    Assert.AreEqual(mod.DimZ, orig.DimZ);
                }

                for (int i = 0; i < orig.Length; i++)
                {
                    Assert.AreEqual(simplei[i], a[i]);
                    Assert.AreEqual(simpled[i], b[i]);
                    Assert.AreEqual(simplef[i], c[i]);
                    Assert.AreEqual(simpleb[i], d[i]);
                    Assert.AreEqual(simples[i], e[i]);
                }
            }
        }

        [Test]
        public void TestConvert2()
        {
            int[,] simplei = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            double[,] simpled = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            float[,] simplef = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            short[,] simples = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            Sensored[,] simplesn = {{ new Sensored(1, Sensored.DataState.Ok),
                                    new Sensored(2, Sensored.DataState.Ok)},
                                    {new Sensored(3, Sensored.DataState.Ok),
                                    new Sensored(4, Sensored.DataState.Ok)},
                                    {new Sensored(5, Sensored.DataState.Ok),
                                    new Sensored(6, Sensored.DataState.Ok)},
                                    {new Sensored(7, Sensored.DataState.Ok),
                                    new Sensored(8, Sensored.DataState.Ok)}};
            DataArray[] origs = {
                                    new DataArrayInt(_info, simplei),
                                    new DataArrayDouble(_info, simpled),
                                    new DataArrayFloat(_info, simplef),
                                    new DataArrayByte(_info, simpleb),
                                    new DataArrayShort(_info, simples),
                                    new DataArraySensored(_info, simplesn)
                                };
            foreach (DataArray orig in origs)
            {
                DataArrayInt a = (DataArrayInt)orig.ConvertTo(_info, typeof(int));
                DataArrayDouble b = (DataArrayDouble)orig.ConvertTo(_info, typeof(double));
                DataArrayFloat c = (DataArrayFloat)orig.ConvertTo(_info, typeof(float));
                DataArrayByte d = (DataArrayByte)orig.ConvertTo(_info, typeof(byte));
                DataArrayShort e = (DataArrayShort)orig.ConvertTo(_info, typeof(short));

                DataArray[] mods = { a, b, c, d, e };
                foreach (DataArray mod in mods)
                {
                    Assert.AreEqual(mod.Rank, orig.Rank);
                    Assert.AreEqual(mod.Length, orig.Length);

                    Assert.AreEqual(mod.DimX, orig.DimX);
                    Assert.AreEqual(mod.DimY, orig.DimY);
                    Assert.AreEqual(mod.DimZ, orig.DimZ);
                }

                for (int i = 0; i < orig.DimX; i++)
                    for (int j = 0; j < orig.DimY; j++)
                    {
                        Assert.AreEqual(simplei[i, j], a[i, j]);
                        Assert.AreEqual(simpled[i, j], b[i, j]);
                        Assert.AreEqual(simplef[i, j], c[i, j]);
                        Assert.AreEqual(simpleb[i, j], d[i, j]);
                        Assert.AreEqual(simples[i, j], e[i, j]);
                    }
            }
        }

        [Test]
        public void TestConvert3()
        {
            int[, ,] simplei = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            double[, ,] simpled = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            float[, ,] simplef = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            byte[, ,] simpleb = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            short[, ,] simples = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            Sensored[, ,] simplesn = {{{ new Sensored(1, Sensored.DataState.Ok),
                                    new Sensored(2, Sensored.DataState.Ok)},
                                    {new Sensored(3, Sensored.DataState.Ok),
                                    new Sensored(4, Sensored.DataState.Ok)}},
                                    {{new Sensored(5, Sensored.DataState.Ok),
                                    new Sensored(6, Sensored.DataState.Ok)},
                                    {new Sensored(7, Sensored.DataState.Ok),
                                    new Sensored(8, Sensored.DataState.Ok)}}};

            DataArray[] origs = {
                                    new DataArrayInt(_info, simplei),
                                    new DataArrayDouble(_info, simpled),
                                    new DataArrayFloat(_info, simplef),
                                    new DataArrayByte(_info, simpleb),
                                    new DataArrayShort(_info, simples),
                                    new DataArraySensored(_info, simplesn)
                                };
            foreach (DataArray orig in origs)
            {
                DataArrayInt a = (DataArrayInt)orig.ConvertTo(_info, typeof(int));
                DataArrayDouble b = (DataArrayDouble)orig.ConvertTo(_info, typeof(double));
                DataArrayFloat c = (DataArrayFloat)orig.ConvertTo(_info, typeof(float));
                DataArrayByte d = (DataArrayByte)orig.ConvertTo(_info, typeof(byte));
                DataArrayShort e = (DataArrayShort)orig.ConvertTo(_info, typeof(short));

                DataArray[] mods = { a, b, c, d, e };
                foreach (DataArray mod in mods)
                {
                    Assert.AreEqual(mod.Rank, orig.Rank);
                    Assert.AreEqual(mod.Length, orig.Length);

                    Assert.AreEqual(mod.DimX, orig.DimX);
                    Assert.AreEqual(mod.DimY, orig.DimY);
                    Assert.AreEqual(mod.DimZ, orig.DimZ);
                }

                for (int i = 0; i < orig.DimX; i++)
                    for (int j = 0; j < orig.DimY; j++)
                        for (int k = 0; k < orig.DimZ; k++)
                        {
                            Assert.AreEqual(simplei[i, j, k], a[i, j, k]);
                            Assert.AreEqual(simpled[i, j, k], b[i, j, k]);
                            Assert.AreEqual(simplef[i, j, k], c[i, j, k]);
                            Assert.AreEqual(simpleb[i, j, k], d[i, j, k]);
                            Assert.AreEqual(simples[i, j, k], e[i, j, k]);
                        }
            }
        }

        [Test]
        public void TestEnumerator1()
        {
            short[] simple2 = { 1, 2, 3, 4, 5, 6, 7 };
            DataArrayShort x = new DataArrayShort(_info, simple2);

            IEnumerator xx = simple2.GetEnumerator();
            foreach (short i in x)
            {
                xx.MoveNext();

                Assert.AreEqual(i, (short)xx.Current);
            }
        }

        [Test]
        public void TestEnumerator2()
        {
            byte[,] simple2 = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            DataArrayByte x = new DataArrayByte(_info, simple2);

            IEnumerator xx = simple2.GetEnumerator();
            foreach (byte i in x)
            {
                xx.MoveNext();

                Assert.AreEqual(i, (byte)xx.Current);
            }
        }

        [Test]
        public void TestEnumerator3()
        {
            float[, ,] simple2 = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            DataArrayFloat x = new DataArrayFloat(_info, simple2);

            IEnumerator xx = simple2.GetEnumerator();
            foreach (float i in x)
            {
                xx.MoveNext();

                Assert.AreEqual(i, (float)xx.Current);
            }            
        }


        [Test]
        public void TestSerialization1()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            double[] simpled = { 1, 2, 3, 4, 5, 6, 7, 8 };
            float[] simplef = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] simpleb = { 1, 2, 3, 4, 5, 6, 7, 8 };
            short[] simples = { 1, 2, 3, 4, 5, 6, 7, 8 };

            Sensored[] simplesn = { new Sensored(1), new Sensored(2), new Sensored(3), new Sensored(4),
                                    new Sensored(4), new Sensored(5), new Sensored(6), new Sensored(7)};

            Coords[] simplec = { new Coords(10,10), new Coords(10,11), new Coords(10,12), new Coords(10,13),
                                 new Coords(11,10), new Coords(12,11), new Coords(13,12), new Coords(14,13)};

            FiberCoords[] simplefc = { new FiberCoords(10,10), new FiberCoords(10,11), new FiberCoords(10,12), new FiberCoords(10,13),
                                       new FiberCoords(11,10), new FiberCoords(12,11), new FiberCoords(13,12), new FiberCoords(14,13)};

            MultiIntFloat[] simplem = { new MultiIntFloat(1), new MultiIntFloat(2), new MultiIntFloat(3), new MultiIntFloat(4),
                                        new MultiIntFloat(4.0f), new MultiIntFloat(5.0f), new MultiIntFloat(6.0f), new MultiIntFloat(7.0f)};

            DataArray[] origs = {
                                    new DataArrayInt(_info, simplei),
                                    new DataArrayDouble(_info, simpled),
                                    new DataArrayFloat(_info, simplef),
                                    new DataArrayByte(_info, simpleb),
                                    new DataArrayShort(_info, simples),
                                    new DataArraySensored(_info, simplesn),
                                    new DataArrayCoords(_info, simplec),
                                    new DataArrayFiberCoords(_info, simplefc),
                                    new DataArrayMultiIntFloat(_info, simplem)
                                };
            byte[][] data = new byte[origs.Length][];

            DataArray[] news = new DataArray[origs.Length];

            for (int i = 0; i < origs.Length; i++)
            {
                data[i] = origs[i].Serialize().GetData();
            }

            //data[8][20] = 34;

            for (int i = 0; i < origs.Length; i++)
            {
                news[i] = DataArray.Create(new StreamDeserializer(data[i], 0));

                Assert.AreEqual(origs[i].Rank, news[i].Rank);
                Assert.AreEqual(origs[i].Length, news[i].Length);

                Assert.AreEqual(origs[i].DimX, news[i].DimX);
                Assert.AreEqual(origs[i].DimY, news[i].DimY);
                Assert.AreEqual(origs[i].DimZ, news[i].DimZ);

                for (int j = 0; j < origs[i].Length; j++)
                {
                    Assert.AreEqual(origs[i][j], news[i][j]);
                }
            }
        }

        [Test]
        public void TestSerialization2()
        {
            int[,] simplei = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            double[,] simpled = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            float[,] simplef = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            short[,] simples = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            Sensored[,] simplesn = { {new Sensored(1), new Sensored(2)}, {new Sensored(3), new Sensored(4)},
                                     {new Sensored(4), new Sensored(5)}, {new Sensored(6), new Sensored(7)}};

            Coords[,] simplec = { {new Coords(10,10), new Coords(10,11)}, {new Coords(10,12), new Coords(10,13)},
                                  {new Coords(11,10), new Coords(12,11)}, {new Coords(13,12), new Coords(14,13)}};

            FiberCoords[,] simplefc = { {new FiberCoords(10,10), new FiberCoords(10,11)}, {new FiberCoords(10,12), new FiberCoords(10,13)},
                                        {new FiberCoords(11,10), new FiberCoords(12,11)},{ new FiberCoords(13,12), new FiberCoords(14,13)}};

            MultiIntFloat[,] simplem = { {new MultiIntFloat(1), new MultiIntFloat(2)}, {new MultiIntFloat(3), new MultiIntFloat(4)},
                                         {new MultiIntFloat(4.0f), new MultiIntFloat(5.0f)}, {new MultiIntFloat(6.0f), new MultiIntFloat(7.0f)}};

            DataArray[] origs = {
                                    new DataArrayInt(_info, simplei),
                                    new DataArrayDouble(_info, simpled),
                                    new DataArrayFloat(_info, simplef),
                                    new DataArrayByte(_info, simpleb),
                                    new DataArrayShort(_info, simples),
                                    new DataArraySensored(_info, simplesn),
                                    new DataArrayCoords(_info, simplec),
                                    new DataArrayFiberCoords(_info, simplefc),
                                    new DataArrayMultiIntFloat(_info, simplem)
                                };
            byte[][] data = new byte[origs.Length][];

            DataArray[] news = new DataArray[origs.Length];

            for (int i = 0; i < origs.Length; i++)
            {
                data[i] = origs[i].Serialize().GetData();
            }

            //data[8][20] = 34;

            for (int i = 0; i < origs.Length; i++)
            {
                news[i] = DataArray.Create(new StreamDeserializer(data[i], 0));

                Assert.AreEqual(origs[i].Rank, news[i].Rank);
                Assert.AreEqual(origs[i].Length, news[i].Length);

                Assert.AreEqual(origs[i].DimX, news[i].DimX);
                Assert.AreEqual(origs[i].DimY, news[i].DimY);
                Assert.AreEqual(origs[i].DimZ, news[i].DimZ);

                for (int j = 0; j < origs[i].DimX; j++)
                    for (int k = 0; k < origs[i].DimY; k++)
                    {
                        Assert.AreEqual(origs[i][j, k], news[i][j, k]);
                    }
            }
        }

        [Test]
        public void TestSerialization3()
        {
            int[, ,] simplei = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            double[, ,] simpled = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            float[, ,] simplef = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            byte[, ,] simpleb = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            short[, ,] simples = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };

            Sensored[, ,] simplesn = { {{new Sensored(1), new Sensored(2)}, {new Sensored(3), new Sensored(4)}},
                                     {{new Sensored(4), new Sensored(5)}, {new Sensored(6), new Sensored(7)}}};

            Coords[, ,] simplec = { {{new Coords(10,10), new Coords(10,11)}, {new Coords(10,12), new Coords(10,13)}},
                                  {{new Coords(11,10), new Coords(12,11)}, {new Coords(13,12), new Coords(14,13)}}};

            FiberCoords[, ,] simplefc = {{ {new FiberCoords(10,10), new FiberCoords(10,11)}, {new FiberCoords(10,12), new FiberCoords(10,13)}},
                                        {{new FiberCoords(11,10), new FiberCoords(12,11)},{ new FiberCoords(13,12), new FiberCoords(14,13)}}};

            MultiIntFloat[, ,] simplem = {{ {new MultiIntFloat(1), new MultiIntFloat(2)}, {new MultiIntFloat(3), new MultiIntFloat(4)}},
                                         {{new MultiIntFloat(4.0f), new MultiIntFloat(5.0f)}, {new MultiIntFloat(6.0f), new MultiIntFloat(7.0f)}}};

            DataArray[] origs = {
                                    new DataArrayInt(_info, simplei),
                                    new DataArrayDouble(_info, simpled),
                                    new DataArrayFloat(_info, simplef),
                                    new DataArrayByte(_info, simpleb),
                                    new DataArrayShort(_info, simples),
                                    new DataArraySensored(_info, simplesn),
                                    new DataArrayCoords(_info, simplec),
                                    new DataArrayFiberCoords(_info, simplefc),
                                    new DataArrayMultiIntFloat(_info, simplem)
                                };
            byte[][] data = new byte[origs.Length][];            

            DataArray[] news = new DataArray[origs.Length];

            for (int i = 0; i < origs.Length; i++)
            {
                data[i] = origs[i].Serialize().GetData();
            }

            //data[8][20] = 34;

            for (int i = 0; i < origs.Length; i++)
            {
                news[i] = DataArray.Create(new StreamDeserializer(data[i], 0));

                Assert.AreEqual(origs[i].Rank, news[i].Rank);
                Assert.AreEqual(origs[i].Length, news[i].Length);

                Assert.AreEqual(origs[i].DimX, news[i].DimX);
                Assert.AreEqual(origs[i].DimY, news[i].DimY);
                Assert.AreEqual(origs[i].DimZ, news[i].DimZ);

                for (int j = 0; j < origs[i].DimX; j++)
                    for (int k = 0; k < origs[i].DimY; k++)
                        for (int l = 0; l < origs[i].DimZ; l++)
                        {
                            Assert.AreEqual(origs[i][j, k, l], news[i][j, k, l]);
                        }
            }
        }

        [Test]
        public void TestConvertToArray()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            float[, ,] simplef = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };

            DataArrayInt ix = new DataArrayInt(_info, simplei);
            DataArrayByte bx = new DataArrayByte(_info, simpleb);
            DataArrayFloat fx = new DataArrayFloat(_info, simplef);

            double[] dix = (double[])ix.ConvertToArray(typeof(double));
            int[,] ibx = (int[,])bx.ConvertToArray(typeof(int));
            short[,,] sfx = (short[,,])fx.ConvertToArray(typeof(short));


            Assert.AreEqual(dix.Length, ix.Length);
            Assert.AreEqual(ibx.Rank, bx.Rank);
            Assert.AreEqual(sfx.Rank, fx.Rank);

        }

        [Test]
        public void TestCastTo()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            float[, ,] simplef = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };

            DataArrayInt ix = new DataArrayInt(_info, simplei);
            DataArrayByte bx = new DataArrayByte(_info, simpleb);
            DataArrayFloat fx = new DataArrayFloat(_info, simplef);

            DataArrayInt ixc0 = (DataArrayInt)ix.CastTo(new AttributeTypeRules("a as array"));
            DataArrayDouble ixc = (DataArrayDouble)ix.CastTo(new AttributeTypeRules("a as array(double)"));
            DataArrayDouble ixc2 = (DataArrayDouble)ix.CastTo(new AttributeTypeRules("a as array(double,1)"));

            DataArrayShort bxc = (DataArrayShort)bx.CastTo(new AttributeTypeRules("a as array(short)"));
            DataArrayShort bxc2 = (DataArrayShort)bx.CastTo(new AttributeTypeRules("a as array(short,2)"));

            DataArrayInt fxc = (DataArrayInt)fx.CastTo(new AttributeTypeRules("a as array(int)"));
            DataArrayInt fxc2 = (DataArrayInt)fx.CastTo(new AttributeTypeRules("a as array(int,3)"));

            Assert.AreEqual(ixc0.Length, ix.Length);
            Assert.AreEqual(ixc.Length, ix.Length);
            Assert.AreEqual(ixc2.Length, ix.Length);

            Assert.AreEqual(bxc.Rank, bx.Rank);
            Assert.AreEqual(bxc2.Rank, bx.Rank);

            Assert.AreEqual(fxc.Rank, fx.Rank);
            Assert.AreEqual(fxc2.Rank, fx.Rank);
        }

        [Test]
        public void TestCastToArray()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            DataArrayInt ix = new DataArrayInt(_info, simplei);
            DataArrayByte bx = new DataArrayByte(_info, simpleb);

            double[,] dbx = (double[,])bx.CastToArray(new AttributeTypeRules("a as double[,]"));

            int[] ixc0 = (int[])ix.CastToArray(new AttributeTypeRules("a as int[]"));
            float[] ixc = (float[])ix.CastToArray(new AttributeTypeRules("a as float[]"));
            double[] ixc2 = (double[])ix.CastToArray(new AttributeTypeRules("a as double[8]"));            
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestCastToArrayBadSize()
        {
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            DataArrayByte bx = new DataArrayByte(_info, simpleb);

            double[,] dbx = (double[,])bx.CastToArray(new AttributeTypeRules("a as double[3,2]"));
        }
        
        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestCastToArrayBadDim()
        {
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            DataArrayByte bx = new DataArrayByte(_info, simpleb);

            double[] dbx = (double[])bx.CastToArray(new AttributeTypeRules("a as double[]"));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestBadCastToDataType()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            DataArrayInt ix = new DataArrayInt(_info, simplei);

            ix.CastTo(new AttributeTypeRules("a as badarray(double)"));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestBadCastToType()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            DataArrayInt ix = new DataArrayInt(_info, simplei);

            ix.CastTo(new AttributeTypeRules("a as array(unknown)"));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestBadCastToRank()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            DataArrayInt ix = new DataArrayInt(_info, simplei);

            ix.CastTo(new AttributeTypeRules("a as array(double,2)"));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestBadCastToUnknownArgumant()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            DataArrayInt ix = new DataArrayInt(_info, simplei);

            ix.CastTo(new AttributeTypeRules("a as array(double,2,d)"));
        }


        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestBadCastToUnsupportedType()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            DataArrayInt ix = new DataArrayInt(_info, simplei);

            ix.CastTo(new AttributeTypeRules("a as array(coords)"));
        }

        [Test]
        public void TestCastToNew()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            DataArrayInt ix = new DataArrayInt(_info, simplei);
            DataArrayByte bx = new DataArrayByte(_info, simpleb);

            double[,] dbx = (double[,])bx.CastToArray(new AttributeTypeRules("a as double[,]"));

            int[] ixc0 = (int[])ix.CastTo(null, new AttributeTypeRules("a as int[]"));
            float[] ixc = (float[])ix.CastTo(null, new AttributeTypeRules("a as float[]"));
            double[] ixc2 = (double[])ix.CastTo(null, new AttributeTypeRules("a as double[8]"));   
        }

        [Test]
        public void TesNormalize()
        {
            float[] simplei = { 1, 2, float.NaN, 4, 5, 6, float.NegativeInfinity, 8 };
            double[,] simpleb = { { double.NaN, 2 }, { 3, double.NegativeInfinity }, { 5, 6 }, { double.PositiveInfinity, 8 } };
            float[, ,] simplef = { { { 1, 2 }, { float.NaN, 4 } }, { { float.NegativeInfinity, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };

            DataArrayFloat ix = new DataArrayFloat(_info, simplei);
            DataArrayDouble bx = new DataArrayDouble(_info, simpleb);
            DataArrayFloat fx = new DataArrayFloat(_info, simplef);

            DataArrayDouble nix = ix.Normalize();
            DataArrayDouble bix = bx.Normalize();
            DataArrayDouble fix = fx.Normalize();

            DataArrayDouble[] conv = { nix, bix, fix };
            foreach(DataArrayDouble c in conv)
            {
                foreach(double v in c)
                {
                    Assert.IsFalse(Double.IsNaN(v));
                    Assert.IsFalse(Double.IsInfinity(v));
                }
            }
        }

        [Test]
        public void TestDataGrid()
        {
            int[] simplei = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[,] simpleb = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            float[, ,] simplef = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };

            DataArrayInt ix = new DataArrayInt(_info, simplei);
            DataArrayByte bx = new DataArrayByte(_info, simpleb);
            DataArrayFloat fx = new DataArrayFloat(_info, simplef);            

            DataGrid dix = ix.DataTable;
            DataGrid bix = bx.DataTable;
            DataGrid fix = fx.DataTable;
        }

#if !DOTNET_V11
        [Test]
        public void TestMean()
        {
            short[] simples2 = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[,] simpleb2 = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            float[, ,] simplef2 = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };

            DataArrayShort sx = new DataArrayShort(_info, simples2);
            DataArrayByte bx = new DataArrayByte(_info, simpleb2);
            DataArrayFloat fx = new DataArrayFloat(_info, simplef2);

            Assert.AreEqual(sx.Mean(), 4.5);
            Assert.AreEqual(bx.Mean(), 4.5);
            Assert.AreEqual(fx.Mean(), 4.5);
        }
#endif
    }
}
