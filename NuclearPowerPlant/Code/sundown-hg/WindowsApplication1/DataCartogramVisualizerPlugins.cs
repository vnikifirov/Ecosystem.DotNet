using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace corelib
{
#if !DOTNET_V11
    using DataArrayTimestamp = DataArray<Timestamp>;
#endif


    public class DataCartogramVisualizerPlugins
    {

#if !DOTNET_V11
        /// <summary>
        /// Сбор данных из колеекции <paramref name="tupels"/>. Сбор осуществляется по <paramref name="item"/> используя имя, присвоение
        /// номера происходит лишь по проверке ссылок.
        /// </summary>
        /// <param name="item">Параметр, по которому стоит производить отбор</param>
        /// <param name="tuples">Коллекция данных</param>
        /// <param name="coords">Координаты ячейки</param>
        /// <param name="currentPos">ВЫВОД: Номер <paramref name="item"/> в выходном массиве</param>
        /// <param name="dates">ВЫВОД: Список дат</param>
        /// <returns>Массив элементов, приведенный к double</returns>
        public static double[] CollectDataFromCoord(ITupleItem item, IEnumerable<IDataTuple> tuples, Coords coords, out int currentPos, out Timestamp[] dates)
#else
        public static double[] CollectDataFromCoord(ITupleItem item, IEnumerable tuples, Coords coords, out int currentPos, out Timestamp[] dates)
#endif
        {
            dates = null;
            currentPos = -1;
            if ((item != null) && (tuples != null))
            {
                string tname = item.GetName();
                return CollectDataFromCoord(tuples, coords, tname, item, null, out currentPos, out dates);
            }
            return null;
        }

#if !DOTNET_V11
        public static double[] CollectDataFromCoord(string sitem, IEnumerable<IDataTuple> tuples, Coords coords, Timestamp[] indates, out int currentPos, out Timestamp[] dates)
#else
        public static double[] CollectDataFromCoord(string sitem, IEnumerable tuples, Coords coords, Timestamp[] indates, out int currentPos, out Timestamp[] dates)
#endif
        {
            ITupleItem item = null;

            foreach (IDataTuple it in tuples)
            {
                if (it.GetParamSafe(sitem) != null)
                {
                    item = it[sitem];
                    break;
                }
            }

            dates = null;
            currentPos = -1;
            if (item == null)
                return null;
            else
                return CollectDataFromCoord(item, tuples, coords, indates, out currentPos, out dates);
        }

#if !DOTNET_V11
        public static double[] CollectDataFromCoord(string sitem, IEnumerable<IDataTuple> tuples, Coords coords)
#else
        public static double[] CollectDataFromCoord(string sitem, IEnumerable tuples, Coords coords)
#endif
        {
            Timestamp[] dates;
            int currentPos;
            return CollectDataFromCoord(sitem, tuples, coords, null, out currentPos, out dates);
        }

#if !DOTNET_V11
        public static double[] CollectDataFromCoord(ITupleItem item, IEnumerable<IDataTuple> tuples, Coords coords)
#else
        public static double[] CollectDataFromCoord(ITupleItem sitem, IEnumerable tuples, Coords coords)
#endif
        {
            Timestamp[] dates;
            int currentPos;
            return CollectDataFromCoord(item, tuples, coords, null, out currentPos, out dates);
        }



#if !DOTNET_V11
        /// <summary>
        /// Сбор данных из колеекции <paramref name="tupels"/>. Сбор осуществляется по <paramref name="item"/> используя имя, присвоение
        /// номера происходит лишь по проверке ссылок.
        /// </summary>
        /// <param name="item">Параметр, по которому стоит производить отбор</param>
        /// <param name="tuples">Коллекция данных</param>
        /// <param name="coords">Координаты ячейки</param>
        /// <param name="indates">Координаты интересующих дат</param>
        /// <param name="currentPos">ВЫВОД: Номер <paramref name="item"/> в выходном массиве</param>
        /// <param name="dates">ВЫВОД: Список дат</param>
        /// <returns>Массив элементов, приведенный к double</returns>
        public static double[] CollectDataFromCoord(ITupleItem item, IEnumerable<IDataTuple> tuples, Coords coords, Timestamp[] indates, out int currentPos, out Timestamp[] dates)
#else
        public static double[] CollectDataFromCoord(ITupleItem item, IEnumerable tuples, Coords coords,Timestamp[] indates, out int currentPos, out Timestamp[] dates)
#endif
        {
            dates = null;
            currentPos = -1;
            if ((item != null) && (tuples != null))
            {
                string tname = item.GetName();
                return CollectDataFromCoord(tuples, coords, tname, item, indates, out currentPos, out dates);
            }
            return null;
        }

#if !DOTNET_V11
        public static double[] CollectDataFromCoord(IEnumerable<IDataTuple> tuples, Coords coords, string tname, ITupleItem currentItem, Timestamp[] indates, out int currentPos, out Timestamp[] dates)
#else
        public static double[] CollectDataFromCoord(IEnumerable tuples, Coords coords, string tname, ITupleItem currentItem, Timestamp[] indates, out int currentPos, out Timestamp[] dates)
#endif
        {
            int countItems = 0;
            currentPos = -1;
            dates = null;

            foreach (IDataTuple t in tuples)
            {
                if ((indates == null) && (t.GetParamSafe(tname) as IDataCartogram) != null)
                    countItems++;
                else if (indates != null)
                {
                    foreach (Timestamp ts in indates)
                    {
                        if ((ts.DateTime == t.GetTimeDate()) && (t.GetParamSafe(tname) as IDataCartogram) != null)
                        {
                            countItems++;
                            break;
                        }
                    }
                }
            }

            if (countItems > 0)
            {
                Coords c = coords;
                double[] ar = new double[countItems];
                dates = new Timestamp[countItems];

                int current = -1;
                int k = 0;
                int i = 0;
                foreach (IDataTuple t in tuples)
                {
                    IDataCartogram cart = t.GetParamSafe(tname) as IDataCartogram;
                    if ((indates != null) && (cart != null))
                    {
                        cart = null;
                        foreach (Timestamp ts in indates)
                        {
                            if (ts.DateTime == t.GetTimeDate())
                            {
                                cart = t.GetParamSafe(tname) as IDataCartogram;
                                break;
                            }
                        }
                    }

                    if (cart != null)
                    {
                        dates[k] = t.GetTimeDate();
                        if (cart.IsValidCoord(c))
                            ar[k++] = cart[c, 0];


                        if (cart == currentItem)
                            current = i;
                    }
                    i++;
                }

                currentPos = current;
                return ar;


            }
            return null;
        }


#if !DOTNET_V11
        public static IEnumerable<IDataTuple> GetTuples(IDataCartogramVisualizerUI ui, string stream)
        {
            
            /////*********Роман*********/////
            //сортировка dates
            /*
            DateTime[] dates = ui.GetDataTupleVisualizer().GetMultiDataProvider().GetDates(stream);
            bool t = true;
            while (t == true)
            {
                t = false;
                for (int i = 0; i < dates.Length - 1; i++)
                {
                    if (dates[i] > dates[i + 1])
                    {
                        DateTime temp_date = dates[i];
                        dates[i] = dates[i + 1];
                        dates[i + 1] = temp_date;
                        t = true;
                    }
                }
            }
            foreach (DateTime dt in dates)
            {
                yield return ui.GetDataTupleVisualizer().GetMultiDataProvider().GetData(dt, stream)[0];
            }
            */
            /////*********Роман*********/////
            
            foreach (DateTime dt in ui.GetDataTupleVisualizer().GetMultiDataProvider().GetDates(stream))
            {
                yield return ui.GetDataTupleVisualizer().GetMultiDataProvider().GetData(dt, stream)[0];
            }
            
        }
#else		
		class GetTuplesEnumerable : IEnumerable
		{
			String _stream;
			IMultiDataProvider _row;

			public GetTuplesEnumerable(IMultiDataProvider r, string stream)
			{
				_stream = stream;
				_row = r;
			}
		
			class GetTuplesEnumarator : IEnumerator
			{
				IEnumerator _dates;
				String _stream;
				IMultiDataProvider _row;

				public GetTuplesEnumarator(IMultiDataProvider r, string stream)
				{
					_stream = stream;
					_row = r;
					_dates = _row.GetDates(_stream).GetEnumerator();
				}

        #region IEnumerator Members

				public object Current
				{
					get { return _row.GetData((DateTime)_dates.Current, _stream); }
				}

				public bool MoveNext()
				{
					return _dates.MoveNext();
				}

				public void Reset()
				{
					_dates.Reset();
				}

                #endregion
			}

			public IEnumerator GetEnumerator()
			{
				return new GetTuplesEnumarator(_row, _stream);
			}
		}

		public static IEnumerable GetTuples(IDataCartogramVisualizerUI ui, string stream)
        {
            return new GetTuplesEnumerable(ui.GetDataTupleVisualizer().GetMultiDataProvider(), stream);
        }
#endif
    }

    public class DCVPluginCellsReport : ActionDataCartogramVisualizerUI
    {
        public DCVPluginCellsReport()
        {
            _name = "PluginCellsReport";
            _humaneName = "Отчет по выделенным ячейкам";
            _descr = "Отчет по выделенным ячейкам";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADCVEventArgs args = (ADCVEventArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            doReport(ui);
        }

        protected void doReport(IDataCartogramVisualizerUI ui)
        {
            Coords[] cdr = ui.GetSelectedCells();

            ITupleItem itemSingle = ui.GetActiveTupleItem();
            string stream = ui.GetActiveTupleItemStream();

            //if (item == null)
            //    item = currectTuple;
            /*
            IPvkInfo pvk = null;
            if (pvk == null)
            {
                DataCartogram c = ui.GetActiveTupleItem() as DataCartogram;
                if (c != null)
                    pvk = c.PvkInfo;
            }
            */
            CoordsConverter pvk = ui.GetDataTupleVisualizer().GetEnviroment().GetSpecialConverter(
                CoordsConverter.SpecialFlag.PVK, new TupleMetaData());

            StringBuilder sb = new StringBuilder();
            IDataTuple fullTuple = ui.GetDataTupleVisualizer().GetActiveDataTuple();

            if ((itemSingle != null) && (fullTuple != null) && (DataCartogramVisualizerPlugins.GetTuples(ui, stream) != null))
            {
                //ITupleItem[] origs = fullTuple.GetData();

                foreach (Coords c in cdr)
                {
                    if (c.IsOk)
                    {
                        string fiber = "";
                        if (pvk != null)
                        {
                            fiber = " ПВК неразведено";

                            FiberCoords fc = pvk.GetFiberCoords(c);
                            if (fc.IsValid)
                                fiber = String.Format(" ПВК {0} Нитка {1}", fc.Pvk + 1, fc.Fiber + 1);
                        }
                        sb.AppendFormat("Ячейка {0}{2}\r\n{1,-40};", c, "Параметер", fiber);
                        foreach (IDataTuple tup in DataCartogramVisualizerPlugins.GetTuples(ui, stream))
                        {
                            sb.AppendFormat("{0,-40};", tup.GetTimeDate());
                        }
                        sb.Append("\r\n");

                        //foreach (ITupleItem item in origs)
                        for (int itemIdx = 0; itemIdx < fullTuple.ItemsCount; itemIdx++ )
                        {
                            ITupleItem item = fullTuple[itemIdx];
                            string name = item.GetName();

                            //if (_tuples[0].GetParamSafe(name) as DataCartogram != null)
                            if (fullTuple.GetParamSafe(name) as IDataCartogram != null)
                            {
                                sb.AppendFormat("{0,-40};", item.GetHumanName());
                                foreach (IDataTuple tup in DataCartogramVisualizerPlugins.GetTuples(ui, stream))
                                {
                                    IDataCartogram it = tup.GetParamSafe(name) as IDataCartogram;
                                    if (it != null)
                                    {
                                        string val;
                                        if (!it.IsValidCoord(c))
                                            val = "[нет]";
                                        else
                                        {
                                            if (it.Layers > 1)
                                            {
                                                StringBuilder sbn = new StringBuilder();
                                                sbn.Append("{");
                                                for (int j = 0; j < it.Layers; j++)
                                                {
                                                    sbn.Append(it[c, j].ToString());
                                                    if (j < it.Layers - 1)
                                                        sbn.Append("; ");
                                                }
                                                sbn.Append("}");
                                                val = sbn.ToString();
                                            }
                                            else
                                            {
                                                val = it[c, 0].ToString();
                                            }
                                        }

                                        sb.AppendFormat("{0,-40};", val);
                                    }
                                    else
                                        sb.AppendFormat("{0,-40};", "");

                                }

                                sb.Append("\r\n");
                            }
                        }
                        sb.Append("\r\n");
                    }
                }

                if (cdr.Length > 0)
                {
                    Form f = new Form();
                    f.Text = "Отчет по выделенным каналам";
                    f.Size = new Size(600, 400);
                    TextBox tb = new TextBox();
                    tb.Multiline = true;
                    tb.ScrollBars = ScrollBars.Both;
                    tb.ReadOnly = true;
                    tb.Dock = DockStyle.Fill;
                    tb.Text = sb.ToString();
                    tb.Font = new Font(FontFamily.GenericMonospace, 8.25f);
                    f.Controls.Add(tb);

                    f.ShowDialog();
                }
            }

        }
    }

    public class DCVPluginClearSelection : ActionDataCartogramVisualizerUI
    {
        public DCVPluginClearSelection()
        {
            _name = "PluginClearSelection";
            _humaneName = "Снять выделения";
            _descr = "Снять выделения";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADCVEventArgs args = (ADCVEventArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            ui.SetSelectedCells(new Coords[0] { });
        }
    }


    public class DCVPluginCellHistory : ActionDataCartogramVisualizerUI
    {
        public DCVPluginCellHistory()
        {
            _name = "PluginCellHistory";
            _humaneName = "История только этой ячейки";
            _descr = "История только этой ячейки";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }


        public void onClick(object sender, EventArgs e)
        {
            ADCVEventCoordArgs args = (ADCVEventCoordArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            Coords coord = args.Coords;
            Timestamp[] dates;
            ITupleItem item = ui.GetActiveTupleItem();
            string stream = ui.GetActiveTupleItemStream();

            int current;
            double[] data = DataCartogramVisualizerPlugins.CollectDataFromCoord(ui.GetActiveTupleItem(), DataCartogramVisualizerPlugins.GetTuples(ui, stream), coord, out current, out dates);
            if ((data != null) && (data.Length > 1))
            {
                DataArray ar = DataArray.Create(
                    new TupleMetaData("history", String.Format("{0} ({1})", item.GetHumanName(), coord), DateTime.Now, TupleMetaData.StreamAuto),
                    data);

                DataArrayTimestamp ar1 = new DataArrayTimestamp(
                    new TupleMetaData("history", "Дата", DateTime.Now, TupleMetaData.StreamAuto), dates);

                DataGrid d = DataArrayVisualizer.CombineDataArrays(new DataArray[] { ar1, ar });
                d.Columns.RemoveAt(0);
                d.HeadColumns = 1;

                ui.GetDataTupleVisualizer().SetDataGrid(d);

                //ui.GetDataTupleVisualizer().SetActiveTupleItem(ar, stream);
            }
            else
            {
                MessageBox.Show("Невозможно построить историю в данном режиме", "Ошибка");
            }
        }
    }

    public delegate double[] CollectData(ITupleItem it, IEnumerable<IDataTuple> tupels, Coords c, out int pos, out Timestamp[] dates);
    public class PluginDataCollectorHelper
    {
        static public void onClick(IDataCartogramVisualizerUI ui, CollectData dataCollector)
        {
            //ADCVEventArgs args = (ADCVEventArgs)e;
            //IDataCartogramVisualizerUI ui = args._ui;

            Coords[] coords = ui.GetSelectedCells();
            if (coords.Length == 0)
                return;

            ITupleItem item = ui.GetActiveTupleItem();
            string stream = ui.GetActiveTupleItemStream();

            int current = -1;

            bool ok = true;

            DataArray[] ar = new DataArray[1 + coords.Length];
            Timestamp[] dates = null;
            for (int i = 0; i < coords.Length; i++)
            {
                double[] data = dataCollector(item, DataCartogramVisualizerPlugins.GetTuples(ui, stream), coords[i], out current, out dates);
                    
                ok = ok && (data != null) && (data.Length > 1);

                if (ok)
                {
                    ar[1 + i] = DataArray.Create(
                        new TupleMetaData("history", String.Format("{1} ({0})", item.GetHumanName(), coords[i]), DateTime.Now, TupleMetaData.StreamAuto), data);
                }
            }
            if (dates != null)
                ar[0] = DataArray.Create(
                    new TupleMetaData("history", "Дата", DateTime.Now, TupleMetaData.StreamAuto), dates);

            if (ok)
            {
                DataGrid d = DataArrayVisualizer.CombineDataArrays(ar);
                d.Columns.RemoveAt(0);
                d.HeadColumns = 1;

                ui.GetDataTupleVisualizer().SetDataGrid(d);

                //ui.GetDataTupleVisualizer().SetActiveTupleItems(ar, stream);
            }
            else
            {
                MessageBox.Show("Невозможно построить историю в данном режиме", "Ошибка");
            }
        }

    }


    public class DCVPluginSelectedCellsHistory : ActionDataCartogramVisualizerUI
    {
        public DCVPluginSelectedCellsHistory()
        {
            _name = "PluginSelectedCellsHistory";
            _humaneName = "История выделенных ячеек";
            _descr = "История выделенных ячеек";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }


        public void onClick(object sender, EventArgs e)
        {
            ADCVEventArgs args = (ADCVEventArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            //ITupleItem item = ui.GetActiveTupleItem();

            PluginDataCollectorHelper.onClick(ui, 
                DataCartogramVisualizerPlugins.CollectDataFromCoord);
        }
    }



    public class DCVPluginTripleFlow : ActionDataCartogramVisualizerUI
    {
        public DCVPluginTripleFlow()
        {
            _name = "PluginTripleFlow";
            _humaneName = "По 3 методикам";
            _descr = "По 3 методикам";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }


        public static double[] sCollectData(ITupleItem it, IEnumerable<IDataTuple> tupels, Coords c, out int pos, out Timestamp[] dates)
        {
            Timestamp[] tmp;
            IDataTuple tup = null;
            foreach (IDataTuple tup2 in tupels)
            {
                tup = tup2;
                if (tup.GetParamSafe("flow_az1_cart") != null &&
                    tup.GetParamSafe("flow_dp_cart") != null &&
                    tup.GetParamSafe("flow") != null)
                break;
            }

            if (tup == null)
            {
                pos = -1;
                dates = null;
                return null;
            }

            double[] flow_az = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow_az1_cart"], tupels, c, out pos, out dates);
            double[] flow_dp = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow_dp_cart"], tupels, c, out pos, out dates);

            double[] flow = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow"], tupels, c, dates, out pos, out tmp);

            double[] ret = new double[flow.Length];

            for (int i = 0; i < flow.Length; i++)
                ret[i] = (flow_az[i] - flow[i]) * (flow_dp[i] - flow[i]) / (flow[i] * flow[i]);

            return ret;
        }


        public void onClick(object sender, EventArgs e)
        {
            ADCVEventArgs args = (ADCVEventArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            //ITupleItem item = ui.GetActiveTupleItem();

            PluginDataCollectorHelper.onClick(ui,
                sCollectData);
        }
    }


    public class DCVPluginErrFlowAz : ActionDataCartogramVisualizerUI
    {
        public DCVPluginErrFlowAz()
        {
            _name = "PluginErrFlowAz";
            _humaneName = "Рассогласование: Азот";
            _descr = "Рассогласование: Азот";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }


        public static double[] sCollectData(ITupleItem it, IEnumerable<IDataTuple> tupels, Coords c, out int pos, out Timestamp[] dates)
        {
            Timestamp[] tmp;
            IDataTuple tup = null;
            foreach (IDataTuple tup2 in tupels)
            {
                tup = tup2;
                if (tup.GetParamSafe("flow_az1_cart") != null &&
                    tup.GetParamSafe("flow") != null)
                    break;
            }

            double[] flow_az = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow_az1_cart"], tupels, c, out pos, out dates);
            double[] flow = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow"], tupels, c, dates, out pos, out tmp);            

            double[] ret = new double[flow.Length];

            for (int i = 0; i < flow.Length; i++)
                ret[i] = (flow_az[i] - flow[i]) / flow[i];

            return ret;
        }


        public void onClick(object sender, EventArgs e)
        {
            ADCVEventArgs args = (ADCVEventArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            //ITupleItem item = ui.GetActiveTupleItem();

            PluginDataCollectorHelper.onClick(ui,
                sCollectData);
        }
    }


    public class DCVPluginErrFlowDp : ActionDataCartogramVisualizerUI
    {
        public DCVPluginErrFlowDp()
        {
            _name = "PluginErrFlowDp";
            _humaneName = "Рассогласование: DP";
            _descr = "Рассогласование: DP";
            _action = Actions.CellItemContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageSave;
        }


        public static double[] sCollectData(ITupleItem it, IEnumerable<IDataTuple> tupels, Coords c, out int pos, out Timestamp[] dates)
        {
            Timestamp[] tmp;
            IDataTuple tup = null;
            foreach (IDataTuple tup2 in tupels)
            {
                tup = tup2;
                if (tup.GetParamSafe("flow_dp_cart") != null &&
                    tup.GetParamSafe("flow") != null)
                    break;
            }

            double[] flow_dp = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow_dp_cart"], tupels, c, out pos, out dates);
            double[] flow = DataCartogramVisualizerPlugins.CollectDataFromCoord(tup["flow"], tupels, c, dates, out pos, out tmp); 

            double[] ret = new double[flow.Length];

            for (int i = 0; i < flow.Length; i++)
                ret[i] = (flow_dp[i] - flow[i]) / flow[i];

            return ret;
        }


        public void onClick(object sender, EventArgs e)
        {
            ADCVEventArgs args = (ADCVEventArgs)e;
            IDataCartogramVisualizerUI ui = args._ui;

            //ITupleItem item = ui.GetActiveTupleItem();

            PluginDataCollectorHelper.onClick(ui,
                sCollectData);
        }
    }

}
