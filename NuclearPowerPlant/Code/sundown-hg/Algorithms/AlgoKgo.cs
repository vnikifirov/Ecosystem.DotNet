#define MODIFIED_METHOD
//#define FILTER_KIZER
//#define USE_FILTER

using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using corelib;


namespace Algorithms
{

    public struct WarnDetail
    {
        public int num;
        public bool manual_assigned;
        public ChannelType detected;
        public ChannelType real;
        public int weight;
    }

    public enum DetectBadness
    {
        NOT_ALL_PVK = 2000,
        FAIL_TVS = 500,
        UNKNOWN = 50,
        WATER_TVS = 25,
        WATER_EMPTY = 10
    }

    public class KgoDetect
    {
        [Flags]
        public enum DetectInfo
        {
            NoPvk = 1,
            LowSignal =  2,
            BigGap = 4
        }

        static double[,] foundMaxes(double[] data, out int pvk_count)
        {
            int i, j;
            int p = data.Length;
#if MODIFIED_METHOD
            p = (p * 4) / 5;
#endif
            double[] daccor = MathOp.Accor(data, p);

            //Determine fundamental frequency
            int[] maxes = new int[p];
            double emax = 0;
            int maxi = 0, mxs = 0;

            for (i = 0; i < p / 2; i++)
            {
                if ((daccor[i] > 0) && (daccor[i] > emax))
                {
                    emax = daccor[i];
                    maxi = i;
                }
                else if ((daccor[i] < 0) && (emax > 0))
                {
                    maxes[mxs] = maxi + 1;
                    mxs++;
                    emax = 0;
                }
            }

            double aa;
            double tmp1 = 0, tmp2 = 0, tmp3 = 0;
            for (i = 0; i < mxs; i++)
            {
                tmp1 += maxes[i] * (i + 1);
                tmp2 += maxes[i] * maxes[i];
                tmp3 += maxes[i];
            }
            aa = (tmp1 * mxs - tmp3 * mxs * (mxs + 1) / 2) / (mxs * tmp2 - tmp3 * tmp3);

            //Compute correlation with optimal triangle and compute it's second derivate
            int k = (int)Math.Ceiling(0.4 / aa);
            double[] qq = new double[data.Length];

#if FILTER_KIZER
            double[] filter2 = {0.0192355,  0.0075185,  -0.0101490, -0.0280057, -0.0387952,  -0.0359261,
 -0.0156895,   0.0211852,   0.0691956,  0.1191492,  0.1604181,  0.1837399,  0.1837399,
  0.1604181,   0.1191492,  0.0691956,  0.0211852, -0.0156895, -0.0359261,  -0.0387952,
 -0.0280057, -0.0101490,  0.0075185,  0.0192355};
            double[] filter3 = { 2.6680e-002, 1.7073e-002, -2.0548e-002, -3.5439e-002, 9.8052e-004, 4.7077e-002, 3.3380e-002, -4.2573e-002, -8.6628e-002, 9.8165e-004, 2.0319e-001, 3.7540e-001, 3.7540e-001, 2.0319e-001, 9.8165e-004, -8.6628e-002, -4.2573e-002, 3.3380e-002, 4.7077e-002, 9.8052e-004, -3.5439e-002, -2.0548e-002, 1.7073e-002, 2.6680e-002 };

            double[] filter5 = { -0.0121891, -0.0039439, 0.0064907, 0.0187174, 0.0321963, 0.0462758, 0.0602333, 0.0733214, 0.0848178, 0.0940741, 0.1005601, 0.1039003, 0.1039003, 0.1005601, 0.0940741, 0.0848178, 0.0733214, 0.0602333, 0.0462758, 0.0321963, 0.0187174, 0.0064907, -0.0039439, -0.0121891 };
            double[] filter6 = { 0.076987, 0.081894, 0.085989, 0.089161, 0.091325, 0.092422, 0.092422, 0.091325, 0.089161, 0.085989, 0.081894, 0.076987 };
            double[] filter7 = { 0.063379, 0.078693, 0.092472, 0.103769, 0.111786, 0.115946, 0.115946, 0.111786, 0.103769, 0.092472, 0.078693, 0.063379 };
            double[] filter8 = { 0.031249, 0.062443, 0.094694, 0.123864, 0.145971, 0.157885, 0.157885, 0.145971, 0.123864, 0.094694, 0.062443, 0.031249 };
            double[] filter9 = { 0.017379, 0.046738, 0.078911, 0.109982, 0.135873, 0.153030, 0.159034, 0.153030, 0.135873, 0.109982, 0.078911, 0.046738, 0.017379 };
            double[] filter = { -0.0061476, 0.0176087, 0.0471131, 0.0792155, 0.1100549, 0.1356570, 0.1525813, 0.1584974, 0.1525813, 0.1356570, 0.1100549, 0.0792155, 0.0471131, 0.0176087, -0.0061476 };
            
            k = (filter.Length - 1) / 2;
#endif

            for (i = 0; i < data.Length; i++)
            {
                double sm = 0;
                int l = 0;

                for (j = -k; j <= k; j++)
                {
                    if ((i + j) < 0)
                        continue;
                    else if (i + j >= data.Length)
                        break;
                    else
                    {
#if FILTER_KIZER
                        sm += data[i + j] * (filter[k+j]);
#else
                        sm += data[i + j] * (k + 1 - Math.Abs(j));
#endif
                        l++;
                    }
                }
#if FILTER_KIZER
                qq[i] =  sm;
#else
                //qq[i] = l * sm / k;
                qq[i] = l * sm / (2 * k * k * k);  ///////////////////////////
#endif
            }

            double[] qqr = new double[data.Length];
            for (i = 1; i < data.Length - 1; i++)
            {
                qqr[i] = qq[i + 1] - 2 * qq[i] + qq[i - 1];
            }

            // Find first MAX in signal
            emax = qq[0];
            maxi = 0;

            ///int st_an = data.Length/10;
            //// double mn = MathOp.imean(qq, 0, st_an);
            int stat_len = qq.Length;
            //int stat_len = (qq.Length * 9) / 10;
            double mean_qq;
            double sig_qq = Math.Sqrt(MathOp.IntVar(qq, 0, stat_len, out mean_qq));
            double mn = mean_qq - 0.75 * sig_qq;

            /// Method 2
#if MODIFIED_METHOD
            tmp1 = Math.Sqrt(MathOp.IntVar(qq, k, k + (stat_len / 4), out tmp2));
            mn = tmp2 - tmp1;
#endif

            for (i = 1; i < data.Length; i++)
            {
                if (qq[i] > emax)
                {
                    emax = qq[i];
                    maxi = i;
                }
                else if (qq[i] > mn)
                {
                    break;
                }
            }
            int starti = maxi;


            // Fill information about other maximums
            int ex = (int)Math.Ceiling(0.3 / aa);
            double[,] pvks = new double[116, 5];

            // Normalize corr
            ////double mqq = MathOp.IntVar(qq, 0, qq.Length, tmp1);
            ////mqq = tmp1 - 0.5 * Math.Sqrt(mqq);
            double mqq = mean_qq - 0.5 * sig_qq;

            double vqqr = MathOp.IntVar(qqr, 0, stat_len, out tmp1);

            for (i = 0; i < 116; i++)
            {
                int pp = (int)Math.Round(starti + i / aa);
                if (pp + ex > data.Length)
                    break;
                int pcorr;
                int p2;
                double m = MathOp.IntMax(data, pp - ex, pp + ex, out pcorr);
                double m2 = MathOp.IntMax(qq, pp - ex, pp + ex, out p2);
                if (pcorr + ex > data.Length)
                    break;

                pvks[i, 0] = pp;
                pvks[i, 1] = pcorr;

                DetectInfo di = 0;
                // No PVK
                /*pvks[i, 2]*/
                di |= (MathOp.IntMean(qqr, pcorr - ex / 2, pcorr + ex / 2) < 0) ? DetectInfo.NoPvk : 0;
                // Low signal rate OR not so good signal
                /*pvks[i, 3]*/
                di |= (qq[pcorr] <= mqq) ? DetectInfo.LowSignal : 0;
                // Big gap in signal
                tmp2 = MathOp.IntVar(qqr, pcorr - ex, pcorr + ex, out tmp3);
                /*pvks[i, 4]*/ 
                di |= ((tmp2 / 0.01) < vqqr) ? DetectInfo.BigGap : 0;

                pvks[i, 3] = (int)di;

                pvks[i, 4] = m2;
            }
            pvk_count = i;
            return pvks;
        }



