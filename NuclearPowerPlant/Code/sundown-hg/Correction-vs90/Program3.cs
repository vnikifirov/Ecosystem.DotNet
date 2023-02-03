using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using RockMicoPlugin;
using corelib;




namespace Correction_vs90
{


    public interface Ian
    {
        void Analyze();
    }

    class TT : Ian
    {
        //RockMicroSingleProvider skala;
        //DataTupleProvider azot;
        public int a;
        public int b;
        public TT(int i, int j)
        {
            a = i;
            b = j;
        }

       

        #region Ian Members

        public void Analyze()
        {
           
            //throw new NotImplementedException();
        }

        #endregion
    }

    /*class RockAn : Ian
    {
        public void Analyze()
        {
            //Form1 obF1 = new Form1(sPath);

            //Application.Run(obF1);
           
        }
    }

    class AzotAn : Ian
    {
        public void Analyze()
        {
            //Form1 obF1 = new Form1(sPath);

            //Application.Run(obF1);

        }
    }*/


           

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //TT t = new TT();
            //List<TT> c;
           // c= new List<TT>();
            //c.Add(t);
          
      
            /*List<int> myInts = new List<int>();
            myInts.Add(5);

            // Без восстановления значения! 
            int i = myInts[0];*/ 
            
           
            
            
             
            

            Application.Run(new Form1());
        }
    }
}
