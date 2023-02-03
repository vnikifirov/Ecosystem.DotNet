using System;
using System.Text;

namespace corelib
{
    public class MathOp
    {
        public static double[] Accor(double[] ar, int n)
        {
            int p = Math.Min(ar.Length, n);
            double mean = IntMean(ar, 0, p);
            double[] acc = new double[p];

            int i, j;

            for (i = 0; i < p; i++)
            {
                double m = 0;
                for (j = i; j < p; j++)
                {
                    m += (ar[j] - mean) * (ar[j - i] - mean);
                }
                acc[i] = m;
            }
            return acc;
        }

        public static double IntMean(int[] ar, int st, int en)
        {
            int sum = 0, k = 0;
            int i;
            for (i = (st < 0) ? 0 : st; i <= en && i < ar.Length; i++, k++)
            {
                sum += ar[i];
            }
            return (double)sum / k;
        }

        public static double IntMean(double[] ar, int st, int en)
        {
            double sum = 0;
            int k = 0;
            int i;
            for (i = (st < 0) ? 0 : st; i <= en && i < ar.Length; i++, k++)
            {
                sum += ar[i];
            }
            return (double)sum / k;
        }

        public static double Mean(double[] ar)
        {
            return IntMean(ar, 0, ar.Length);
        }

                public static double Var(double[] ar, out double mn)
                {
                    return IntVar(ar, 0, ar.Length, out mn);
                }

                public static double Var(double[] ar)
                {
                    double mn;
                    return IntVar(ar, 0, ar.Length, out mn);
                }

        public static double Korr(double[] x, double[] y)
        {
            if (x.Length != y.Length)
                throw new ArgumentException();

            double mx = Mean(x);
            double my = Mean(y);

            double tmp = 0;
            for (int i = 0; i < x.Length; i++)
                tmp += (x[i] - mx)*(y[i] - my);

            return tmp / x.Length;
        }

        public static double IntVar(double[] ar, int st, int en, out double mn)
        {
            double mean = IntMean(ar, st, en);
            double var = 0;
            int i, k = 0;
            for (i = (st < 0) ? 0 : st; i <= en && i < ar.Length; i++, k++)
            {
                var += (ar[i] - mean) * (ar[i] - mean);
            }
            mn = mean;
            return var / k;
        }

        public static int IntMax(int[] ar, int st, int en, out int imax)
        {
            if (st < 0)
                st = 0;

            int mx = ar[st], imx = st;
            int i;

            for (i = st + 1; (i < ar.Length) && (i <= en); i++)
            {
                if (ar[i] > mx)
                {
                    mx = ar[i];
                    imx = i;
                }
            }
            imax = imx;
            return mx;
        }

        public static double IntMax(double[] ar, int st, int en, out int imx)
        {
            if (st < 0)
                st = 0;

            double mx = ar[st]; imx = st;
            int i;

            for (i = st + 1; (i < ar.Length) && (i <= en); i++)
            {
                if (ar[i] > mx)
                {
                    mx = ar[i];
                    imx = i;
                }
            }
            return mx;
        }


        public static double IntMin(double[] ar, int st, int en, out int imn)
        {
            if (st < 0)
                st = 0;

            double mn = ar[st]; imn = st;
            int i;

            for (i = st + 1; (i < ar.Length) && (i <= en); i++)
            {
                if (ar[i] < mn)
                {
                    mn = ar[i];
                    imn = i;
                }
            }
            return mn;
        }

        public static double max(double[] ar)
        {
            int dummy;
            return IntMax(ar, 0, ar.Length, out dummy);
        }

        public static double min(double[] ar)
        {
            int dummy;
            return IntMin(ar, 0, ar.Length, out dummy);
        }
    }
}
