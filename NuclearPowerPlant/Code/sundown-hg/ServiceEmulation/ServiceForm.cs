using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using corelib;
//using RockMicoPlugin;
using System.Threading;
//using SqliteStorage;
using SunBrowser;
using System.IO;


namespace ServiceEmulation
{
    /*
#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;

    using DataCartogramNativeDouble = DataCartogramNative<double>;
#endif
    */
    public partial class ServiceForm : Form
    {
        Thread RockThread;
        Thread TupThread;
        public DataParamTable config;
        public IDataResource azotDb;
        public TupEnv env;

        


        public ServiceForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Directory.Exists("C:\\tmp\\skala_copy") == true)
                Directory.Delete("C:\\tmp\\skala_copy", true);
            
            File.Delete("SqliteTestOPE.db3");
            config = DataParamTable.LoadFromXML("config.xml");
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
                MessageBox.Show("Проверьте целостность конфигурационного файла");
                return;
            }


            //string file_info = File.ReadAllText("dates.txt");
            string[] file_lines = File.ReadAllLines("dates.txt");
            string[] coords = new string[file_lines.Length];
            DateTime[] begin_dates = new DateTime[file_lines.Length];
            DateTime[] end_dates = new DateTime[file_lines.Length];
            ServiceForm.CoordsGridView.Rows.Add(file_lines.Length-1);
            for (int i = 0; i < file_lines.Length; i++)
            {
                string[] line_parts = file_lines[i].Split(';');
                coords[i] = line_parts[0];
                begin_dates[i] = Convert.ToDateTime(line_parts[1]);
                end_dates[i] = Convert.ToDateTime(line_parts[2]);
                ServiceForm.CoordsGridView["CoordColumn", i].Value = coords[i];
                ServiceForm.CoordsGridView["Date1Column", i].Value = begin_dates[i];
                ServiceForm.CoordsGridView["Date2Column", i].Value = end_dates[i];
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
                MessageBox.Show("Робота Скалы завершена");
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
                MessageBox.Show("Работа с азотом завершена");
            }
        }

        
      

        private void button2_Click(object sender, EventArgs e)
        {
            RockThread.Abort();
            TupThread.Abort();
            MessageBox.Show("Processes stopped");
        }

        private void ServiceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        public class TupEnv : BasicEnv
        {
            public IDataResource dataResource;
            //public IDataResource pg_dataResource;
            public IDataResource pvk_tup;
            public TupEnv(DataParamTable par)
                :
                base(par, (string)par["componentPath"], (string)par["componentPath"])
            {
               // dataResource = CreateData("SunEnv_DataStorageProvider");
                dataResource = CreateData("SunEnv_PgDataStorageProvider");
            }
        }

    }
}
