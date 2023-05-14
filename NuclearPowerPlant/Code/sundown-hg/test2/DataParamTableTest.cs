using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Xml;

using NUnit.Framework;

namespace corelib
{
    [TestFixture]
    public class DataParamTableTest
    {
        TupleMetaData _info = new TupleMetaData("test", "test", DateTime.Now, TupleMetaData.StreamAuto);
        EnvConverterFormatter _conv = new EnvConverterFormatter(
            new CartogramPresentationConfig(true, false, true),
            new CartogramPresentationConfig(true, false, true));

        [Test]
        public void CreateTest()
        {

            DataParamTable t = new DataParamTable(_info,
                new DataParamTableItem("test", 10),
                new DataParamTableItem("test2", 10.234),
                new DataParamTableItem("date", DateTime.Now),
                new DataParamTableItem("a", new AnyValue(1, 0.01f)));            
        }

        [Test]
        public void Enumeration()
        {
            DataParamTable t = new DataParamTable(_info,
                new DataParamTableItem("test", 10),
                new DataParamTableItem("test2", 10.234),
                new DataParamTableItem("date", DateTime.Now),
                new DataParamTableItem("a", new AnyValue(1, 0.01f)));            
            
            foreach (DataParamTableItem i in t)
            {
                string s = i.ToString();
                Console.WriteLine(s);
            }
        }

        [Test]
        public void TestCastToNew()
        {
            DataParamTable t = new DataParamTable(_info,
                new DataParamTableItem("test", 10),
                new DataParamTableItem("test2", 10.234),
                new DataParamTableItem("date", DateTime.Now),
                new DataParamTableItem("a", new AnyValue(1, 0.01f)));

            int test = (int)t.CastTo(null, new AttributeTypeRules("a as int[test]"));
            float ixc = (float)t.CastTo(null, new AttributeTypeRules("a as float[test2]"));
            double ixc2 = (double)t.CastTo(null, new AttributeTypeRules("a as double[a]"));
        }

        [Test]
        public void TestToXml()
        {
            DataParamTable t = new DataParamTable(_info,
                new DataParamTableItem("test", 10),
                new DataParamTableItem("test2", 10.234),
                new DataParamTableItem("date", DateTime.Now),
                new DataParamTableItem("a", new AnyValue(1, 0.01f)));

            XmlDocument doc = t.ToXml();
            doc.Save("TestToXml.xml");
            
        }


        [Test]
        public void TestToXml2()
        {
            DataParamTable a = new DataParamTable(_info,
                new DataParamTableItem("wer", 10),
                new DataParamTableItem("aqwe", "Linux"),
                new DataParamTableItem("ss", DateTime.Now),
                new DataParamTableItem("a", "nothing"));  

            DataParamTable t = new DataParamTable(_info,
                new DataParamTableItem("test", 10),
                new DataParamTableItem("test2", 10.234),
                new DataParamTableItem("date", DateTime.Now),
                new DataParamTableItem("a", new AnyValue(1, 0.01f)),
                new DataParamTableItem("b", a));

            XmlDocument doc = t.ToXml();
            doc.Save("TestToXml2.xml");

        }
    }
}
