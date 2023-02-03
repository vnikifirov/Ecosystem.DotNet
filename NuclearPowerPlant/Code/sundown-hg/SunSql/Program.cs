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
using RockMicoPlugin;
using PgSqlStorage;

namespace SunSql
{
    public class Program
    {
       [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}