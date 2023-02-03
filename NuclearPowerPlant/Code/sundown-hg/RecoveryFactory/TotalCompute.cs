using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using corelib;

namespace RecoveryFactory
{


    public class DTVPluginFixThis : ActionDataTupleVisualizerUI
    {
        public DTVPluginFixThis()
        {
            _name = "FixThisCompute";
            _humaneName = "Фиксированный расчет";
            _descr = "Расчет при фиксированной дате адаптации";
            _action = Actions.DataTupleContextMenu;

            //_image = Resource1.ImageFullReport;
            _handler = new EventHandler(onClick);
        }

        static public void onClick(object sender, EventArgs e)
        {
            ADTVEventTupleArgs args = (ADTVEventTupleArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            IDataTuple t = args._tuple;

            IMultiDataProvider prov = ui.GetMultiDataProvider();


            SunEnv env = (SunEnv)ui.GetEnviroment();

            string[] names = prov.GetStreamNames();
            if (names.Length != 1)
                return;

            DateTime[] dates = prov.GetDates(names[0]);
            if (dates.Length < 2)
                return;

            int j = -1;

            for (int i = 0; i< dates.Length; i++)
            {
                if (t.GetTimeDate() == dates[i])
                {
                    j = i; break;
                }
            }

            if (j<0)
                return;

            DataGrid[] grids2;
            IMultiDataTuple[][] tuplesR;
            DataGrid combinedZrk;
            DataGrid combinedBad;

            DateTime[] from = new DateTime[dates.Length ];
            DateTime[] to = new DateTime[dates.Length ];

            for (int i = 0; i < dates.Length; i++)
            {
                from[i] = dates[j];
                to[i] = dates[i];
            }

            DataGrid d = DTVPluginDTVSunEnvTotalCompute.compute(ui, from, to, false, out grids2, out tuplesR, out combinedZrk, out combinedBad);
            ui.SetDataGrid(d);


            IMultiDataTuple[] tuplesFull = new IMultiDataTuple[to.Length];
            for (int i = 0; i < to.Length; i++)
                tuplesFull[i] = tuplesR[i][0];

            // Full view
            env.ViewMultiTuple(tuplesFull);
        }
            

    }

    public class DTVPluginDTVSunEnvTotalCompute : ActionDataTupleVisualizerUI
    {
        public DTVPluginDTVSunEnvTotalCompute()
        {
            _name = "TotalCompute";
            _humaneName = "Полный рассчет";
            _descr = "Полный рассчет по всем срезам";
            _action = Actions.ToolBarButton;

            _image = DefImages.ImageRecoveryAll;
            _handler = new EventHandler(onClick);
        }

        static public DataGrid compute(IDataTupleVisualizerUI ui, DateTime[] from, DateTime[] to, bool doGrids2,
            out DataGrid[] grids2, out IMultiDataTuple[][] tuplesR, out DataGrid combinedZrk, out DataGrid combinedBad)
        {
            grids2 = null;
            tuplesR = null;
            combinedZrk = null;
            combinedBad = null;

            IMultiDataProvider prov = ui.GetMultiDataProvider();
            SunEnv env = (SunEnv)ui.GetEnviroment();

            string[] names = prov.GetStreamNames();
            if (names.Length != 1)
                return null;
            if ((from.Length < 1) || (from.Length != to.Length))
                return null;

            DataGrid d;

            DataGrid[] grids = new DataGrid[from.Length];
            tuplesR = new IMultiDataTuple[from.Length][];
            if (doGrids2)
                grids2 = new DataGrid[from.Length];

            DataGrid[] gridsZrk = new DataGrid[from.Length];
            DataGrid[] gridsBad = new DataGrid[from.Length];

            DataGrid.Column[] colCombine = {
                   new DataGrid.Column("Настройка", typeof(DateTime)),
                   new DataGrid.Column("Восстановление", typeof(DateTime))};
            DataGrid.DataRow[] rowCombine = new DataGrid.DataRow[from.Length];

            for (int i = 0; i < from.Length; i++)
            {
                tuplesR[i] = env.SimpleRecoveryWithCoeffRecoveryAndNewGamma(prov, to[i], from[i]);
                grids[i] = RDTVPluginMakeVostReport.makeReport(ui.GetEnviroment(), tuplesR[i][1], tuplesR[i][0]);

                if (doGrids2)
                    grids2[i] = RDTVPluginFindMaxErrors.makeReport(ui.GetEnviroment(), tuplesR[i][1], tuplesR[i][0]);

                gridsZrk[i] = RDTVPluginFindMaxErrorsZrk.makeReport(ui.GetEnviroment(), tuplesR[i][1], tuplesR[i][0]);
                rowCombine[i] = new DataGrid.DataRow(2);
                rowCombine[i][0] = from[i];
                rowCombine[i][1] = to[i];


                gridsBad[i] = RDTVPluginFindBadCells.makeReport(ui.GetEnviroment(), tuplesR[i][1], tuplesR[i][0]);


                ui.SetStatusString(String.Format("Выполение... {0} %", 100 * i / (from.Length)));
            }

            d = grids[0];

            for (int i = 1; i < from.Length; i++)
            {
                d.Rows.Add(grids[i].Rows[0]);
            }

            combinedZrk = DataGrid.combineHorizon(colCombine, rowCombine,
                gridsZrk);
            combinedBad = DataGrid.combineHorizon(colCombine, rowCombine,
                gridsBad);

            return d;
        }

