using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using corelib;
//using RockMicoPlugin;
using System.Threading;
//using SqliteStorage;
using SunBrowser;
using System.IO;

namespace TestServiceEmulation
{
    public partial class TestForm : Form
    {
        Thread StartThread;
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartClassThread sct = new StartClassThread("Главный поток");
            StartThread = new Thread(new ThreadStart(sct.run));
            StartThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
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

    public class StartClass
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
        {
            StreamWriter sw = new StreamWriter("C:\\tmp\\Работа службы П1");
            if (Directory.Exists("C:\\tmp\\skala_copy") == true)
                Directory.Delete("C:\\tmp\\skala_copy", true);

            File.Delete("C:\\sundown-hg\\TestServiceEmulation\\bin\\Debug\\SqliteTestOPE.db3");
            config = DataParamTable.LoadFromXML("C:\\sundown-hg\\TestServiceEmulation\\bin\\Debug\\config.xml");
            //проверка целостности "config.xml"//
            try
            {
                config["kgoExporterUseProvider"].Value.ToString();
                config["SunEnv_DataStorageProvider"].Value.ToString();
                config["SunEnv_ImportSkalaProvider"].Value.ToString();
                config["SunEnv_PvkSchemeProvider"].Value.ToString();
            }
            catch
            {
                sw.WriteLine("Проверьте целостность конфигурационного файла");
                sw.Close();
                //MessageBox.Show("Проверьте целостность конфигурационного файла");
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
                //MessageBox.Show("Робота Скалы завершена");
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
                //MessageBox.Show("Работа с азотом завершена");
            }
        }

        public class TupEnv : BasicEnv
        {
            public IDataResource dataResource;
            public TupEnv(DataParamTable par)
                :
                base(par, (string)par["componentPath"], (string)par["componentPath"])
            {
                //throw new Exception("запоролся!!!");
                dataResource = CreateData("SunEnv_DataStorageProvider");
            }
        }
    }
}
