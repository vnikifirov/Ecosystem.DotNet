using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using SqliteStorage;
using System.Diagnostics;
//using SunBrowser;
using System.IO;
using Algorithms;
using corelib;
using RockMicoPlugin;
using System.Threading;
using Microsoft.Win32;

namespace Service_P2
{
#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
    using DataArraySensored = DataArray<Sensored>;
    using DataCartogramNativeDouble = DataCartogramNative<double>;
    using DataCartogramIndexedSensored = DataCartogramIndexed<Sensored>;
#endif

    public class RecoveryPlugin
    {
        /*DateTime dt1 = new DateTime();
        DateTime dt2 = new DateTime();
        TimeSpan ts1 = new TimeSpan();
        TimeSpan ts2 = new TimeSpan(0, 0, 20);*/
        DateTime date_previous = new DateTime();

        SrvEnv env;
        Int16[,] dp_int_cart = new short[48, 48];

        double[][] flow_az_mas_prev;//сохранение сглаженного расхода предыдущего среза

        double[,] vir_previous;
        double[][] pvk_maxes;
        double[][] pvk_maxes_prev;
        Double[,] kappa = new Double[48, 48];
        DataParamTable config;
        IMultiDataTuple kf_previous;
        MultiDataTuple sm_kf_multitup_prev;
        //колличество имеющихся данных
        int skala_count = 0;
        int azot_count = 0;

        public RecoveryPlugin()
        {
        }

        public static object GetValuee(string key)
        {
            object val = null;
            RegistryKey currRKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\sundown");
            if (currRKey != null)
            {
                val = currRKey.GetValue(key);

            }
            else val = "def";

            currRKey.Close();
            return val;
        }