        static public void onClick(object sender, EventArgs e)
        {
            bool doGrids2 = false;
            //bool doGrids2 = true;

            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            IMultiDataProvider prov = ui.GetMultiDataProvider();
            SunEnv env = (SunEnv)ui.GetEnviroment();

            string[] names = prov.GetStreamNames();
            if (names.Length != 1)
                return;

            DataGrid d;

            DateTime[] dates = prov.GetDates(names[0]);
            if (dates.Length < 2)
                return;

            DataGrid[] grids2;
            IMultiDataTuple[][] tuplesR;
            DataGrid combinedZrk;
            DataGrid combinedBad;

            DateTime[] from = new DateTime[dates.Length - 1];
            DateTime[] to = new DateTime[dates.Length - 1];

            for (int i = 1; i < dates.Length; i++)
            {
                from[i - 1] = dates[i - 1];
                to[i - 1] = dates[i];
            }

            
            d = compute(ui, from, to, doGrids2, out grids2, out tuplesR, out combinedZrk, out combinedBad);

            //combinedZrk.ExportExcel();
            //combinedBad.ExportExcel();

            ui.SetDataGrid(d);

            IMultiDataTuple[] tuplesFull = new IMultiDataTuple[to.Length + 1];
            tuplesFull[0] = tuplesR[0][1];
            for (int i = 0; i < to.Length; i++)
                tuplesFull[i + 1] = tuplesR[i][0];

            // Full view
            env.ViewMultiTuple(tuplesFull);

            if (doGrids2)
            {
                OleObject excel = new OleObject("Excel.Application");
                excel.GetProperty("Workbooks").Invoke("Add");
                excel.GetProperty("Worksheets", 1).Invoke("Delete");
                excel.GetProperty("Worksheets", 1).Invoke("Delete");
                //excel.GetProperty("Worksheets", 1).Invoke("Delete");

                for (int i = 1; i < dates.Length; i++)
                {
                    if (i > 1)
                        excel.GetProperty("Worksheets").Invoke("Add");

                    string str;
                    excel.GetProperty("Worksheets", 1).SetProperty("Cells", 1, 1,
                        str = String.Format("С {0:ddMMyy hh_mm} на {1:ddMMyy hh_mm}", dates[i - 1], dates[i]));
                    excel.GetProperty("Sheets", 1).SetProperty("Name", str);

                    grids2[i - 1].ExportExcelToWorkSheet(excel.GetProperty("WorkSheets", 1), 2, 1, true);


                    ui.SetStatusString(String.Format("Экспорт в Excel... {0} %", 100 * i / (dates.Length - 1)));
                }


                excel.GetProperty("Worksheets").Invoke("Add");
                excel.GetProperty("Worksheets", 1).SetProperty("Cells", 1, 1, "Не удалось восстановить");
                excel.GetProperty("Sheets", 1).SetProperty("Name", "Не удалось восстановить");
                combinedBad.ExportExcelToWorkSheet(excel.GetProperty("WorkSheets", 1), 2, 1, true);

                excel.GetProperty("Worksheets").Invoke("Add");
                excel.GetProperty("Worksheets", 1).SetProperty("Cells", 1, 1, "Ячейки с изменением ЗРК");
                excel.GetProperty("Sheets", 1).SetProperty("Name", "Ячейки с изменением ЗРК");
                combinedZrk.ExportExcelToWorkSheet(excel.GetProperty("WorkSheets", 1), 2, 1, true);


                excel.GetProperty("Worksheets").Invoke("Add");
                excel.GetProperty("Worksheets", 1).SetProperty("Cells", 1, 1, "Сводная информация");
                excel.GetProperty("Sheets", 1).SetProperty("Name", "Сводная информация");
                d.ExportExcelToWorkSheet(excel.GetProperty("WorkSheets", 1), 2, 1, true);


                excel.SetProperty("Visible", true);
            }

            ui.SetStatusString("Готово");

        }
    }
}
