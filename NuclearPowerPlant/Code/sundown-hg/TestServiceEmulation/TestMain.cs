using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using corelib;
//using RockMicoPlugin;
using SunBrowser;
using System.Threading;
using System.IO;

namespace TestServiceEmulation
{
    public class RockDataPlugin
    {
        public IDataResource skala;
        //переменная для проверки занесения повторных данных
        IMultiDataTuple pre_skala_tuple;
        IMultiDataTuple skala_tuple;

        DateTime dt1 = new DateTime();
        DateTime dt2 = new DateTime();
        TimeSpan ts1 = new TimeSpan();
        TimeSpan ts2 = new TimeSpan(0, 0, 10);

        public RockDataPlugin(StartClass.TupEnv env)
        {
            //DataParamTable config = DataParamTable.LoadFromXML("config.xml");
            //Mutex mut = new Mutex(false, "Global\\MyMutex");

            while (true)
            {

                //сохранение данных скалы во временной папке
                if (Directory.Exists("C:\\tmp\\skala_copy") == false)
                    Directory.CreateDirectory("C:\\tmp\\skala_copy");
                //mut.WaitOne();
                try
                {
                    foreach (string s1 in Directory.GetFiles("C:\\tmp\\skala"))
                    {
                        string s2 = "C:\\tmp\\skala_copy" + "\\" + Path.GetFileName(s1);

                        File.Copy(s1, s2, true);

                    }
                }
                catch
                {
                    Thread.Sleep(2000);
                    foreach (string s1 in Directory.GetFiles("C:\\tmp\\skala"))
                    {
                        string s2 = "C:\\tmp\\skala_copy" + "\\" + Path.GetFileName(s1);

                        File.Copy(s1, s2, true);

                    }
                }
                //mut.ReleaseMutex();

                //Thread.Sleep(500);
                //Form1.TupEnv env = new Form1.TupEnv(config);
                //mut.WaitOne();

                dt1 = DateTime.Now;
                skala = env.CreateData("SunEnv_ImportSkalaProvider");


                try
                {
                    skala_tuple = skala.GetMultiProvider().GetData(
                         skala.GetMultiProvider().GetDates()[0],
                         env.GetGlobalParam("SunEnv_ImportSkalaStream"));
                }
                catch
                {
                    env.CloseData("SunEnv_ImportSkalaProvider");
                    continue;
                }
                //проверка на повторной занесение
                if (pre_skala_tuple != null)
                {
                    if (pre_skala_tuple.GetTimeDate() != skala_tuple.GetTimeDate())
                    {
                        env.dataResource.GetMultiProvider().PushData(skala_tuple);
                    }
                    else
                    {
                        env.CloseData("SunEnv_ImportSkalaProvider");
                        continue;
                        //MessageBox.Show("The same skala");
                    }
                }
                else
                {
                    env.dataResource.GetMultiProvider().PushData(skala_tuple);
                }
                env.CloseData("SunEnv_ImportSkalaProvider");

                //mut.ReleaseMutex();
                ///ServiceForm.TB_skala.Text = skala_tuple.GetTimeDate().ToString();
                pre_skala_tuple = skala_tuple;
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
            //DataParamTable config = DataParamTable.LoadFromXML("config.xml");
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
                    continue;
                }
                for (int i = 1; i < azotDb.GetMultiProvider().GetDates().Length; i++)
                {
                    if (azotDb.GetMultiProvider().GetDates()[i] - azotDb.GetMultiProvider().GetDates()[i - 1] > ts)
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
                        //MessageBox.Show("The same azot");
                    }
                }
                else
                {
                    env.dataResource.GetMultiProvider().PushData(azot_tuple);
                }

                ///ServiceForm.TB_azot.Text = azot_tuple.GetTimeDate().ToString();

                pre_azot_tuple = azot_tuple;
                dt2 = DateTime.Now;
                ts1 = dt2 - dt1;
                Thread.Sleep(ts2 - ts1);
                env.CloseData("kgoExporterUseProvider");
            }
        }
    }

    static class TestMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
        }
    }
}
