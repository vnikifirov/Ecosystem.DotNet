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
using BooPlugin;

namespace WindowsApplication1
{
    public class DTVPluginDsl : ActionDataTupleVisualizerUI
    {
        DslManageWindow _window;

        public DTVPluginDsl()
        {
            _name = "DslPlugin";
            _humaneName = "DSL";
            _descr = "Запустить сценарий DSL";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageDSL;
        }

        public override void OnRegistered(IDataTupleVisualizerUI ui)
        {
            IEnviroment env = ui.GetEnviroment();
            string path = (env != null) ? (string)env.ParamTuple.GetParamSafe("booBluginPath") : null;
            List<DslBooPluginGen> gv = DslBooPluginGen.GetScripts(path != null ?  path : "dsl" );

            foreach (DslBooPluginGen g in gv)
            {
                if (g.ActionVisualizerUI is ActionDataTupleVisualizerUI)
                {
                    ui.RegisterAction((ActionDataTupleVisualizerUI)g.ActionVisualizerUI);
                }
            }
        }



        void ctrl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
        }

        void ctrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_window.CheckSaved();
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;

            _window = new DslManageWindow();
            _window.SetUI(ui);
            _window.Show();

            Form ctrl = (Form)((Control)ui).Parent;
            ctrl.FormClosed += new FormClosedEventHandler(ctrl_FormClosed);
            ctrl.FormClosing += new FormClosingEventHandler(ctrl_FormClosing);
        }
    }
}