        public KgoDetect(double[] data, ChannelType[] pInfo, bool detailInfo)
        {
            StringBuilder sb = new StringBuilder();
            int j;
            WarnDetail wd;
            signal_data = data;
            channInfo = pInfo;

            detected_pvk = KgoDetect.foundMaxes(data, out  pvk_count);
            pvk_z = new ChannelType[115];
            badness = 0;

            // calc offset
            for (j = 0, offset = 0; j < 5; j++, offset++)
            {
                if ((pInfo[j] == ChannelType.TVS) || (pInfo[j] == ChannelType.WATER))
                    break;
            }

            if (pvk_count + offset < 115)
            {
                count_not_found = 115 - pvk_count - offset;
                badness += (int)DetectBadness.NOT_ALL_PVK * (count_not_found);
                sb.AppendFormat("NOT_FOUNDED({0}) ", count_not_found);
            }

            for (j = 0; j < offset; j++)
                pvk_z[j] = pInfo[j];
            for (j = offset; j < Math.Min(pvk_count, pvk_z.Length); j++)
            {
                DetectInfo di = (DetectInfo)detected_pvk[j - offset, 3];

                int tvs = (int)di & (int)DetectInfo.NoPvk;
                int low = (int)di & (int)DetectInfo.LowSignal;
                int dlow = (int)di & (int)DetectInfo.BigGap;

                wd.weight = 0;
                wd.manual_assigned = false;

                if (dlow != 0)
                {
                    pvk_z[j] = ChannelType.EMPTY;
                }
                else if ((low != 0) && (tvs == 0))
                {
                    pvk_z[j] = ChannelType.EMPTY;
                }
                else if ((low != 0) && (tvs != 0))
                {
                    pvk_z[j] = ChannelType.WATER;
                }
                else if (tvs != 0)
                {
                    pvk_z[j] = ChannelType.TVS;
                }
                else
                {
                    pvk_z[j] = ChannelType.WATER;
                    wd.weight += (int)DetectBadness.UNKNOWN;
                    wd.manual_assigned = true;
                }

                if (pvk_z[j] == ChannelType.EMPTY && pInfo[j] == ChannelType.DP)
                {
                    // It's ok. Nothing to do
                }
                else if ((pvk_z[j] == ChannelType.WATER && pInfo[j] == ChannelType.DP) ||
                   (pvk_z[j] == ChannelType.WATER && pInfo[j] == ChannelType.EMPTY) ||
                   (pvk_z[j] == ChannelType.EMPTY && pInfo[j] == ChannelType.WATER))
                {
                    wd.weight += (int)DetectBadness.WATER_EMPTY;
                    sb.AppendFormat("we({0}@{1}) ", j, detected_pvk[j, 0]);
                }
                else if ((pvk_z[j] == ChannelType.TVS && pInfo[j] == ChannelType.WATER) ||
                 (pvk_z[j] == ChannelType.WATER && pInfo[j] == ChannelType.TVS))
                {
                    wd.weight += (int)DetectBadness.WATER_TVS;
                    sb.AppendFormat("wt({0}@{1}) ", j, detected_pvk[j, 0]);
                }
                else if (pvk_z[j] != pInfo[j])
                {
                    wd.weight += (int)DetectBadness.FAIL_TVS;
                    sb.AppendFormat("FT({0}@{1}) ", j, detected_pvk[j, 0]);
                }

                if ((!detailInfo && (wd.weight > (int)DetectBadness.WATER_EMPTY)) || detailInfo)
                {
                    badness += wd.weight;

                    if (wd.manual_assigned || wd.weight > 0)
                    {
                        wd.num = j;
                        wd.real = pInfo[j];
                        wd.detected = pvk_z[j];

                        if (details == null)
#if !DOTNET_V11
                            details = new List<WarnDetail>();
#else
                            details = new ArrayList();
#endif

                        details.Add(wd);
                    }
                }
            }

            report = sb.ToString();
        }