       public void WorkService()
        {

            /*
            if (File.Exists("C:\\tmp\\перегрузки.txt") == true)
                File.Delete("C:\\tmp\\перегрузки.txt");
            StreamWriter sw = new StreamWriter("C:\\tmp\\перегрузки.txt", false, Encoding.GetEncoding(1251));
            sw.WriteLine("ячейка;каппа;текущая энерговыработка;");*/
            File.Delete("SqliteTestOPE.db3");

            string se = GetValuee("P2conf").ToString();
            
            string P2conf = se.Replace("\\", "\\\\");
    

            config = DataParamTable.LoadFromXML(P2conf);
            //config = DataParamTable.LoadFromXML("config.xml");
            //проверка целостности "config.xml"//
            try
            {
                config["kgoExporterUseProvider"].Value.ToString();
                config["SunEnv_DataStorageProvider"].Value.ToString();
                config["SunEnv_ImportSkalaStream"].Value.ToString();
                config["SunEnv_ImportAzotStream"].Value.ToString();
                config["Service_DataStorageProvider"].Value.ToString();
                config["SunEnv_PgDataStorageProvider"].Value.ToString();
                config["SunEnv_PgDataStorageProvider2"].Value.ToString();

            }
            catch
            {
                //MessageBox.Show("Проверьте целостность конфигурационного файла");
                return;
            }

            env = new SrvEnv(config);

            while (true)
            {
                string[] str = env.dataResource_connect.GetMultiProvider().GetStreamNames();

                DateTime[] dt_sk = env.dataResource_connect.GetMultiProvider().GetDates("rock_micro");
                DateTime[] dt_az = env.dataResource_connect.GetMultiProvider().GetDates("prp");
                //сортировка dt_sk
                bool t = true;
                while (t == true)
                {
                    t = false;
                    for (int i = 0; i < dt_sk.Length - 1; i++)
                    {
                        if (dt_sk[i] > dt_sk[i + 1])
                        {
                            DateTime temp_date = dt_sk[i];
                            dt_sk[i] = dt_sk[i + 1];
                            dt_sk[i + 1] = temp_date;
                            t = true;
                        }
                    }
                }
                //сортировка dt_az
                t = true;
                while (t == true)
                {
                    t = false;
                    for (int i = 0; i < dt_az.Length - 1; i++)
                    {
                        if (dt_az[i] > dt_az[i + 1])
                        {
                            DateTime temp_date = dt_az[i];
                            dt_az[i] = dt_az[i + 1];
                            dt_az[i + 1] = temp_date;
                            t = true;
                        }
                    }
                }
                //отыскание последних занесенных данных по азоту
                //и соотнесенных с ними данными скалы

                if (dt_az.Length > azot_count)//если поступила новая прописка
                {
                    //для работы в сети обмена данных
                    //необходимо откомментировать строку в конце главного цикла while

                    /*TimeSpan nul = new TimeSpan(0, 0, 0);
                    TimeSpan comp = dt_az[azot_count] - dt_sk[skala_count];
                    DateTime min_time = dt_sk[skala_count];

                    for (int i = 1; i < dt_sk.Length - skala_count; i++)
                    {
                        TimeSpan deltaT = dt_az[azot_count] - dt_sk[skala_count + i];
                        if ((deltaT < comp) && (deltaT > nul))
                        {
                            comp = deltaT;
                            min_time = dt_sk[skala_count + i];
                        }
                        else if ((deltaT < nul) && (-deltaT < comp))
                        {
                            comp = -deltaT;
                            min_time = dt_sk[skala_count + i];
                        }
                    }*/
                    //для анализа базы данных
                    TimeSpan nul = new TimeSpan(0, 0, 0);
                    TimeSpan comp = dt_az[azot_count] - dt_sk[0];
                    if (comp < nul)
                        comp = -comp;
                    DateTime min_time = dt_sk[0];

                    for (int i = 1; i < dt_sk.Length; i++)
                    {
                        TimeSpan deltaT = dt_az[azot_count] - dt_sk[i];
                        if (deltaT < nul)
                            deltaT = -deltaT;
                        if (deltaT < comp)
                        {
                            comp = deltaT;
                            min_time = dt_sk[i];
                        }
                    }
                    //
                    IMultiDataTuple mdt_sk = env.dataResource_connect.GetMultiProvider().GetData(min_time, "rock_micro");
                    IMultiDataTuple mdt_az = env.dataResource_connect.GetMultiProvider().GetData(dt_az[azot_count], "prp");
                    /*************************************/

                    //получение картограммы загрузки и остальных параметров,
                    //необходимых для расчета максимумов активности
                    DataCartogram zagr_rockmicro = (DataCartogram)mdt_sk[0]["zagr_rockmicro"];
                    double[,] zagrN = zagr_rockmicro.ScaleNativeArray(0);
                    RockMicroAlgo.zagrConvertToInternal(zagrN);
                    DataCartogramNative<Double> c =
                        new DataCartogramNative<Double>(zagr_rockmicro.Info, ScaleIndex.Default, zagrN);
                    double[,] zagr = c.ScaleToIndexed2(env.PVK, 0);

                    int[] fb = new int[mdt_az.Count];
                    double[][] azt = new double[mdt_az.Count][];
                    double[] badness;

                    string[] report;

                    for (int i = 0; i < mdt_az.Count; i++)
                    {
                        int a = ((DataParamTable)mdt_az[i]["kgoprp_info"])["fiberNum"].ValueInt;
                        fb[i] = a;
                        ((DataArray)mdt_az[i]["kgoprp_azot"]).Normalize().ConvertToArray(out azt[i]);
                    }
                    KgoDetectAlgo.kgoDetectMaxPos(fb, azt, zagr, out  badness, out pvk_maxes, out report);

                    DataArray[] maxs = new DataArray[mdt_az["kgoprp_azot"].Count];
                    TupleMetaData info = new TupleMetaData("pvk_maxes_cart", "pvk_maxes_cart",
                        mdt_sk.GetTimeDate(), TupleMetaData.StreamAuto);
                    for (int i = 0; i < mdt_az["kgoprp_azot"].Count; i++)
                    {
                        maxs[i] = new DataArray<double>(info, pvk_maxes[i]);
                    }
                    if (pvk_maxes_prev == null)
                        pvk_maxes_prev = pvk_maxes;
                    IMultiTupleItem mi = new MultiTupleItem(maxs);

                    DataCartogram coeffDP = DataCartogram.CreateFromParts(info,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, info), mi);

                    string azotStream = "azot";
                    string mainStream = "main";

                    DataTuple dataSkala = new DataTuple((IDataTuple)mdt_sk, mainStream);

                    DataTuple azotComact = new DataTuple(azotStream, dataSkala.GetTimeDate(), coeffDP);

                    //связка скалы и азота
                    string[] names = new string[dataSkala.ItemsCount + azotComact.ItemsCount];
                    dataSkala.CopyNamesTo(names);
                    azotComact.CopyNamesTo(names, dataSkala.ItemsCount);
                    DataTuple combined = DataTuple.Combine(names, dataSkala, azotComact);
                    dataSkala = new DataTuple(combined, mainStream, dataSkala.GetTimeDate());

                    //создание зрк...особый путь к файлу зрк, небходимо проверять
                    /*
                    string sday = null;
                    if (azotComact.GetTimeDate().Day < 10)
                        sday = "0" + azotComact.GetTimeDate().Day.ToString();
                    else
                        sday = azotComact.GetTimeDate().Day.ToString();
                    string zrk_path = "C:\\tmp\\Тест2\\1_" + sday + "0609.txt";
                    OvlInfoProvider zrk = new OvlInfoProvider(zrk_path);
                    IDataTuple tn = (IDataTuple)(zrk.GetData());
                    ITupleItem i_zrk = tn[0];

                    i_zrk = i_zrk.Rename(new TupleMetaData(i_zrk.Name, i_zrk.HumaneName, dataSkala.GetTimeDate(), mainStream));
                    tn = new DataTuple(mainStream, dataSkala.GetTimeDate(), i_zrk);
                    string[] names_zrk = new string[dataSkala.ItemsCount + tn.ItemsCount];
                    dataSkala.CopyNamesTo(names_zrk);
                    tn.CopyNamesTo(names_zrk, dataSkala.ItemsCount);
                    combined = DataTuple.Combine(names_zrk, dataSkala, tn);

                    dataSkala = new DataTuple(combined, mainStream, dataSkala.GetTimeDate());
                    */

                    //набор параметров, необходимых для создания DataTuple для глубины
                    TupleMetaData tmd = new TupleMetaData("depth", "Глубина",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);

                    DataCartogram suz_rockmicro = (DataCartogram)mdt_sk[0]["suz"];
                    double[,] suz = suz_rockmicro.ScaleNativeArray(-1);
                    DataCartogram energovir_rockmicro = (DataCartogram)mdt_sk[0]["energovir"];
                    double[,] vir = energovir_rockmicro.ScaleNativeArray(-1);
                    DataCartogram power_rockmicro = (DataCartogram)mdt_sk[0]["power"];
                    double[,] power = power_rockmicro.ScaleNativeArray(-1);
                    double dtime;//разность времен срезов данных
                    if (vir_previous == null)//если данные поступают впервые
                    {
                        for (int n = 0; n < 48; n++)
                        {
                            for (int m = 0; m < 48; m++)
                            {
                                if (vir[n, m] != -1)
                                {
                                    dp_int_cart[n, m] = 0;//картограмма глубины
                                    kappa[n, m] = 0;

                                }
                                else
                                {
                                    dp_int_cart[n, m] = -1;
                                    kappa[n, m] = -1;
                                }
                            }
                        }
                        vir_previous = vir;//сохраняем текущую картограмму енерговыработки
                    }
                    else
                    {
                        TimeSpan ts = dataSkala.GetTimeDate() - date_previous;
                        //разность во временных срезах для определения перегрузок будем считать в днях
                        dtime = ts.Days + Convert.ToDouble(ts.Hours) / 24 + Convert.ToDouble(ts.Minutes) / (60 * 24);
                        dtime = Math.Round(dtime, 3);

                        for (int n = 0; n < 48; n++)
                        {
                            for (int m = 0; m < 48; m++)
                            {
                                if (vir[n, m] != -1)
                                {
                                    double vir1 = vir[n, m];
                                    double vir2 = vir_previous[n, m];
                                    double pw = power[n, m];

                                    if (vir1 == 0.0 || vir2 == 0.0 || pw == 0.0)
                                    {
                                        if (suz[n, m] != -1)
                                            kappa[n, m] = 5;//значение каппы для каналов суз
                                        else
                                            kappa[n, m] = 8;//ограничение выбросов(для наглядности построение гистограммы)

                                    }
                                    else
                                    {
                                        kappa[n, m] = Math.Abs(vir2 - vir1) * (1 / pw) * (1 / dtime);//формула для определения перегрузок
                                        if (kappa[n, m] > 5)
                                            kappa[n, m] = 10;//ограничение выбросов(для наглядности построение гистограммы)
                                        if (suz[n, m] != -1)
                                            kappa[n, m] = 5;//значение каппы для каналов суз
                                    }
                                    if ((kappa[n, m] == 5) || (kappa[n, m] > 0.5) && (kappa[n, m] < 1.5))
                                    {
                                        dp_int_cart[n, m] += 1;
                                    }
                                    else
                                    {
                                        dp_int_cart[n, m] = 0;
                                    }
                                }
                            }
                        }
                        vir_previous = vir;
                    }

                    DataCartogramNative<Int16> dp_dc =
                       new DataCartogramNative<Int16>(tmd, ScaleIndex.Default, dp_int_cart);

                    DateTime dtcart = dp_dc.GetTimeDate();
                    //создание DataTuple
                    DataTuple datadeep = new DataTuple(TupleMetaData.StreamAuto, dataSkala.GetTimeDate(), dp_dc);
                    string[] names2 = new string[dataSkala.ItemsCount + datadeep.ItemsCount];
                    dataSkala.CopyNamesTo(names2);
                    datadeep.CopyNamesTo(names2, dataSkala.ItemsCount);
                    combined = DataTuple.Combine(names2, dataSkala, datadeep);
                    dataSkala = new DataTuple(combined, mainStream, dataSkala.GetTimeDate());

                    //связка для каппы
                    TupleMetaData tmd_kappa = new TupleMetaData("kappa", "Каппа",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    DataCartogramNative<Double> kappa_cart =
                       new DataCartogramNative<Double>(tmd_kappa, ScaleIndex.Default, kappa);
                    DataTuple kappa_tuple = new DataTuple(TupleMetaData.StreamAuto, dataSkala.GetTimeDate(), kappa_cart);
                    string[] names3 = new string[dataSkala.ItemsCount + kappa_tuple.ItemsCount];
                    dataSkala.CopyNamesTo(names3);
                    kappa_tuple.CopyNamesTo(names3, dataSkala.ItemsCount);
                    combined = DataTuple.Combine(names3, dataSkala, kappa_tuple);
                    dataSkala = new DataTuple(combined, mainStream, dataSkala.GetTimeDate());


                    env.dataResource_input.GetMultiProvider().PushData(dataSkala);

                    //данные длин пароводяных коммуникаций из файла
                    ParseLenPvk plpvk = new ParseLenPvk("KNPP1.csv");
                    DataCartogramNativeDouble pvk_dc = plpvk.PvkLength;

                    //рассчет коэффициентов
                    IMultiDataProvider mdp = env.dataResource_input.GetMultiProvider();

                    IMultiDataTuple kf = ComputeAdaptationCoefficient(mdp, dataSkala.GetTimeDate(), mdt_az, mdt_sk, pvk_dc);
                    //занесение в базу коэффициентов
                    //адаплатции по азоту "bet1"
                    //адаплатции по давлению "kpd"
                    //нормировки активности "gamma"
                    DataArray[] bet1_array = new DataArray[kf.Count];
                    DataArray[] kpd_array = new DataArray[kf.Count];
                    DataParamTable[] gamma_dpt = new DataParamTable[kf.Count];
                    TupleMetaData bet1_info = new TupleMetaData("kf_bet1", "Коэффициент адаптации по азоту",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    TupleMetaData kpd_info = new TupleMetaData("kf_kpd", "Коэффициент адаптации по давлению",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    TupleMetaData gamma_info = new TupleMetaData("kf_gamma", "Гамма",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    for (int i = 0; i < kf.Count; i++)
                    {
                        bet1_array[i] = (DataArray)kf[i]["bet1"];
                        kpd_array[i] = (DataArray)kf[i]["kpd"];
                        gamma_dpt[i] = (DataParamTable)kf[i]["gamma"];
                    }
                    //начало для занесения гаммы
                    //формируется особенно, т.к. значение гаммы для всех каналов в одной нитке одинаково
                    CoordsConverter cc_gamma = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, gamma_info);
                    DataArray[] gamma_array = new DataArray[kf.Count];
                    double[][] gamma_matrix = new double[16][];
                    for (int i = 0; i < kf.Count; i++)
                    {
                        gamma_matrix[i] = new double[115];
                    }
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc_kf = cc_gamma.GetFiberCoords(new Coords(i, j));
                            if (fc_kf.Fiber != -1)
                            {
                                gamma_matrix[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(gamma_dpt[fc_kf.Fiber]["gamma"]);
                            }
                        }
                    }
                    for (int i = 0; i < kf.Count; i++)
                    {
                        gamma_array[i] = DataArray.Create(gamma_info, gamma_matrix[i]);
                    }
                    //конец гаммы
                    IMultiTupleItem bet1_mti = new MultiTupleItem(bet1_array);
                    IMultiTupleItem kpd_mti = new MultiTupleItem(kpd_array);
                    IMultiTupleItem gamma_mti = new MultiTupleItem(gamma_array);
                    DataCartogram bet1_coeffDP = DataCartogram.CreateFromParts(bet1_info,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, bet1_info), bet1_mti);
                    DataCartogram kpd_coeffDP = DataCartogram.CreateFromParts(kpd_info,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, kpd_info), kpd_mti);
                    DataCartogram gamma_coeffDP = DataCartogram.CreateFromParts(gamma_info,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, gamma_info), gamma_mti);
                    DataTuple bet1_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), bet1_coeffDP);
                    DataTuple kpd_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), kpd_coeffDP);
                    DataTuple gamma_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), gamma_coeffDP);

                    env.dataResource_input.GetMultiProvider().PushData(bet1_tuple);
                    env.dataResource_input.GetMultiProvider().PushData(kpd_tuple);
                    env.dataResource_input.GetMultiProvider().PushData(gamma_tuple);

                    if (kf_previous == null)
                    {
                        kf_previous = kf;
                        sm_kf_multitup_prev = (MultiDataTuple)kf;
                    }

                    //сглаживание коэффициентов
                    double alpha = 0.2;
                    DataArray[] sm_bet1_array = new DataArray[kf.Count];
                    DataArray[] sm_kpd_array = new DataArray[kf.Count];
                    DataParamTable[] sm_gamma_table = new DataParamTable[kf.Count];
                    ITupleItem[] sm_kf_iti = new ITupleItem[3];//массив для формирования datatuple

                    TupleMetaData sm_bet1_info = new TupleMetaData("bet1", "Коэффициент адаптации по азоту(сглаженный)",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    TupleMetaData sm_kpd_info = new TupleMetaData("kpd", "Коэффициент адаптации по давлению(сглаженный)",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    TupleMetaData sm_gamma_info = new TupleMetaData("gamma", "Cглаженный гамма",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    DataTuple[] sm_kf_tups = new DataTuple[kf.Count];//массив для создания multidatatuple
                    //сглаженных коэффициентов


                    /*******************Сброс коэффициентов********************/
                    DataArray[] bet1_dar = new DataArray[kf.Count];
                    DataArray[] kpd_dar = new DataArray[kf.Count];
                    DataParamTable[] gamma_data = new DataParamTable[kf.Count];
                    DataArray[] sm_bet1_prev_dar = new DataArray[kf.Count];
                    DataArray[] sm_kpd_prev_dar = new DataArray[kf.Count];
                    DataParamTable[] sm_gamma_prev_data = new DataParamTable[kf.Count];
                    double[][] sm_bet1_mas = new double[kf.Count][];
                    double[][] sm_kpd_mas = new double[kf.Count][];
                    double[] sm_gamma = new double[kf.Count];
                    DataParamTableItem[] sm_gamma_item = new DataParamTableItem[kf.Count];
                    DataParamTableItem[] sm_fiber_item = new DataParamTableItem[kf.Count];
                    /*****/
                    //для отслеживание скачков коэффициентов
                    DataArray[] kpd_prev_dar = new DataArray[kf.Count];
                    DataArray[] bet1_prev_dar = new DataArray[kf.Count];
                    DataParamTable[] gamma_prev_data = new DataParamTable[kf.Count];
                    for (int i = 0; i < kf.Count; i++)
                    {
                        kpd_prev_dar[i] = (DataArray)kf_previous[i]["kpd"];
                        bet1_prev_dar[i] = (DataArray)kf_previous[i]["bet1"];
                        gamma_prev_data[i] = (DataParamTable)kf_previous[i]["gamma"];
                    }
                    /*****/
                    for (int i = 0; i < kf.Count; i++)
                    {
                        bet1_dar[i] = (DataArray)kf[i]["bet1"];
                        kpd_dar[i] = (DataArray)kf[i]["kpd"];
                        gamma_data[i] = (DataParamTable)kf[i]["gamma"];
                        sm_bet1_prev_dar[i] = (DataArray)sm_kf_multitup_prev[i]["bet1"];
                        sm_kpd_prev_dar[i] = (DataArray)sm_kf_multitup_prev[i]["kpd"];
                        sm_gamma_prev_data[i] = (DataParamTable)sm_kf_multitup_prev[i]["gamma"];
                        sm_bet1_mas[i] = new double[bet1_dar[i].Length];
                        sm_kpd_mas[i] = new double[kpd_dar[i].Length];
                        sm_gamma[i] = (1 - alpha) * Convert.ToDouble(sm_gamma_prev_data[i]["gamma"]) + alpha * Convert.ToDouble(gamma_data[i]["gamma"]);

                        for (int k = 0; k < bet1_dar[i].Length; k++)
                        {
                            sm_bet1_mas[i][k] = (1 - alpha) * Convert.ToDouble(sm_bet1_prev_dar[i][k]) + alpha * Convert.ToDouble(bet1_dar[i][k]);
                            sm_kpd_mas[i][k] = (1 - alpha) * Convert.ToDouble(sm_kpd_prev_dar[i][k]) + alpha * Convert.ToDouble(kpd_dar[i][k]);

                        }
                    }
                    CoordsConverter cc_kf = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, bet1_info);
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc_kf = cc_kf.GetFiberCoords(new Coords(i, j));
                            if (fc_kf.Fiber != -1)
                            {
                                bool a0 = false;
                                bool b = false;
                                bool d = false;
                                bool e = false;
                                bool g = false;
                                bool f = false;
                                if (azot_count > 0)
                                    a0 = true;
                                if (dp_int_cart[i, j] == 0)
                                    b = true;
                                if (Math.Abs(pvk_maxes_prev[fc_kf.Fiber][fc_kf.Pvk] - pvk_maxes[fc_kf.Fiber][fc_kf.Pvk]) > 0.3 * Math.Abs(pvk_maxes_prev[fc_kf.Fiber][fc_kf.Pvk]))
                                    d = true;
                                if (Math.Abs(Convert.ToDouble(kpd_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) - Convert.ToDouble(kpd_dar[fc_kf.Fiber][fc_kf.Pvk])) > 0.3 * Math.Abs(Convert.ToDouble(kpd_prev_dar[fc_kf.Fiber][fc_kf.Pvk])))
                                    e = true;
                                if (Math.Abs(Convert.ToDouble(gamma_prev_data[fc_kf.Fiber]["gamma"]) - Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"])) > 0.2 * Math.Abs(Convert.ToDouble(gamma_prev_data[fc_kf.Fiber]["gamma"])))
                                    g = true;
                                if (Math.Abs(Convert.ToDouble(bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) - Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk])) > 0.3 * Math.Abs(Convert.ToDouble(bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk])))
                                    f = true;

                                if (dt_az.Length == 2)//для рассмотрения второго среза
                                {                   //(когда коэффициенты скачут сразу после первого среза)
                                    if (a0 && b)
                                    {
                                        sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                        sm_kpd_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(kpd_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                        sm_gamma[fc_kf.Fiber] = Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"]);
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            if (pvk_maxes_prev[fc_kf.Fiber][fc_kf.Pvk] < pvk_maxes[fc_kf.Fiber][fc_kf.Pvk])
                                            {
                                                sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                sm_gamma[fc_kf.Fiber] = Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"]);
                                            }
                                            else
                                            {
                                                sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                sm_gamma[fc_kf.Fiber] = Convert.ToDouble(sm_gamma_prev_data[fc_kf.Fiber]["gamma"]);
                                            }
                                        }
                                        else
                                        {
                                            if (g)
                                                sm_gamma[fc_kf.Fiber] = Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"]);
                                            if (f)
                                            {
                                                if (Convert.ToDouble(bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) < Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]))
                                                    sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                else
                                                    sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                            }
                                        }
                                        if (e)
                                        {
                                            if (Convert.ToDouble(kpd_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) < Convert.ToDouble(kpd_dar[fc_kf.Fiber][fc_kf.Pvk]))
                                                sm_kpd_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(kpd_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                            else
                                                sm_kpd_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_kpd_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                        }

                                    }
                                }
                                else
                                {
                                    if (a0 && b)
                                    {
                                        sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                        sm_kpd_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(kpd_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                        sm_gamma[fc_kf.Fiber] = Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"]);
                                    }
                                    else
                                    {
                                        if (d || (Convert.ToInt32(pvk_maxes[fc_kf.Fiber][fc_kf.Pvk]) == 0))
                                        {
                                            if (d && (pvk_maxes_prev[fc_kf.Fiber][fc_kf.Pvk] == 0))
                                            {
                                                sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                sm_gamma[fc_kf.Fiber] = Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"]);
                                            }
                                            else
                                            {
                                                sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                sm_gamma[fc_kf.Fiber] = Convert.ToDouble(sm_gamma_prev_data[fc_kf.Fiber]["gamma"]);
                                            }
                                        }
                                        else
                                        {
                                            if (g)
                                                sm_gamma[fc_kf.Fiber] = Convert.ToDouble(gamma_data[fc_kf.Fiber]["gamma"]);
                                            if (f)
                                            {
                                                sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                if (Convert.ToDouble(bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) < Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]))
                                                {
                                                    if (Convert.ToDouble(bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) == 0.0)
                                                        sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                                }
                                            }
                                            if (Convert.ToDouble(bet1_dar[fc_kf.Fiber][fc_kf.Pvk]) == 0.0)
                                            {
                                                if (Convert.ToDouble(bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]) == 0.0)
                                                    sm_bet1_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_bet1_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);
                                            }
                                        }
                                        if (e)
                                            sm_kpd_mas[fc_kf.Fiber][fc_kf.Pvk] = Convert.ToDouble(sm_kpd_prev_dar[fc_kf.Fiber][fc_kf.Pvk]);

                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < kf.Count; i++)
                    {
                        sm_gamma_item[i] = new DataParamTableItem("gamma", sm_gamma[i]);
                        sm_fiber_item[i] = new DataParamTableItem("fibernum", i.ToString());

                        sm_bet1_array[i] = DataArray.Create(sm_bet1_info, sm_bet1_mas[i]);
                        sm_kpd_array[i] = DataArray.Create(sm_kpd_info, sm_kpd_mas[i]);
                        sm_gamma_table[i] = new DataParamTable(sm_gamma_info, sm_gamma_item[i], sm_fiber_item[i]);
                        sm_kf_iti[0] = (ITupleItem)sm_bet1_array[i];
                        sm_kf_iti[1] = (ITupleItem)sm_kpd_array[i];
                        sm_kf_iti[2] = (ITupleItem)sm_gamma_table[i];

                        sm_kf_tups[i] = new DataTuple(mainStream, dataSkala.GetTimeDate(), sm_kf_iti);
                    }
                    /****************************************************/

                    //начало для занесения сглаженной гаммы
                    //формирование картограммы
                    DataArray[] sm_gamma_array = new DataArray[kf.Count];
                    double[][] sm_gamma_matrix = new double[16][];
                    for (int i = 0; i < kf.Count; i++)
                    {
                        sm_gamma_matrix[i] = new double[115];
                    }
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc_kf = cc_gamma.GetFiberCoords(new Coords(i, j));
                            if (fc_kf.Fiber != -1)
                            {
                                sm_gamma_matrix[fc_kf.Fiber][fc_kf.Pvk] = sm_gamma[fc_kf.Fiber];
                            }
                        }
                    }
                    for (int i = 0; i < kf.Count; i++)
                    {
                        sm_gamma_array[i] = DataArray.Create(sm_gamma_info, sm_gamma_matrix[i]);
                    }
                    //конец гаммы

                    IMultiTupleItem sm_bet1_mti = new MultiTupleItem(sm_bet1_array);
                    IMultiTupleItem sm_kpd_mti = new MultiTupleItem(sm_kpd_array);
                    IMultiTupleItem sm_gamma_mti = new MultiTupleItem(sm_gamma_array);
                    DataCartogram sm_bet1_coeffDP = DataCartogram.CreateFromParts(sm_bet1_info,
                            env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, sm_bet1_info), sm_bet1_mti);
                    DataCartogram sm_kpd_coeffDP = DataCartogram.CreateFromParts(sm_kpd_info,
                            env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, sm_kpd_info), sm_kpd_mti);
                    DataCartogram sm_gamma_coeffDP = DataCartogram.CreateFromParts(sm_gamma_info,
                            env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, sm_gamma_info), sm_gamma_mti);
                    DataTuple sm_bet1_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), sm_bet1_coeffDP);
                    DataTuple sm_kpd_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), sm_kpd_coeffDP);
                    DataTuple sm_gamma_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), sm_gamma_coeffDP);


                    MultiDataTuple sm_kf_multitup = new MultiDataTuple(sm_kf_tups);

                    //если закоментить этот if ничего не изменится
                    if (sm_kf_multitup_prev == (MultiDataTuple)kf)
                    {
                        sm_kf_multitup_prev = sm_kf_multitup;
                    }

                    env.dataResource_input.GetMultiProvider().PushData(sm_bet1_tuple);
                    env.dataResource_input.GetMultiProvider().PushData(sm_kpd_tuple);
                    env.dataResource_input.GetMultiProvider().PushData(sm_gamma_tuple);

                    IMultiDataTuple recovered = RecoveryData(mdp, dataSkala.GetTimeDate(), mdt_sk, sm_kf_multitup_prev, pvk_dc);
                    IMultiDataTuple recovered2 = RecoveryData(mdp, dataSkala.GetTimeDate(), mdt_sk, (MultiDataTuple)kf_previous, pvk_dc);

                    //занесение восстановленного расхода "recovered" в базу
                    DataArray[] flow_az_array = new DataArray[recovered.Count];
                    DataArray[] flow_dp_array = new DataArray[recovered.Count];

                    DataArray[] flow_az_array2 = new DataArray[recovered.Count];
                    DataArray[] flow_dp_array2 = new DataArray[recovered.Count];

                    TupleMetaData flow_az_info = new TupleMetaData("flow_az_sm", "Восстановленный расход по азоту(сглаженный)",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    TupleMetaData flow_dp_info = new TupleMetaData("flow_dp_sm", "Восстановленный расход по давлению(сглаженный)",
                       dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);

                    TupleMetaData flow_az_info2 = new TupleMetaData("flow_az", "Восстановленный расход по азоту",
                        dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                    TupleMetaData flow_dp_info2 = new TupleMetaData("flow_dp", "Восстановленный расход по давлению",
                       dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);

                    CoordsConverter cc = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_az_info);

                    DataCartogram flow_skala = (DataCartogram)dataSkala["flow"];
                    double[,] flow_skala_matrix = flow_skala.ScaleNativeArray(-1);

                    double[][] flow_az_mas = new double[16][];
                    double[][] flow_dp_mas = new double[16][];
                    double[][] flow_az_mas2 = new double[16][];
                    double[][] flow_dp_mas2 = new double[16][];
                    for (int i = 0; i < recovered.Count; i++)
                    {
                        flow_az_mas[i] = new double[115];
                        flow_dp_mas[i] = new double[115];
                        flow_az_mas2[i] = new double[115];
                        flow_dp_mas2[i] = new double[115];
                        for (int j = 0; j < 115; j++)
                        {
                            flow_az_mas[i][j] = Convert.ToDouble(((DataArray)recovered[i]["flow_az1"])[j]);
                            flow_dp_mas[i][j] = Convert.ToDouble(((DataArray)recovered[i]["flow_dp"])[j]);
                            flow_az_mas2[i][j] = Convert.ToDouble(((DataArray)recovered2[i]["flow_az1"])[j]);
                            flow_dp_mas2[i][j] = Convert.ToDouble(((DataArray)recovered2[i]["flow_dp"])[j]);
                        }
                    }
                    if (flow_az_mas_prev == null)
                        flow_az_mas_prev = flow_az_mas;
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc = cc.GetFiberCoords(new Coords(i, j));
                            if (fc.Fiber != -1)
                            {
                                bool d = false;
                                bool f = false;
                                if (Math.Abs(pvk_maxes_prev[fc.Fiber][fc.Pvk] - pvk_maxes[fc.Fiber][fc.Pvk]) > 0.3 * Math.Abs(pvk_maxes_prev[fc.Fiber][fc.Pvk]))
                                    d = true;//скачок pvk_maxes
                                if (Math.Abs(Convert.ToDouble(bet1_prev_dar[fc.Fiber][fc.Pvk]) - Convert.ToDouble(bet1_dar[fc.Fiber][fc.Pvk])) > 0.3 * Math.Abs(Convert.ToDouble(bet1_prev_dar[fc.Fiber][fc.Pvk])))
                                    f = true;//скачок азотного коэффициента
                                if (dp_int_cart[i, j] == 0)
                                {
                                    flow_az_mas[fc.Fiber][fc.Pvk] = -1;
                                    flow_dp_mas[fc.Fiber][fc.Pvk] = -1;
                                    flow_az_mas2[fc.Fiber][fc.Pvk] = -1;
                                    flow_dp_mas2[fc.Fiber][fc.Pvk] = -1;
                                }
                                else
                                {
                                    if (dt_az.Length == 2)
                                    {
                                        if (d && (pvk_maxes_prev[fc.Fiber][fc.Pvk] > pvk_maxes[fc.Fiber][fc.Pvk]))
                                            flow_az_mas[fc.Fiber][fc.Pvk] = flow_az_mas_prev[fc.Fiber][fc.Pvk];
                                        /*else if (f && (Convert.ToDouble(bet1_prev_dar[fc.Fiber][fc.Pvk]) > Convert.ToDouble(bet1_dar[fc.Fiber][fc.Pvk])))
                                            flow_az_mas[fc.Fiber][fc.Pvk] = flow_az_mas_prev[fc.Fiber][fc.Pvk];*/
                                    }
                                    else
                                    {
                                        if (((Convert.ToInt32(pvk_maxes[fc.Fiber][fc.Pvk]) == 0) || d || f) && flow_skala_matrix[i, j] != 0.0)
                                            flow_az_mas[fc.Fiber][fc.Pvk] = flow_az_mas_prev[fc.Fiber][fc.Pvk];
                                    }
                                }

                            }
                        }
                    }
                    for (int i = 0; i < recovered.Count; i++)
                    {
                        flow_az_array[i] = DataArray.Create(flow_az_info, flow_az_mas[i]);
                        flow_dp_array[i] = DataArray.Create(flow_dp_info, flow_dp_mas[i]);
                    }
                    for (int i = 0; i < recovered.Count; i++)
                    {
                        flow_az_array2[i] = DataArray.Create(flow_az_info2, flow_az_mas2[i]);
                        flow_dp_array2[i] = DataArray.Create(flow_dp_info2, flow_dp_mas2[i]);
                    }
                    /***********************************************************************/
