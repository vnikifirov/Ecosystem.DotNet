using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Windows.Forms;
using corelib;

namespace RecoveryFactory
{
    class Program
    {
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


                using (SunEnv s = new SunEnv(config))
                {
                    bool hide = config.GetParamSafe("SunEnv_HideActionForm").IsNotNull ? Convert.ToBoolean(config["SunEnv_HideActionForm"]) : false;

                    if (hide)
                    {
                        s.Init();
                        s.UpdateDates();
                        Form f = s.ViewMultiTupleForm(new ListMultiDataProvider(s.GetAllData(s.DefProvider)), true);
                        f.Size = new System.Drawing.Size(800, 550);
                        f.Text = "База данных по работе с восстановлением расходов по азотной активности";
                        Application.Run(f);
                    }
                    else
                    {
                        Application.Run(new MainForm(s));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "В ходе работы программы произошла ошибка");
            }

        }
    }
}