        public int[] PvkMaxIdx
        {
            get
            {
                int j;
                if (maxIdxCached == null)
                {
                    maxIdxCached = new int[115];
                    for (j = 0; j < offset; j++)
                        maxIdxCached[j] = 0;
                    for (j = offset; j < Math.Min(pvk_count, pvk_z.Length); j++)
                        maxIdxCached[j] = (int)detected_pvk[j - offset, 1];

                }
                return maxIdxCached;
            }
        }

        public double[] PvkMaxVal
        {
            get
            {
                int j;
                if (maxValCached == null)
                {
                    maxValCached = new double[115];

                    for (j = 0; j < offset; j++)
                        maxValCached[j] = 0;
                    for (j = offset; j < Math.Min(pvk_count, pvk_z.Length); j++)
#if USE_FILTER
                        maxValCached[j] = detected_pvk[j - offset, 4];
#else
                        maxValCached[j] = signal_data[(int)detected_pvk[j - offset, 1]];
#endif


                    //Фон БС
                    //int n;
                    //double m = MathOp.IntMax(maxValCached, 0, maxValCached.Length, out n) * 0.008;
                    //for (j = offset; j < pvk_count; j++)
                    //    maxValCached[j] = (maxValCached[j] < m) ? 0 : maxValCached[j] - m;
                }

                return maxValCached;
            }
        }

#if !DOTNET_V11
        public List<WarnDetail> Details
        {
            get { return details; }
        }
#else
        public ArrayList Details
        {
            get { return details; }
        }
#endif