#if NOT_DEF
                    /****************отладка(проверка соответствия координат)***************/
                    if (azot_count == 1)
                    {
                        File.Delete("C:\\tmp\\coords.log");
                        StreamWriter sw = new StreamWriter(File.Open("C:\\tmp\\coords.log",
                                  System.IO.FileMode.Append));
                        CoordsConverter cc1 = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_az_info);
                        CoordsConverter cc2 = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_dp_info);
                        int f = -1;
                        for (int i = 0; i < 48; i++)
                        {
                            for (int j = 0; j < 48; j++)
                            {
                                FiberCoords fc1 = cc1.GetFiberCoords(new Coords(i, j));
                                FiberCoords fc2 = cc2.GetFiberCoords(new Coords(i, j));
                                sw.Write(fc1.Fiber);
                                sw.Write("-");
                                sw.Write(fc1.Pvk);
                                sw.Write(" | ");
                                sw.Write(fc2.Fiber);
                                sw.Write("-");
                                sw.Write(fc2.Pvk);
                                sw.Write(" | ");
                                sw.Write(i);
                                sw.Write("-");
                                sw.Write(j);
                                sw.Write(" | ");
                                sw.Write(dp_int_cart[i, j]);
                                sw.Write(" | ");
                                if (fc1.Fiber == -1)
                                    sw.Write(-1);
                                else
                                    sw.Write(flow_az_mas[fc1.Fiber][fc1.Pvk]);
                                sw.Write(" | ");
                                if (fc2.Fiber == -1)
                                    sw.Write(-1);
                                else
                                    sw.Write(flow_dp_mas[fc2.Fiber][fc2.Pvk]);
                                sw.WriteLine();
                            }
                        }
                        sw.Close();
                    }
                    /***********************************************************************/
