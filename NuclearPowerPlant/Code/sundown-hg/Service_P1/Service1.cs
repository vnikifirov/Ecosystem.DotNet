using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using corelib;
using System.Threading;
using SunBrowser;
using System.IO;
using Microsoft.Win32;

namespace Service_P1
{
    public partial class Service_P1 : ServiceBase
    {
        /**/
        Thread StartThread;
        /**/

        public void AddLog(string log)
        {
            try
            {
                if (!EventLog.SourceExists("Service_P1"))
                {
                    EventLog.CreateEventSource("Service_P1", "Service_P1");
                }
                eventLog1.Source = "Service_P1";
                eventLog1.WriteEntry(log);
            }
            catch { }
        }

        public Service_P1()
        {
            InitializeComponent();
        }

        public static object GetValuee(string key) 
        {
            object val = null;
            RegistryKey currRKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\sundown");
            if (currRKey != null)
            {
                val = currRKey.GetValue(key);
                
            }
            else val = "def";

            currRKey.Close();
            return val;
        }

        protected override void OnStart(string[] args)
        {
            Directory.SetCurrentDirectory("C:\\sundown-hg\\Service_P1\\bin\\Debug");

            StartClassThread sct = new StartClassThread("Главный поток");
            StartThread = new Thread(new ThreadStart(sct.run));
            StartThread.Start();
            AddLog("Servise_P1 started");               
        }

        protected override void OnStop()
        {
            StartThread.Abort();
        }
        public class StartClassThread
        {
            public StartClassThread(string name)
            {
            }
            public void run()
            {
                StartClass SC = new StartClass();
                SC.StartFunction();
            }
        }
    }


    public class StartClass : Service_P1
    {
        Thread RockThread;
        Thread TupThread;
        public DataParamTable config;
        public IDataResource azotDb;
        public TupEnv env;

        public StartClass()
        {
        }
        public void StartFunction()
        {/*
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Service_P1", true);
            string keyValueInt = "272";
            try
            {
                regkey.SetValue("Type", keyValueInt, Microsoft.Win32.RegistryValueKind.DWord);
          
                regkey.Close();
            }
            catch { }*/

            if (Directory.Exists("C:\\tmp\\skala_copy") == true)
                Directory.Delete("C:\\tmp\\skala_copy", true);

            //File.Delete("C:\\sundown-hg\\Service_P1\\bin\\Debug\\SqliteTestOPE.db3");
          
            string se = GetValuee("P1conf").ToString();
            AddLog("path config read from registry: " + se);
            string P1conf = se.Replace("\\", "\\\\");
            AddLog("path config correct: " + P1conf);

            config = DataParamTable.LoadFromXML(P1conf);
           
            //проверка целостности "config.xml"//
            try
            {
                config["kgoExporterUseProvider"].Value.ToString();
                config["SunEnv_DataStorageProvider"].Value.ToString();
                config["SunEnv_PgDataStorageProvider"].Value.ToString();
                config["SunEnv_ImportSkalaProvider"].Value.ToString();
                config["SunEnv_PvkSchemeProvider"].Value.ToString();
            }
            catch
            {
                return;
            }

            env = new TupEnv(config);

            //занесение в базу "pvkTuple.tup"
            IDataResource pvk_tup = env.CreateData("SunEnv_PvkSchemeProvider");
            IMultiDataTuple pvk = pvk_tup.GetMultiProvider().GetData(
                pvk_tup.GetMultiProvider().GetDates()[0],
                env.GetGlobalParam("SunEnv_PvkSchemeProvider"));
            env.dataResource.GetMultiProvider().PushData(pvk);

            RockMicroThread rmt = new RockMicroThread("Поток #1", config, env);
            RockThread = new Thread(new ThreadStart(rmt.run));
            AzotTupleThread att = new AzotTupleThread("Поток #2", config, env);
            TupThread = new Thread(new ThreadStart(att.run));
            RockThread.Start();
            TupThread.Start();
        }

        public class RockMicroThread
        {
            string thrdName;
            DataParamTable config;
            TupEnv RockEnv;
            public RockMicroThread(string name, DataParamTable conf, TupEnv env)
            {
                thrdName = name;
                config = conf;
                RockEnv = env;
            }
            public void run()
            {
                RockDataPlugin RDP = new RockDataPlugin(RockEnv);
            }
        }

        public class AzotTupleThread
        {
            string thrdName;
            DataParamTable config;

            TupEnv AzotEnv;
            public AzotTupleThread(string name, DataParamTable conf, TupEnv env)
            {
                thrdName = name;
                config = conf;
                AzotEnv = env;

            }
            public void run()
            {
                AzotDataPlugin ADP = new AzotDataPlugin(AzotEnv);
            }
        }

        public class TupEnv : BasicEnv
        {
            public IDataResource dataResource;
            public TupEnv(DataParamTable par)
                :
                base(par, (string)par["componentPath"], (string)par["componentPath"])
            {
                //dataResource = CreateData("SunEnv_DataStorageProvider");
                dataResource = CreateData("SunEnv_PgDataStorageProvider");
            }
        }
    }



}

