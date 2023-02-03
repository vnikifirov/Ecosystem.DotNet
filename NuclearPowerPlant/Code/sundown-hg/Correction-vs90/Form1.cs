using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

//using RockMicoPlugin;
using corelib;
//using Algorithms;

namespace Correction_vs90
{
    public partial class Form1 : Form
    {
        public BasicEnv env;
        List<Item> fitems;
        double[][] fbadness;

        public Form1()
        {
            InitializeComponent();           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog brd = new FolderBrowserDialog();
            brd.Description = "Выберете папку для анализа ";
            brd.ShowNewFolderButton = false;

            DialogResult dr = brd.ShowDialog();
            
            if (dr == DialogResult.OK)
            {
                TextPath.Text = brd.SelectedPath;
                //ob.Analyze();
            }                                     
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            AnalyzeGrid.Rows.Clear();

  
            fitems = new List<Item>();

            List<Item> items = Program.parseDirectory(TextPath.Text,env);
            

            int i = 0;
            int k = 0;
            fbadness = new double[items.Count][];
            foreach (Item j in items)
            {
                double sbadness;
                string vklad;
                j.Init(env);
                try
                {
                    fbadness[k] = j.Analyze(env, out sbadness, out vklad);
                    k++;
                }
                catch (Exception ex)
                {
                    
                    string exep = ex.ToString();
                    int idx1 = exep.LastIndexOf("Program.cs:")+11;
                    int idx2 = exep.IndexOf("\r",idx1);
                    j.result = exep.Substring(idx1, idx2 - idx1);

                    sbadness = -100;
                    vklad = "-100";
                }

                fitems.Add(j); // для обработки дойного клика
                i = AnalyzeGrid.Rows.Add();

                AnalyzeGrid.Rows[i].Cells[0].Value = j.data;
                AnalyzeGrid.Rows[i].Cells[1].Value = j.folder;
                AnalyzeGrid.Rows[i].Cells[2].Value = j.filename;
                AnalyzeGrid.Rows[i].Cells[3].Value = j.result;
                if ((sbadness == -100)||(sbadness==-1))
                {
                    AnalyzeGrid.Rows[i].Cells[4].Value = "-";
                    AnalyzeGrid.Rows[i].Cells[5].Value = "-";
                }
                else
                {
                    AnalyzeGrid.Rows[i].Cells[4].Value = sbadness;
                    AnalyzeGrid.Rows[i].Cells[5].Value = vklad;
                }

                

                
            }

          
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StreamWriter fin;
           
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.csv)|*.csv|(*.txt)|*.txt";
 
            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.Cancel) return;

            fin = new StreamWriter(sfd.FileName, false, Encoding.GetEncoding(1251));

            int nrow = AnalyzeGrid.Rows.Count;
            int ncol = AnalyzeGrid.ColumnCount;

            fin.Write("№;Папка;Скала;Прописка;Отчет;Badness;Fail_Fiber;Нитка;");
            for (int j = 1; j <= 16; j++)
            {
                //fin.Write("Нитка №");
                fin.Write(j);
                fin.Write(";");
            }
            fin.WriteLine();
            

            for (int i = 0; i < nrow-1; i++)
            {
                fin.Write((i + 1).ToString());
                fin.Write(";");
                fin.Write("'");
                for (int j = 0; j < ncol; j++)
                {
                    fin.Write(AnalyzeGrid.Rows[i].Cells[j].Value.ToString());
                    fin.Write(";");
                }
                fin.Write("Ошибка;");
                for (int j = 0; j < fbadness[i].Length; j++)
                {
                    fin.Write(fbadness[i][j]);
                    fin.Write(";");
                }
                fin.WriteLine();
            }
            fin.Close();
            

        }

        

        private void AnalyzeGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //List<Item> items = Program.parseDirectory(TextPath.Text, env);
            if (e.ColumnIndex == 1)
            {
                /*string folder = TextPath.Text + "\\";
                folder = folder + AnalyzeGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                folder = folder + "\\" + AnalyzeGrid.Rows[e.RowIndex].Cells[1].Value.ToString();
                RockMicroSingleProvider skala_click = new RockMicroSingleProvider(env, folder);
                DataTuple skala_tuple = skala_click.GetDataTuple();*/
         
                DataTupleVisualizer vs = new DataTupleVisualizer(env);
                ListMultiDataProvider prv = new ListMultiDataProvider();
                prv.PushData(fitems[e.RowIndex].skala.GetDataTuple());
                vs.SetDataProvider(prv);
                Form f = new Form();
                vs.Dock = DockStyle.Fill;
                f.Controls.Add(vs);
                f.Width = 1000;
                f.Height = 300;
                f.ShowDialog();
            }
            else if (e.ColumnIndex == 2)
            {
                /*string filename = TextPath.Text + "\\";
                filename = filename + AnalyzeGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                filename = filename + "\\" + AnalyzeGrid.Rows[e.RowIndex].Cells[2].Value.ToString();
                DataTupleProvider azot_click = new DataTupleProvider(env, filename);
                IMultiDataTuple azot_tuple = azot_click.GetData();*/

                DataTupleVisualizer vs = new DataTupleVisualizer(env);
                ListMultiDataProvider prv = new ListMultiDataProvider();
                prv.PushData(fitems[e.RowIndex].azot.GetData());
                vs.SetDataProvider(prv);
                Form f = new Form();
                vs.Dock = DockStyle.Fill;
                f.Controls.Add(vs);
                f.Width = 1000;
                f.Height = 300;
                f.ShowDialog();
            }
            else if (e.ColumnIndex == 4)
            {
                MessageBox.Show(fitems[e.RowIndex].sb_badness_click.ToString(), "Величины ошибок");
            }
            else if (e.ColumnIndex == 5)
            {
                MessageBox.Show(fitems[e.RowIndex].sb_click.ToString(), "Виды ошибок");
            }


            /*else
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat("{0} = {1}", "ColumnIndex", e.ColumnIndex);
                message.AppendLine();
                message.AppendFormat("{0} = {1}", "RowIndex", e.RowIndex);
                message.AppendLine();
                message.AppendFormat("{0} = {1}", "Значение", AnalyzeGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                MessageBox.Show(message.ToString(), "Ячейка");
            }*/
            
        }

       
        
       
    }
}
