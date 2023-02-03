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

using corelib;
using WindowsApplication1;

namespace corelib
{
    public class DefImages
    {
        public static Image ImageSaveGreen
        {
            get { return Resource1.ImageSaveGreen; }
        }
        public static Image ImageRun
        {
            get { return Resource1.ImageRun; }
        }
        public static Image ImageDSL
        {
            get { return Resource1.ImageDSL; }
        }
        public static Image ImageRecoveryAll
        {
            get { return Resource1.ImageRecoveryAll; }
        }

        public static Image ImageOpen
        {
            get { return Resource1.ImageOpen; }
        }

        public static Image ImageAdd
        {
            get { return Resource1.ImageAdd; }
        }

        public static Image ImageExport
        {
            get { return Resource1.ImageExport; }
        }

        public static Image ImageCalc
        {
            get { return Resource1.ImageCalc; }
        }

        public static Image ImageDslScript
        {
            get { return Resource1.ImageDslScript; }
        }
    }

    public class DTVPluginExportExcel : ActionDataTupleVisualizerUI
    {
        public DTVPluginExportExcel()
        {
            _name = "ExportExcelPluginItem";
            _humaneName = "Экспорт в Excel";
            _descr = "Экспорт в Excel";
            _action = Actions.TupleItemContextMenu;

            _handler = new EventHandler(onClick);

            _image = Resource1.ImageExcel;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventItemArgs args = (ADTVEventItemArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            ITupleItem currectTuple = args._item;

            currectTuple.CreateDataGrid(ui.GetEnviroment()).ExpandAll().ExportExcel();
        }
    }

    public class DTVPluginSaveItem : ActionDataTupleVisualizerUI
    {
        public DTVPluginSaveItem()
        {
            _name = "SavePluginItem";
            _humaneName = "Сохранить в .CSV формате";
            _descr = "Сохранить в .CSV формате";
            _action = Actions.TupleItemContextMenu;

            _handler = new EventHandler(onClick);

            _image = Resource1.ImageSave;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventItemArgs args = (ADTVEventItemArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            ITupleItem currectTuple = args._item;

            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "CSV Files|*.csv";
            fd.Title = String.Format("Сохранение, {0}", currectTuple.GetHumanName());
            fd.FileName = String.Format("{0:d}_{1}.csv", currectTuple.GetTimeDate(), currectTuple.GetName());
            if (fd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sr = new StreamWriter(fd.FileName))
                {
                    sr.Write(currectTuple.DumpCSV());
                }
            }
        }
    }


    public class DTVPluginExportExcelButton : ActionDataTupleVisualizerUI
    {
        public DTVPluginExportExcelButton()
        {
            _name = "ExportExcelPluginButton";
            _humaneName = "Экспорт в Excel";
            _descr = "Экспорт в Excel";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = Resource1.ImageExcel;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            DataGrid d = ui.GetActiveDataGrid();
            if (d != null)
            {
                d.ExpandAll().ExportExcel();
            }
            else
            {
                ITupleItem currectTuple = ui.GetActiveTupleItem();
                if (currectTuple != null)
                {
                    currectTuple.CreateDataGrid(ui.GetEnviroment()).ExpandAll().ExportExcel();
                }
            }
        }
    }

    public class DTVPluginSaveTuple : ActionDataTupleVisualizerUI
    {
        public DTVPluginSaveTuple()
        {
            _name = "SavePluginTuple";
            _humaneName = "Сохранить все в .CSV формате";
            _descr = "Сохранить в .CSV формате";
            _action = Actions.DataTupleContextMenu;

            _handler = new EventHandler(onClick);

            _image = Resource1.ImageSave;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventTupleArgs args = (ADTVEventTupleArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            IDataTuple currectTuple = args._tuple;

            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.Description = "Выберете папку для экспорта всего списка";
            fb.ShowNewFolderButton = true;
            if (fb.ShowDialog() == DialogResult.OK)
            {
                DumpItems(fb.SelectedPath + "/", currectTuple);
            }
        }

        public static void DumpItems(String path, IDataTuple nodes)
        {
            DumpItems(path, nodes, false);
        }

        public static void DumpItems(String path, IDataTuple nodes, bool time)
        {
            //foreach (ITupleItem ct in nodes.GetData())
            for (int i = 0; i < nodes.ItemsCount; i++)
            {
                ITupleItem ct = nodes[i];
                String filename = path + ((!time) ?
                    String.Format("{0:d}_{1}.csv", ct.GetTimeDate(), ct.GetName()) :
                    String.Format("{0:d}_{2:D2}.{3:D2}_{1}.csv", ct.GetTimeDate(), ct.GetName(),
                        ct.GetTimeDate().Hour, ct.GetTimeDate().Minute));

                using (StreamWriter sr = new StreamWriter(filename))
                {
                    sr.Write(ct.DumpCSV());
                }
            }
        }
    }

