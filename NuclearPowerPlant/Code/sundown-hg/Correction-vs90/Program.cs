using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;

using RockMicoPlugin;
using corelib;
using Algorithms;
using System.Text;




namespace Correction_vs90
{


    public class Item 
    {
        public string filename;
        public string fullFilename;
        public string folder;
        public string fullFolder;

        public string data;
        public string result;
        //public string fibResult;
        public string vklad;
        //public string[] str_click;
        public StringBuilder sb_click;
        public StringBuilder sb_badness_click;


        public RockMicroSingleProvider skala;
        public DataTupleProvider azot;

        bool init;
        public double sbadness;

        public bool Init(IEnviromentEx env)
        {
            bool err = false;
            try
            {
                skala = new RockMicroSingleProvider(env, fullFolder);
            }
            catch
            {
                result = "ошибка в скале";
                err = true;
            }

            try
            {
                azot = new DataTupleProvider(env, fullFilename);
            }
            catch
            {
                result = result + "ошибка в азоте";
                err = true;
            }

            init = !err;
            return err;
        }



        public double[] Analyze(BasicEnv pvk, out double sbadness, out string vklad)
        {
            double[] db = { 0.0 };
            sbadness = -1;
            //vklad = "-1";
            if (!init)
            {
                vklad = "-1";
                return db;
            }
            //создание скалы для анализа
            DataTuple skala_tuple = skala.GetDataTuple();
            DataCartogram zagr_rockmicro = (DataCartogram)skala_tuple["zagr_rockmicro"];       
            double[,] zagrN = zagr_rockmicro.ScaleNativeArray(0);
            RockMicroAlgo.zagrConvertToInternal(zagrN);
            DataCartogramNative<Double> c = new DataCartogramNative<Double>(zagr_rockmicro.Info, ScaleIndex.Default, zagrN);
           
            double[,] zagr = c.ScaleToIndexed2(pvk.PVK, 0);

            //получение прописки
            IMultiDataTuple azot_tuple = azot.GetData();


            //получение параметров для вызова функции разбора
            int[] fb = new int[azot_tuple.Count];
            double[][] azt = new double[azot_tuple.Count][];

            for (int i = 0; i < azot_tuple.Count; i++)
            {               
                int a = ((DataParamTable)azot_tuple[i]["kgoprp_info"])["fiberNum"].ValueInt;
                fb[i] = a;
                ((DataArray)azot_tuple[i]["kgoprp_azot"]).Normalize().ConvertToArray(out azt[i]);
            }
            IDataTuple azot0 = azot_tuple[0];
            double[] badness;
            double[][] pvk_maxes;
            string[] report;
            
            KgoDetectAlgo.kgoDetectMaxPos(fb, azt, zagr, out  badness, out pvk_maxes, out report);
            //разбор репорта
            
            result =  ReportEdit(report,out vklad);
            ////////////////////////
            sb_badness_click = new StringBuilder();
            

            sbadness = 0;
            for (int i = 0; i < badness.Length; i++)
            {
                sbadness = sbadness + badness[i];
                sb_badness_click.AppendFormat("Нитка {0}: {1}                ", i, badness[i]);
                sb_badness_click.AppendLine();
            }
            return badness;
            

        }
        //Функция определения максимального вклада в погрешность
        public string ReportEdit(string[] rep, out string vklad)
        {
            /*
             * wt-water tvs
             * we-water empty
             * FT-Fail TVS
             * NF-Not Founded
            */
            sb_click = new StringBuilder();

           

            int wt = 0, we = 0, FT = 0, NF = 0;
           
            string mainrep;            //отчет по всем ниткам
            double[] fiber_badness=new double[rep.Length];    //badness по нитке
            string[] fiberrep=new string [rep.Length];   //отчет по тяжелым ниткам
            int[] fib_n=new int [rep.Length];               //номер тяжелой нитки
            int k =0;

            for (int i = 0; i < rep.Length; i++)
            {
                int wtf = 0, wef = 0, FTf = 0, NFf = 0;
                string[] repSp = rep[i].Split(' ');
                foreach (string dBadness in repSp)
                {
                    
                    if (dBadness.Length == 0) continue;
                    if (dBadness.Substring(0, 2) == "wt") { wt++; wtf++; }
                    else if (dBadness.Substring(0, 2) == "we") { we++; wef++; }
                    else if (dBadness.Substring(0, 2) == "FT") { FT++; FTf++; }

                    else if (dBadness.Substring(0, 11) == "NOT_FOUNDED") { NF++; NFf++; }
                }


                sb_click.AppendFormat("Нитка {0}: ({1})Water Empty ({2})Water Tvs ({3})Fail Tvs ({4})NOT_FOUNDED", i, wef, wtf, FTf, NFf);
                sb_click.AppendLine();
                
                //разбор отчета по ниткам(определение тяжелых ниток)
                fiber_badness[i]=wef*10+wtf*25+FTf*500+NFf*2000;
                if (fiber_badness[i] >= 500)
                {
                    fib_n[k] = i;
                    if ((NF != 0) && (FT != 0)) fiberrep[k] = i.ToString() +"-"+ NFf.ToString() + "NF" + FTf.ToString() + "FT";
                    else if (FT != 0) fiberrep[k] = i.ToString() + "-" + FTf.ToString() + "FT";
                    else if (NF != 0) fiberrep[k] = i.ToString() + "-" + NFf.ToString() + "NF";

                    if ((NF != 0) && (FT != 0)) fiberrep[k] = i.ToString() + "-" + NFf.ToString() + "NF" + FTf.ToString() + "FT";
                    else if (FT != 0) fiberrep[k] = i.ToString() + "-" + FTf.ToString() + "FT";
                    else if (NF != 0) fiberrep[k] = i.ToString() + "-" + NFf.ToString() + "NF";


                    k++;
                }


            }
            
            vklad=null;
            if (k > 0)
            {
                for (int i = 0; i < k; i++)
                {
                    if (k == 0) continue;
                    vklad = vklad + " " + fiberrep[i];
                }
                vklad = "(ошибки) Нитки: " + vklad;
            }
            //для тежелых ниток с we или wt
            double sumBad = we * 10 + wt * 25 + FT * 500 + NF * 2000; 
            if ((vklad == null) && (sumBad >= 500))
            {
                if (10 * we > wt * 25) vklad = "Предупреждение! " + we.ToString() + "we";
                if (10 * we < wt * 25) vklad = "Предупреждение! " + wt.ToString() + "wt";
                if (10 * we == wt * 25) vklad = "Предупреждение! " + we.ToString() + "we " + wt.ToString() + "wt";
            }
            if (vklad == null) vklad = "Корректно";
          
            if (wt != 0 || we != 0 || FT != 0 || NF != 0)
            {
                int weval = we * 10, wtval = wt * 25, FTval = FT * 500, NFval = NF * 2000;
                if (weval < wtval)
                {
                    if (wtval < FTval)
                    {
                        if (FTval < NFval) mainrep = "Not Founded";
                        else if (FTval == NFval) mainrep = "Not Founded _ Fail Tvs";
                        else mainrep = "Fail Tvs";
                    }
                    else if (wtval == FTval)
                    {
                        if (wtval < NFval) mainrep = "Not Founded";
                        else if (wtval == NFval) mainrep = "Water Tvs _ Fail Tvs _ Not Founded";
                        else mainrep = "Water Tvs _ Not Founded";
                    }
                    else
                    {
                        if (wtval < NFval) mainrep = "Not Founded";
                        else if (wtval == NFval) mainrep = "Water Tvs _ Not Founded";
                        else mainrep = "Water Tvs";
                    }
                }
                else if (weval == wtval)
                {
                    if (wtval < FTval)
                    {
                        if (FTval < NFval) mainrep = "Not Founded";
                        else if (FTval == NFval) mainrep = "Not Founded _ Fail Tvs";
                        else mainrep = "Fail Tvs";
                    }
                    else if (wtval == FTval)
                    {
                        if (wtval < NFval) mainrep = "Not Founded";
                        else if (wtval == NFval) mainrep = "Water Empty _ Water Tvs _ Fail Tvs _ Not Founded";
                        else mainrep = "Water Empty _ Water Tvs _ Not Founded";
                    }
                    else
                    {
                        if (wtval < NFval) mainrep = "Not Founded";
                        else if (wtval == NFval) mainrep = "Water Empty _ Water Tvs _ Not Founded";
                        else mainrep = "Water Empty _ Water Tvs";
                    }
                }
                else //(weval > wtval)
                {
                    if (weval < FTval)
                    {
                        if (FTval < NFval) mainrep = "Not Founded";
                        else if (FTval == NFval) mainrep = "Not Founded _ Fail Tvs";
                        else mainrep = "Fail Tvs";
                    }
                    else if (weval == FTval)
                    {
                        if (weval < NFval) mainrep = "Not Founded";
                        else if (weval == NFval) mainrep = "Water Empty _ Fail Tvs _ Not Founded";
                        else mainrep = "Water Empty _ Not Founded";
                    }
                    else
                    {
                        if (weval < NFval) mainrep = "Not Founded";
                        else if (weval == NFval) mainrep = "Water Empty _ Not Founded";
                        else mainrep = "Water Empty";
                    }
                }
            }
            else mainrep = "Идеально";
            return mainrep;
        }
                


    }

    


           

