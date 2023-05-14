using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using corelib;

namespace RecoveryFactory
{
#if !DOTNET_V11
    using DataArrayInt = DataArray<int>;
    using DataArrayDouble = DataArray<double>;
#endif

    class AzotEditorPlugins
    {
        static public void ShowAzotEditor(BasicEnv env)
        {
            DataTupleVisualizer vis = new DataTupleVisualizer(env);

            vis.SetDataProvider(new ListMultiDataProvider());
            vis.Dock = DockStyle.Fill;

            vis.RegisterAction(new DTVAzotPluginLoadTuple());
            vis.RegisterAction(new DTVAzotPluginExportAverage());

            Form form = new Form();
            form.Controls.Add(vis);
            form.Text = "Редактор азотной активности";
            form.ShowDialog(); 
        }
    }


    public class DTVAzotPluginLoadTuple : ActionDataTupleVisualizerUI
    {
        public DTVAzotPluginLoadTuple()
        {
            _name = "AzotPluginLoadTuple";
            _humaneName = "Загрузить";
            _descr = "Загрузить данный из файла";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunEnv env = (SunEnv)ui.GetEnviroment();

            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Tup Files|*.tup";
            fd.Multiselect = true;

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool multifiles = fd.FileNames.Length > 1;
            
                foreach (String fn in fd.FileNames)
                {
                    try
                    {
                        DataTuple tup = DataTuple.LoadFromFile(env, fn);

                        DataParamTable tbl = (DataParamTable)tup["kgopc_info"];
                        DateTime originalDateTime = DateTime.FromOADate((double)tbl["prpDate"]);

                        tup = new DataTuple(tup, tup.GetStreamName(), originalDateTime);
                        
                       /* String format = String.Format(
                            "{1:d}:Н{0}П{2} {3} {4}",
                            (int)(tbl["fiberNum"]) + 1, DateTime.FromOADate((double)tbl["prpDate"]), (int)tbl["pcPvk"],
                            env.ArrayPvkInfo.GetByPvk(new FiberCoords((int)tbl["fiberNum"], ((int)tbl["pcPvk"]) - 1)),
                            tbl["specClock"]);
                        */
                        ui.GetMultiDataProvider().PushData(tup);
                    }
                    catch (ArgumentException)
                    {
                        try
                        {
                            //IDataTuple[] tupels = DataTuple.LoadMultiFromFile(env, fd.FileName);
                            MultiDataTuple tupels = new MultiDataTuple(env, fd.FileName);
                            DataParamTable tbl = (DataParamTable)tupels[0]["kgoprp_info"];
                            DateTime originalDateTime = DateTime.FromOADate((double)tbl["prpDate"]);
                            tupels = tupels.NewDate(originalDateTime);

                            //for(int i = 0; i < tupels.Count;i++)
                            //{
                            //    tupels[i] = new DataTuple(tupels[i], tupels[i].GetStreamName(), originalDateTime);
                            //}

                            String format = String.Format(
                                "{0}: ({1})",
                                originalDateTime, tupels.Count);

                            IMultiDataTuple sckala = env.GetSkalaData(tupels[0].GetTimeDate(), true);
                            DataTuple nsckala = new DataTuple(sckala[0], sckala.GetStreamName(), originalDateTime);
                            //sckala.SetPvkInfo(env.ArrayPvkInfo);

                            ui.GetMultiDataProvider().PushData(nsckala);
                            ui.GetMultiDataProvider().PushData(tupels);
                        }
                        catch
                        {
                            if (!multifiles)
                                MessageBox.Show(String.Format("Ошибка при открытии прописки {0}", fn));
                        }

                    }
                    catch
                    {
                        if (!multifiles)
                            MessageBox.Show(String.Format("Ошибка при открытии прописки {0}", fn));
                    }
                }
            }
        }
    }


    public class DTVAzotPluginExportAverage : ActionDataTupleVisualizerUI
    {
        public DTVAzotPluginExportAverage()
        {
            _name = "AzotPluginExportAverage";
            _humaneName = "Осреднить";
            _descr = "Осреднить и записать в файл";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            //_image = DefImages.ImageOpen;
        }


        class AvgData
        {
            public double[] signal = new double[115];
            public int count = 0;
            public DateTime dt = new DateTime(0);
        }

        static MultiDataTuple MakeAverageAzot(SunEnv env, IMultiDataProvider datasrc)
        {
            AvgData[] avg = new AvgData[16];
            for (int i = 0; i < 16; i++)
                avg[i] = new AvgData();

            DateTime[] prps = datasrc.GetDates("prp");
            DateTime[] pcs = datasrc.GetDates("pc");

            for (int i = 0; i < prps.Length; i++)
            {
                IMultiDataTuple kgo = datasrc.GetData(prps[i], "prp");
                IMultiDataTuple skala = datasrc.GetData(prps[i], "rock_micro");

                IMultiDataTuple parsed = env.ParseKgo(skala, kgo);

                for (int j = 0; j < parsed.Count; j++)
                {
                    int fiber = (int)((DataParamTable)kgo[j]["kgoprp_info"]).GetParam("fiberNum");
                    DataArray data = (DataArray)parsed[j]["pvk_maxes"];
                    //int[] data = (int[])(DataArray)parsed[j]["pvk_maxes"];
                    DateTime dt = DateTime.FromOADate((double)((DataParamTable)kgo[j]["kgoprp_info"]).GetParam("prpDate"));

                    avg[fiber].count++;
                    if (avg[fiber].dt < dt)
                        avg[fiber].dt = dt;

                    for (int k = 0; k < 115; k++)
                        avg[fiber].signal[k] = data.GetAnyValue(k).ToDouble();

                }
            }

            for (int i = 0; i < ((pcs != null) ? pcs.Length : 0); i++)
            {
                IDataTuple single = (IDataTuple)datasrc.GetData(pcs[i], "pc");

                int pvk = (int)((DataParamTable)single["kgopc_info"]).GetParam("pcPvk") - 1;
                if (pvk < 0)
                    continue;

                int fiber = (int)((DataParamTable)single["kgopc_info"]).GetParam("fiberNum");
                //int[] data = (int[])(DataArray)single["kgopc_azot"];
                DataArray data = (DataArray)single["kgopc_azot"];

                //int average = (int)Math.Round(MathOp.IntMean(data, 0, data.Length));
                double average = data.Mean();

                avg[fiber].signal[pvk] = average * avg[fiber].count;
            }


            ArrayList res = new ArrayList();

            for (int i = 0; i < 16; i++)
            {
                if (avg[i].count > 0)
                {
                    if (avg[i].count > 1)
                    {
                        for (int k = 0; k < 115; k++)
                            avg[i].signal[k] = avg[i].signal[k] / avg[i].count;
                    }

                    DataArrayDouble resulting = new DataArrayDouble(
                        new TupleMetaData("pvk_maxes", "pvk_maxes", avg[i].dt, TupleMetaData.StreamAuto),
                        avg[i].signal);
                    DataParamTable tbl = new DataParamTable(
                        new TupleMetaData("kgoprp_info", "kgoprp_info", avg[i].dt, TupleMetaData.StreamAuto),
                        new DataParamTableItem[] {
                            new DataParamTableItem("fiberNum", i),
                            new DataParamTableItem("prpDate", avg[i].dt.ToOADate())
                            }
                        );

                    IDataTuple tup = new DataTuple("prp", avg[i].dt, resulting, tbl);
                    res.Add(tup);
                }
            }

            return new MultiDataTuple(res);
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunEnv env = (SunEnv)ui.GetEnviroment();


            IMultiDataProvider datasrc = ui.GetMultiDataProvider();
            DateTime[] prps = datasrc.GetDates("prp");
            DateTime[] pcs = datasrc.GetDates("pc");

            if ((prps == null) || (prps.Length < 1) /*|| (_singles.Count < 1)*/)
            {
                MessageBox.Show("Необходимо выбрать хотябы по одной прописке");
                return;
            }

            MultiDataTuple res = MakeAverageAzot(env, datasrc);            

            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Tup Files|*.tup";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                res.SaveToFile(fd.FileName);
            }

            //datasrc.PushData(new MultiDataTuple("average", res.GetTimeDate(),
            //    res["pvk_maxes"],
            //    res["kgoprp_info"]));
            
            IMultiDataTuple nd = MultiDataTuple.Create(
                env,
                new AttributeSingleTupleRules(@"average contains (pvk_maxes2, kgoprp_info2)"),
                new TupleMaps(@"pvk_maxes as pvk_maxes2, kgoprp_info as kgoprp_info2"),
                res);

            datasrc.PushData(nd);
             

        }
    }
}