    public class _DTVPluginSaveAllDataTuple : ActionDataTupleVisualizerUI
    {
        public _DTVPluginSaveAllDataTuple()
        {
            _name = "SavePluginAllTuple";
            _humaneName = "Экспорт";
            _descr = "Сохранить все данные в .CSV формате";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = Resource1.ImageSave;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.Description = "Выберете папку для экспорта всех данных по всем спискам";
            fb.ShowNewFolderButton = true;
            if (fb.ShowDialog() == DialogResult.OK)
            {
                string[] streams = ui.GetMultiDataProvider().GetStreamNames();
                foreach (string stream in streams)
                {
                    foreach (DateTime dt in ui.GetMultiDataProvider().GetDates(stream))
                    {
                        IDataTuple n = ui.GetMultiDataProvider().GetData(dt, stream)[0];

                        string date = String.Format("{0:yyyy}{0:MM}{0:dd}_{0:HH}{0:mm}", dt);
                        if (streams.Length > 1)
                            DTVPluginSaveTuple.DumpItems(fb.SelectedPath + "/" + stream + "_" + "d" + date + "_", n, true);
                        else
                            DTVPluginSaveTuple.DumpItems(fb.SelectedPath + "/" + date + "_", n, true);
                    }
                }
            }
        }
    }


    public class DTVPluginCombineSheets : ActionDataTupleVisualizerUI
    {
        public DTVPluginCombineSheets()
        {
            _name = "CombineSheets";
            _humaneName = "Комбинированный отчет";
            _descr = "Комбинированный отчет по топливным ячейкам";
            _action = Actions.DataTupleContextMenu;

            _handler = new EventHandler(onClick);

            //_image = Resource1.ImageFullReport;
        }

        static public DataGrid combineTuple(IEnviroment env, CoordsConverter c, IDataTuple t, string[] skip)
        {
            DataGrid d = new DataGrid();

            //CoordsConverter c = env.GetSpecialConverter(coordsType, new TupleMetaData());


            d.Columns.Add(new DataGrid.Column("Координата", typeof(Coords), env.GetDefFormatter(FormatterType.Coords)));

            int total = 0;
            for (int i = 0; i < t.ItemsCount; i++)
            {
                ITupleItem item = t[i];
                IDataCartogram cart = item as IDataCartogram;
                if (cart == null)
                    continue;
                if (cart.Layers > 1)
                    continue;
                if (skip != null)
                {
                    foreach (string s in skip)
                    {
                        if (s == cart.Name)
                            goto skip_column;
                    }
                }

                d.Columns.Add(new DataGrid.Column(cart.GetHumanName(), cart.ElementType, cart.GetDefForamtter(env)));
                total++;

            skip_column: ;
            }

            d.Preserve(c.Length);

            int[] count = new int[total];
            for (int j = 0; j < c.Length; j++)
            {
                int m = 1;
                DataGrid.DataRow r = new DataGrid.DataRow(total + 1);
                Coords crd = c[j];
                r[0] = crd;

                for (int i = 0; i < t.ItemsCount; i++)
                {
                    ITupleItem item = t[i];
                    IDataCartogram cart = item as IDataCartogram;
                    if (cart == null)
                        continue;
                    if (cart.Layers > 1)
                        continue;
                    if (skip != null)
                    {
                        foreach (string s in skip)
                        {
                            if (s == cart.Name)
                                goto skip_column2;
                        }
                    }


                    if (cart.IsValidCoord(crd))
                    {
                        r[m] = cart.GetObject(crd, 0);
                        count[m - 1]++;
                    }

                    m++;

                skip_column2: ;
                }
                d.Rows.Add(r);

            }
            return d;
        }

