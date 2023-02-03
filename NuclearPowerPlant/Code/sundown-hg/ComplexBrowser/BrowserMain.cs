using System;
using System.Drawing;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;

using corelib;
using RockMicoPlugin;

namespace ComplexBrowser
{
#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
#endif

    public class ComplexBrowserEnviroment : GuiEnv
    {
        static DataParamTable Combine(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
        {
            return new DataParamTable(new TupleMetaData("enviroment", "enviroment", DateTime.Now, "enviroment"),
                new DataParamTableItem("coordsCalculation", lenearConf.Value),
                new DataParamTableItem("coordsVisualization", visualConf.Value));
        }

        public ComplexBrowserEnviroment(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
            : base(Combine(lenearConf, visualConf), ".", ".")
        {

            // Cхема ПВК
            /*using (IDataResource dataPvk = CreateData("SunEnv_PvkSchemeProvider"))
            {
                IDataTuple consts = dataPvk.GetConstData()[0];
                DataArrayCoords c = (DataArrayCoords)consts["pvk_scheme"];

                CoordsConverter pvk = new CoordsConverter(
                    c.Info,
                    CoordsConverter.SpecialFlag.PVK,
                    c);

                SetPVK(pvk);
            }*/
            using (DataTupleProvider dataPvk = new DataTupleProvider(this, "pvkTuple.tup"))
            {
                IDataTuple consts = dataPvk.GetData()[0];
                DataArray<Coords> c = (DataArray<Coords>)consts["pvk_scheme"];

                CoordsConverter pvk = new CoordsConverter(
                    c.Info,
                    CoordsConverter.SpecialFlag.PVK,
                    c);

                SetPVK(pvk);
            }
        }
        /***********************************************************/
        /**************************Роман****************************/
        public ComplexBrowserEnviroment(DataParamTable par) :
            base(par, (string)par["componentPath"], (string)par["componentPath"])
        {
            using (IDataResource dataPvk = CreateData("SunEnv_PvkSchemeProvider"))
            {
                IDataTuple consts = dataPvk.GetConstData()[0];
                DataArrayCoords c = (DataArrayCoords)consts["pvk_scheme"];

                CoordsConverter pvk = new CoordsConverter(
                    c.Info,
                    CoordsConverter.SpecialFlag.PVK,
                    c);

                SetPVK(pvk);
            }
            CreateData("Service_DataStorageProvider");
            CreateData("SunEnv_DataStorageProvider");
            CreateData("SunEnv_PgDataStorageProvider");
            CreateData("SunEnv_PgDataStorageProvider2");
        }
        /***********************************************************/
        /***********************************************************/


        public IMultiDataProvider CreateProvider(IDataComponent c, string path)
        {
            SingleSearchToMultiProvider p = new SingleSearchToMultiProvider(this, c, path);
            if (p.IsErrors)
            {
                MessageBox.Show(p.Errors, "Следушие файлы содержат ошибки");
            }
            return p;
        }
    }

    class DTVComplexBrowserGatherDataPlugin : ActionDataTupleVisualizerUI
    {
        public DTVComplexBrowserGatherDataPlugin()
        {
            _name = "GatherDataPlugin";
            _humaneName = "Открыть базу Б1";
            _descr = "Открыть источник данных";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
        }
        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            ComplexBrowserEnviroment env = (ComplexBrowserEnviroment)ui.GetEnviroment();
            try
            {

                IDataResource res = env.OpenData("SunEnv_DataStorageProvider");
                //IDataResource res = env.OpenData("SunEnv_PgDataStorageProvider");

                //IDataResource res = env.OpenData("Service_DataStorageProvider");
                //IDataResource res = env.OpenData("SunEnv_PgDataStorageProvider2");

                if (res != null)
                {
                    DataTupleVisualizer v = (DataTupleVisualizer)ui;
                    v.SetDataProvider(res.GetMultiProvider());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе открытия файла произошла ошибка");
            }
        }
    }

    class DTVComplexBrowserRecoveryDataPlugin : ActionDataTupleVisualizerUI
    {
        public DTVComplexBrowserRecoveryDataPlugin()
        {
            _name = "RecoveryDataPlugin";
            _humaneName = "Открыть базу Б2";
            _descr = "Открыть источник данных";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
        }
        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            ComplexBrowserEnviroment env = (ComplexBrowserEnviroment)ui.GetEnviroment();
            try
            {
                IDataResource res = env.OpenData("Service_DataStorageProvider");
                //IDataResource res = env.OpenData("SunEnv_PgDataStorageProvider2");

                //IDataResource res = env.OpenData("SunEnv_DataStorageProvider");
                //IDataResource res = env.OpenData("SunEnv_PgDataStorageProvider");


                if (res != null)
                {
                    DataTupleVisualizer v = (DataTupleVisualizer)ui;
                    v.SetDataProvider(res.GetMultiProvider());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе открытия файла произошла ошибка");
            }
        }
    }
    class DTVComplexBrowserCurrentDataPlugin : ActionDataTupleVisualizerUI
    {
        public DTVComplexBrowserCurrentDataPlugin()
        {
            _name = "CurrentDataPlugin";
            _humaneName = "Текущий срез";
            _descr = "Открыть источник данных";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
        }
        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            ComplexBrowserEnviroment env = (ComplexBrowserEnviroment)ui.GetEnviroment();
            try
            {
                IDataResource res = env.OpenData("Service_DataStorageProvider");
                //IDataResource res = env.OpenData("SunEnv_PgDataStorageProvider2");

                if (res != null)
                {
                    DataTupleVisualizer v = (DataTupleVisualizer)ui;
                    v.SetRenewTupleProvider(res.GetMultiProvider(), "last");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе открытия файла произошла ошибка");
            }
        }
    }
    class DTVComplexBrowserPrevDataPlugin : ActionDataTupleVisualizerUI
    {
        public DTVComplexBrowserPrevDataPlugin()
        {
            _name = "PrevDataPlugin";
            _humaneName = "Предыдущий срез";
            _descr = "Открыть источник данных";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
            
        }
        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            ComplexBrowserEnviroment env = (ComplexBrowserEnviroment)ui.GetEnviroment();
            try
            {
                IDataResource res = env.OpenData("Service_DataStorageProvider");
                //IDataResource res = env.OpenData("SunEnv_PgDataStorageProvider2");
                if (res != null)
                {
                    DataTupleVisualizer v = (DataTupleVisualizer)ui;
                    v.SetRenewTupleProvider(res.GetMultiProvider(), "prev");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе открытия файла произошла ошибка");
            }
        }
    }

    public class ComplexBrowser
    {
        static void TestDarwMuliFiles(ComplexBrowserEnviroment env, string path, IDataComponent component)
        {

            IMultiDataProvider prov = path == null ? null : env.CreateProvider(component, path);

            DataTupleVisualizer dv = new DataTupleVisualizer(env);
            if (prov != null)
                dv.SetDataProvider(prov);
            dv.RegisterAction(new DTVComplexBrowserGatherDataPlugin());
            dv.RegisterAction(new DTVComplexBrowserRecoveryDataPlugin());
            dv.RegisterAction(new DTVComplexBrowserCurrentDataPlugin());
            dv.RegisterAction(new DTVComplexBrowserPrevDataPlugin());


            Form dppppp = new Form();
            dv.Dock = System.Windows.Forms.DockStyle.Fill;
            dppppp.Controls.Add(dv);
            dppppp.Text = "Обзор срезов";// СКАЛА-Микро";
#if !DOTNET_V11
            dppppp.Show();
            dppppp.Activate();
#endif
            Application.Run(dppppp);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            ComplexBrowserEnviroment env;
            /*
            if (args.Length > 2)
                env = new ComplexBrowserEnviroment(new CartogramPresentationConfig(Convert.ToInt32(args[1])),
                                        new CartogramPresentationConfig(Convert.ToInt32(args[2])));
            else
                env = new ComplexBrowserEnviroment(new CartogramPresentationConfig(5),
                                        new CartogramPresentationConfig(5));
            */
            /***********************************************************/
            /**************************Роман****************************/
            
            DataParamTable config = DataParamTable.LoadFromXML("config.xml");

            env = new ComplexBrowserEnviroment(config);
            /***********************************************************/
            /***********************************************************/

            IDataComponent component;

            if (args.Length > 0)
                component = env.Data.Find(args[0]);
            else
                component = new DataComponents.DataComponent(typeof(RockMicroSingleProvider));

            TestDarwMuliFiles(env, null, component);

        }
    }
}
