using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;
using corelib;
using System.IO;



namespace corelib
{
    public class DTVPluginEnvExportToTXT : ActionDataTupleVisualizerUI
    {
        public DTVPluginEnvExportToTXT()
        {
            _name = "TXTExportData";
            _humaneName = "В ТХТ";
            _descr = "Экспортировать текущую базу";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageExport;
        }
        public void onClick(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("C:\\Users\\tem2m\\Desktop\\2.txt", FileMode.OpenOrCreate);

            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            BasicEnv env = (BasicEnv)ui.GetEnviroment();

            try
            {
                const string provider = "SqliteProvider2";
                DataParamTable pt = new DataParamTable(new TupleMetaData(provider, provider, DateTime.Now, provider),
                    new DataParamTableItem("databaseFileString", "c:\\export_sn.dq3"),
                    new DataParamTableItem("readOnlyAccess", false),
                    new DataParamTableItem("autoInit", true),
                    new DataParamTableItem("abiVer", "v1")
                    );

                
                
                const string temp = "tempData";

                DataParamTable pt2 = new DataParamTable(new TupleMetaData(temp, temp, DateTime.Now, temp),
                    new DataParamTableItem("tempData", pt));
                MessageBox.Show(temp);
                //ParamTuple pt2 = new ParamTuple("tempData");
                //pt2.Add("tempData", pt);

                env.CreateData("tempData", pt2);
                env.ExportToBase(ui.GetMultiDataProvider(), "tempData");
            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе экспортирования произошла ошибка");
            }
            finally
            {
                env.CloseData("tempData");
            }
        }
    }


    public class DTVPluginEnvSnapshot : ActionDataTupleVisualizerUI
    {
        public DTVPluginEnvSnapshot()
        {
            _name = "SnapshotData2";
            _humaneName = "Снимок";
            _descr = "Экспортировать текущую базу";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageExport;
        }

        

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            BasicEnv env = (BasicEnv)ui.GetEnviroment();

            try
            {
//                ParamTuple pt = new ParamTuple("SqliteProvider");
//                pt.Add("databaseFileString", "c:\\export_sn.db3");
                //ParamTuple pt = new ParamTuple("SqliteProvider2");
                //pt.Add("databaseFileString", "c:\\export_sn.dq3");

                //pt.Add("readOnlyAccess", false);
                //pt.Add("autoInit", true);
                //pt.Add("abiVer", "v1");


                const string provider = "SqliteProvider2";
                DataParamTable pt = new DataParamTable(new TupleMetaData(provider, provider, DateTime.Now, provider),
                    new DataParamTableItem("databaseFileString", "c:\\export_sn.dq3"),
                    new DataParamTableItem("readOnlyAccess", false),
                    new DataParamTableItem("autoInit", true),
                    new DataParamTableItem("abiVer", "v1")
                    );

                const string temp = "tempData";
                DataParamTable pt2 = new DataParamTable(new TupleMetaData(temp, temp, DateTime.Now, temp),
                    new DataParamTableItem("tempData", pt));

                //ParamTuple pt2 = new ParamTuple("tempData");
                //pt2.Add("tempData", pt);

                env.CreateData("tempData", pt2);
                env.ExportToBase(ui.GetMultiDataProvider(), "tempData");
            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе экспортирования произошла ошибка");
            }
            finally
            {
                env.CloseData("tempData");
            }
        }
    }

}