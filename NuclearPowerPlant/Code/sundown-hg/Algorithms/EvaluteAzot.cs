//#define EXCLUDE_DOUBLE
//#define CORRECT

using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Threading;

using ChannelType = corelib.ChannelType;
using IResourceInstance = corelib.IResourceInstance;
using AttributeRules = corelib.AttributeRules;
using AttributeAlgoComponent = corelib.AttributeAlgoComponent;


namespace Algorithms
{
    public class EvaluteAzot
    {
        const int GIDRA_CONST_LEN = 12;

        //       Тумпература воды на входе в канал,C
        static readonly double[] t_l = {
		    10, 20, 50, 90, 100, 110, 200,
		    250, 260, 270, 280, 290};

        //         Энтальпия недогретой воды, ккал/кг
        static readonly double[] i_t = {
	        11.6,  21.6, 51.4, 91.3, 101.3, 111.4,
	        204.1, 259.3, 271.0, 283.0, 295.3, 308.1
	        };
        //         Удельный   об"ем,  м**3/кг
        static readonly double[] v_t = {
	        0.9972e-3, 0.9988e-3 , 0.1009e-2,
	        0.10325e-2, 0.10399e-2, 0.1048e-2,
	        0.11513e-2, 0.12459e-2, 0.12708e-2,
	        0.12988e-2, 0.13307e-2, 0.13653e-2
	        };
        //        Динамическая  вязкость н.в.,  кг*с/м**2
        static readonly double[] mu_t = {
	        1.33e-3, 0.102e-3, 0.562e-4, 0.323e-4,
	        0.29e-4, 0.266e-4, 0.141e-4, 0.113e-4,
	        0.109e-4, 0.105e-4, 0.101e-4, 0.97e-5
	        };
        //       Давления,    кг/см**2
        static readonly double[] p_t = {
	        12.8, 15.9, 19.5, 23.7, 28.5, 34.1,
	        40.6, 47.9, 56.1, 65.5, 75.9, 87.6
	        };

        //        Динамическая  вязкость на линии насыщения,  кг*с/м**2
        static readonly double[] mu1_t = {
           0.148e-4, 0.141e-4, 0.134e-4,
           0.128e-4, 0.122e-4, 0.117e-4,
           0.112e-4, 0.108e-4, 0.104e-4,
           1.0e-5,  0.96e-5, 0.93e-5
        };


        const double Param_A = 1.36e+11;
        // Переходной коэфф. от мощности к потоку нейтронов,1/(Вт*c*кг)
        const int m_az = 50;
        /* число расчетных точек на АЗ,первая точка находится
        на краю активной зоны, а последняя внутри АЗ,
        первая точка имеет номер 0,а последняя (m_az-1) */
        const int m_pvk = 50;
        /* число расчетных точек на ПВК, первая точка находится
        на границе АЗ, а последняя на конце ПВК,
        первая точка имеет номер m_az, а последняя m_az+m_pvk-1 */



        const double grav = 9.81;
        const double H_AZ = 7.0;    /* Высота  АЗ,м */
        const double d_tk = 0.8562e-2;/* Гидр. диаметр ТК с ТВС,м             */
        const double eta_gr = 0.054;	/* Доля тепла, идущего из графита       */
        const double F_w = 5.35;    /* Поверхность теплообмена ТВС,м**2     */
        const double hi = 0.561;	/* Неподобие ТВС и трубы                */
        const double K_cell = 1.0;	    /* Коэф. неравномерности тепловыделения */
        const double P_kr = 225.65e+4; /* Критическое давление                 */
        const double S_az = 0.2273e-2;	/* Проходное сечение ТК,м**2            */
        const double sh = 1.0e-5;	/* Шероховатость канала в АЗ            */

        const double alf = 0.07;/* Доля мощности от окружения */



        // Длины участков в отводящей зоне,м
        static readonly double[] c_len_pvk = {
           0.4, 1.5, 1.38, 7.0, 0.4, 2.3, 1.5, 1.0, 1.5, 1.5, 1.0, 1.75,
           1.0, 0.47, 1.0, 0.12, 0.8, 1.0, 0.24, 2.0, 0.4, 0.512, 0.148
        };
        // Перепады  высот,м
        static readonly double[] h_pvk = {
           0.17, 1.3, 2.38, 3.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.465,
           1.0, 0.12, 0.835, 1.0, 0.24, 2.0, 0.4, 0.512, 0.148
        };
        // Гидр. диаметры,м
        static readonly double[] d_pvk = {
           0.068, 0.068, 0.068, 0.068, 0.068, 0.068, 0.068, 0.068, 0.068, 0.068,
           0.068, 0.068, 0.068, 0.042, 0.042, 0.039, 0.04, 0.0207, 0.043, 0.049,
           0.049, 0.047, 0.008562
        };
        // Проходные   сечения,м**2
        static readonly double[] s_pvk = {
           0.003632, 0.003632, 0.003632, 0.003632,
           0.003632, 0.003632, 0.003632, 0.003632,
           0.003632, 0.003632, 0.003632, 0.003632,
           0.003632, 0.007323, 0.007323, 0.003951,
           0.004084, 0.00232 , 0.004289, 0.004657,
           0.004657, 0.004171, 0.002273
        };
        //  Коэф. местного сопротивления
        static readonly double[] csi_pvk = {
           1.2,  0.0635, 0.127, 0.0, 0.4, 0.05,
           0.115, 0.0515, 0.0892, 0.0892, 0.0892,
           0.0   , 0.0858, 2.19,   0.0835, 1.6,
           0.1, 0.5, 0.046, 0.0, 0.087, 0.1, 0.963
        };
        //  Шероховатости
        static readonly double[] sh_pvk = {
           0.25e-4, 0.25e-4, 0.25e-4, 0.25e-4,
           0.25e-4, 0.25e-4, 0.25e-4, 0.25e-4,
           0.25e-4, 0.25e-4, 0.25e-4, 0.25e-4,
           0.25e-4,   1.0e-5,   1.0e-5,   1.0e-5,
           1.0e-5,     1.0e-5,   1.0e-5,   1.0e-5,
           1.0e-5,     1.0e-5,   1.0e-5
        };


        /*----------------   Активная   зона    --------------------------------*/
        //     Длины участков, м

        static readonly double[] len_az = {
           0.05,	0.025,	0.155,0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.12,	   0.0,
           0.12,	0.0,		0.12,	0.0,		0.005,	0.01,
           0.015,0.015,	0.01,	0.005,	0.0,		0.18,
           0.18,	0.0,		0.18,	0.18,	   0.0,		0.18,
           0.18,	0.0,		0.18,	0.18,	   0.0,		0.18,
           0.18,	0.0,		0.18,	0.18,	   0.0,		0.18,
           0.18,	0.0,		0.18,	0.18,	   0.0,		0.18,
           0.18,	0.0,		0.1,	0.055,	0.025,	0.05
        };

