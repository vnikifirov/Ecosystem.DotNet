using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Odbc;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Threading;
using RecoveryFactory;
using corelib;
using RockMicoPlugin;

namespace EmulationComplex_vs90
{
#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
#endif


    public class SkalaGen
    {
        public string outFolder;
        
        public SkalaGen(string path)
        {
            outFolder = path;
        }
        public void CopySkala(Int32 period, string[] paths,string inpath)
        {
            //Mutex mut = new Mutex(false, "Global\\MyMutex");
            for (int i = 0; i < paths.Length; i++)
            {
                MyEnv env = new MyEnv();
                RockMicroSingleProvider skala = new RockMicroSingleProvider(env, paths[i] + "\\KATKR.INI");
                if (i != 0)
                {
                    Thread.Sleep(period);
                }

                if (Directory.Exists(inpath) == false)
                    Directory.CreateDirectory(inpath);
                //mut.WaitOne();
                while (true)
                {
                    try
                    {
                        foreach (string s1 in Directory.GetFiles(paths[i]))
                        {
                            string s2 = inpath + "\\" + Path.GetFileName(s1);
                            File.Copy(s1, s2, true);
                        }
                        break;
                    }
                    catch
                    {
                    }
                }
                ComplexForm.Current_skala.Text = skala.GetDataTuple().GetTimeDate().ToString();
                //mut.ReleaseMutex();
               
            }
        }
    }