        public static readonly string[] gSkipList = { "dkv_1", "dkv_2", "dkr_corr_1", "dkr_corr_2", "dkr_1", "dkr_2", "suz" };
        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventTupleArgs args = (ADTVEventTupleArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IDataTuple t = args._tuple;

            ui.SetDataGrid(combineTuple(ui.GetEnviroment(),
                ui.GetEnviroment().GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()),
                t, gSkipList));
        }
    }



    public class DTVPluginCombineAllActive : ActionDataTupleVisualizerUI
    {
        public DTVPluginCombineAllActive()
        {
            _name = "CombineAllActive";
            _humaneName = "Все срезы";
            _descr = "Полный отчет по топливным ячейкам во всех срезах";
            _action = Actions.ToolBarButton;

            _image = Resource1.ImageFullReport;
            _handler = new EventHandler(onClick);
        }

        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IMultiDataProvider prov = ui.GetMultiDataProvider();

            string[] names = prov.GetStreamNames();
            if (names.Length != 1)
                return;

            DataGrid d = new DataGrid();

            DateTime[] dates = prov.GetDates(names[0]);
            DataGrid[] grids = new DataGrid[dates.Length];

            IDataTuple[] tuples = new IDataTuple[dates.Length];

            int items = 0;
            int rows = 1;
            for (int i = 0; i < dates.Length; i++)
            {
                IMultiDataTuple mdt = prov.GetData(dates[i], names[0]);
                if (mdt.Count > 1)
                    continue;

                IDataTuple t = tuples [i] = mdt[0];
                grids[i] = DTVPluginCombineSheets.combineTuple(ui.GetEnviroment(),
                    ui.GetEnviroment().GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()),
                    t, DTVPluginCombineSheets.gSkipList);

                if (items == 0)
                {
                    items = grids[i].RowCount;
                    d.Columns.Add(grids[i].Columns[0]);
                }
                
                for (int k = 1; k < grids[i].ColumnCount; k++, rows++)
                {
                    DataGrid.Column c = grids[i].Columns[k];
                    d.Columns.Add(new DataGrid.Column(
                        String.Format("{0}: {1}", c.Header, dates[i]), c.ColumnType, c.Converter));
                }

                ui.SetStatusString(String.Format("Выполение... {0} %", 100 * (i+1) / (dates.Length)));
            }

            d.Preserve(items);            

            for (int j = 0; j < items;j++ )
            {
                int k = 1;
                bool first = true;
                DataGrid.DataRow r = new DataGrid.DataRow(rows);
                for (int i = 0; i < dates.Length; i++)
                {
                    DataGrid g = grids[i];
                    if (g == null)
                        continue;

                    if (first)
                    {
                        r[0] = g.Rows[j][0];
                        first = false;
                    }

                    for (int t = 1; t < g.ColumnCount; t++)
                        r[k++] = g.Rows[j][t];                    
                }

                d.Rows.Add(r);
            }

