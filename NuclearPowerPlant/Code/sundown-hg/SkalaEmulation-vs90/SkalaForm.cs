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

namespace SkalaEmulation_vs90
{
    public partial class SkalaForm : Form
    {
        Thread skalaThread;

        public string[] paths;

        SunBrowserEnviroment env;
        IDataComponent component;

        public SkalaForm()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog brd = new FolderBrowserDialog();
            brd.Description = "Выберете папку для извлечения данных ";
            brd.ShowNewFolderButton = false;

            DialogResult dr = brd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                OutPath.Text = brd.SelectedPath;
            }

            //Сортировка по данных скалы по дате
            env = new SunBrowserEnviroment(new CartogramPresentationConfig(5), new CartogramPresentationConfig(5));
            component = new DataComponents.DataComponent(typeof(RockMicroSingleProvider));

            SingleSearchToMultiProvider p = (SingleSearchToMultiProvider)env.CreateProvider(component, OutPath.Text);
            paths = p.Names;
            for (int i = 0; i < paths.Count(); i++)
            {
                int idx = paths[i].IndexOf("\\KATKR.INI");
                paths[i] = paths[i].Substring(0, idx);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog brd = new FolderBrowserDialog();
            brd.Description = "Выберете место для хранения данных ";
            brd.ShowNewFolderButton = true;
     
            DialogResult dr = brd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                InPath.Text = brd.SelectedPath;
            }
            SkPeriod.Text = "1000";
            BeginBox.Text = "0";
            EndBox.Text = paths.Count().ToString();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (string dir in Directory.GetDirectories(InPath.Text))
                Directory.Delete(dir,true);
            foreach (string dir in Directory.GetFiles(InPath.Text))
                File.Delete(dir);

            MyThread mt = new MyThread("Поток #1", OutPath.Text, InPath.Text,SkPeriod.Text,BeginBox.Text,EndBox.Text,paths);
            skalaThread = new Thread(new ThreadStart(mt.run));
            skalaThread.Start();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            skalaThread.Abort();
            MessageBox.Show("Завершено принудительно");
        }
      
    }


   
    public class MyThread
    {
        string outpath;
        string inpath;
        string period;
        string[] sk_paths;
        int begin_id;
        int end_id;
        string thrdName;
        public MyThread(string name,string path1,string path2,string p,string text1,string text2,string[] paths)
        {
            thrdName = name;
            outpath = path1;
            inpath = path2;
            period = p;
            begin_id = Convert.ToInt32(text1);
            end_id = Convert.ToInt32(text2);
            sk_paths = paths;
        }

        public void run()
        {
            SkalaGen SG = new SkalaGen(outpath, inpath);
            SG.CopySkala(Convert.ToInt32(period), begin_id, end_id,sk_paths);
            MessageBox.Show("Робота комплекса завершена");

        }
    }



}
