using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using corelib;
using System.Data.Odbc;

namespace AzotEmulation_vs90
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DB db = new DB();
            db.CreateDB();
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {

                db.InsertToDB(ofd.FileName);

            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DB db = new DB();
            db.DeleteDB();
        }

       
    }
}
