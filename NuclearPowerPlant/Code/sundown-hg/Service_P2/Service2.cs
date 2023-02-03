using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using corelib;
using RockMicoPlugin;
using SqliteStorage;
//using SunBrowser;
using Algorithms;
using System.IO;
using Microsoft.Win32;

namespace Service_P2
{
    public partial class Service_P2 : ServiceBase
    {
        Thread RThread;
        
        public void AddLog(string log)
        {
            try
            {
                if (!EventLog.SourceExists("Service_P2"))
                {
                    EventLog.CreateEventSource("Service_P2", "Service_P2");
                }
                eventLog1.Source = "Service_P2";
                eventLog1.WriteEntry(log);
            }
            catch { }
        }

        public Service_P2()
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

        public class RecoveryStartClass
        {
            public RecoveryStartClass(string name)
            {
            }
            public void run()
            {
                RecoveryPlugin RP = new RecoveryPlugin();
                RP.WorkService();
            }
        }

        protected override void OnStart(string[] args)
        {
            Directory.SetCurrentDirectory("C:\\sundown-hg\\Service_P2\\bin\\Debug");
            //формирование потока
            RecoveryStartClass rst = new RecoveryStartClass("Поток #1");
            RThread = new Thread(new ThreadStart(rst.run));
            RThread.Start();
            AddLog("Servise_P2 started");     
        }

        protected override void OnStop()
        {
            RThread.Abort();
        }
    }
}
