using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;
using corelib;

namespace AzotExporter
{
#if !DOTNET_V11
    using DataCartogramIndexedInt = DataCartogramIndexed<int>;
#endif

    class AzotExporter
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();

			try
			{
                DataParamTable config = DataParamTable.LoadFromXML("config.xml");
			
				//Application.Run(new ChoisePrp(config));
                AzotEnv env = new AzotEnv(config);
                try
                {
                    Application.Run(env.CreateForm());
                }
                finally
                {
                    env.Dispose();
                }
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "В ходе работы программы произошла ошибка");
			}

		}
    }


    public class DTVConnectionsPlugin : ActionDataTupleVisualizerUI
    {
        public DTVConnectionsPlugin()
        {
            _name = "ConnectionsPlugin";
            _humaneName = "Разводка";
            _descr = "Показать разводку";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = AkgoExporter.AkgoResources.ImageCart;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            AzotEnv env = (AzotEnv)ui.GetEnviroment();

            int[,] r = new int[16, 115];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 115; j++)
                {
                    r[i, j] = i + 1;
                }
            }

            DataCartogramIndexedInt dc = new DataCartogramIndexedInt(
                new TupleMetaData("Fiber", "Нитка", DateTime.Now, TupleMetaData.StreamConst), ScaleIndex.Default,
                env.PVK, r);

            ui.SetActiveTupleItem(dc, "const");
        }
    }


    public class DTVExportPlugin : ActionDataTupleVisualizerUI
    {
        public const string Name = "ExportPlugin";

        IDataTupleVisualizerUI _ui;
        bool _validExport = false;
        bool _prpExport;

        public DTVExportPlugin(IDataTupleVisualizerUI ui)
        {
            _name = Name;
            _humaneName = "Экспорт";
            _descr = "Сохранить текущую иформацию";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);
            _ui = ui;

            _ui.OnDataTuple += new OnDataTupleHandler(_ui_OnDataTuple);
            _ui.OnTupleItem += new OnTupleItemHandler(_ui_OnTupleItem);
            _ui.OnDataTupleMulti += new OnDataTupleMultiHandler(_ui_OnDataTupleMulti);
            _ui.OnTupleItemMulti += new OnTupleItemMultiHandler(_ui_OnTupleItemMulti);

            _image = AkgoExporter.AkgoResources.ImageExport;
        }

        void _ui_OnTupleItemMulti(IDataTupleVisualizerUI sender, IMultiDataTuple dataTuple, IMultiTupleItem item)
        {
            _ui_OnDataTupleMulti(sender, dataTuple);
        }

        void _ui_OnDataTupleMulti(IDataTupleVisualizerUI sender, IMultiDataTuple dataTuple)
        {
            if (dataTuple == null)
            {
                _validExport = false;
                _ui.VisibleAction(this, false);
            }
            else
            {
                if (dataTuple[0].GetStreamName() == "prp" && dataTuple.Count == 16 )
                {
                    _validExport = true;
                    _prpExport = true;
                    _ui.VisibleAction(this, true);
                }
            }
        }

        void _ui_OnTupleItem(IDataTupleVisualizerUI sender, IDataTuple dataTuple, ITupleItem item)
        {
            _ui_OnDataTuple(sender, dataTuple);
        }

        void _ui_OnDataTuple(IDataTupleVisualizerUI sender, IDataTuple dataTuple)
        {
            if (dataTuple == null)
            {
                _validExport = false;
                _ui.VisibleAction(this, false);
            }
            else
            {
                if (dataTuple.GetStreamName() == "pc")
                {
                    _validExport = true;
                    _prpExport = false;
                    _ui.VisibleAction(this, true);
                }
            }
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            AzotEnv env = (AzotEnv)ui.GetEnviroment();

            if (!_validExport)
                ui.EnableAction(this, false);
            else
            {
                if (_prpExport)
                {
                    if (_ui.GetActiveDataTuples() != null)
                    {
                        IMultiDataTuple tuples = _ui.GetActiveDataTuples();

                        SaveFileDialog fd = new SaveFileDialog();
                        fd.Title = "Экспорт прописок азотной активности";
                        fd.Filter = "Tuple files *.tup|*.tup";
                        fd.FileName = String.Format("prp_{0:yyyy-MM-dd_HHmm}.tup", tuples[0].GetTimeDate());
                        if (fd.ShowDialog() == DialogResult.OK)
                        {
                            MultiDataTuple.SaveToFile(tuples, fd.FileName);
                        }
                    }                    
                }
                else
                {
                    if (_ui.GetActiveDataTuple() != null)
                    {
                        DataTuple t = new DataTuple(_ui.GetActiveDataTuple());
                        DataParamTable info = (DataParamTable)t["kgopc_info"];

                        double specClock = Convert.ToDouble(info["specClock"]);
                        int fiberNum = Convert.ToInt32(info["fiberNum"]);
                        int pcPvk = Convert.ToInt32(info["pcPvk"]) - 1;

                        SaveFileDialog fd = new SaveFileDialog();
                        fd.Title = "Экспорт азотной активности на постоянном контроле";
                        fd.Filter = "Tuple files *.tup|*.tup";
                        fd.FileName = String.Format("pc_{0:yyyy-MM-dd_HHmm}_f{1}_pvk{2}_exp{3}.tup", t.GetTimeDate(),
                            fiberNum + 1, pcPvk + 1, (int)(specClock * 10));

                        if (fd.ShowDialog() == DialogResult.OK)
                        {
                            t.SaveToFile(fd.FileName);
                        }
                    }
                }
            }
        }
    }
    class AzotEnv : BasicEnv
    {
        IDataResource azotDb;

        public AzotEnv(DataParamTable par)
            :
            base(par, (string)par["componentPath"], (string)par["componentPath"])
        {

            azotDb = CreateData((string)ParamTuple["kgoExporterUseProvider"], ParamTuple);

            IDataTuple pvk = azotDb.GetMultiProvider().GetData(DateTime.Now, new string[] { "pvk_scheme" })[0];
            CoordsConverter pvk_array = (CoordsConverter)pvk["pvk_scheme"];

            SetPVK(pvk_array);            
        }


        public Form CreateForm()
        {
            DTVExportPlugin tmp;
            DataTupleVisualizer vis = new DataTupleVisualizer(this);
            vis.Dock = DockStyle.Fill;

            vis.SetDataProvider(azotDb.GetMultiProvider());
            vis.SetTreeFormatter(new DataTupleTreeAzotFormatter(azotDb.GetMultiProvider(), this));
            vis.RegisterAction(new DTVConnectionsPlugin());
            vis.RegisterAction(tmp = new DTVExportPlugin(vis));
            vis.VisibleAction(tmp, false);

            vis.OnDataTuple += new OnDataTupleHandler(vis_OnDataTuple);
            vis.OnDataTupleMulti += new OnDataTupleMultiHandler(vis_OnDataTupleMulti);

            Form form = new Form();
            form.Controls.Add(vis);
            form.Text = "Просмотр базы АКГО";
            return form;
        }

        void vis_OnDataTupleMulti(IDataTupleVisualizerUI sender, IMultiDataTuple dataTuple)
        {
            if (dataTuple != null)
            {
                if (dataTuple[0].GetStreamName() == "prp")
                {
                    IMultiTupleItem items = dataTuple["kgoprp_azot"];
                    sender.SetActiveTupleItems(items, items.GetStreamName());
                    /*
                    DataArray[] list = new DataArray[items.Length];
                    for (int i = 0; i < items.Length; i++)
                        list[i] = (DataArray)items[i];

                    sender.SetDataGrid(DataArrayVisualizer.CombineDataArrays(list), items, dataTuple, dataTuple[0].GetStreamName());
                     */
                }
            }
        }

        static void vis_OnDataTuple(IDataTupleVisualizerUI sender, IDataTuple dataTuple)
        {
            if (dataTuple != null)
            {
                if ((dataTuple.GetStreamName() == "pc") || (dataTuple.GetStreamName() == "prp"))
                {
                    ArrayList data = new ArrayList();
                    for (int j = 0; j < dataTuple.ItemsCount; j++ )
                    {
                        ITupleItem i = dataTuple[j];
                        if (i is DataArray)
                            data.Add(i);
                    }                    
                    sender.SetActiveTupleItems(new MultiTupleItem(data), dataTuple.GetStreamName());
                    //sender.SetDataGrid(DataArrayVisualizer.CombineDataArrays(list), null, dataTuple, dataTuple.GetStreamName());
                }
            }
        }
    }

    public class DataTupleTreeAzotFormatter : ITreeItemFormatter
    {
        IMultiDataProvider _provider;
        IEnviromentEx _env;
        CoordsConverter _pvk;

        public DataTupleTreeAzotFormatter(IMultiDataProvider provider, IEnviromentEx env)
        {
            _provider = provider;
            _env = env;
            _pvk = _env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, new TupleMetaData());
        }

        #region ITreeItemFormatter Members

        public string GetStreamName(string streamName)
        {
            if (streamName == "pc")
                return "Постоянный контроль";
            else if (streamName == "prp")
                return "Прописки АЗ";
            else if (streamName == "const")
                return "Константы";

            return streamName;
        }

        public string GetSingleDataTupleName(string streamName, DateTime dateTime)
        {
            if (streamName == "pc")
            {
                IDataTuple tup = _provider.GetData(dateTime, new string[] { "kgopc_info" })[0];
                DataParamTable info = (DataParamTable)tup["kgopc_info"];

                double specClock = Convert.ToDouble(info["specClock"]);
                int fiberNum = Convert.ToInt32(info["fiberNum"]);
                int pcPvk = Convert.ToInt32(info["pcPvk"]) - 1;

                if ((pcPvk >= 0) && ( pcPvk <115 ))
                {
                    Coords c = _pvk[fiberNum, pcPvk]; //(new FiberCoords(fiberNum, pcPvk));

                    return String.Format("{0:yyyy-MM-dd HH:mm}   ({4}) ПВК:{1} Нитка:{2}    Экс:{3:.#}",
                        dateTime, pcPvk + 1, fiberNum + 1, specClock, c);
                }
                else if (pcPvk == -1)
                {
                    return String.Format("{0:yyyy-MM-dd HH:mm}   [перемещение] Нитка:{1}    Экс:{2:.#}",
                        dateTime,  fiberNum + 1, specClock);
                }
                else
                {
                    return String.Format("{0:yyyy-MM-dd HH:mm}   [ПВК {3} ?] Нитка:{1}    Экс:{2:.#}",
                        dateTime,  fiberNum + 1, specClock, pcPvk+1);
                }

            }
            

            return dateTime.ToString();
        }

        public string GetSingleTupleItemName(string streamName, DateTime dateTime, TupleMetaData itemInfo)
        {
            return itemInfo.HumaneName;
        }

        public string GetMultiDataTupleName(string streamName, DateTime dateTime, int idx)
        {
            if (streamName == "prp")
            {
                if (idx == -1)
                {
                    IMultiDataTuple tup = _provider.GetData(dateTime, new string[] { "kgoprp_info" });
                    DataParamTable info = (DataParamTable)tup[0]["kgoprp_info"];
                    double specClock = Convert.ToDouble(info["specClock"]);

                    return String.Format("{0:yyyy-MM-dd HH:mm}   Экс:{1:.#}",
                        dateTime, specClock);
                }
                else
                {
                    IMultiDataTuple tup = _provider.GetData(dateTime, new string[] { "kgoprp_info" });
                    DataParamTable info = (DataParamTable)tup[idx]["kgoprp_info"];                    
                    int fiberNum = Convert.ToInt32(info["fiberNum"]);

                    return String.Format("Нитка {0}", fiberNum + 1);
                }
            }
            else
            {
                if (idx == -1)
                    return dateTime.ToString();
                else
                    return idx.ToString();
            }
        }

        public string GetMultiTupleItemName(string streamName, DateTime dateTime, int idx, TupleMetaData itemInfo)
        {
            if (idx == -1)
                return "[" + itemInfo.HumaneName + "]";
            else
                return itemInfo.HumaneName;
        }

        #endregion
    }
}
