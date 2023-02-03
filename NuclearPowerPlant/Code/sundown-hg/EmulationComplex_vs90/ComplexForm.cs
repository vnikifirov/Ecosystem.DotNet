using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using corelib;
using RockMicoPlugin;
using SunBrowser;

namespace EmulationComplex_vs90
{
    public partial class ComplexForm : Form
    {
        Thread RockThread;
        Thread TupThread;

        public string[] paths;
        public string[] tup_paths;

        SunBrowserEnviroment env;
        IDataComponent component;

        public ComplexForm()
        {
            InitializeComponent();
        }

        private void OutFolderButton_Click(object sender, EventArgs e)
        {
            
            FolderBrowserDialog brd = new FolderBrowserDialog();
            brd.Description = "Выберете папку для извлечения данных ";
            brd.ShowNewFolderButton = false;
            
            DialogResult dr = brd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                OutFolder.Text = brd.SelectedPath;
            }
            if (dr == DialogResult.Cancel)
            {
                //источник по умолчанию
                OutFolder.Text = "C:\\tmp\\Тест";
            }
            //Сортировка по данных скалы по дате
            env = new SunBrowserEnviroment(new CartogramPresentationConfig(5), new CartogramPresentationConfig(5));
            component = new DataComponents.DataComponent(typeof(RockMicroSingleProvider));

            SingleSearchToMultiProvider p = (SingleSearchToMultiProvider)env.CreateProvider(component, OutFolder.Text);
            paths = p.Names;
            for (int i = 0; i < paths.Count(); i++)
            {
                int idx = paths[i].IndexOf("\\KATKR.INI");
                
                paths[i] = paths[i].Substring(0, idx);
            }
            //файлы .tup
            tup_paths = System.IO.Directory.GetFiles(OutFolder.Text, "*.tup");
        }

        private void CleanButton_Click(object sender, EventArgs e)
        {
            DB db = new DB();
            db.DeleteDB();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Current_skala.Clear();
            Current_azot.Clear();
            DB db = new DB();
            db.CreateDB();

            foreach (string dir in Directory.GetDirectories(InFolder.Text))
                Directory.Delete(dir, true);
            foreach (string dir in Directory.GetFiles(InFolder.Text))
                File.Delete(dir);


            SkalaThread mt = new SkalaThread("Поток #1", OutFolder.Text, InFolder.Text, Time.Text, paths);
            RockThread = new Thread(new ThreadStart(mt.run));
            AzotThread at = new AzotThread("Поток #2", Time.Text, tup_paths);
            TupThread = new Thread(new ThreadStart(at.run));
            RockThread.Start();
            TupThread.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            RockThread.Abort();
            TupThread.Abort();
            MessageBox.Show("Работа комплекса завершена принудительно");
        }


        public class SkalaThread
        {
            string outpath;
            string inpath;
            Int32 period;
            string[] sk_paths;
            string thrdName;
            public SkalaThread(string name, string path1, string path2, string time, string[] paths)
            {
                thrdName = name;
                outpath = path1;
                inpath = path2;
                period = (Convert.ToInt32(time)*60*1000/paths.Length);//:200 означает период в одну секунду
                sk_paths = paths;
            }

            public void run()
            {
                SkalaGen SG = new SkalaGen(outpath);
                SG.CopySkala(period, sk_paths,inpath);
                MessageBox.Show("Работа Скалы завершена");
            }
        }


        public class AzotThread
        {
           
            string[] azot_paths;
            string thrdName;
            public AzotThread(string name, string time, string[] tup_paths)
            {
                thrdName = name;
                
                azot_paths = tup_paths;
            }

            public void run()
            {
                DB db = new DB();
                DateTime dt1 = new DateTime();
                DateTime dt2 = new DateTime();
                TimeSpan ts0 = new TimeSpan(0, 0, 0);
                TimeSpan ts1 = new TimeSpan();
                TimeSpan ts2 = new TimeSpan(0,5,0);
                TimeSpan ts3 = new TimeSpan();
                //DateTime dt_azot = new DateTime();
                for (int i = 0; i < azot_paths.Length; i++)
                {
                    dt1 = DateTime.Now;
                    db.InsertToDB(azot_paths[i]);
                    /*dt2 = DateTime.Now;
                    ts1 = dt2 - dt1;
                    ts3 = ts2-ts1;
                    if (ts3 > ts0)
                    {
                        Thread.Sleep(ts3);
                    }*/
                }
                MessageBox.Show("Работа с азотом завершена");

            }
        }

        private void InFolderButton_Click(object sender, EventArgs e)
        {
            
            FolderBrowserDialog brd = new FolderBrowserDialog();
            brd.Description = "Выберете папку для хранения данных ";
            brd.ShowNewFolderButton = false;

            DialogResult dr = brd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                InFolder.Text = brd.SelectedPath;
            }
            if (dr == DialogResult.Cancel)
            {
                //хранилище по умолчанию
                InFolder.Text = "C:\\tmp\\skala";
            }

        }

        
        

        
    }
}
