using System;
using System.Linq;
using System.Drawing;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using corelib;
//using RockMicoPlugin;
using PluginMDX_vs90;
using PgSqlStorage;
using SqliteStorage;

namespace SunSql
{
    public partial class Form1 : Form
    {
        static void TestFiles(SunSqlEnv env, string path, IDataComponent component, string serverNameString, string serverPort, string pgsqlUserId, string pgsqlPassword, string baseName)
        {
            SingleSearchToPgSqlProvider prov = path == null ? null : env.CreateProvider(component, path, env, serverNameString, serverPort, pgsqlUserId, pgsqlPassword, baseName);
            
        }

        static void TestFiles2(SunSqliteEnv env, string path, IDataComponent component)
        {
            SingleSearchToPgSqlProvider prov = path == null ? null : env.CreateProvider(component, path, env);
        }

        public Form1()
        {
            InitializeComponent();
            

        }

        int timeadd;

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SunSqlEnv env;
            //if (args.Length > 2) 
            //env = new SunSqlEnv(new CartogramPresentationConfig(Convert.ToInt32(args[1])),new CartogramPresentationConfig(Convert.ToInt32(args[2])));            
            //else
            env = new SunSqlEnv(new CartogramPresentationConfig(false, false, true), new CartogramPresentationConfig(false, false, true));
            IDataComponent component;
            //if (args.Length > 0)
            //component = env.Data.Find(args[0]);
            //else
            component = new DataComponents.DataComponent(typeof(RockMicroSingleProviderMDX));
            FolderBrowserDialog bd = new FolderBrowserDialog();
            bd.Description = String.Format("Выберете папку для: {0}", component.Info.HumanDescribe);
            bd.ShowNewFolderButton = false;
            DialogResult dr = bd.ShowDialog();
            label4.Text = DateTime.Now.ToString();
            TestFiles(env, dr == DialogResult.OK ? bd.SelectedPath : null, component, "127.0.0.1", "5432", "postgres", "skala", "mdx");
            label5.Text = DateTime.Now.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SunSqliteEnv env;
            //if (args.Length > 2) 
            //env = new SunSqlEnv(new CartogramPresentationConfig(Convert.ToInt32(args[1])),new CartogramPresentationConfig(Convert.ToInt32(args[2])));            
            //else
            env = new SunSqliteEnv(new CartogramPresentationConfig(false, false, true), new CartogramPresentationConfig(false, false, true));
            IDataComponent component;
            //if (args.Length > 0)
            //component = env.Data.Find(args[0]);
            //else
            component = new DataComponents.DataComponent(typeof(RockMicroSingleProviderMDX));
            FolderBrowserDialog bd = new FolderBrowserDialog();
            bd.Description = String.Format("Выберете папку для: {0}", component.Info.HumanDescribe);
            bd.ShowNewFolderButton = false;
            DialogResult dr = bd.ShowDialog();
            label7.Text = DateTime.Now.ToString();
            TestFiles2(env, dr == DialogResult.OK ? bd.SelectedPath : null, component);
            label6.Text = DateTime.Now.ToString();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


    }

    public class SunSqliteEnv : GuiEnv
    {
        static DataParamTable Combine(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
        {
            return new DataParamTable(new TupleMetaData("enviroment", "enviroment", DateTime.Now, "enviroment"),
                new DataParamTableItem("coordsCalculation", lenearConf.Value),
                new DataParamTableItem("coordsVisualization", visualConf.Value));
        }
        public SunSqliteEnv(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
            : base(Combine(lenearConf, visualConf), ".", ".")
        { }

        public SingleSearchToPgSqlProvider CreateProvider(IDataComponent c, string path, IEnviromentEx env)
        {
            
            SqliteProvider pgprovider = new SqliteProvider(env, "testfile.db3", false, true, "v1");/////////;;;;;;;;;;;;;;;;;;;

            SingleSearchToPgSqlProvider p = new SingleSearchToPgSqlProvider(this, c, path, pgprovider);
            if (p.IsErrors)
            {
                MessageBox.Show(p.Errors, "Следующие файлы содержат ошибки");
            }
            return p;
        }
    }


    public class SunSqlEnv : GuiEnv
    {
        static DataParamTable Combine(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
        {
            return new DataParamTable(new TupleMetaData("enviroment", "enviroment", DateTime.Now, "enviroment"),
                new DataParamTableItem("coordsCalculation", lenearConf.Value),
                new DataParamTableItem("coordsVisualization", visualConf.Value));
        }
        public SunSqlEnv(CartogramPresentationConfig lenearConf, CartogramPresentationConfig visualConf)
            : base(Combine(lenearConf, visualConf), ".", ".")
        {
            IDataResource idr = CreateData("SunEnv_PgDataStorageProvider");
        }

        public SingleSearchToPgSqlProvider CreateProvider(IDataComponent c, string path, IEnviromentEx env, string serverNameString, string serverPort, string pgsqlUserId, string pgsqlPassword, string baseName)
        {
            PgSqlProvider pgprovider = new PgSqlProvider(env, serverNameString, serverPort, pgsqlUserId, pgsqlPassword, baseName);

            SingleSearchToPgSqlProvider p = new SingleSearchToPgSqlProvider(this, c, path, pgprovider);
            if (p.IsErrors)
            {
                MessageBox.Show(p.Errors, "Следующие файлы содержат ошибки");
            }
            return p;
        }
    }
}