        public int Badness
        {
            get { return badness; }
        }
        public int CountNotFound
        {
            get { return count_not_found; }
        }

        public double[] Signal
        {
            get { return signal_data; }
        }

        public ChannelType[] ChannelInfo
        {
            get { return channInfo; }
        }

        public string Report
        {
            get { return report; }
        }

        double[] signal_data;
        double[,] detected_pvk;
        int pvk_count;

        string report;

        ChannelType[] pvk_z;
        int badness;

#if !DOTNET_V11
        List<WarnDetail> details;
#else
        ArrayList details;
#endif
        ChannelType[] channInfo;

        int offset;
        int count_not_found;

        int[] maxIdxCached;
        double[] maxValCached;
    }


    public class KgoDetectAlgo
    {


        [AttributeAlgoComponent("Определение максимумов азотной активности")]
        [AttributeRules("kgo[] is (kgoprp_info as int[fiberNum] to fiberNum, kgoprp_azot as double[] to kgoprp_azot) ;" +
                      "zagr is (zagr as double[pvk]) ;" +
                   " return[] is (pvk_errorinfo('Информация') as ParamTable[badness,fiberNum,report] to (badness,fiberNum,report) , pvk_maxes('Максимумы прописки') as Array to pvk_maxes)")]
        public static void kgoDetectMaxPos(int[] fiberNum, double[][] kgoprp_azot, double[,] zagr, out double[] badness, out double[][] pvk_maxes, out string[] report)
        {
            badness = new double[fiberNum.Length];
            pvk_maxes = new double[fiberNum.Length][];
            report = new string[fiberNum.Length];

            for (int i = 0; i < fiberNum.Length; i++)
            {
                ChannelType[] cht = new ChannelType[115];
                int fiber = fiberNum[i];
                for (int j = 0; j < 115; j++)
                    cht[j] = (ChannelType)zagr[fiber, j];

                KgoDetect kdt = new KgoDetect(kgoprp_azot[i], cht, false);

                pvk_maxes[i] = kdt.PvkMaxVal;
                badness[i] = kdt.Badness;
                report[i] = kdt.Report;
            }
        }
    }
#if OLD
        [AttributeAlgoComponent("Определение максимумов азотной активности")]
        [AttributeRules("kgo[] contains (kgoprp_info, kgoprp_azot) cast (kgoprp_info as ParamTable, kgoprp_azot as Array) ;" +
                      "zagr contains (zagr) cast (zagr as Cart(pvk,int)) ;" +
                   " return[] contains (pvk_errorinfo, pvk_maxes)  cast (pvk_errorinfo as Array, pvk_maxes as Array)")]
        public static IDataTuple[] kgoDetectMaxPosOld(IDataTuple zagr, IDataTuple[] kgo)
        {
            DataCartogram c = (DataCartogram)zagr["zagr"];
            IDataTuple[] output = new IDataTuple[kgo.Length];

            for (int i = 0; i < kgo.Length; i++)
            {
                DateTime dt = ((IDataTuple)kgo[i]).GetTimeDate();
                ChannelType[] cht = new ChannelType[115];

                int fiber = (int)((DataParamTable)kgo[i]["kgoprp_info"]).GetParam("fiberNum");

                for (int j = 0; j < 115; j++)
                    cht[j] = (ChannelType)((int[,])c)[fiber, j];

                int[] data = (int[])(DataArray)kgo[i]["kgoprp_azot"];

                KgoDetect kdt = new KgoDetect(data, cht, false);
                DataArray maxes = new DataArray("pvk_maxes", kgo[i]["kgoprp_azot"].GetHumanName(), dt, kdt.PvkMaxVal);

                //Hashtable ht = new Hashtable();
                //ht.Add("badness", kdt.Badness);
                //ht.Add("fiberNum", fiber);
                //DataParamTable pvk_errorinfo = new DataParamTable("pvk_errorinfo", "pvk_errorinfo", dt, ht);
                DataParamTable pvk_errorinfo = new DataParamTable("pvk_errorinfo", "pvk_errorinfo", dt,
                    "badness", kdt.Badness,
                    "fiberNum", fiber);

                output[i] = new DataTuple("out", dt, pvk_errorinfo, maxes);
            }

            return output;
        }