        //   Коэф. местного сопротивления
        static readonly double[] csi_az =  {
           0.45,   0.0,             0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.45,
           0.0,             0.45,   0.0,             1.0,             0.0,     0.0,
           0.0,             0.0,     0.0,             0.0,             0.5,    0.0,
           0.0,             0.5,    0.0,             0.0,             0.5,    0.0,
           0.0,             0.5,    0.0,             0.0,             0.5,    0.0,
           0.0,             0.5,    0.0,             0.0,             0.5,    0.0,
           0.0,             0.5,    0.0,             0.0,             0.5,    0.0,
           0.0,             0.5,    0.0,             0.0,             0.0,     0.0
        };

        static readonly double[] S_cnst = {
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,

           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,

           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           S_az,     S_az,     S_az,     S_az,
           0.002273, 0.004171, 0.004657, 0.004657,
           0.004289, 0.00232,  0.004084, 0.003951,
           0.007323, 0.007323, 0.003632, 0.003632,
           0.003632, 0.003632, 0.003632, 0.003632,
           0.003632, 0.003632, 0.003632, 0.003632,
           0.003632, 0.003632, 0.003632, 0.003632
        };

        static readonly double[] Z_cnst = {
           0.0250, 0.0625, 0.1025, 0.1800, 0.2300, 0.3200, 0.5000, 0.5900, 
           0.6800, 0.8600, 0.9500, 1.0400, 1.2200, 1.3100, 1.4000, 1.5800, 
           1.6700, 1.7600, 1.9400, 2.0300, 2.1200, 2.3000, 2.3900, 2.4800, 
           2.6600, 2.7500, 2.8400, 3.0200, 3.1100, 3.2000, 3.3800, 3.4700, 
           3.4725, 3.4800, 3.4925, 3.5075, 3.5200, 3.5275, 3.5300, 3.5900, 
           3.6500, 3.7100, 3.7700, 3.8300, 3.8900, 3.9500, 4.0100, 4.0700, 
           4.1300, 4.1900, 4.2500, 4.3100, 4.3700, 4.4300, 4.4900, 4.5500, 
           4.6100, 4.6700, 4.7300, 4.7900, 4.8500, 4.9100, 4.9700, 5.0300, 
           5.0900, 5.1500, 5.2100, 5.2700, 5.3300, 5.3900, 5.4500, 5.5100, 
           5.5700, 5.6300, 5.6900, 5.7500, 5.8100, 5.8700, 5.9300, 5.9900, 
           6.0500, 6.1100, 6.1700, 6.2300, 6.2900, 6.3500, 6.4100, 6.4700, 
           6.5300, 6.5900, 6.6500, 6.7100, 6.7700, 6.8475, 6.9375, 6.9750,

           7.0740, 7.4040, 7.8600, 9.0600, 10.1800,10.8000,11.7000,12.1600,
           12.7200,13.4550,14.1900,15.5650,16.9400,18.1900,19.6900,20.9400,
           22.1900,24.0900,25.4400,32.6400,40.3300,41.7700,42.7200,99.9999
        };



        static double li1(int Max_Index, double[] Arg_X, double[] Arg_Y, double x)
        {
            double res;
            int i = 0;
            if (x < Arg_X[0])
                res = Arg_Y[0];
            else if (x > Arg_X[Max_Index - 1])
                res = Arg_Y[Max_Index - 1];
            else
            {
                while (x >= Arg_X[++i]) ;
                res = (Arg_Y[i - 1] +
                   (Arg_Y[i] - Arg_Y[i - 1]) * (x - Arg_X[i - 1]) / (Arg_X[i] - Arg_X[i - 1]));
            }
            //__syncthreads();
            return res;
        }

        static void one_phase(double i, double G, double d, double s, out double ptr_psi_m,
                        out double ptr_psi_t, out double ptr_R, out double ptr_fi, out double ptr_Re,
                        out double ptr_gamma, /*out double ptr_om1,*/ out double ptr_om, double v1)
        {
            double mu;
            double v;

            mu = li1(GIDRA_CONST_LEN, i_t, mu_t, i);
            v = li1(GIDRA_CONST_LEN, i_t, v_t, i);

            ptr_gamma = 1.0 / v;
            ptr_psi_m = ptr_psi_t = 1.0 / ((ptr_gamma) * v1);
            ptr_R = 1.0;
            ptr_fi = 0.0; // паросодержание 
            ptr_Re = G * d / (3600.0 * s * mu * grav);
            //	*ptr_om1=(G*v)/(3600.0*s);// скорость теплоностителя на экономазерном участке

            ptr_om = (G * v) / (3600.0 * s);// скорость теплоностителя на экономазерном участке

        }

        static void two_phase(double p, double x, double nu1, double G, double d,
                                      double s, out double ptr_psi_m, out double ptr_psi_t, out double ptr_R,
                                      out double ptr_fi, out double ptr_Re, out double ptr_gamma, /*out double ptr_om1,
                                                                               out double ptr_om2, */
                        out double ptr_om, double v1, double v2, double gamma1, double gamma2)
        {
            double w0;
            double temp, K_pr;
            double beta, Fr;
            if (x >= 1.0)
                x = 0.999;
            ptr_psi_m = 1.0;
            w0 = G * v1 / (3600.0 * s);
            ptr_psi_t = 1.0 + 0.57 * Math.Sqrt(Math.Sqrt(Math.Sqrt(x))) * (1.0 - x) * (1.0 - x) *
               ((1.0 / (0.2 + w0 * v1 / (v2 * Math.Sqrt(grav * d)))) - 5.2 * x * x);
            ptr_R = 1.0 + x * (v2 / v1 - 1.0);
            Fr = w0 * w0 / (grav * d);
            temp = (1.0 - x) * v1 / (x * v2);
            beta = 1.0 / (1.0 + temp);
            K_pr = 1.0 + (0.6 + 1.5 * beta * beta) * (1.0 - p / P_kr) /
               Math.Sqrt(Math.Sqrt(Fr));
            ptr_fi = 1.0 / (1.0 + temp * K_pr); // паросодержание на испарительном участке
            ptr_Re = w0 * d / nu1;
            ptr_gamma = gamma1 * (1.0 - ptr_fi) + gamma2 * (ptr_fi); //средняя плотность теплоносителя \rho = \rho^{'} (1 - \phi) + \rho^{''} \phi
            //	*ptr_om1=G/(3600.0*s*((1-*ptr_fi)/v1 + (*ptr_fi*K_pr)/v2)); //скорость смеси
            //	*ptr_om2=K_pr*(*ptr_om1); // скорость смеси с учётом проскальзывания
            ptr_om = G / (3600.0 * s * (ptr_gamma)); // скорость смеси
        }

