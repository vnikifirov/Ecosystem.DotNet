using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.Odbc;
using corelib;
using SunBrowser;
using System.Threading;
using System.IO;
using Algorithms;

namespace Service_P1
{
#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
    using DataCartogramNativeDouble = DataCartogramNative<double>;
#endif

    public class RockDataPlugin
    {
        public IDataResource skala;
        //переменная для проверки занесения повторных данных
        IMultiDataTuple pre_skala_tuple;
        IMultiDataTuple skala_tuple;
        DataTuple dataSkala_new;
        DataTuple pre_dataSkala_new;
        public OvlInfoProvider zrk;

        DateTime dt1 = new DateTime();
        DateTime dt2 = new DateTime();
        TimeSpan ts1 = new TimeSpan();
        TimeSpan ts2 = new TimeSpan(0, 0, 10);

        public RockDataPlugin(StartClass.TupEnv env)
        {
            string[] file_lines = File.ReadAllLines("dates.txt");
            string[] coords = new string[file_lines.Length];
            DateTime[] begin_dates = new DateTime[file_lines.Length];
            DateTime[] end_dates = new DateTime[file_lines.Length];
            for (int i = 0; i < file_lines.Length; i++)
            {
                string[] line_parts = file_lines[i].Split(';');
                coords[i] = line_parts[0];
                begin_dates[i] = Convert.ToDateTime(line_parts[1]);
                end_dates[i] = Convert.ToDateTime(line_parts[2]);
            }

            while (true)
            {
                //сохранение данных скалы во временной папке
                if (Directory.Exists("C:\\tmp\\skala_copy") == false)
                    Directory.CreateDirectory("C:\\tmp\\skala_copy");
                while (true)
                {
                    try
                    {
                        foreach (string s1 in Directory.GetFiles("C:\\tmp\\skala"))
                        {
                            string s2 = "C:\\tmp\\skala_copy" + "\\" + Path.GetFileName(s1);
                            File.Copy(s1, s2, true);
                            File.Delete(s1);
                        }
                        break;
                    }
                    catch
                    {
                    }
                }

                dt1 = DateTime.Now;
                skala = env.CreateData("SunEnv_ImportSkalaProvider");

                try
                {
                    skala_tuple = skala.GetMultiProvider().GetData(
                         skala.GetMultiProvider().GetDates()[0],
                         env.GetGlobalParam("SunEnv_ImportSkalaStream"));
                    //проверка на повторное занесение
                    if (pre_skala_tuple != null)
                    {
                        if (pre_skala_tuple.GetTimeDate() == skala_tuple.GetTimeDate())
                        {
                            env.CloseData("SunEnv_ImportSkalaProvider");
                            Thread.Sleep(1);
                            continue;
                        }
                    }
                    pre_skala_tuple = skala_tuple;
                    //*******************************//
                    DataTuple dataSkala = new DataTuple((IDataTuple)skala_tuple, skala_tuple.GetStreamName());

                    //создание новой скалы

                    DataCartogram dkr_1 = (DataCartogram)dataSkala["dkr_1"];
                    DataCartogram dkr_2 = (DataCartogram)dataSkala["dkr_2"];
                    DataCartogram dkr_corr_1 = (DataCartogram)dataSkala["dkr_corr_1"];
                    DataCartogram dkr_corr_2 = (DataCartogram)dataSkala["dkr_corr_2"];
                    DataCartogram dkv_1 = (DataCartogram)dataSkala["dkv_1"];
                    DataCartogram dkv_2 = (DataCartogram)dataSkala["dkv_2"];
                    DataCartogram energovir = (DataCartogram)dataSkala["energovir"];
                    DataCartogram fizras = (DataCartogram)dataSkala["fizras"];

                    DataCartogram flow_cart = (DataCartogram)dataSkala["flow"];

                    //формирование картограммы расхода с учетом запрещенных ячеек
                    double[,] flow_matrix = flow_cart.ScaleNativeArray(-1);
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            Coords c = new Coords(i, j);
                            bool idx = false;
                            for (int k = 0; k < coords.Length; k++)
                            {
                                if (coords[k] == c.HumaneLabelYX)
                                {
                                    bool start = begin_dates[k] < dataSkala.GetTimeDate();
                                    bool end = end_dates[k] > dataSkala.GetTimeDate();
                                    if (start && end)
                                    {
                                        flow_matrix[i, j] = 0.0;
                                        idx = true;
                                    }
                                }
                            }
                            if (!idx)
                                flow_matrix[i, j] = flow_cart[c, flow_cart.Layers];
                        }
                    }
                    TupleMetaData flow_info = new TupleMetaData("flow", "Расход", dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    DataCartogramNative<Double> flow =
                       new DataCartogramNative<Double>(flow_info, flow_cart.ScaleIndex, flow_matrix);

                    DataCartogram power = (DataCartogram)dataSkala["power"];
                    DataParamTable rbmk_params = (DataParamTable)dataSkala["rbmk_params"];
                    DataCartogram suz = (DataCartogram)dataSkala["suz"];
                    DataCartogram zagr_rockmicro = (DataCartogram)dataSkala["zagr_rockmicro"];
                    DataCartogram zapk = (DataCartogram)dataSkala["zapk"];

                    DataTuple dkr_1_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), dkr_1);
                    DataTuple dkr_2_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), dkr_2);
                    DataTuple dkr_corr_1_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), dkr_corr_1);
                    DataTuple dkr_corr_2_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), dkr_corr_2);
                    DataTuple dkv_1_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), dkv_1);
                    DataTuple dkv_2_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), dkv_2);
                    DataTuple energovir_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), energovir);
                    DataTuple fizras_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), fizras);
                    DataTuple flow_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), flow);
                    DataTuple power_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), power);
                    DataTuple rbmk_params_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), rbmk_params);
                    DataTuple suz_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), suz);
                    DataTuple zagr_rockmicro_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), zagr_rockmicro);
                    DataTuple zapk_tuple = new DataTuple(dataSkala.StreamName, dataSkala.GetTimeDate(), zapk);

                    DataTuple[] tuples = new DataTuple[14];
                    tuples[0] = dkr_1_tuple;
                    tuples[1] = dkr_2_tuple;
                    tuples[2] = dkr_corr_1_tuple;
                    tuples[3] = dkr_corr_2_tuple;
                    tuples[4] = dkv_1_tuple;
                    tuples[5] = dkv_2_tuple;
                    tuples[6] = energovir_tuple;
                    tuples[7] = fizras_tuple;
                    tuples[8] = flow_tuple;
                    tuples[9] = power_tuple;
                    tuples[10] = rbmk_params_tuple;
                    tuples[11] = suz_tuple;
                    tuples[12] = zagr_rockmicro_tuple;
                    tuples[13] = zapk_tuple;

                    string[] names = new string[14];
                    dkr_1_tuple.CopyNamesTo(names);
                    dkr_2_tuple.CopyNamesTo(names, 1);
                    dkr_corr_1_tuple.CopyNamesTo(names, 2);
                    dkr_corr_2_tuple.CopyNamesTo(names, 3);
                    dkv_1_tuple.CopyNamesTo(names, 4);
                    dkv_2_tuple.CopyNamesTo(names, 5);
                    energovir_tuple.CopyNamesTo(names, 6);
                    fizras_tuple.CopyNamesTo(names, 7);
                    flow_tuple.CopyNamesTo(names, 8);
                    power_tuple.CopyNamesTo(names, 9);
                    rbmk_params_tuple.CopyNamesTo(names, 10);
                    suz_tuple.CopyNamesTo(names, 11);
                    zagr_rockmicro_tuple.CopyNamesTo(names, 12);
                    zapk_tuple.CopyNamesTo(names, 13);
                    //flow_tuple.CopyNamesTo(names, dataSkala.ItemsCount);
                    DataTuple combined = DataTuple.Combine(names, tuples);
                    dataSkala_new = new DataTuple(combined, dataSkala.StreamName, dataSkala.GetTimeDate());

                }
                catch
                {
                    env.CloseData("SunEnv_ImportSkalaProvider");
                    Thread.Sleep(1);
                    continue;
                }
                env.dataResource.GetMultiProvider().PushData(dataSkala_new);
                /*
                //проверка на повторное занесение
                if (pre_dataSkala_new != null)
                {
                    if (pre_dataSkala_new.GetTimeDate() != dataSkala_new.GetTimeDate())
                    {
                        env.dataResource.GetMultiProvider().PushData(dataSkala_new);
                    }
                    else
                    {
                        env.CloseData("SunEnv_ImportSkalaProvider");
                        Thread.Sleep(1);
                        continue;
                        //MessageBox.Show("The same skala");
                    }
                }
                else
                {
                    env.dataResource.GetMultiProvider().PushData(dataSkala_new);

                }
                */
                env.CloseData("SunEnv_ImportSkalaProvider");

                //ServiceForm.TB_skala.Text = dataSkala_new.GetTimeDate().ToString();
                pre_dataSkala_new = dataSkala_new;

                //для тестировния службы время ожидания не задано,
                //т.к. данные скалы поступают с заданным пользователем интервалом
                //для боевого запуска необходимо скоординировать время с интервалами поступления срезов на станции

                /*dt2 = DateTime.Now;
                ts1 = dt2 - dt1;
                Thread.Sleep(ts2 - ts1);
                */
            }
        }
 

    }

    public class AzotDataPlugin
    {
        public IDataResource azotDb;
        //переменная для проверки занесения повторных данных
        IMultiDataTuple pre_azot_tuple;

        DateTime dt = new DateTime();
        DateTime dt1 = new DateTime();
        DateTime dt2 = new DateTime();
        TimeSpan ts = new TimeSpan(0, 0, 0);
        TimeSpan ts1 = new TimeSpan();
        TimeSpan ts2 = new TimeSpan(0, 0, 20);

        public AzotDataPlugin(StartClass.TupEnv env)
        {
            while (true)
            {
                try
                {
                    azotDb = env.CreateData("kgoExporterUseProvider");
                    dt = azotDb.GetMultiProvider().GetDates()[0];
                }
                catch
                {
                    env.CloseData("kgoExporterUseProvider");
                    Thread.Sleep(10);
                    continue;
                }
                for (int i = 1; i < azotDb.GetMultiProvider().GetDates().Length; i++)
                {
                    /*if(azotDb.GetMultiProvider().GetDates()[i] - azotDb.GetMultiProvider().GetDates()[i - 1] > ts)
                    dt = azotDb.GetMultiProvider().GetDates()[i];*/
                    if (azotDb.GetMultiProvider().GetDates()[i] > dt)
                        dt = azotDb.GetMultiProvider().GetDates()[i];
                }
                dt1 = DateTime.Now;

                IMultiDataTuple azot_tuple = azotDb.GetMultiProvider().GetData(
                        dt,
                        env.GetGlobalParam("SunEnv_ImportAzotStream"));

                //проверка на повторной занесение
                if (pre_azot_tuple != null)
                {
                    if (pre_azot_tuple.GetTimeDate() != azot_tuple.GetTimeDate())
                    {
                        env.dataResource.GetMultiProvider().PushData(azot_tuple);
                    }
                    else
                    {
                        continue;
                        //MessageBox.Show("The same azot");
                    }
                }
                else
                {
                    env.dataResource.GetMultiProvider().PushData(azot_tuple);
                }

                //ServiceForm.TB_azot.Text = azot_tuple.GetTimeDate().ToString();

                pre_azot_tuple = azot_tuple;
                dt2 = DateTime.Now;
                ts1 = dt2 - dt1;
                //Thread.Sleep(ts2 - ts1);
                env.CloseData("kgoExporterUseProvider");
                //Thread.Sleep(1000*60);
            }
        }



    }

    static class Service1Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new Service_P1() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }

}