        [AttributeAlgoComponent("Упаковка ниток в картограмму")]
        [AttributeRules(@" maxes[] contains (kgoprp_info, pvk_maxes)  cast (kgoprp_info as ParamTable, pvk_maxes as Array);
                           pvks contains (pvk_scheme) cast (pvk_scheme as Array);
                           return contains (pvk_maxes_cart) cast (pvk_maxes_cart as Cart)")]
        public static IDataTuple kgoCompactMax(IDataTuple pvks, IDataTuple[] maxes)
        {
            DataArray dc = new DataArray("pvk_maxes_cart",
                maxes[0]["pvk_maxes"].GetHumanName(),
                maxes[0].GetTimeDate(),
                ((DataArray)maxes[0]["pvk_maxes"]).ElementType, 48, 48, 0);

            // FIXME
            ArrayPvkInfo ai = new ArrayPvkInfo((DataArray)pvks["pvk_scheme"]);

            for (int i = 0; i < maxes.Length; i++)
            {
                DataArray data = (DataArray)maxes[i]["pvk_maxes"];
                int fiber = (int)((DataParamTable)maxes[i]["kgoprp_info"]).GetParam("fiberNum");

                for (int j = 0; j < 115; j++)
                {
                    Coords c = ai.GetByPvk(new FiberCoords(fiber, j));
                    if (c.IsOk)
                    {
                        dc[c.Y, c.X] = data[j];
                    }
                }                
            }

            DataCartogram dci = DataCartogram.Create(dc);
            dci.SetPvkInfo(ai);
            return new DataTuple("pvks", maxes[0].GetTimeDate(), dci);
        }





        [AttributeAlgoComponent("Распаковка ниток")]
        [AttributeRules(@" pvks contains (pvk_maxes_cart) cast (pvk_maxes_cart as Cart(pvk)); 
                           return[] contains (kgoprp_info, pvk_maxes)  cast (kgoprp_info as ParamTable, pvk_maxes as Array);")]
        public static IDataTuple[] kgoDeCompactMaxOld(IDataTuple pvks)
        {
            DataParamTable[] prm = new DataParamTable[16];
            DataArray[] lst = new DataArray[16];
            DataCartogram c = (DataCartogram)pvks["pvk_maxes_cart"];

            for (int i = 0; i < 16; i++)
            {
                //Hashtable hi = new Hashtable();
                //hi.Add("fiberNum", i);

                prm[i] = new DataParamTable("kgoprp_info", "kgoprp_info", pvks.GetTimeDate(), "fiberNum", i);
                lst[i] = new DataArray("pvk_maxes", "pvk_maxes", pvks.GetTimeDate(), c.ElementType, 115, 0, 0);

                for (int j = 0; j < 115; j++)
                {
                    lst[i][j] = c.GetItem(new FiberCoords(i, j));
                }
            }

            return DataTuple.CreateMultiTuple("out", prm, lst);
        }
    }

#endif
}