        static void spl(double ps, out double ptr_ts, out double ptr_xi1, out double ptr_v1, out double ptr_ga1,
                           out double ptr_xi2, out double ptr_v2, out double ptr_ga2, out double ptr_r)
        {
            double x;
            double Ts;
            //x = log(ps * 0.0001);
            x = Math.Log(ps * 0.0001);

            //  *ptr_ts - температура насыщения, град.С
            ptr_ts =
            Ts = ((((((((((-4.29246029e-8 * x - 4.26856851e-7) * x
               + 1.53437313e-6) * x + 2.20717118e-5) * x - 1.74177519e-5) * x
               - 3.73934843e-4) * x + 1.32837729e-3) + 2.12968201e-2) * x
               + 2.10778046e-1) * x + 2.37535765) * x + 27.8542422) * x + 99.0927120;
            //x = *ptr_ts / 100.;
            x = Ts / 100.0;
            //  *ptr_xi1 - энтальпия воды на линии насыщения,  ккал/кг
            ptr_xi1 = (((((((((1.1936422e-4 * x - 1.699491e-3) * x
               + 1.0322371e-2) * x - 3.453679e-2) * x + 6.921469e-2) * x
               - 8.382342e-2) * x + 6.222956e-2) * x - 2.5507948e-2) * x
               + 4.2290733e-1) * x - 4.16e-5) / 4.18887e-3;
            //  *ptr_v1 - удельный об"ем воды,   м**3/кг
            ptr_v1 = ((((((((5.968357e-7 * x - 8.267041e-6) * x + 4.8021276e-5) * x
               - 1.5042234e-4) * x + 2.7254276e-4) * x - 2.7979589e-4) * x
               + 1.4001053e-4) * x + 2.0639347e-5) * x + 1.390533e-7) * x + 1.0001789e-3;
            ptr_ga1 = 1.0 / (ptr_v1);  /*удельный вес, кг/м**3  */
            //  *ptr_xi2 - энтальпия   пара,   ккал/кг
            ptr_xi2 = (((((((-3.4714286e-4 * x + 2.7325329e-3) * x
               - 8.0477568e-3) * x + 9.38497e-3) * x - 1.2737746e-2) * x +
               1.95911e-4) * x + 1.843054e-1) * x + 2.50096) / 4.18887e-3;
            //  *ptr_v2 - удельный об'ем пара,   м**3/кг
            ptr_v2 = ((((((((2.0409848e-6 * x - 2.8638253e-5) * x + 1.4519652e-4) * x
               - 3.4194103e-4) * x + 3.3979946e-4) * x - 3.168401e-4) * x - 3.1913345e-5)
               * x + 4.5945234e-3) * x + 1.26e-2) / (ps * 9.81e-7);
            ptr_ga2 = 1.0 / (ptr_v2); /*удельный вес, кг/м**3  */
            //  *ptr_r - теплота парообразования,    ккал/кг
            ptr_r = ptr_xi2 - ptr_xi1;
        }


        /// <summary>
        /// Гидра расчет
        /// </summary>
        /// <param name="t0">Температура БС (НК), град цельсия</param>
        /// <param name="p0">Давление в БС, в МПа</param>
        /// <param name="Lpvk">Длина пара водяной коммуникации, м</param>
        /// <param name="dP">Записывает сюда падение давления</param>
        /// <param name="G">Расход воды в канале, м**3/час</param>
        /// <param name="N">Мощность канала, МВт</param>
        /// <param name="Gamma">Вывод: плотность смеси, кг/м**3</param>
        /// <param name="OM">Вывод: лямбда азота/ скорость</param>
        /// <param name="Ts">Вывод: температоура град цельсия</param>
        public static void gidra(double t0, double p0, double Lpvk, out double dP, double G, double N,
            ref double[] Gamma, ref double[] OM, ref double[] Ts)
        {

            //  double Lpvk = ppd->Lpvk;

            double dPm, dPt, dPn, dPy = 0.0;
            double z = 0.0;
            double p, nu1, x;
            double psi_m, psi_t, R, fi, Re/*,om1,om2*/, om, mu1;
            double lambda, gamma;
            double temp, temp1;
            double e;
            double	/*sigma,*/ w, q;
            double i, i_boil, i_liq, vy, vy_old = 0.0;
            //   double	length = 0.0;
            double delta;
            int j;
            int k = 118;
            double g0;//Удельный объем недогретой воды на входе в канал
            double i0;//Энтальпия воды на входе в канал,ккал/кг

            double i1, i2, v1, v2, gamma1, gamma2, r;
            double cur_len_pvk;

            double Tsi;
            dP = 0;

            // Lpvk = Lpvk + H_AZ;
            i0 = li1(12, t_l, i_t, t0);
            g0 = li1(12, i_t, v_t, i0);
            G = G / g0;

            N *= 1000.0;
            p = p0 * 10000.0;
            delta = 29.0 - (Lpvk + H_AZ);
            i = i0 + 860.0 * N / G;

            q = 860.0 * N * K_cell * (1.0 - eta_gr) / (F_w);

            for (j = 0; j < 23; j++, k--)
            {
                cur_len_pvk = c_len_pvk[j] - (j == 3 ? delta : 0);

                spl(p, out Tsi, out i1, out v1, out gamma1, out i2, out v2, out gamma2, out r);
                mu1 = li1(12, p_t, mu1_t, p * 0.0001);

                nu1 = grav * mu1 * v1;
                x = (i - i1) / r;
                if (x <= 0.00001)
                {
                    one_phase(i, G, d_pvk[j], s_pvk[j], out psi_m, out psi_t,
                       out R, out fi, out Re, out gamma/*,out om1*/, out om, v1);
                    fi = 0;
                    //om2=om1;
                }
                else
                {
                    two_phase(p, x, nu1, G, d_pvk[j], s_pvk[j],
                       out psi_m, out psi_t, out R, out fi, out Re, out gamma/*,out om1,out om2*/, out om, v1, v2, gamma1, gamma2);
                    if (fi > 0.999)
                        fi = 0.999;
                }
                lambda = 0.1 * Math.Sqrt(Math.Sqrt(1.46 * sh_pvk[j] / d_pvk[j] + 100.0 / Re));
                temp = G * G * v1 / (3600.0 * 3600.0 * s_pvk[j] * s_pvk[j] * 2.0 * grav);
                dPm = csi_pvk[j] * psi_m * temp * R;
                dPt = lambda * cur_len_pvk / d_pvk[j] * psi_t * temp * R;
                dPn = h_pvk[j] * gamma;
                p += dPm + dPt + dPn;
                Gamma[k] = gamma;
                OM[k] = 0.1 / om;
                if (Ts != null)
                    Ts[k] = Tsi;
                ///pp->P[k] = p;
                if (j == 0)
                    dP = -p;

            }     // for
            e = i;

            temp1 = G * G / (3600.0 * 3600.0 * S_az * S_az);
            for (j = 0; j < 96; j++, k--)
            {
                spl(p, out Tsi, out i1, out v1, out gamma1, out i2, out v2, out gamma2, out r);
                mu1 = li1(12, p_t, mu1_t, p * 0.0001);
                nu1 = grav * mu1 * v1;
                x = (i - i1) / r;
                i_boil = i1 - (55.0 * q * S_az / G) * Math.Pow((v1 * d_tk * Math.Sqrt(q * G *
                   v1 * v2 / (S_az * r)) / (v2 * nu1 * 3600.0)), 0.3);
                if (i <= i_boil)
                {
                    one_phase(i, G, d_tk, S_az, out psi_m, out psi_t, out R, out fi,
                       out Re, out gamma/*,out om1*/, out om, v1);
                    vy = 0.0;
                }
                else
                {
                    i_liq = i1 - ((i - i_boil) + (i1 - i_boil)) *
                       Math.Exp(-2.0 * (i - i_boil) / (i1 - i_boil));
                    x = (i - i_liq) / (i2 - i_liq);
                    if (x <= 0.00001)
                    {
                        one_phase(i, G, d_tk, S_az, out psi_m, out psi_t, out R, out fi, out Re,
                           out gamma/*,out om1*/, out om, v1);
                        vy = 0.0;
                    }
                    else
                    {
                        two_phase(p, x, nu1, G, d_tk, S_az, out psi_m, out psi_t, out R, out fi,
                           out Re, out gamma/*,out om1,out om2*/, out om, v1, v2, gamma1, gamma2);
                        if (fi > 0.999)
                            fi = 0.999;
                        vy = x * x * v2 / fi + (1.0 - x) * (1.0 - x) *
                           v1 / (1.0 - fi);
                    }
                }                 // i > i_boil
                lambda = 0.1 * Math.Sqrt(Math.Sqrt(1.46 * sh / d_tk + 100.0 / (Re * hi)));
                z += len_az[j];
                if (j > 0)
                    dPy = temp1 * (vy_old - vy) / grav;
                vy_old = vy;
                temp = temp1 * v1 / (2.0 * grav);
                dPm = csi_az[j] * psi_m * temp * R;
                dPt = lambda * len_az[j] / d_tk * psi_t * temp * R;
                dPn = len_az[j] * gamma;
                p += dPm + dPt + dPn + dPy;
                w = z * 860.0 * N / (H_AZ);
                i = e - w / G;
                Gamma[k] = gamma;
                OM[k] = .1 / om;
                if (Ts != null)
                    Ts[k] = Tsi;

                ///pp->P[k] = p;
                if (k == 0)
                    dP += p;
            }
        }