#endif
                    /***********************************************************************/
                    IMultiTupleItem flow_az_mti = new MultiTupleItem(flow_az_array);
                    IMultiTupleItem flow_dp_mti = new MultiTupleItem(flow_dp_array);
                    DataCartogram flow_az_coeffDP = DataCartogram.CreateFromParts(flow_az_info,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_az_info), flow_az_mti);
                    DataCartogram flow_dp_coeffDP = DataCartogram.CreateFromParts(flow_dp_info,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_dp_info), flow_dp_mti);
                    DataTuple flow_az_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), flow_az_coeffDP);
                    DataTuple flow_dp_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), flow_dp_coeffDP);


                    IMultiTupleItem flow_az_mti2 = new MultiTupleItem(flow_az_array2);
                    IMultiTupleItem flow_dp_mti2 = new MultiTupleItem(flow_dp_array2);
                    DataCartogram flow_az_coeffDP2 = DataCartogram.CreateFromParts(flow_az_info2,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_az_info2), flow_az_mti2);
                    DataCartogram flow_dp_coeffDP2 = DataCartogram.CreateFromParts(flow_dp_info2,
                        env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, flow_dp_info2), flow_dp_mti2);
                    DataTuple flow_az_tuple2 = new DataTuple(mainStream, dataSkala.GetTimeDate(), flow_az_coeffDP2);
                    DataTuple flow_dp_tuple2 = new DataTuple(mainStream, dataSkala.GetTimeDate(), flow_dp_coeffDP2);


                    env.dataResource_input.GetMultiProvider().PushData(flow_az_tuple);
                    env.dataResource_input.GetMultiProvider().PushData(flow_dp_tuple);

                    env.dataResource_input.GetMultiProvider().PushData(flow_az_tuple2);
                    env.dataResource_input.GetMultiProvider().PushData(flow_dp_tuple2);

                    sm_kf_multitup_prev = sm_kf_multitup;
                    kf_previous = kf;
                    /**************************/
                    //Создание картограммы восстановленных запрещеннных ячеек

                    DataCartogram flow_cart = (DataCartogram)dataSkala["flow"];
                    double[,] flow_matrix = flow_cart.ScaleNativeArray(-1);
                    double[,] sm_flow_matrix = flow_az_coeffDP.ScaleNativeArray(-1);
                    int num_coords = 0;
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc = cc.GetFiberCoords(new Coords(i, j));
                            if (fc.Fiber != -1)
                            {
                                num_coords++;
                            }
                        }
                    }
                    Coords[] wrong_coords = new Coords[num_coords];
                    double[] recovered_flow = new double[num_coords];
                    int key = 0;//колличество восстановленных запрещенных ячеек

                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc = cc.GetFiberCoords(new Coords(i, j));
                            if (fc.Fiber != -1)
                            {
                                if ((flow_matrix[i, j] == 0.0) && (sm_flow_matrix[i, j] != 0.0) && (sm_flow_matrix[i, j] != -1))
                                {
                                    wrong_coords[key] = new Coords(i, j);
                                    recovered_flow[key] = sm_flow_matrix[i, j];
                                    key++;
                                }
                            }
                        }
                    }

                    if (key > 0)
                    {
                        Coords[] crds = new Coords[key];
                        double[] flow_crds = new double[key];
                        for (int m = 0; m < key; m++)
                        {
                            crds[m] = wrong_coords[m];
                            flow_crds[m] = recovered_flow[m];
                        }
                        TupleMetaData illegal_data = new TupleMetaData("flow_ill_coords", "Расход в запрещенных ячейках",
                           dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                        CoordsConverter cc_illegal = new CoordsConverter(illegal_data, CoordsConverter.SpecialFlag.Named, crds);

                        //DataArrayCoords array_crds = new DataArray<Coords>(illegal_data, crds);
                        DataCartogramIndexed<double> ill_cart =
                            new DataCartogramIndexed<double>(illegal_data, ScaleIndex.Default, cc_illegal, flow_crds);

                        DataTuple flow_az_illegal_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), ill_cart);
                        env.dataResource_input.GetMultiProvider().PushData(flow_az_illegal_tuple);
                    }
                    else
                    {
                        Coords[] crds = new Coords[0];
                        double[] flow_crds = new double[0];
                        TupleMetaData illegal_data = new TupleMetaData("flow_ill_coords", "Расход в запрещенных ячейках",
                           dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                        CoordsConverter cc_illegal = new CoordsConverter(illegal_data, CoordsConverter.SpecialFlag.Named, crds);

                        //DataArrayCoords array_crds = new DataArray<Coords>(illegal_data, crds);
                        DataCartogramIndexed<double> ill_cart =
                            new DataCartogramIndexed<double>(illegal_data, ScaleIndex.Default, cc_illegal, flow_crds);

                        DataTuple flow_az_illegal_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), ill_cart);
                        env.dataResource_input.GetMultiProvider().PushData(flow_az_illegal_tuple);
                    }

                    //Создание картограммы ячеек, где произошли резкие изменения расхода
                    Coords[] change_coords = new Coords[num_coords];
                    double[] change_flow = new double[num_coords];
                    key = 0;//колличество восстановленных запрещенных ячеек
                    for (int i = 0; i < 48; i++)
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            FiberCoords fc = cc.GetFiberCoords(new Coords(i, j));
                            if (fc.Fiber != -1)
                            {
                                double delta = Math.Abs(flow_az_mas_prev[fc.Fiber][fc.Pvk]) - Math.Abs(flow_az_mas[fc.Fiber][fc.Pvk]);
                                if (delta > 0.3 * Math.Abs(flow_az_mas_prev[fc.Fiber][fc.Pvk]))
                                {
                                    change_coords[key] = new Coords(i, j);
                                    change_flow[key] = delta;
                                    key++;
                                }
                            }
                        }
                    }

                    if (key > 0)
                    {
                        Coords[] crds = new Coords[key];
                        double[] flow_crds = new double[key];
                        for (int m = 0; m < key; m++)
                        {
                            crds[m] = change_coords[m];
                            flow_crds[m] = change_flow[m];
                        }
                        TupleMetaData illegal_data = new TupleMetaData("flow_change_coords", "Колебания расхода",
                           dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                        CoordsConverter cc_illegal = new CoordsConverter(illegal_data, CoordsConverter.SpecialFlag.Named, crds);

                        //DataArrayCoords array_crds = new DataArray<Coords>(illegal_data, crds);
                        DataCartogramIndexed<double> ill_cart =
                            new DataCartogramIndexed<double>(illegal_data, ScaleIndex.Default, cc_illegal, flow_crds);

                        DataTuple flow_az_illegal_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), ill_cart);
                        env.dataResource_input.GetMultiProvider().PushData(flow_az_illegal_tuple);
                    }
                    else
                    {
                        Coords[] crds = new Coords[0];
                        double[] flow_crds = new double[0];
                        TupleMetaData illegal_data = new TupleMetaData("flow_change_coords", "Колебания расхода",
                           dataSkala.GetTimeDate(), TupleMetaData.StreamAuto);
                        CoordsConverter cc_illegal = new CoordsConverter(illegal_data, CoordsConverter.SpecialFlag.Named, crds);

                        //DataArrayCoords array_crds = new DataArray<Coords>(illegal_data, crds);
                        DataCartogramIndexed<double> ill_cart =
                            new DataCartogramIndexed<double>(illegal_data, ScaleIndex.Default, cc_illegal, flow_crds);

                        DataTuple flow_az_illegal_tuple = new DataTuple(mainStream, dataSkala.GetTimeDate(), ill_cart);
                        env.dataResource_input.GetMultiProvider().PushData(flow_az_illegal_tuple);
                    }


                    /************************/
                    //skala_count = dt_sk.Length;//для работы в сети обмена данных
                    azot_count = dt_az.Length;
                    date_previous = dataSkala.GetTimeDate();
                    pvk_maxes_prev = pvk_maxes;
                    flow_az_mas_prev = flow_az_mas;
                    string s = dataSkala.GetTimeDate().ToString() + "   " + dt_az.Length.ToString();
                    //RecoveryForm.TBox.Text = dataSkala.GetTimeDate().ToString() + "   " + dt_az.Length.ToString();

                    //Thread.Sleep(1000 * 60);
                    /*dt2 = DateTime.Now;
                    ts1 = dt2 - dt1;
                    Thread.Sleep(ts2 - ts1);*/

                    //if (dt_az.Length == 15)
                    // sw.Close();
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        //функция вызова рассчета коэффициентов
        public IMultiDataTuple ComputeAdaptationCoefficient(IMultiDataProvider p, DateTime dt,
                                                            IMultiDataTuple mdt_az, IMultiDataTuple zagr,
                                                            DataCartogramNative<Double> dcn)
        {
            string[] names = new string[5];
            names[0] = "power";
            names[1] = "flow";
            names[2] = "energovir";
            names[3] = "rbmk_params";
            names[4] = "pvk_maxes_cart";

            IMultiDataTuple azot = CartToFibers("pvk_maxes", p.GetData(dt, names), true);
            IMultiDataTuple tmp_wockr = env.GetAlgorithm("calculateMiddlePower").CallIntelli(zagr);
            // //Evaluate adaptation coefficients
            IMultiDataTuple koeffs = env.GetAlgorithm("evaluteAzotDpCoeff").CallIntelli(
                zagr,
                tmp_wockr,
                dcn,
                azot);

            return koeffs;
        }

        //получение IMultiDataTuple для рассчета коэффициентов
        public MultiDataTuple CartToFibers(string name, IMultiDataTuple azotCart, bool param)
        {
            DataCartogram c = (DataCartogram)azotCart[0][name + "_cart"];
            DataArray[] ar = c.ConvertToPartArray(
                new TupleMetaData(name, name, c.Date, TupleMetaData.StreamAuto), env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, c.Info));
            if (param)
            {
                DataParamTable[] pt = new DataParamTable[16];
                for (int i = 0; i < pt.Length; i++)
                    pt[i] = new DataParamTable(new TupleMetaData("kgoprp_info", "kgoprp_info", c.Date, TupleMetaData.StreamAuto),
                            new DataParamTableItem("fiberNum", i));

                return new MultiDataTuple(name, azotCart.GetTimeDate(), new MultiTupleItem(ar), new MultiTupleItem(pt));
            }
            return new MultiDataTuple(name, azotCart.GetTimeDate(), new MultiTupleItem(ar));
        }

        //функции восстановления
        public IMultiDataTuple RecoveryData(IMultiDataProvider p, DateTime dt,
                                            IMultiDataTuple zagr, IMultiDataTuple coeff,
                                            DataCartogramNative<Double> dcn)
        {
            string[] names = new string[5];
            names[0] = "power";
            names[1] = "flow";
            names[2] = "energovir";
            names[3] = "rbmk_params";
            names[4] = "pvk_maxes_cart";

            IMultiDataTuple azot = CartToFibers("pvk_maxes", p.GetData(dt, names), true);

            IMultiDataTuple tmp_wockr = env.GetAlgorithm("calculateMiddlePower").CallIntelli(zagr);

            IMultiDataTuple recovered = env.GetAlgorithm("evaluteAzotDpFlow").CallIntelli(
                zagr,
                tmp_wockr,
                dcn,
                azot,
                coeff);

            return recovered;
        }


    }


    public class SrvEnv : BasicEnv
    {
        public IDataResource dataResource_connect;
        public IDataResource dataResource_input;
        public SrvEnv(DataParamTable par)
            :
            base(par, (string)par["componentPath"], (string)par["componentPath"])
        {
            /*
            dataResource_connect = CreateData("SunEnv_DataStorageProvider");
            dataResource_input = CreateData("Service_DataStorageProvider");
            */
            dataResource_connect = CreateData("SunEnv_PgDataStorageProvider");
            dataResource_input = CreateData("SunEnv_PgDataStorageProvider2");
            

            IMultiDataTuple scheme = dataResource_connect.GetMultiProvider().GetConstData("const");
            IDataTuple dtp_tuple = scheme[0];
            DataArrayCoords dac = (DataArrayCoords)dtp_tuple["pvk_scheme"];

            CoordsConverter pvk2 = new CoordsConverter(
                dac.Info,
                CoordsConverter.SpecialFlag.PVK,
                dac);

            SetPVK(pvk2);
        }
    }

    static class Service2Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new Service_P2() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
