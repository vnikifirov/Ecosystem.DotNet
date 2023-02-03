using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using corelib;
using RecoveryFactory;

namespace CorrectionPlugin_vs90
{
    public partial class FormCorrectionPlugin : Form
    {
        public BasicEnv env;
        List<Item> fitems;
        double[][] fbadness;

        //public IMultiDataProvider prov;

        List<Item> item_good;

        public FormCorrectionPlugin()
        {
            InitializeComponent();
        }

        private void TextPath_TextChanged(object sender, EventArgs e)
        {

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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AnalyzeGrid.Rows.Clear();
            fitems = new List<Item>();

            List<Item> items = Program.parseDirectory(TextPath.Text, env);
            //ListMultiDataProvider p = new ListMultiDataProvider();
            /*****************/
            item_good = new List<Item>();
            /*****************/

            int i = 0;
            int k = 0;
            fbadness = new double[items.Count][];
            foreach (Item j in items)
            {
                double sbadness;
                string vklad;
                bool res = j.Init(env);
                try
                {
                    fbadness[k] = j.Analyze(env, out vklad);
                    k++;
                }
                catch (Exception ex)
                {
                    string exep = ex.ToString();
                    int idx1 = exep.LastIndexOf("Program.cs:") + 11;
                    int idx2 = exep.IndexOf("\r", idx1);
                    j.result = exep.Substring(idx1, idx2 - idx1);
                    j.sbadness = -100;
                    vklad = "-100";
                }

                sbadness = j.sbadness;
                fitems.Add(j); // для обработки дойного клика
                i = AnalyzeGrid.Rows.Add();

                AnalyzeGrid.Rows[i].Cells[0].Value = j.data;
                AnalyzeGrid.Rows[i].Cells[1].Value = j.folder;
                AnalyzeGrid.Rows[i].Cells[2].Value = j.filename;
                if(fitems[i].zrk != null)
                    AnalyzeGrid.Rows[i].Cells[3].Value = j.zrk_Filename;
                else
                    AnalyzeGrid.Rows[i].Cells[3].Value = "-";
                AnalyzeGrid.Rows[i].Cells[4].Value = j.result;
                AnalyzeGrid.Rows[i].Cells[5].Value = sbadness;
                ////////
                if ((sbadness < 500)&&(sbadness >= 0))
                    item_good.Add(j);
                ////////
                if ((sbadness == -100) || (sbadness == -1))
                {
                    AnalyzeGrid.Rows[i].Cells[5].Value = "-";
                    AnalyzeGrid.Rows[i].Cells[6].Value = "-";
                }
                else
                {
                    AnalyzeGrid.Rows[i].Cells[5].Value = sbadness;
                    AnalyzeGrid.Rows[i].Cells[6].Value = vklad;
                }
                //////////////////////////////////////////////////////////////////////////////////////
                if ((sbadness >= 500) && (sbadness < 1550))
                {
                    for (int l = 0; l < AnalyzeGrid.Rows[i].Cells.Count; l++)
                        AnalyzeGrid.Rows[i].Cells[l].Style.BackColor = Color.Yellow;
                }
                if ((sbadness >= 1550)||(sbadness == -100)||(sbadness == -1))
                {
                    for (int l = 0; l < AnalyzeGrid.Rows[i].Cells.Count; l++)
                        AnalyzeGrid.Rows[i].Cells[l].Style.BackColor = Color.Red;
                }
                    
                //////////////////////////////////////////////////////////////////////////////////////
            }

#if NOT_DEF            

            string[] global_array = System.IO.Directory.GetDirectories(TextPath.Text, "*");
            int k = 0;
           
            foreach (string name in global_array)
            {

                string[] array1 = System.IO.Directory.GetDirectories(name, "1B*");
                string[] array2 = System.IO.Directory.GetFiles(name, "*.tup");



                int n;
               
                if (array1.Length < array2.Length) n = array1.Length;
                else n = array2.Length;
                if (n == 0) continue;
                AnalyzeGrid.Rows.Add(n);
                
                for (int i = k; i < n+k; i++)
                {
                    //Выделение подстроки с названием папки
                    int l;
                    for (l = name.Length; name[l - 1] != '\\'; l--) ;
                    AnalyzeGrid.Rows[i].Cells[0].Value = name.Substring(l, name.Length - l);

                     

                    /*********************/
                    //Выделение подстроки с названием папки
                 
                    for (l = array1[i - k].Length; array1[i - k][l - 1] != '\\'; l--) ;
                    string s=array1[i - k].Substring(l, array1[i - k].Length - l);
                    /*********************/

                    
                                      
                    AnalyzeGrid.Rows[i].Cells[1].Value = s;
                                       
                    AnalyzeGrid.Rows[i].Cells[2].Value = System.IO.Path.GetFileName(array2[i - k]);
                   
                    AnalyzeGrid.Rows[i].Cells[3].Value = "+";
                   
                }
                k = k+n;
                
            }       

            //ob.Analyze();
            Program.parseDirectory(TextPath.Text);
           
#endif


           
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

            fin.Write("№;Папка;Скала;Прописка;ЗРК;Отчет;Badness;Fail_Fiber;Нитка;");
            for (int j = 1; j <= 16; j++)
            {
                //fin.Write("Нитка №");
                fin.Write(j);
                fin.Write(";");
            }
            fin.WriteLine();


            for (int i = 0; i < nrow - 1; i++)
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

        private void button4_Click(object sender, EventArgs e)
        {

            /*File.Delete("C:\\sundown-hg\\test2\\bin\\Debug_NET2\\SqliteTestOPE.db3");
            Thread.Sleep(100);
            */
            
            for (int i = 0; i < item_good.Count; i++)
            {
                TupleMetaData info = new TupleMetaData("pvk_maxes_cart", "pvk_maxes_cart", item_good[i].azot_tuple.GetTimeDate(), TupleMetaData.StreamAuto);
                IMultiTupleItem kpd = item_good[i].mi/*azot_tuple["pvk_maxes"]*/;
                DataCartogram coeffDP = DataCartogram.CreateFromParts(info,
                    env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, info), kpd);



                string azotStream = "azot";
                string mainStream = "main";

                DateTime dt0 = ((IDataTuple)item_good[i].skala.GetDataTuple()).GetTimeDate();
                DataTuple dataSkala = new DataTuple((IDataTuple)item_good[i].skala.GetDataTuple(), mainStream);
                DateTime dt = dt0.AddSeconds(i+1);
                dataSkala = dataSkala.ReDate(dt);


                DataTuple azotComact = new DataTuple(azotStream, dataSkala.GetTimeDate(), coeffDP);

                /****************************************/
                /****************************************/
                DataTuple tupelsS1 = new DataTuple(dataSkala, "Скала");
                DataTuple tupelsS2 = new DataTuple(azotComact, "Активность компактно");

                ListMultiDataProvider l = new ListMultiDataProvider();
                l.PushData(item_good[i].azot_tuple);
                l.PushData(tupelsS1);
                l.PushData(tupelsS2);
                //l.PushData(item_good[i].zrk);
                //DateTime dtime = l.GetDates("zrk")[0];
                //IDataTuple tn = l.GetData(dt, "zrk")[0];
                IDataTuple tn = (IDataTuple)(item_good[i].zrk.GetData());
                ITupleItem zrk = tn[0];

                zrk = zrk.Rename(new TupleMetaData(zrk.Name, zrk.HumaneName, dataSkala.GetTimeDate(), mainStream));
                tn = new DataTuple(mainStream, dataSkala.GetTimeDate(), zrk);

                // Объединение с dataSkala
                string[] names = new string[dataSkala.ItemsCount + tn.ItemsCount];
                dataSkala.CopyNamesTo(names);
                tn.CopyNamesTo(names, dataSkala.ItemsCount);
                DataTuple combined = DataTuple.Combine(names, dataSkala, tn);

                dataSkala = new DataTuple(combined, mainStream, dataSkala.GetTimeDate());
                /****************************************/
                /****************************************/


                // Store it 
                IDataResource dataResource = ((GuiEnv)env).OpenData("SunEnv_DataStorageProvider");

                dataResource.GetMultiProvider().PushData(azotComact);
                dataResource.GetMultiProvider().PushData(dataSkala);
                
            }
            MessageBox.Show("Данные успешно занесены в базу");

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
                f.Text = "Данные скалы";
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
                f.Text = "Данные азотной прописки";
                f.ShowDialog();
            }
            else if ((e.ColumnIndex == 3) && (fitems[e.RowIndex].zrk != null))
            {
                /*string filename = TextPath.Text + "\\";
                filename = filename + AnalyzeGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                filename = filename + "\\" + AnalyzeGrid.Rows[e.RowIndex].Cells[2].Value.ToString();
                DataTupleProvider azot_click = new DataTupleProvider(env, filename);
                IMultiDataTuple azot_tuple = azot_click.GetData();*/

                DataTupleVisualizer vs = new DataTupleVisualizer(env);
                ListMultiDataProvider prv = new ListMultiDataProvider();
                prv.PushData(fitems[e.RowIndex].zrk.GetData());
                vs.SetDataProvider(prv);
                Form f = new Form();
                vs.Dock = DockStyle.Fill;
                f.Controls.Add(vs);
                f.Width = 1000;
                f.Height = 300;
                f.Text = "Данные по ЗРК";
                f.ShowDialog();
            }
            else if (e.ColumnIndex == 5)
            {
                MessageBox.Show(fitems[e.RowIndex].sb_badness_click.ToString(), "Величины ошибок");
            }
            else if (e.ColumnIndex == 6)
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