        public static void gidra(double t0, double p0, double Lpvk, out double dP, double G, double N,
    ref double[] Gamma, ref double[] OM)
        {
            double[] dummy = null;
            gidra(t0, p0, Lpvk, out dP, G, N, ref Gamma, ref OM, ref dummy);
        }


        public static void azgone(double t0, double p0, double Lpvk, out double dP, double G, double N, double Wmidl, out double azgoned,
            ref double[] Gamma, ref double[] OM)
        //static public void azgone(struct working_data *ppd, struct thread_temp_data *pp)
        {
            double pz = 0;
            double pz1, pz2;
            double pff; /*комплекс ff=Param_A* ( N + Wmidl *  alf) */
            double pomz, psr0z, pomz1, psr0z1, pomz2, psr0z2;
            double pcs, pcs1, pcs2;
            double pzh = H_AZ / m_az;   /* шаг по активвной зоне,м */
            double pzl = Lpvk / (m_pvk - 1); /* шаг по ПВК */
            double ps1, ps2, ps3, ps4;
            int pkh = m_az;	/* число точек по активной зоне */
            int pkl = m_pvk; /* число точек по ПВК */
            int k;

            double q1one = 0;
            double Naone = 0;

            gidra(t0, p0, Lpvk, out dP, G, N, ref Gamma, ref OM);

            // pp->OM[119] = pp->OM[118];
            // pp->Gamma[119] = pp->Gamma[118];

            pff = Param_A * (N + Wmidl * alf);
            pcs = pcs1 = pcs2 = S_az; // Сечения постоянны в АЗ

            pomz2 = OM[0];
            psr0z2 = Gamma[0];
            double[] pOm = OM;
            double[] pGm = Gamma;

            int pOmIdx = 0;
            int pGmIdx = 0;


            int index = 1;
            double old_z = Z_cnst[0];
            double cur_z = Z_cnst[1];

            for (k = 0; k < pkh; k++)
            {
                pz = pzh * k;   // координата расчетного узла по АЗ
                pz1 = pz + pzh / 2; // координата середины между расчетными узлами по АЗ
                pz2 = pz + pzh;    // координата следующего расчетного узла по АЗ
                pomz = pomz2; // OM в координате расчетного узла
                psr0z = psr0z2; //  плотность пароводяной смеси в расчетном узле

                while (pz1 >= cur_z)
                {
                    pOmIdx++; pGmIdx++; index++; old_z = cur_z; cur_z = Z_cnst[index];
                }
                pomz1 = pOm[pOmIdx] + (pOm[pOmIdx + 1] - pOm[pOmIdx]) * (pz1 - old_z) / (cur_z - old_z);
                psr0z1 = pGm[pGmIdx] + (pGm[pGmIdx + 1] - pGm[pGmIdx]) * (pz1 - old_z) / (cur_z - old_z);

                while (pz2 >= cur_z)
                {
                    pOmIdx++; pGmIdx++; index++; old_z = cur_z; cur_z = Z_cnst[index];
                }
                pomz2 = pOm[pOmIdx] + (pOm[pOmIdx + 1] - pOm[pOmIdx]) * (pz2 - old_z) / (cur_z - old_z);
                psr0z2 = pGm[pGmIdx] + (pGm[pGmIdx + 1] - pGm[pGmIdx]) * (pz2 - old_z) / (cur_z - old_z);

                ps1 = -pomz * q1one + pff * psr0z * pcs;  /* коэффициенты Рунге-Кутта */
                ps2 = -pomz1 * (q1one + ps1 * pzh / 2) + pff * psr0z1 * pcs1;
                ps3 = -pomz1 * (q1one + ps2 * pzh / 2) + pff * psr0z1 * pcs1;
                ps4 = -pomz2 * (q1one + ps3 * pzh) + pff * psr0z2 * pcs2;
                Naone = q1one * pomz / (.1 * pcs);  /* концентрация азота */
                q1one = q1one + (pzh / 6) * (ps1 + 2 * ps2 + 2 * ps3 + ps4); /* поток азота */
            }

            //double* pScnt = S_cnst + index;
            double[] pScnt = S_cnst;
            int pScntIdx = index;

            double delta = -29 + Lpvk; // on index = 96+3 == 99   

            for (; k < pkh + pkl; k++)
            {
                pz = H_AZ + pzl * (k - pkh); /* координата узла на ПВК */
                pz1 = pz + pzl / 2; /* 	координата середины между узлами */
                pz2 = pz + pzl;   /* координата следующего узла на ПВК */
                pomz = pomz2; // OM в координате расчетного узла
                pcs = pcs2; // сечение в координате расчетного узла по АЗ


                while (pz1 >= cur_z)
                {
                    pOmIdx++; pScntIdx++; index++; old_z = cur_z;
                    cur_z = Z_cnst[index] +
                       ((index < 115) ? 0 : (index == 115) ? delta / 2 : delta);
                }
                pomz1 = pOm[pOmIdx] + (pOm[pOmIdx + 1] - pOm[pOmIdx]) * (pz1 - old_z) / (cur_z - old_z);
                pcs1 = pScnt[pScntIdx] + (pScnt[pScntIdx + 1] - pScnt[pScntIdx]) * (pz1 - old_z) / (cur_z - old_z);

                while (pz2 >= cur_z)
                {
                    pOmIdx++; pScntIdx++; index++; old_z = cur_z;
                    cur_z = Z_cnst[index] +
                       ((index < 115) ? 0 : (index == 115) ? delta / 2 : delta);
                }
                pomz2 = pOm[pOmIdx] + (pOm[pOmIdx + 1] - pOm[pOmIdx]) * (pz2 - old_z) / (cur_z - old_z);
                pcs2 = pScnt[pScntIdx] + (pScnt[pScntIdx + 1] - pScnt[pScntIdx]) * (pz2 - old_z) / (cur_z - old_z);

                ps1 = -pomz * q1one; /* коэффициенты Рунге -Кутта для смеси */
                ps2 = -pomz1 * (q1one + ps1 * pzl / 2);
                ps3 = -pomz1 * (q1one + ps2 * pzl / 2);
                ps4 = -pomz2 * (q1one + ps3 * pzl);
                Naone = q1one * pomz / (.1 * pcs); /* концентрация азота в смеси */
                q1one = q1one + (pzl / 6) * (ps1 + 2 * ps2 + 2 * ps3 + ps4); // поток азота в паровй смеси
            }

            azgoned = Naone;
        }


