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
using PluginMDX_vs90;

namespace SunBrowser
{
    
#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
#endif

    public class SunBrowserEnviroment : GuiEnv
    {
        static DataParamTable Combine(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
        {
            return new DataParamTable(new TupleMetaData("enviroment", "enviroment", DateTime.Now, "enviroment"),
                new DataParamTableItem("coordsCalculation", lenearConf.Value),
                new DataParamTableItem("coordsVisualization", visualConf.Value));
        }

        public SunBrowserEnviroment(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
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
        public SunBrowserEnviroment(DataParamTable par) :
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


    class DTVSunBrowserOpenDataPlugin : ActionDataTupleVisualizerUI
    {
        public DTVSunBrowserOpenDataPlugin()
        {
            _name = "OpenDataPlugin";
            _humaneName = "Открыть";
            _descr = "Открыть источник данных";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunBrowserEnviroment env = (SunBrowserEnviroment)ui.GetEnviroment();

            try
            {
                IDataResource res = env.GetOpenFileComponent();
                if (res != null)
                {
                    DataTupleVisualizer v = (DataTupleVisualizer)ui;
                    v.SetDataProvider(res.GetMultiProvider());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе открытия файла произошла ошибка");
            }
        }


    }

    public class SunBrowser
	{
        static void TestDarwMuliFiles(SunBrowserEnviroment env, string path, IDataComponent component)
		{

            IMultiDataProvider prov = path == null ? null : env.CreateProvider(component, path);

            DataTupleVisualizer dv = new DataTupleVisualizer(env);
            if (prov != null)
                dv.SetDataProvider(prov);
            dv.RegisterAction(new DTVSunBrowserOpenDataPlugin());
            

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

            SunBrowserEnviroment env;
            
            if (args.Length > 2)
                env = new SunBrowserEnviroment(new CartogramPresentationConfig(Convert.ToInt32(args[1])),
                                        new CartogramPresentationConfig(Convert.ToInt32(args[2])));
            else
                env = new SunBrowserEnviroment(new CartogramPresentationConfig(true, false, true),
                                        new CartogramPresentationConfig(true, false, true));
            //*/
            
            /***********************************************************/
            /**************************Роман****************************/
            /*
            DataParamTable config = DataParamTable.LoadFromXML("config.xml");
            env = new SunBrowserEnviroment(config);
            //*/
            /***********************************************************/
            /***********************************************************/

            IDataComponent component;
           

            if (args.Length > 0)
                component = env.Data.Find(args[0]);
            else
                component = new DataComponents.DataComponent(typeof(RockMicroSingleProviderMDX));

			FolderBrowserDialog bd = new FolderBrowserDialog();
			bd.Description = String.Format("Выберете папку для: {0}", component.Info.HumanDescribe);
			bd.ShowNewFolderButton = false;

			DialogResult dr = bd.ShowDialog();
    		TestDarwMuliFiles(env, dr == DialogResult.OK ? bd.SelectedPath : null, component);
	
		}
	}
}
