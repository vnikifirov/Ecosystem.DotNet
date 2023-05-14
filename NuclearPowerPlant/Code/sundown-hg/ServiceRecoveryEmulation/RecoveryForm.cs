using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using corelib;
using RockMicoPlugin;
using System.Threading;
using SqliteStorage;
//using SunBrowser;
using System.IO;
using Algorithms;

namespace ServiceRecoveryEmulation
{
    public partial class RecoveryForm : Form
    {
        Thread RThread;
        public RecoveryForm()
        {
            InitializeComponent();
        }

        public class RecoveryThread
        {
            public RecoveryThread(string name)
            {
            }
            public void run()
            {
                RecoveryPlugin RP = new RecoveryPlugin();
                RP.WorkService();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //формирование потока
            RecoveryThread rt = new RecoveryThread("Поток #1");
            RThread = new Thread(new ThreadStart(rt.run));
            RThread.Start();
        }

        private void RecoveryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(RThread.IsAlive == true)
                RThread.Abort();
            MessageBox.Show("Остановка");
        }

  /*      private void TBox_TextChanged(object sender, EventArgs e)
        {

        }*/
        
    }
}
