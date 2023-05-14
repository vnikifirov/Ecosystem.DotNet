using System;
using System.Reflection;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.IO;
using System.Text;

using NUnit.Framework;

using corelib;

using System.Windows.Forms;

namespace test2
{
    [TestFixture]
    public class TestAutoAttributes
    {
        [Test]
        public void TestAuto()
        {            
            AutoParameterInfo[] sParameters = 
               {
                    new AutoParameterInfo("enviromentObject", "", AutoParameterInfoType.Enviroment),
                    new AutoParameterInfo("connctionString", "Строка подключения", AutoParameterInfoType.String),
                    new AutoParameterInfo("readOnly", "Только для чтения", AutoParameterInfoType.Bool),
                    new AutoParameterInfo("password", "Пароль", AutoParameterInfoType.Password),
                    new AutoParameterInfo("file", "Путь к файлу *.tup|*.tup", AutoParameterInfoType.FilePath),
                    new AutoParameterInfo("connctionString2", "Строка подключения2", AutoParameterInfoType.String),
                    new AutoParameterInfo("connctionString3", "Строка подключения3", AutoParameterInfoType.String),
                    new AutoParameterInfo("connctionString4", "Строка подключения3", AutoParameterInfoType.DirectoryPath),
               };

            DataParamTable t = new DataParamTable(new TupleMetaData("settings", "settings", DateTime.Now, "settings"),
                new DataParamTableItem("enviromentObject", 1),
                new DataParamTableItem("readOnly", true));

            Form f = new Form();
            ParametersListVisualizer v = new ParametersListVisualizer(sParameters, t, true);
            f.Controls.Add(v);

            f.ShowDialog();

            DataParamTable d = v.GetParameters();

        }

        public void TesMethod(IEnviromentEx enviromentObject, string connctionString, string connctionPath, string connctionFolder, bool readOnly,
            string password, string subInfo, string mode)
        {            

        }

        [Test]
        public void TestParametersDsl()
        {
            string tt = @"enviromentObject('', Enviroment), connctionString('Строка подключения SQLite', String), 
                         connctionPath('Путь *.*|*.*', FilePath), connctionFolder('dddd', DirectoryPath), 
                         readOnly('RO', Bool, 'true'), password('Password', Password), subInfo('Sub info', String),
                         mode('Режим', List, 'Авто', False, contains('Авто', 'Ручной', 'Неизвествно'))";

            AttributeDataComponent s = new AttributeDataComponent("s");
            s.ParametrsInfoString = tt;
            s.ParametrsInfoMethod = typeof(TestAutoAttributes).GetMethod("TesMethod").GetParameters();

            AutoParameterInfo[] w = s.GetParametrsInfo();

            DataParamTable t = new DataParamTable(new TupleMetaData("settings", "settings", DateTime.Now, "settings"),
                new DataParamTableItem("enviromentObject", 1),
                new DataParamTableItem("mode", "Ручной"));

            ParametersListVisualizerForm f = new ParametersListVisualizerForm(w, t, true, "Выберете параметр");
            f.ShowDialog();
        }

    }
}