        public struct FiberInfo
        {
            public double[] N;
            public double[] Wmidl;
            public double[] Lpvk;
            public double[] G;
            public double[] Nitro;
            public double perepad;
            public double pBS;
            public double t0;

            public int fiberNum;
        }

        public struct OutputAlgoData
        {
            public double[] azgoned;
            public double[] dP;
            public bool[] excluded;

            public static OutputAlgoData NewData()
            {
                OutputAlgoData o;
                o.excluded = new bool[115];
                o.azgoned = new double[115];
                o.dP = new double[115];
                return o;
            }
        }

        public class IntermediateData
        {
            public int algNo;
            public int startIdx;
            public int stepIdx;
            public double[] Gamma;
            public double[] OM;

            public IntermediateData(int pStartI, int pT)
            {
                Gamma = new double[120];
                OM = new double[120];

                startIdx = pStartI;
                stepIdx = pT;
            }
        }


        public static void MakeFiberPadonel(FiberInfo fi, OutputAlgoData od, IntermediateData id)
        {
            for (int i = id.startIdx; i < fi.N.Length; i += id.stepIdx)
            {
                if ((fi.N[i] > 0) && (fi.Lpvk[i] > 0) && (fi.Nitro[i] > 0) && (fi.G[i] > 0))
                {
                    od.excluded[i] = false;

                    gidra(fi.t0, fi.pBS, fi.Lpvk[i], out od.dP[i], fi.G[i], fi.N[i], ref id.Gamma, ref id.OM);
                }
                else
                {
                    od.excluded[i] = true;
                }
            }
        }

        public static void MakeFiberAzgone(FiberInfo fi, OutputAlgoData od, IntermediateData id)
        {
            for (int i = id.startIdx; i < fi.N.Length; i += id.stepIdx)
            {
                if ((fi.N[i] > 0) && (fi.Lpvk[i] > 0) && ((fi.Nitro != null && (fi.Nitro[i] > 0)) || fi.Nitro == null) && (fi.G[i] > 0))
                {
                    od.excluded[i] = false;

                    azgone(fi.t0, fi.pBS, fi.Lpvk[i], out od.dP[i], fi.G[i], fi.N[i], fi.Wmidl[i], out od.azgoned[i], ref id.Gamma, ref id.OM);
                }
                else
                {
                    od.excluded[i] = true;
                }
            }
        }


