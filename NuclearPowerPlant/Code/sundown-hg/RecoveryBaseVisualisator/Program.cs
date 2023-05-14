using System;
using System.Threading;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Windows.Forms;
using corelib;
using RecoveryBaseVisualisator;
using System.ComponentModel;

public struct struct1
{
    public Form form;
    public RBVEnv env;
}

namespace RecoveryBaseVisualisator
{
    static class Program
    {
        static BackgroundWorker bw = new BackgroundWorker();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
#if !DOTNET_V11
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            
            try
            {
                DataParamTable config = DataParamTable.LoadFromXML("config.xml");

                ISerializeStream st = config.Serialize();
                byte[] data = st.GetData();

                StreamDeserializer ds = new StreamDeserializer(data, 0);
                DataParamTable cnf = DataParamTable.Create(ds);
 
                
                using (RBVEnv s = new RBVEnv(config))
                {
                   // bool hide = config.GetParamSafe("SunEnv_HideActionForm").IsNotNull ? Convert.ToBoolean(config["SunEnv_HideActionForm"]) : false;
                    struct1 struct1_1;
                    struct1_1.env = s;
                                        
                    s.Init();
                    s.UpdateDates();
                    
                    Form f = s.ViewMultiTupleForm(new ListMultiDataProvider(s.GetAllData(s.DefProvider)));
                    
                    f.WindowState = FormWindowState.Maximized;
                    f.Text = "База данных архивной информации с восстановлением расходов, последнее обновление " + DateTime.Now.ToString() ;

                    struct1_1.form = f;
                 //   bw.DoWork += bw_DoWork;
                 //   bw.RunWorkerAsync(struct1_1);
                    Application.Run(f);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "В ходе работы программы произошла ошибка");
            }
        }

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {  
            struct1 strnew = (struct1)e.Argument;
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                strnew.env.UpdateDates();
                DataTupleVisualizer v = (DataTupleVisualizer)strnew.form.Controls[0];
                string NN = DateTime.Now.ToString();
                MessageBox.Show("База обновлена в " + NN);
                strnew.form.Text = "База данных архивной информации с восстановлением расходов, последнее обновление " + NN;
                v.SetDataProvider(strnew.env.DefProvider);
            }
        }
    }
}
