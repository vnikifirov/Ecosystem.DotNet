using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using corelib;
using RockMicoPlugin;
using SunBrowser;


namespace SkalaEmulation_vs90
{
    public class SkalaGen
    {
        
        public string outFolder;
        public string inFolder;

        public SkalaGen(string path1, string path2)
        {
            outFolder = path1;
            inFolder = path2;
        }
        public void CopySkala(int period,int begin_id,int end_id,string[] paths)
        {
            
            if (Directory.Exists(inFolder) == false)
                Directory.CreateDirectory(inFolder);
            for (int i = begin_id; i < end_id; i++)
            {
                Thread.Sleep(period);
                if (Directory.Exists(inFolder + "\\Скала") == false)
                    Directory.CreateDirectory(inFolder + "\\Скала");
                foreach (string s1 in Directory.GetFiles(paths[i]))
                {
                    string s2 = inFolder + "\\Скала" + "\\" + Path.GetFileName(s1);
                    File.Copy(s1, s2, true);
                }
            }
        }
    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SkalaForm());
        }
    }
}