        [AttributeAlgoComponent("Расчет стартовых расходов")]
        [AttributeRules(@"prev is (rbmk_params as double[totalHeatPower] to totalHeatPower, power as double[native] to power, flow as double[native] to flow);
                          curr is (rbmk_params as double[totalHeatPower] to totalHeatPower_new, power as double[native] to power_new);
                          return is (flow_start('Стартовые расходы') as Cart(native))")]
        public static void calculateStartG(double totalHeatPower, double[,] power, double[,] flow, double totalHeatPower_new, double[,] power_new,
                                                 out double[,] flow_start)
        {
            double power_curr_to_prev = totalHeatPower_new / totalHeatPower;

            flow_start = new double[48, 48];

            for (int i = 0; i < 48; i++)
            {
                for (int j = 0; j < 48; j++)
                {
                    if (power[i, j] > 0)
                    {
                        flow_start[i, j] = (power_new[i, j] * flow[i, j]) / (power[i, j] * power_curr_to_prev);//стартовое значение расхода реальной нитки
                    }

                    if (flow_start[i, j] == 0)
                        flow_start[i, j] = 25.0;
                }
            }
        }


        [AttributeAlgoComponent("Расчет мощности окружения")]
        [AttributeRules(@"zagr is (power as double[native] to power);
                          return contains (power_midl) cast (power_midl('Мощность окружения') as Cart(native) to power_midl)")]
        public static void calculateMiddlePower(double[,] power, out double[,] power_midl)
        {            
            power_midl = new double[48, 48];

            for (int i = 0; i < 48; i++)
            {
                for (int j = 0; j < 48; j++)
                {
                    double wokr = 0.0;

                    for (int k = i - 1; k <= i + 1; k++)
                    {
                        for (int l = j - 1; l <= j + 1; l++)
                        {
                            if ((k != i) || (l != j))
                            {
                                if ((k >= 0) && (k < 48) && (l >= 0) && (l < 48))
                                {
                                    double tmp = power[k, l];
                                    if (tmp > 0)
                                        wokr += tmp;
                                }
                            }
                        }
                    }

                    power_midl[i, j] = wokr / 8;
                }
            }
        }


        [AttributeAlgoComponent("Условное присваивание коэффициентов в запрещенных расходомерах")]
        [AttributeRules(@"zagr is ( zagr as double[pvk]);
                          coeff[] is (bet1 as double[] to bet1, gamma as double[fiberNum] to fiberNum); 
                          return[] is (bet1 as Array)")]
        public static void correctUnknownAzotCoeff(double[,] zagr, double[][] bet1, double[] fiberNum)
        {
            for (int i = 0; i < fiberNum.Length; i++)
                for (int k = 0; k < bet1[i].Length; k++)
                    if ((zagr[i, k] == (double)ChannelType.TVS) && (bet1[i][k] <= 0))
                        bet1[i][k] = 1.0;
        }

        ///////////////////////////////////////////////////////////////////////////////////

        public class AzotAlgorithmBase : IResourceInstance, IDisposable
        {
            IntermediateData[] _threads;
            Thread[] _threadPool;
            Semaphore[] _threadSemPool;

            Semaphore _barrierSem;

            int _maxThreads;

            bool _disposed = false;
            Mutex _processing = new Mutex();


            FiberInfo _globalFi;
            OutputAlgoData _gloabalOd;


            class ThreadWrap
            {
                AzotAlgorithmBase _parent;
                readonly int _index;

                public ThreadWrap(AzotAlgorithmBase parent, int i)
                {
                    _parent = parent;
                    _index = i;
                }

                public void ThreadPoolProc()
                {
                    while (_parent._threadSemPool[_index].WaitOne())
                    {
                        if (_parent._threads[_index].algNo == 0)
                        {
                            EvaluteAzot.MakeFiberAzgone(_parent._globalFi,
                                                        _parent._gloabalOd,
                                                        _parent._threads[_index]);
                        }
                        else
                        {
                            EvaluteAzot.MakeFiberPadonel(_parent._globalFi,
                                                        _parent._gloabalOd,
                                                        _parent._threads[_index]);

                        }

                        _parent._barrierSem.Release();
                    }
                }
            }

            public AzotAlgorithmBase()
            {
                int maxThreads = System.Environment.ProcessorCount;

                _maxThreads = maxThreads;

                InitTask(_maxThreads);

                _threadPool = new Thread[_maxThreads];
                _threadSemPool = new Semaphore[_maxThreads];
                for (int i = 0; i < _maxThreads; i++)
                {
                    ThreadWrap tw = new ThreadWrap(this, i);
                    ThreadStart ts = new ThreadStart(tw.ThreadPoolProc);

                    _threadPool[i] = new Thread(ts);
                    _threadSemPool[i] = new Semaphore(0, 1);

                    _threadPool[i].Start();
                }

                _barrierSem = new Semaphore(0, _maxThreads);
            }

            void KillThreads()
            {
                for (int i = 0; i < _maxThreads; i++)
                {
                    _threadPool[i].Abort();
                    _threadPool[i] = null;
                }
            }

            void InitTask(int threadCount)
            {
                _threads = new IntermediateData[threadCount];
                for (int i = 0; i < threadCount; i++)
                    _threads[i] = new IntermediateData(i, threadCount);
            }

            void MakeParAlgo(FiberInfo fi, OutputAlgoData od, int alg)
            {
                if (_disposed)
                    throw new ObjectDisposedException("AzotAlgorithmBase");

                _globalFi = fi;
                _gloabalOd = od;

                for (int i = 0; i < _maxThreads; i++)
                {
                    _threads[i].algNo = alg;
                    _threadSemPool[i].Release();
                }

                for (int i = 0; i < _maxThreads; i++)
                    _barrierSem.WaitOne();


                _gloabalOd = new OutputAlgoData();
                _globalFi = new FiberInfo();
            }

            void MakeParFiberAzgone(FiberInfo fi, OutputAlgoData od)
            {
                MakeParAlgo(fi, od, 0);
            }

            void MakeParFiberPadonel(FiberInfo fi, OutputAlgoData od)
            {
                MakeParAlgo(fi, od, 1);
            }

            public void RecoverAzot(FiberInfo[] localInData, double[][] cbet1, double[][] ckdp, double[] gamma, out double[][] flow_az, out double[][] flow_dp, double precision)
            {
                flow_az = new double[localInData.Length][];
                flow_dp = new double[localInData.Length][];

                double step = 5;
                int start = 10;
                int end = 55;

                for (int i = 0; i < localInData.Length; i++)
                {
                    int dstep = (int)((end - start) / step) + 1;
                    OutputAlgoData[] od = new OutputAlgoData[dstep];

                    double[] bet1 = (double[])cbet1[i];
                    double[] kdp = (double[])ckdp[i];

                    double[] Nitro = new double[115];
                    for (int k = 0; k < 115; k++)
                        Nitro[k] = localInData[i].Nitro[k] * gamma[i] / bet1[k];

                    int[] excluded = new int[115];
                    double[][] g = new double[dstep][];
                    double[][] deltaN = new double[dstep][];

                    double[] g_max = new double[115];
                    double[] g_min = new double[115];
                    double[] g_mean = new double[115];
                    OutputAlgoData od_max = OutputAlgoData.NewData();
                    OutputAlgoData od_min = OutputAlgoData.NewData();
                    OutputAlgoData od_mean = OutputAlgoData.NewData();



                    for (int j = 0; j < od.Length; j++)
                    {
                        g[j] = new double[115];
                        deltaN[j] = new double[115];

                        od[j] = OutputAlgoData.NewData();
                        for (int k = 0; k < 115; k++)
                            g[j][k] = start + step * j;

                    }

                    for (int j = 0; j < od.Length; j++)
                    {
                        localInData[i].G = g[j];

                        MakeParFiberAzgone(localInData[i], od[j]);

                        for (int k = 0; k < 115; k++)
                            if (!od[j].excluded[k])
                                deltaN[j][k] = od[j].azgoned[k] - Nitro[k];
                    }

                    for (int k = 0; k < 115; k++)
                    {
                        int firstRoot = 0;
                        int cnt = 0;
                        int prev = Math.Sign(deltaN[0][k]);
                        for (int j = 1; j < od.Length; j++)
                        {
                            if (Math.Sign(deltaN[j][k]) != prev)
                            {
                                if (firstRoot == 0)
                                    firstRoot = j;
                                cnt++;
                            }
                            prev = Math.Sign(deltaN[j][k]);
                        }
                        excluded[k] = cnt;
                        if (cnt > 1)
                        {
                            Console.WriteLine(String.Format("Double {0} - {1}", i, k));

                            od_min.excluded[k] = true;
                            od_max.excluded[k] = true;
                            od_mean.excluded[k] = true;
                        }
                        if (firstRoot > 0)
                        {
                            g_max[k] = g[firstRoot][k];
                            g_min[k] = g[firstRoot - 1][k];

                            od_max.azgoned[k] = od[firstRoot].azgoned[k];
                            od_min.azgoned[k] = od[firstRoot - 1].azgoned[k];

                            g_mean[k] = (g_max[k] + g_min[k]) / 2;
                        }
                    }

                    int iteration = (int)Math.Ceiling(Math.Log(step / precision, 2));

                    for (int iter = 0; iter < iteration; iter++)
                    {
                        localInData[i].G = g_mean;

                        MakeParFiberAzgone(localInData[i], od_mean);

                        for (int k = 0; k < 115; k++)
                        {
                            if (excluded[k] != 0)
                            {
                                double dmax = od_max.azgoned[k] - Nitro[k];
                                double dmean = od_mean.azgoned[k] - Nitro[k];
                                double dmin = od_min.azgoned[k] - Nitro[k];

                                if (Math.Sign(dmax) != Math.Sign(dmean))
                                {
                                    g_min[k] = g_mean[k];
                                    od_min.azgoned[k] = od_mean.azgoned[k];

                                    g_mean[k] = (g_mean[k] + g_max[k]) / 2;
                                }
                                else
                                {
                                    g_max[k] = g_mean[k];
                                    od_max.azgoned[k] = od_max.azgoned[k];

                                    g_mean[k] = (g_mean[k] + g_min[k]) / 2;
                                }
                            }
                        }
                    }

                    //flow_az[i] = new DataArray("flow_az1", "Восстановленный расход по азоту", t, (double[])g_mean.Clone());
                    //flow_az[i] = g_mean;
                    flow_az[i] = (double[])g_mean.Clone();
#if EXCLUDE_DOUBLE
                    for (int k = 0; k < 115; k++)
                        if (excluded[k]>1)
                            flow_az[i][k] = 0;
#endif
                    ///////////////////////////////////////////////////////////////////////////////////
                    //Восстановление по перепаду 

                    for (int k = 0; k < 115; k++)
                    {
                        int firstRoot = 0;
                        int cnt = 0;
                        int prev = Math.Sign(localInData[i].perepad - (od[0].dP[k] / 10000) - kdp[k] * g[0][k] * g[0][k]);
                        for (int j = 1; j < od.Length; j++)
                        {
                            int tmp;
                            if ((tmp = Math.Sign(localInData[i].perepad - (od[j].dP[k] / 10000) - kdp[k] * g[j][k] * g[j][k])) != prev)
                            {
                                if (firstRoot == 0)
                                    firstRoot = j;
                                cnt++;
                            }
                            prev = tmp;
                        }
                        excluded[k] = cnt;
                        if (cnt > 1)
                        {
                            Console.WriteLine(String.Format("DP_Double{0} - {1}", i, k));
                        }
                        if (firstRoot > 0)
                        {
                            g_max[k] = g[firstRoot][k];
                            g_min[k] = g[firstRoot - 1][k];


                            od_max.dP[k] = od[firstRoot].dP[k];
                            od_min.dP[k] = od[firstRoot - 1].dP[k];

                            g_mean[k] = (g_max[k] + g_min[k]) / 2;
                        }
                    }

                    for (int iter = 0; iter < iteration; iter++)
                    {
                        localInData[i].G = g_mean;

                        MakeParFiberPadonel(localInData[i], od_mean);

                        for (int k = 0; k < 115; k++)
                        {
                            if (excluded[k] != 0)
                            {
                                double dmax = localInData[i].perepad - (od_max.dP[k] / 10000) - kdp[k] * g_max[k] * g_max[k];
                                double dmean = localInData[i].perepad - (od_mean.dP[k] / 10000) - kdp[k] * g_mean[k] * g_mean[k];
                                double dmin = localInData[i].perepad - (od_min.dP[k] / 10000) - kdp[k] * g_min[k] * g_min[k];

                                if (Math.Sign(dmax) != Math.Sign(dmean))
                                {
                                    g_min[k] = g_mean[k];
                                    od_min.azgoned[k] = od_mean.azgoned[k];

                                    g_mean[k] = (g_mean[k] + g_max[k]) / 2;
                                }
                                else
                                {
                                    g_max[k] = g_mean[k];
                                    od_max.azgoned[k] = od_max.azgoned[k];

                                    g_mean[k] = (g_mean[k] + g_min[k]) / 2;
                                }
                            }
                        }
                    }

                    //flow_dp[i] = new DataArray("flow_dp", "Восстановленный расход по DP", t, g_mean);
                    flow_dp[i] = g_mean;
                }

            }


            public void CalcGradCoeffNew(FiberInfo[] localInData, out double[][] kpd, out double[][] bet1, out double[] gamma)
            {
                kpd = new double[localInData.Length][];
                bet1 = new double[localInData.Length][];
                gamma = new double[localInData.Length];
                OutputAlgoData[] outData = new OutputAlgoData[localInData.Length];

                for (int i = 0; i < localInData.Length; i++)
                {
                    outData[i] = OutputAlgoData.NewData();

                    MakeParFiberAzgone(localInData[i], outData[i]);

                    kpd[i] = new double[115];
                    bet1[i] = new double[115];
                }

                for (int i = 0; i < localInData.Length; i++)
                {
                    double Bev = 0;
                    double B = 0;

                    double[] kpd_i = (double[])kpd[i];
                    double[] bet1_i = (double[])bet1[i];

                    for (int j = 0; j < 115; j++)
                    {
                        if (!outData[i].excluded[j])
                        {
                            B += localInData[i].Nitro[j];
                            Bev += outData[i].azgoned[j];

                            kpd_i[j] = (localInData[i].perepad - (outData[i].dP[j] / 10000)) / (localInData[i].G[j] * localInData[i].G[j]);
                        }
                        else
                        {
                            kpd_i[j] = 0;
                            bet1_i[j] = 0;
                        }
                    }

                    gamma[i] = Bev / B;

                    for (int j = 0; j < 115; j++)
                        if (!outData[i].excluded[j])
                            bet1_i[j] = (localInData[i].Nitro[j] * (gamma[i])) / outData[i].azgoned[j];
                }
            }


            static FiberInfo[] FillStandartDataNew(int[] fiberNum, double[][] pvk_maxes,
                                                   double[,] power, double[,] power_midl, double[,] pvk_length,
                double pLeftBS, double pLeftNK, double tLeftVK,
                double pRightBS, double pRightNK, double tRightVK)
            {
                int fibers = (fiberNum == null) ? 16 : fiberNum.Length;
                FiberInfo[] _inData = new FiberInfo[fibers];

                for (int i = 0; i < fiberNum.Length; i++)
                {
                    _inData[i].N = new double[115];
                    _inData[i].Wmidl = new double[115];
                    _inData[i].Lpvk = new double[115];
                    _inData[i].G = new double[115];

                    int fiber = _inData[i].fiberNum = (fiberNum == null) ? i : fiberNum[i];

                    _inData[i].Nitro = pvk_maxes[i];
                    int n;
#if CORRECT
                    double m = -corelib.MathOp.IntMax(_inData[i].Nitro, 0, _inData[i].Nitro.Length, out n) * 0.025;
                    for (int j = 0; j < 115; j++)
                        _inData[i].Nitro[j] = (_inData[i].Nitro[j] < m) ? 0 : _inData[i].Nitro[j] - m;
#endif

                    FillPvk(power, fiber, _inData[i].N);
                    FillPvk(power_midl, fiber, _inData[i].Wmidl);
                    FillPvk(pvk_length, fiber, _inData[i].Lpvk);

                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // FIXME
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (fiber > 7)
                    {
                        _inData[i].pBS = pLeftBS;
                        _inData[i].perepad = pLeftNK - _inData[i].pBS;
                        _inData[i].t0 = tLeftVK;
                    }
                    else
                    {
                        _inData[i].pBS = pRightBS;
                        _inData[i].perepad = pRightNK - _inData[i].pBS;
                        _inData[i].t0 = tRightVK;
                    }
                }

                return _inData;
            }

            public static void FillPvk(double[,] source, int pvk, double[] single)
            {
                for (int i = 0; i < 115; i++)
                {
                    single[i] = source[pvk, i];
                }
            }
            


            [AttributeAlgoComponent("Восстановление расходов")]
            [AttributeRules(
       @"     kgo[] is (kgoprp_info as int[fiberNum], pvk_maxes as double[115],
                        kpd as double[115], bet1 as double[115], gamma as double[gamma] );
              zagr is (power as double[pvk] to power,
                       power_midl as double[pvk] to power_midl, pvk_length as double[pvk] to pvk_length , 
                       rbmk_params as double[pLeftBS1] to pLeftBS1,
                       rbmk_params as double[pLeftBS2] to pLeftBS2,
                       rbmk_params as double[pLeftNK] to pLeftNK,
                       rbmk_params as double[tLeftVK1] to tLeftVK1,
                       rbmk_params as double[tLeftVK2] to tLeftVK2,
                       rbmk_params as double[pRightBS1] to pRightBS1,
                       rbmk_params as double[pRightBS2] to pRightBS2,
                       rbmk_params as double[pRightNK] to pRightNK,
                       rbmk_params as double[tRightVK1] to tRightVK1,
                       rbmk_params as double[tRightVK2] to tRightVK2   );
              return[] is (flow_az1('Восстановленный расход по азоту') as Array, flow_dp('Восстановленный расход по DP') as Array)")]
            public void evaluteAzotDpFlow(int[] kgoprp_info, double[][] pvk_maxes, double[][] kpd, double[][] bet1, double[] gamma,
                                           double[,] power, double[,] power_midl, double[,] pvk_length,
                double pLeftBS1, double pLeftBS2, double pLeftNK, double tLeftVK1, double tLeftVK2,
                double pRightBS1, double pRightBS2, double pRightNK, double tRightVK1, double tRightVK2,
                                           out double[][] flow_az1, out double[][] flow_dp)
            {
                if (gamma.Length != 16)
                    throw new ArgumentException();

                _processing.WaitOne();

                FiberInfo[] inData;

                double pLeftBS = (pLeftBS1 + pLeftBS2) / 2;
                double tLeftVK = (tLeftVK1 + tLeftVK2) / 2;

                double pRightBS = (pRightBS1 + pRightBS2) / 2;
                double tRightVK = (tRightVK1 + tRightVK2) / 2; 

                inData = FillStandartDataNew(kgoprp_info, pvk_maxes, power, power_midl, pvk_length, pLeftBS, pLeftNK, tLeftVK, pRightBS, pRightNK, tRightVK);

                RecoverAzot(inData, bet1, kpd, gamma, out flow_az1, out flow_dp, 0.01);

                _processing.ReleaseMutex();
            }


            [AttributeAlgoComponent("Оценка коэффициентов адаптации")]
            [AttributeRules(
           @"kgo[] is (kgoprp_info as int[fiberNum], pvk_maxes as double[115]);
              zagr is (flow as double[pvk] to flow, power as double[pvk] to power,
                       power_midl as double[pvk] to power_midl, pvk_length as double[pvk] to pvk_length , 
                       rbmk_params as double[pLeftBS1] to pLeftBS1,
                       rbmk_params as double[pLeftBS2] to pLeftBS2,
                       rbmk_params as double[pLeftNK] to pLeftNK,
                       rbmk_params as double[tLeftVK1] to tLeftVK1,
                       rbmk_params as double[tLeftVK2] to tLeftVK2,
                       rbmk_params as double[pRightBS1] to pRightBS1,
                       rbmk_params as double[pRightBS2] to pRightBS2,
                       rbmk_params as double[pRightNK] to pRightNK,
                       rbmk_params as double[tRightVK1] to tRightVK1,
                       rbmk_params as double[tRightVK2] to tRightVK2   );
              return[] is (kpd('Коэффициент адаптации по давлению') as Array to kpd, 
                           bet1('Коэффициент адаптации по азоту') as Array to bet1,
                           gamma as ParamTable[gamma, fiberNum] to (gamma, kgoprp_info) )")]
            public void evaluteAzotDpCoeff(int[] kgoprp_info, double[][] pvk_maxes,
                                           double[,] flow, double[,] power, double[,] power_midl, double[,] pvk_length,
                double pLeftBS1, double pLeftBS2, double pLeftNK, double tLeftVK1, double tLeftVK2,
                double pRightBS1, double pRightBS2, double pRightNK, double tRightVK1, double tRightVK2,
                                           out double[][] kpd, out double[][] bet1, out double[] gamma)
            {
                _processing.WaitOne();

                double pLeftBS = (pLeftBS1 + pLeftBS2) / 2; 
                double tLeftVK = (tLeftVK1 + tLeftVK2) / 2;

                double pRightBS = (pRightBS1 + pRightBS2) / 2;
                double tRightVK = (tRightVK1 + tRightVK2) / 2; 

                FiberInfo[] inData;

                inData = FillStandartDataNew(kgoprp_info, pvk_maxes, power, power_midl, pvk_length, pLeftBS, pLeftNK, tLeftVK, pRightBS, pRightNK, tRightVK);
                for (int i = 0; i < kgoprp_info.Length; i++)
                {
                    FillPvk(flow, kgoprp_info[i], inData[i].G);
                }
                
                CalcGradCoeffNew(inData, out kpd, out bet1, out gamma);

                _processing.ReleaseMutex();
            }

            [AttributeAlgoComponent("Расчет азотной активности по модели")]
            [AttributeRules(@"zagr is (flow as double[pvk] to flow, power as double[pvk] to power,
                       power_midl as double[pvk] to power_midl, pvk_length as double[pvk] to pvk_length , 
                       rbmk_params as double[pLeftBS1] to pLeftBS1,
                       rbmk_params as double[pLeftBS2] to pLeftBS2,
                       rbmk_params as double[pLeftNK] to pLeftNK,
                       rbmk_params as double[tLeftVK1] to tLeftVK1,
                       rbmk_params as double[tLeftVK2] to tLeftVK2,
                       rbmk_params as double[pRightBS1] to pRightBS1,
                       rbmk_params as double[pRightBS2] to pRightBS2,
                       rbmk_params as double[pRightNK] to pRightNK,
                       rbmk_params as double[tRightVK1] to tRightVK1,
                       rbmk_params as double[tRightVK2] to tRightVK2   );
                          return[] is (nitro('Расчетное значение активности') as Array)")]
            public void calculateNitroModel(double[,] flow, double[,] power, double[,] power_midl, double[,] pvk_length,
                double pLeftBS1, double pLeftBS2, double pLeftNK, double tLeftVK1, double tLeftVK2,
                double pRightBS1, double pRightBS2, double pRightNK, double tRightVK1, double tRightVK2,
                double[][] nitro)
            {
                double pLeftBS = (pLeftBS1 + pLeftBS2) / 2;
                double tLeftVK = (tLeftVK1 + tLeftVK2) / 2;

                double pRightBS = (pRightBS1 + pRightBS2) / 2;
                double tRightVK = (tRightVK1 + tRightVK2) / 2; 
                
                _processing.WaitOne();

                FiberInfo[] localInData = FillStandartDataNew(null, null, power, power_midl, pvk_length, pLeftBS, pLeftNK, tLeftVK, pRightBS, pRightNK, tRightVK);
                nitro = new double[localInData.Length][];
                OutputAlgoData[] outData = new OutputAlgoData[localInData.Length];

                for (int i = 0; i < localInData.Length; i++)
                {
                    outData[i] = OutputAlgoData.NewData();

                    FillPvk(flow, i, localInData[i].G);

                    MakeParFiberAzgone(localInData[i], outData[i]);

                    nitro[i] = outData[i].azgoned;
                }
                _processing.ReleaseMutex();                
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (!_disposed)
                {
                    KillThreads();
                    _disposed = true;
                }
            }

            #endregion


        }
    }
}

