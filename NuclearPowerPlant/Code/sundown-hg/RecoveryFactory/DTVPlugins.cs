using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;
using corelib;

using RecoveryFactory;

namespace corelib
{

    class DTVSunEnvOpendb : ActionDataTupleVisualizerUI
    {
        public DTVSunEnvOpendb()
        {
            _name = "OpendbData";
            _humaneName = "Открыть в базу...";
            _descr = "Открыть внешние данные";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageAdd;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunEnv env = (SunEnv)ui.GetEnviroment();

            try
            {
                const string provider = "SqliteProvider2";
                const string resource = "tempDataOpen";

                DataParamTable pt = new DataParamTable(new TupleMetaData(provider, provider, DateTime.Now, provider));
                DataParamTable pt2 = new DataParamTable(new TupleMetaData(resource, resource, DateTime.Now, resource), 
                    new DataParamTableItem(resource, pt));

                IMultiDataProvider prov = env.CreateData(resource, pt2).GetMultiProvider();
                env.ViewMultiTuple(prov);

            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе открытия данных произошла ошибка");
            }
            finally
            {
                env.CloseData("tempDataOpen");
            }
        }
    }


    class DTVSunEnvExport : ActionDataTupleVisualizerUI
    {
        public DTVSunEnvExport()
        {
            _name = "ExportData";
            _humaneName = "Экспортировать";
            _descr = "Экспортировать текущую базу";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageExport;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunEnv env = (SunEnv)ui.GetEnviroment();

            try
            {
                env.ExportToBase();
            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе экспортирования произошла ошибка");
            }
        }
    }


    class DTVSunEnvSnapshot : ActionDataTupleVisualizerUI
    {
        public DTVSunEnvSnapshot()
        {
            _name = "DTSnapshotData";
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
                //ParamTuple pt = new ParamTuple("SqliteProvider2");
                //ParamTuple pt = new ParamTuple("SqliteProvider");
                //ParamTuple pt2 = new ParamTuple("tempData");
                //pt2.Add("tempData", pt);

                const string provider = "SqliteProvider2";
                const string resource = "tempData";

                DataParamTable pt = new DataParamTable(new TupleMetaData(provider, provider, DateTime.Now, provider));
                DataParamTable pt2 = new DataParamTable(new TupleMetaData(resource, resource, DateTime.Now, resource),
                    new DataParamTableItem(resource, pt));

                env.CreateData(resource, pt2);
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

    class DTVSunEnvRecover : ActionDataTupleVisualizerUI
    {
        public DTVSunEnvRecover()
        {
            _name = "RecoverData";
            _humaneName = "Востановление расхода...";
            _descr = "Восстановить расход по азотной активности";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageCalc;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunEnv env = (SunEnv)ui.GetEnviroment();

            try
            {
                RecoverySimple rs = new RecoverySimple(env);
                rs.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе экспортирования произошла ошибка");
            }
        }
    }


}