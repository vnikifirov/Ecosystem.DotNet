using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;



using System.IO;

using corelib;

using WindowsApplication1;
//using Correction_vs90;


namespace CorrectionPlugin_vs90
{
    public class DTVPluginCorrections : ActionDataTupleVisualizerUI
    {
        //DslManageWindow _window;

        public DTVPluginCorrections()
        {
            _name = "DTVPluginCorrections";
            _humaneName = "Разбор";
            _descr = "Разбор данных(срезы скалы, азотная прописка)";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageOpen;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            IEnviroment env = ui.GetEnviroment();
            FormCorrectionPlugin frm = new FormCorrectionPlugin();
            frm.env = (BasicEnv)env;
            frm.ShowDialog();


            /* Form1 frm = new Form1();
             frm.env = env;

             Application.Run(frm);*/
            //Application.Run(frm);

        }
    }
}


