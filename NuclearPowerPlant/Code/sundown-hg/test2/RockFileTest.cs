using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using NUnit.Framework;

using corelib;

namespace RockPlugin
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

    public class TestEnviroment : EnvConverterFormatter, corelib.IEnviroment
    {
        public TestEnviroment(CartogramPresentationConfig linearConf, CartogramPresentationConfig visualConf)
            : base(linearConf, visualConf)
        {

        }

        #region IEnviromentSettings Members

        public string GetGlobalParam(string param)
        {
            throw new NotImplementedException();
        }

        public bool IsXmlParam(string param)
        {
            throw new NotImplementedException();
        }

        public System.Xml.XmlElement GetXmlParam(string param)
        {
            throw new NotImplementedException();
        }

        public corelib.DataParamTable ParamTuple
        {
            get { throw new NotImplementedException(); }
        }

        public corelib.DataComponents Data
        {
            get { throw new NotImplementedException(); }
        }

        public corelib.AlogComponents Algo
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [TestFixture]
    public class RockFileTest
    {
        const string testFile = "3b160506_16-01.fil";
        const string testFile2 = "3b160506_16-16.fil";
        const string testFile3 = "3b160506_16-31.fil";

        TupleMetaData _info = new TupleMetaData("test", "test", DateTime.Now, TupleMetaData.StreamAuto);
        TestEnviroment _conv = new TestEnviroment(
            new CartogramPresentationConfig(false, false, true),
            new CartogramPresentationConfig(true, false, true));

        [Test]
        public void Open()
        {
            RockFile fl = new RockFile(testFile);

            DataCartogramIndexedFloat flow = fl.GetCartogramFloat(6, false, _conv, _info, new ScaleIndex(0, 0.1));

            DataCartogramIndexedInt sc = fl.GetCartogramInt(94, true, _conv, _info, new ScaleIndex(0, 1));

        }

        [Test]
        public void OpenPlugin()
        {
            RockSingleProvider p = new RockSingleProvider(_conv, testFile);

            IDataTuple data = p.GetData();
            
        }

        [Test]
        public void BackwardCompatibility()
        {
            RockSingleProvider p = new RockSingleProvider(_conv, testFile);
            DataTuple sd = new DataTuple(p.GetData());

            ISerializeStream stream = sd.Serialize();
            byte[] rawData = stream.GetData();

            DataTuple old = new DataTuple(_conv, new StreamDeserializer(rawData,0));

#if SHOW
            DataTupleVisualizer dv = new DataTupleVisualizer();
            dv.SetTuple(old);

            System.Windows.Forms.Form dppppp = new System.Windows.Forms.Form();
            dv.Dock = System.Windows.Forms.DockStyle.Fill;
            dppppp.Controls.Add(dv);
            dppppp.ShowDialog();
#endif
        }

        [Test]
        public void ShowBackwardCompatibility()
        {
            RockSingleProvider p = new RockSingleProvider(_conv, testFile);
            RockSingleProvider p2 = new RockSingleProvider(_conv, testFile2);
            RockSingleProvider p3 = new RockSingleProvider(_conv, testFile3);   

#if SHOW
            DataTupleVisualizer dv = new DataTupleVisualizer();
            dv.SetEnviroment(_conv);
            dv.SetMultiTuple(new IDataTuple[]{p.GetData(), p2.GetData(), p3.GetData()});

            System.Windows.Forms.Form dppppp = new System.Windows.Forms.Form();
            dv.Dock = System.Windows.Forms.DockStyle.Fill;
            dppppp.Controls.Add(dv);
            dppppp.ShowDialog();
#endif

        }


    }
}