    static class Program
    {

        static public List<Item> parseDirectory(string dir,BasicEnv env_dir)
        {
            List<Item> items = new List<Item>();
           
            
            string[] global_array = System.IO.Directory.GetDirectories(dir, "*");
           

            foreach (string name in global_array)
            {

                string[] array1 = System.IO.Directory.GetDirectories(name, "1B*");
                string[] array2 = System.IO.Directory.GetFiles(name, "*.tup");

                //for (int l = 0; l < array1.Length; l++) array1[l].ToLower();
                
                int n;

                if (array1.Length < array2.Length) n = array1.Length;
                else n = array2.Length;
                if (n == 0) continue;
                

                for (int i = 0; i < n; i++)
                {
                    Item bim = new Item();

                    
                    int l;
                    for (l = name.Length; name[l - 1] != '\\'; l--) ;
                    bim.data = name.Substring(l, name.Length - l);       
                                                         
                    bim.filename = System.IO.Path.GetFileName(array2[i]);
                    bim.fullFilename = array2[i];
                    
                   
                    /*****************************************************/
                    /*****************************************************/
                    /*****************************************************/
                    IMultiDataTuple azot_tuple = new DataTupleProvider(env_dir, bim.fullFilename).GetData();
                    
                    //int hour, minute;
                    DateTime date;
                    //kgoprp_info в некоторых папках может отличаться именем
                    try
                    {
                        date = ((DataParamTable)azot_tuple[i]["kgoprp_info"]).Date;
                        //hour = ((DataParamTable)azot_tuple[i]["kgoprp_info"]).Date.Hour;
                        //minute = ((DataParamTable)azot_tuple[i]["kgoprp_info"]).Date.Minute;
                    }
                    catch
                    {
                        continue;
                    }

                    
                   
                    
                    int[] h = new int[array1.Length];
                    int[] m = new int[array1.Length];
                    //массив для для хранения времени папок скалы
                    DateTime[] dtskala = new DateTime[array1.Length];
                    
                    
                    int id_0 = array1[0].LastIndexOf("_");
                    int idh0 = array1[0].LastIndexOf("ч");
                    int idm0 = array1[0].LastIndexOf("м");
                    int j=0;
                    
                    h[0] = Convert.ToInt32(array1[0].Substring(id_0 + 1, idh0 - id_0 - 1));
                    m[0] = Convert.ToInt32(array1[0].Substring(idh0 + 1, idm0 - idh0 - 1));
                    dtskala[0] = new DateTime(2009, 1, 1, h[0], m[0], 0);
                    TimeSpan dtime;
                    TimeSpan dt = new TimeSpan(0, 0, 0);
                    
                    dtime = dtskala[0].TimeOfDay-date.TimeOfDay;
                    if (dtime < dt) dtime = date.TimeOfDay - dtskala[0].TimeOfDay;
                    
                    int accur=0;
                    //определение наиболее подходящего часа измерений
                    for (j = 1; j < array1.Length; j++)
                    {
                        int id_ = array1[j].LastIndexOf("_");
                        int idh = array1[j].LastIndexOf("ч");
                        int idm = array1[j].LastIndexOf("м");

                        
                        h[j] = Convert.ToInt32(array1[j].Substring(id_ + 1, idh - id_ - 1));
                        m[j] = Convert.ToInt32(array1[j].Substring(idh + 1, idm - idh - 1));
                        dtskala[j] = new DateTime(2009, 1, 1, h[j], m[j], 0);
                        TimeSpan delta_time = dtskala[j].TimeOfDay - date.TimeOfDay;
                        if (delta_time < dt) delta_time = date.TimeOfDay-dtskala[j].TimeOfDay;
                        if (delta_time < dtime)
                        {
                            dtime = delta_time;
                           
                            accur = j;
                            
                        }
                       
                    }
                    bim.fullFolder = array1[accur];

                    for (l = array1[accur].Length; array1[accur][l - 1] != '\\'; l--) ;
                    string s = array1[accur].Substring(l, array1[accur].Length - l);
                    bim.folder = s;

                    /*****************************************************/
                    /*****************************************************/
                    /*****************************************************/
                    
                   
                    items.Add(bim);

                }
            }

            
            return items;
            
        }
       


        class MyEnv : BasicEnv
        {
            public MyEnv() : 
                base(new DataParamTable(new TupleMetaData(), new DataParamTableItem("s", 0)),
                ".", ".", new CartogramPresentationConfig(5), new CartogramPresentationConfig(5))
            {


                using (DataTupleProvider dataPvk = new DataTupleProvider(this, "pvkTuple.tup"))
                {
                    IDataTuple consts = dataPvk.GetData()[0];
                    DataArray<Coords> c = (DataArray<Coords>)consts["pvk_scheme"];

                    CoordsConverter pvk = new CoordsConverter(
                        c.Info,
                        CoordsConverter.SpecialFlag.PVK,
                        c);

                    SetPVK(pvk);
                }
            }
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 frm = new Form1();
            frm.env = new MyEnv();
            frm.Width = 1240;
            frm.Height = 616;
            frm.Left = 50;
            frm.Top = 50;
            
            Application.Run(frm);
        }
    }
}