    public class DB
    {
        public StringBuilder message;
        public OdbcConnection conn;
        public IDbCommand cmd;
        public DB()
        {
            conn = new OdbcConnection("DSN=kgodb");
        }
        public int CreateDB()
        {
            conn.Open();
            cmd = conn.CreateCommand();
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `system` (
                `system_id` int(10) unsigned NOT NULL auto_increment,
                `type` enum('npp','block','bs','cart','thread') NOT NULL default 'npp',
                `owner_id` int(10) unsigned default NULL,
                `name` varchar(20) default NULL,
                PRIMARY KEY (`system_id`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message = new StringBuilder();
                message.Append("'system' уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `kart` (
                `thread_id` int(10) unsigned NOT NULL default '0',
                `n_pvk` int(11) NOT NULL default '0',
                `y` int(11) NOT NULL default '0',
                `x` int(11) NOT NULL default '0',
                `kind` enum('sv','dp','pus','tvs') NOT NULL default 'sv',
                `dt` datetime default NULL,
                PRIMARY KEY (`thread_id`,`n_pvk`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`kart` уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `spec` (
                `spec_id` int(10) unsigned NOT NULL auto_increment,
                `time` float NOT NULL default '0',
                `c1` int(10) unsigned NOT NULL default '0',
                `c2` int(10) unsigned NOT NULL default '0',
                `count0` int(10) unsigned NOT NULL default '0',
                `count1` int(10) unsigned NOT NULL default '0',
                `count2` int(10) unsigned NOT NULL default '0',
                `count3` int(10) unsigned NOT NULL default '0',
                `count4` int(10) unsigned NOT NULL default '0',
                `spec` blob,
                PRIMARY KEY (`spec_id`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`spec` уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `prp_specs` (
                `spec_id` int(10) unsigned NOT NULL default '0',
                `prp_id` int(10) unsigned NOT NULL default '0',
                `n` int(11) NOT NULL default '0',
                PRIMARY KEY (`prp_id`,`spec_id`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`prp_specs` уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `prp` (
                `prp_id` int(10) unsigned NOT NULL auto_increment,
                `kind` enum('prp','pc','srv','reload','fast srv','breload','areload') NOT NULL default 'prp',
                `dt` datetime NOT NULL default '0000-00-00 00:00:00',
                `time` float NOT NULL default '0',
                `grd_id` int(10) unsigned NOT NULL default '0',
                `windows_id` int(10) unsigned NOT NULL default '0',
                PRIMARY KEY (`prp_id`),
                KEY `grd_id` (`grd_id`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`prp` уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `prp_pvk` (
                `prp_pvk_id` int(10) unsigned NOT NULL default '0',
                `n_pvk` int(11) NOT NULL default '0',
                `n` float default NULL,
                `kind` enum('sv','dp','pus','tvs') NOT NULL default 'sv',
                `e` float default NULL,
                `r` float default NULL,
                `m` float default NULL,
                PRIMARY KEY (`prp_pvk_id`,`n_pvk`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`prp_pvk` уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `pc` (
                `pc_id` int(10) unsigned NOT NULL default '0',
                `n_pvk` int(11) default NULL,
                PRIMARY KEY (`pc_id`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`pc` уже создана");
                message.AppendLine();
            }
            /*****************************************************************/
            cmd.CommandText = @"CREATE TABLE `grd` (
                `grd_id` int(10) unsigned NOT NULL default '0',
                `dt` datetime NOT NULL default '0000-00-00 00:00:00',
                `n_pvk` int(11) default NULL,
                `bsm_ch` int(11) NOT NULL default '0',
                `bpv_ch` int(11) NOT NULL default '0',
                `bpv_use` char(0) default NULL,
                `u` float NOT NULL default '0',
                `p` float NOT NULL default '0',
                `s` float NOT NULL default '0',
                `k` float NOT NULL default '0',
                `grd_s` float default NULL,
                `grd_k` float default NULL,
                `grd_d` float default NULL,
                `ch_count` int(10) unsigned NOT NULL default '0',
                `system_id` int(10) unsigned NOT NULL default '0',
                `algor` enum('square','ampl') NOT NULL default 'square',
                PRIMARY KEY (`grd_id`),
                KEY `system_id` (`system_id`)
                );";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                message.Append("`grd` уже создана");
                MessageBox.Show(message.ToString());
            }
            /*****************************************************************/
            conn.Close();
            return 1;
        }
        public void DeleteDB()
        {
            conn.Open();
            cmd = conn.CreateCommand();
            message = new StringBuilder();
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE system;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`system` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE kart;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`kart` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE spec;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`spec` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE prp_specs;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`prp_specs` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE prp;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`prp` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE prp_pvk;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`prp_pvk` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE pc;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`pc` удалена");
                message.AppendLine();
            }
            catch
            {
            }
            /*****************************************************************/
            cmd.CommandText = "DROP TABLE grd;";
            try
            {
                cmd.ExecuteNonQuery();
                message.Append("`grd` удалена");
                MessageBox.Show(message.ToString());
            }
            catch
            {
            }
            /*****************************************************************/
            conn.Close();
        }
        public void InsertToDB(string tup)
        {
            
            MyEnv env = new MyEnv();
            DataTupleProvider azot;
            IMultiDataTuple azot_tuple;
            string fullFilename = tup;
            azot = new DataTupleProvider(env, fullFilename);
            //File.Create("C:\\tmp\\text.txt");
            azot_tuple = azot.GetData();
            //dt_azot = azot_tuple.GetTimeDate();
            
            try
            {
                conn.Open();
            }
            catch
            {
                Thread.Sleep(1000);
                conn.Open();
                
            }
            DataTupleProvider dtp = new DataTupleProvider(env, "pvkTuple.tup");
            IDataTuple dtp_tuple = dtp.GetData()[0];
            DataArrayCoords c = (DataArrayCoords)dtp_tuple["pvk_scheme"];

            CoordsConverter pvk2 = new CoordsConverter(
                c.Info,
                CoordsConverter.SpecialFlag.PVK,
                c);
            //*********************//заполняем таблицу system(system_id)
            cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO system values
                                        (1,'npp',NULL,'666'),
                                        (2,'block',1,1),
                                        (3,'bs',2,22),
                                        (4,'cart',3,1),
                                        (5,'thread',4,1),
                                        (6,'thread',4,2),
                                        (7,'cart',3,2),
                                        (8,'thread',7,3),
                                        (9,'thread',7,4),
                                        (10,'bs',2,21),
                                        (11,'cart',10,3),
                                        (12,'thread',11,5),
                                        (13,'thread',11,6),
                                        (14,'cart',10,4),
                                        (15,'thread',14,7),
                                        (16,'thread',14,8),
                                        (17,'bs',2,12),
                                        (18,'cart',17,5),
                                        (19,'thread',18,9),
                                        (20,'thread',18,10),
                                        (21,'cart',17,6),
                                        (22,'thread',21,11),
                                        (23,'thread',21,12),
                                        (24,'bs',2,11),
                                        (25,'cart',24,7),
                                        (26,'thread',25,13),
                                        (27,'thread',25,14),
                                        (28,'cart',24,8),
                                        (29,'thread',28,15),
                                        (30,'thread',28,16);";
            cmd.Prepare();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
            cmd.Dispose();
            //****************************************//
            //*********************//заполняем таблицу grd(grd_id,system_id)
            cmd = conn.CreateCommand();
            IDbDataParameter grd_id = cmd.CreateParameter();
            cmd.Parameters.Add(grd_id);

            int[] system_thread_id = { 5, 6, 8, 9, 12, 13, 15, 16, 19, 20, 22, 23, 26, 27, 29, 30 };
            IDbDataParameter system_id_grd = cmd.CreateParameter();
            cmd.Parameters.Add(system_id_grd);


            cmd.CommandText = @"INSERT INTO grd (grd_id,system_id)
                                        VALUES (?,?);";
            cmd.Prepare();
            for (int k = 0; k < 16; k++)
            {
                grd_id.Value = k + 1;
                system_id_grd.Value = system_thread_id[k];

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                }
            }
            cmd.Dispose();
            //****************************************//
            //*********************//заполняем таблицу kart(thread_id,n_pvk)
            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT MAX(system_id) FROM system;";
            cmd.Prepare();
            int n = Convert.ToInt32(cmd.ExecuteScalar());
            int th = 0;
            cmd.Dispose();

            for (int i = 1; i <= n; i++)
            {
                cmd = conn.CreateCommand();
                IDataParameter system_id = cmd.CreateParameter();
                cmd.Parameters.Add(system_id);
                system_id.Value = i;
                cmd.CommandText = "SELECT type FROM system WHERE system_id = ?;";
                cmd.Prepare();
                String type = (String)(cmd.ExecuteScalar());
                cmd.Dispose();

                if (type.CompareTo("thread") == 0)
                {
                    cmd = conn.CreateCommand();

                    IDataParameter thread_id = cmd.CreateParameter();
                    cmd.Parameters.Add(thread_id);

                    IDataParameter n_pvk = cmd.CreateParameter();
                    cmd.Parameters.Add(n_pvk);

                    IDataParameter y_pvk = cmd.CreateParameter();
                    cmd.Parameters.Add(y_pvk);

                    IDataParameter x_pvk = cmd.CreateParameter();
                    cmd.Parameters.Add(x_pvk);

                    cmd.CommandText = @"INSERT INTO kart (thread_id,n_pvk,y,x)
                                        VALUES (?,?,?,?);";
                    cmd.Prepare();
                    for (int pvk = 0; pvk < 115; pvk++)
                    {
                        
                        thread_id.Value = i;

                        
                        n_pvk.Value = pvk + 1;

                       
                        y_pvk.Value = pvk2[new FiberCoords(th, pvk)].HumaneY;

                       
                        x_pvk.Value = pvk2[new FiberCoords(th, pvk)].HumaneX;

                       
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                        }
                    }
                    th++;
                    cmd.Dispose();
                }
            }

            //****************************************//
            //*********************//Заполняем таблицу prp(prp_id,dt,time)
            //*********************//заполняем таблицу spec(spec_id,time)
            cmd = conn.CreateCommand();

            int sum_spec_id;
            int sum_prp_id;
            try
            {
                //счетчик для формирования столбца spec_id  в таблице prp_specs

                cmd.CommandText = "SELECT MAX(spec_id) FROM spec;";
                cmd.Prepare();
                sum_spec_id = Convert.ToInt32(cmd.ExecuteScalar());

                //счетчик для формирования столбца prp_id  в таблице prp_specs
                cmd.CommandText = "SELECT MAX(prp_id) FROM prp;";
                cmd.Prepare();
                sum_prp_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch         //если в таблице prp и prp_specs еще нет записей
            {
                sum_spec_id = 0;
                sum_prp_id = 0;
            }
            
            int sum = sum_prp_id;

            IDbDataParameter dt = cmd.CreateParameter();
            IDbDataParameter time2 = cmd.CreateParameter();
            IDbDataParameter prp_grd_id = cmd.CreateParameter();
            cmd.Parameters.Add(dt);
            cmd.Parameters.Add(time2);
            cmd.Parameters.Add(prp_grd_id);
            cmd.CommandText = @"INSERT INTO prp (dt,time,grd_id)
                                        VALUES (?,?,?);";
            cmd.Prepare();
            for (int j = 0; j < azot_tuple.Count; j++)
            {
                //*********************//Заполняем таблицу prp(prp_id,dt,time)
                dt.Value = azot_tuple[j]["kgoprp_info"].Date;
                time2.Value = (((DataParamTable)azot_tuple[j]["kgoprp_info"])["specClock"].ToDouble());
                prp_grd_id.Value = j + 1;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                }
                
                IDbCommand cmd2;
                IDbCommand cmd3;
                cmd2 = conn.CreateCommand();
                IDbDataParameter time = cmd2.CreateParameter();
                cmd2.Parameters.Add(time);
                IDbDataParameter count0 = cmd2.CreateParameter();
                cmd2.Parameters.Add(count0);
                IDbDataParameter count1 = cmd2.CreateParameter();
                cmd2.Parameters.Add(count1);
                IDbDataParameter count2 = cmd2.CreateParameter();
                cmd2.Parameters.Add(count2);
                IDbDataParameter count3 = cmd2.CreateParameter();
                cmd2.Parameters.Add(count3);
                IDbDataParameter count4 = cmd2.CreateParameter();
                cmd2.Parameters.Add(count4);
                cmd2.CommandText = @"INSERT INTO spec (time,count0,count1,count2,count3,count4)
                                        VALUES (?,?,?,?,?,?);";
                cmd2.Prepare();
                //таблица prp_specs(spec_id,prp_id)
                cmd3 = conn.CreateCommand();
                IDbDataParameter spec_id = cmd3.CreateParameter();
                cmd3.Parameters.Add(spec_id);
                IDbDataParameter prp_id = cmd3.CreateParameter();
                cmd3.Parameters.Add(prp_id);
                IDbDataParameter num = cmd3.CreateParameter();
                cmd3.Parameters.Add(num);
                cmd3.CommandText = @"INSERT INTO prp_specs (spec_id,prp_id,n)
                                        VALUES (?,?,?);";
                cmd3.Prepare();

                for (int i = 0; i < ((DataArray)azot_tuple[j]["kgoprp_azot"]).Length; i++)
                {

                    time.Value = ((DataParamTable)azot_tuple[j]["kgoprp_info"])["specClock"].ToDouble();

                    count0.Value = ((DataArray)azot_tuple[j]["kgoprp_azot"])[i];
                    //проверка остальных окон
                    if (azot_tuple[j].GetParamSafe("kgoprp_w1") != null)
                    {
                        count1.Value = ((DataArray)azot_tuple[j]["kgoprp_w1"])[i];
                    }
                    else
                    {
                        count1.Value = 0;
                    }
                    if (azot_tuple[j].GetParamSafe("kgoprp_w2") != null)
                    {
                        count2.Value = ((DataArray)azot_tuple[j]["kgoprp_w2"])[i];
                    }
                    else
                    {
                        count2.Value = 0;
                    }
                    if (azot_tuple[j].GetParamSafe("kgoprp_w3") != null)
                    {
                        count3.Value = ((DataArray)azot_tuple[j]["kgoprp_w3"])[i];
                    }
                    else
                    {
                        count3.Value = 0;
                    }
                    if (azot_tuple[j].GetParamSafe("kgoprp_w4") != null)
                    {
                        count4.Value = ((DataArray)azot_tuple[j]["kgoprp_w4"])[i];
                    }
                    else
                    {
                        count4.Value = 0;
                    }

                    try
                    {
                        cmd2.ExecuteNonQuery();
                    }
                    catch
                    {
                    }
                    
                    sum_spec_id = sum_spec_id + 1;
                    spec_id.Value = sum_spec_id;
                    
                    prp_id.Value = sum_prp_id + j + 1;
                    num.Value = i;
                    try
                    {
                        cmd3.ExecuteNonQuery();
                    }
                    catch
                    {
                    }
                }

                cmd2.Dispose();
                cmd3.Dispose();
                
                
            }
            cmd.Dispose();

            
            //****************************************//
            //*********************//Заполняем таблицу prp_pvk(prp_pvk_id,n_pvk)
            
            for (int fiber = 0; fiber < 16; fiber++)
            {
                cmd = conn.CreateCommand();
                IDbDataParameter prp_pvk_id = cmd.CreateParameter();
                cmd.Parameters.Add(prp_pvk_id);
                IDbDataParameter n_pvk = cmd.CreateParameter();
                cmd.Parameters.Add(n_pvk);
                cmd.CommandText = @"INSERT INTO prp_pvk(prp_pvk_id,n_pvk)
                                        VALUES (?,?);";
                cmd.Prepare();
                for (int pvk = 0; pvk < 115; pvk++)
                {
                    prp_pvk_id.Value = sum + fiber + 1;
                    n_pvk.Value = pvk + 1;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                    }
                }
                cmd.Dispose();
            }
            ComplexForm.Current_azot.Text = azot_tuple.GetTimeDate().ToString();
            //MessageBox.Show("Information has been succesfully added");
            conn.Close();
        }
    }

    static class Emulation
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ComplexForm());
        }
    }

    class MyEnv : BasicEnv
    {
        public MyEnv() :
            base(new DataParamTable(new TupleMetaData(), new DataParamTableItem("s", 0)),
            ".", ".", new CartogramPresentationConfig(5), new CartogramPresentationConfig(5))
        {
        }
    }
}