            ui.SetDataGrid(d);
            ui.SetStatusString("Готово");
        }
    }


    public class RDTVPluginMakeVostReport : ActionDataTupleVisualizerUI
    {
        public RDTVPluginMakeVostReport()
        {
            _name = "MakeVostReport";
            _humaneName = "Отчет по восстановлению";
            _descr = "Отчет по восстановлению";
            _action = Actions.ToolBarButton;

            _image = Resource1.ImageRecInfo;

            _handler = new EventHandler(onClick);
        }

        static public double avgCart(CoordsConverter c, DataCartogram prev, DataCartogram curr)
        {
            double avg = 0;
            for (int i = 0; i < c.Length; i++)
            {
                double delta = Math.Abs((curr[c[i], 0] - prev[c[i], 0]) / curr[c[i], 0]);
                if (Double.IsInfinity(delta))
                    continue;

                avg += delta;
            }
            return avg / c.Length;
        }

        static public int badCount(CoordsConverter c, DataCartogram prev, DataCartogram curr)
        {
            int count = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if ((prev[c[i], 0] != 0) && (curr[c[i], 0] == 0))
                    count++;
            }
            return count;
        }

        static public double avgSqrtCart(CoordsConverter c, DataCartogram prev, DataCartogram curr)
        {
            double avg = 0;
            for (int i = 0; i < c.Length; i++)
            {
                double delta = (curr[c[i], 0] - prev[c[i], 0]) / (curr[c[i], 0]);
                if (Double.IsInfinity(delta))
                    continue;

                avg += delta * delta;
            }
            return Math.Sqrt(avg / c.Length);
        }

        public struct ErrInfo
        {
            public Coords crd;
            public double err;

            public override string ToString()
            {
                return String.Format("{0}: {1}", crd, err);
            }
        }

        static public ErrInfo[] maxErrCart(CoordsConverter c, DataCartogram prev, DataCartogram curr, int cnt)
        {
            return maxErrCart(c, prev, curr, cnt, false);
        }

        static public ErrInfo[] maxErrCart(CoordsConverter c, DataCartogram prev, DataCartogram curr, int cnt, bool exInf)
        {
            ErrInfo[] err = new ErrInfo[cnt];

            for (int i = 0; i < c.Length; i++)
            {
                double delt = ((curr[c[i], 0] - prev[c[i], 0]) / curr[c[i], 0]);
                double delta = Math.Abs(delt);

                if (exInf && Double.IsInfinity(delt))
                    continue;
        
                for (int j = 0; j < err.Length; j++)
                {
                    if (delta > Math.Abs(err[j].err))
                    {
                        for (int l = err.Length - 1; l > j; l--)
                            err[l] = err[l - 1];

                        err[j].err = delt;
                        err[j].crd = c[i];

                        goto next;
                    }
                }
            next: ;
            }
            return err;
        }


        static public int moreThanGivenCart(CoordsConverter c, DataCartogram prev, DataCartogram curr, double val)
        {
            int cnt = 0;
            for (int i = 0; i < c.Length; i++)
            {
                double delta = Math.Abs((curr[c[i], 0] - prev[c[i], 0]) / (curr[c[i], 0]));
                if (delta > val)
                    cnt++;
            }
            return cnt;
        }

        static public Coords[] getDiff(CoordsConverter c, DataCartogram prev, DataCartogram curr, double d)
        {
            Coords[] tmp = new Coords[c.Length];
            int cnt = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (Math.Abs(curr[c[i], 0] - prev[c[i], 0]) > d)
                {
                    tmp[cnt++] = c[i];
                }
            }

            Coords[] m = new Coords[cnt];
            for (int i = 0; i < m.Length; i++)
                m[i] = tmp[i];

            return m;
        }

        static public Coords[] getBad(CoordsConverter c, DataCartogram prev, DataCartogram curr, double d)
        {
            Coords[] tmp = new Coords[c.Length];
            int cnt = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if ((prev[c[i], 0] != 0) && (curr[c[i], 0] == 0))
                {
                    tmp[cnt++] = c[i];
                }
            }

            Coords[] m = new Coords[cnt];
            for (int i = 0; i < m.Length; i++)
                m[i] = tmp[i];

            return m;
        }

        static public DataGrid makeReport(IEnviroment env, IMultiDataTuple mtune, IMultiDataTuple mcurr)
        {
            IDataTuple tune = mtune[0];
            IDataTuple curr = mcurr[0];

            int maxCells = 3;
            IInfoFormatter flf = env.GetDefFormatter(FormatterType.RealHumane);

            CoordsConverter c = ((IDataCartogram)curr["flow_az1_cart"]).AllCoords;

            DataGrid d = new DataGrid();
            d.Columns.Add("Дата настройки", typeof(DateTime));
            d.Columns.Add("Дата восстановления", typeof(DateTime));
            d.Columns.Add("Топливных ячеек", typeof(int));
            d.Columns.Add("Не удалось восстановить ячеек", typeof(int));

            d.Columns.Add("Сред. ош. по азоту %", typeof(double), flf);
            d.Columns.Add("Сред. ош. по dP %", typeof(double), flf);

            d.Columns.Add("Среднкв. ош. по азоту %", typeof(double), flf);
            d.Columns.Add("Среднкв. ош. по dP %", typeof(double), flf);

            for (int i = 0; i < maxCells; i++)
            {
                if (i == 0)
                {
                    d.Columns.Add("Макс ош аз %", typeof(double), flf);
                    d.Columns.Add("Координата", typeof(Coords));
                }
                else
                {
                    d.Columns.Add(String.Format("Макс ош аз {0} %", i), typeof(double), flf);
                    d.Columns.Add(String.Format("Координата {0}", i), typeof(Coords));
                }
            }
            
            d.Columns.Add("Кол-во с ош > 15% по азоту", typeof(int));
            d.Columns.Add("Кол-во с ош > 20% по азоту", typeof(int));            
            d.Columns.Add("Кол-во с ош > 30% по азоту", typeof(int));
            d.Columns.Add("Кол-во с ош > 40% по азоту", typeof(int));
            d.Columns.Add("Кол-во с ош > 50% по азоту", typeof(int));

            DataParamTable tcurr = (DataParamTable)curr["rbmk_params"];
            DataParamTable ttune = (DataParamTable)tune["rbmk_params"];

            d.Columns.Add("Мошность (настр)", typeof(double), flf);
            d.Columns.Add("Мошность (восст)", typeof(double), flf);

            bool doZrk = curr.GetParamSafe("zrk") != null;
            if (doZrk)
                d.Columns.Add("Ячеек с изменением ЗРК", typeof(int));

            DataGrid.DataRow r = new DataGrid.DataRow(d.ColumnCount);
            int m = 0;
            r[m++] = tune.GetTimeDate();
            r[m++] = curr.GetTimeDate();
            r[m++] = c.Length;
            r[m++] = badCount(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"]);

            r[m++] = 100 * avgCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"]);
            r[m++] = 100 * avgCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_dp_cart"]);

            r[m++] = 100 * avgSqrtCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"]);
            r[m++] = 100 * avgSqrtCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_dp_cart"]);

            ErrInfo[] ei = maxErrCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"], maxCells, true);
            for (int i = 0; i < maxCells; i++)
            {
                r[m++] = 100 * ei[i].err;
                r[m++] = ei[i].crd;
            }

            r[m++] = moreThanGivenCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"], 0.15);
            r[m++] = moreThanGivenCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"], 0.20);
            r[m++] = moreThanGivenCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"], 0.30);
            r[m++] = moreThanGivenCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"], 0.40);
            r[m++] = moreThanGivenCart(c, (DataCartogram)curr["flow"], (DataCartogram)curr["flow_az1_cart"], 0.50);

            r[m++] = ttune["totalHeatPower"].ToDouble();
            r[m++] = tcurr["totalHeatPower"].ToDouble();

            if (doZrk)
                r[m++] = getDiff(c, (DataCartogram)tune["zrk"], (DataCartogram)curr["zrk"], 0).Length;

            d.Rows.Add(r);
            return d;
        }

        static public void findCurrTune(IMultiDataProvider prov, out IMultiDataTuple curr, out IMultiDataTuple tune)
        {
            curr = null;
            tune = null;

            string[] names = prov.GetStreamNames();
            if (names.Length != 1)
                return;

            DateTime[] dates = prov.GetDates(names[0]);
            if (dates.Length != 2)
                return;

            IMultiDataTuple m1 = prov.GetData(dates[0], names[0]);
            IMultiDataTuple m2 = prov.GetData(dates[1], names[0]);

            if (m1.Count != 1 || m2.Count != 1)
                return;

            if (m1[0].GetParamSafe("flow_az1_cart") != null)
            {
                curr = m1;
                tune = m2;
            }
            else if (m2[0].GetParamSafe("flow_az1_cart") != null)
            {
                curr = m2;
                tune = m1;
            }
            else
                return;

        }

        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IMultiDataProvider prov = ui.GetMultiDataProvider();

            IMultiDataTuple curr;
            IMultiDataTuple tune;

            findCurrTune(prov, out curr, out tune);

            if (curr == null)
                return;

            ui.SetDataGrid(makeReport(ui.GetEnviroment(), tune, curr));
        }
    }


    public class RDTVPluginFindMaxErrors : ActionDataTupleVisualizerUI
    {
        public RDTVPluginFindMaxErrors()
        {
            _name = "FindMaxErrors";
            _humaneName = "Отчет по рассогласованиям";
            _descr = "Отчет по максимальным рассогласованиям";
            _action = Actions.ToolBarButton;

            _image = Resource1.ImageErrInfo;

            _handler = new EventHandler(onClick);
        }

        static public DataGrid makeReport(IEnviroment env, IMultiDataTuple tune, IMultiDataTuple curr)
        {
            CoordsConverter c = ((IDataCartogram)curr[0]["flow_az1_cart"]).AllCoords;
            return makeReport(env, tune, curr, c);
        }

        static public DataGrid makeReport(IEnviroment env, IMultiDataTuple tune, IMultiDataTuple curr, CoordsConverter c)
        {
            IInfoFormatter flf = env.GetDefFormatter(FormatterType.RealHumane);

            RDTVPluginMakeVostReport.ErrInfo[] ei = RDTVPluginMakeVostReport.maxErrCart(c,
                (DataCartogram)curr[0]["flow"], (DataCartogram)curr[0]["flow_az1_cart"],
                c.Length);

            DataGrid g = new DataGrid();

            g.Columns.Add(new DataGrid.Column("Координата",
                typeof(Coords),
                env.GetDefFormatter(FormatterType.Coords)));

            g.Columns.Add("Рассогласование расх. по азоту отн. шторма %", typeof(double), flf);

            g.Columns.Add("Изменение мощности %", typeof(double), flf);
            g.Columns.Add("Изменение расхода (штатн) %", typeof(double), flf);
            g.Columns.Add("Изменение энерговыработки %", typeof(double), flf);

            //g.Columns.Add("Отношение Расход/Мощность", typeof(double));

            int addOn = g.ColumnCount;

            DataGrid grid_tune = DTVPluginCombineSheets.combineTuple(env,
                //ui.GetEnviroment().GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()),
                c,
                tune[0], DTVPluginCombineSheets.gSkipList);
            DataGrid grid_curr = DTVPluginCombineSheets.combineTuple(env,
                //ui.GetEnviroment().GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()),
                c,
                curr[0], DTVPluginCombineSheets.gSkipList);

            DataGrid[] grids = { grid_curr, grid_tune };
            IMultiDataTuple[] tuples = { curr, tune };
            int rows = addOn;
            for (int i = 0; i < grids.Length; i++)
            {
                for (int k = 1; k < grids[i].ColumnCount; k++, rows++)
                {
                    DataGrid.Column cl = grids[i].Columns[k];
                    g.Columns.Add(new DataGrid.Column(
                        String.Format("{0}: {1}", cl.Header, tuples[i].GetTimeDate()), cl.ColumnType, cl.Converter));
                }
            }




            DataCartogram cPower = (DataCartogram)curr[0]["power"];
            DataCartogram tPower = (DataCartogram)tune[0]["power"];

            DataCartogram cEnerg = (DataCartogram)curr[0]["energovir"];
            DataCartogram tEnerg = (DataCartogram)tune[0]["energovir"];

            DataCartogram cFlow = (DataCartogram)curr[0]["flow"];
            DataCartogram tFlow = (DataCartogram)tune[0]["flow"];

            int items = ei.Length;

            for (int u = 0; u < items; u++)
            {
                int k = addOn;
                DataGrid.DataRow r = new DataGrid.DataRow(rows);
                r[0] = ei[u].crd;
                r[1] = 100 * ei[u].err;
                Coords cCrd = ei[u].crd;

                r[2] = 100.0 * (cPower[cCrd, 0] - tPower[cCrd, 0]) / cPower[cCrd, 0];
                r[3] = 100.0 * (cFlow[cCrd, 0] - tFlow[cCrd, 0]) / cFlow[cCrd, 0];
                r[4] = 100.0 * (cEnerg[cCrd, 0] - tEnerg[cCrd, 0]) / cEnerg[cCrd, 0];

                int j = c[cCrd];

                if (j >= 0)
                {
                    for (int i = 0; i < grids.Length; i++)
                    {
                        DataGrid gd = grids[i];
                        for (int t = 1; t < gd.ColumnCount; t++)
                            r[k++] = gd.Rows[j][t];
                    }
                }
                g.Rows.Add(r);


            }
            return g;
        }

        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IMultiDataProvider prov = ui.GetMultiDataProvider();

            IMultiDataTuple curr;
            IMultiDataTuple tune;

            RDTVPluginMakeVostReport.findCurrTune(prov, out curr, out tune);

            if (curr == null)
                return;

            ui.SetDataGrid(makeReport(ui.GetEnviroment(), tune, curr));

        }
    }


    public class RDTVPluginFindMaxErrorsZrk : ActionDataTupleVisualizerUI
    {
        public RDTVPluginFindMaxErrorsZrk()
        {
            _name = "FindMaxErrorsZrk";
            _humaneName = "Отчет при изм. ЗРК";
            _descr = "Отчет по рассогласованиям при изменении ЗРК";
            _action = Actions.ToolBarButton;

            //_image = Resource1.ImageErrInfo;

            _handler = new EventHandler(onClick);
        }

        static public DataGrid makeReport(IEnviroment env, IMultiDataTuple tune, IMultiDataTuple curr)
        {
            CoordsConverter c = ((IDataCartogram)curr[0]["flow_az1_cart"]).AllCoords;
            if (tune[0].GetParamSafe("zrk") == null ||
                curr[0].GetParamSafe("zrk") == null)
                return new DataGrid();

            Coords[] crd = RDTVPluginMakeVostReport.getDiff(c, (DataCartogram)tune[0]["zrk"], (DataCartogram)curr[0]["zrk"], 0);

            if (crd.Length == 0)
                return new DataGrid();

            CoordsConverter cnv = new CoordsConverter(new TupleMetaData(), CoordsConverter.SpecialFlag.Named, crd);

            return RDTVPluginFindMaxErrors.makeReport(env, tune, curr, cnv);
        }

        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IMultiDataProvider prov = ui.GetMultiDataProvider();

            IMultiDataTuple curr;
            IMultiDataTuple tune;

            RDTVPluginMakeVostReport.findCurrTune(prov, out curr, out tune);

            if (curr == null)
                return;

            ui.SetDataGrid(makeReport(ui.GetEnviroment(), tune, curr));

        }

    }



    public class RDTVPluginFindBadCells : ActionDataTupleVisualizerUI
    {
        public RDTVPluginFindBadCells()
        {
            _name = "FindBadCells";
            _humaneName = "Невосстановленные";
            _descr = "Отчет по по каналам в которых не удалось восстановить";
            _action = Actions.ToolBarButton;

            //_image = Resource1.ImageErrInfo;

            _handler = new EventHandler(onClick);
        }

        static public DataGrid makeReport(IEnviroment env, IMultiDataTuple tune, IMultiDataTuple curr)
        {
            CoordsConverter c = ((IDataCartogram)curr[0]["flow_az1_cart"]).AllCoords;
            Coords[] crd = RDTVPluginMakeVostReport.getBad(c, (DataCartogram)curr[0]["flow"], (DataCartogram)curr[0]["flow_az1_cart"], 0);

            if (crd.Length == 0)
                return new DataGrid();

            CoordsConverter cnv = new CoordsConverter(new TupleMetaData(), CoordsConverter.SpecialFlag.Named, crd);

            return RDTVPluginFindMaxErrors.makeReport(env, tune, curr, cnv);
        }

        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IMultiDataProvider prov = ui.GetMultiDataProvider();

            IMultiDataTuple curr;
            IMultiDataTuple tune;

            RDTVPluginMakeVostReport.findCurrTune(prov, out curr, out tune);

            if (curr == null)
                return;

            ui.SetDataGrid(makeReport(ui.GetEnviroment(), tune, curr));

        }

    }
}
