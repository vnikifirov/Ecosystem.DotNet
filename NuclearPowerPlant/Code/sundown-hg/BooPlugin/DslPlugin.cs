using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using corelib;

using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.Steps.MacroProcessing;
using Boo.Lang.Compiler.IO;

using Rhino.DSL;

using ICSharpCode.TextEditor;

using Assembly = System.Reflection.Assembly;

using WindowsApplication1;
using BooPlugin;

namespace BooPlugin
{
    public class DslBooPlugin
    {
        CompilerContext _cc;
        public readonly string Name;

        private DslBooPlugin()
        {
        }

        public string StringErrors
        {
            get
            {
                return _cc.Errors.ToString(true);
            }
        }

        public CompilerContext CompilerContext
        {
            get
            {
                return _cc;
            }
        }

        public DslBooPlugin(string name, CompilerContext cc)
        {
            Name = name;
            _cc = cc;
        }

        static public DslBooPlugin FromString(string str, Type baseType, string filename, string name)
        {
            // DSL
            BooCompiler compiler = new BooCompiler();
            compiler.Parameters.Pipeline = new CompileToMemory();
            compiler.Parameters.OutputType = CompilerOutputType.Library;
            compiler.Parameters.GenerateInMemory = true;
            compiler.Parameters.Debug = true;
            compiler.Parameters.Ducky = true;

            compiler.Parameters.Input.Add(new StringInput(filename, str));

            compiler.Parameters.Pipeline.Insert(1,
                new ImplicitBaseClassCompilerStep(
                // the base type
                    baseType,
                // the method to override
                    "onClick",
                // import the following namespaces
                    "WindowsApplication1",
                    "Algorithms",
                    "corelib",
                    "System",
                    "BooPlugin"));


            compiler.Parameters.Pipeline.Insert(2, new AutoReferenceFilesCompilerStep());
            compiler.Parameters.Pipeline.Insert(3, new UseCoordsSymbolsStep());

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                compiler.Parameters.References.Add(asm);
            }

            compiler.Parameters.Strict = false;

            CompilerContext cc = compiler.Run();
            DslBooPlugin bp = new DslBooPlugin(name, cc);
            return bp;
        }


    }

    class DslADTVUI : ActionDataTupleVisualizerUI
    {
    }

    public class DslBooPluginGen
    {
        DslBooPlugin _plugin;
        ActionVisualizerUI _action;


        public DslBooPluginGen(string filename)
        {
            StreamReader r = new StreamReader(filename);

            string header = r.ReadLine().Substring(1).Trim();
            string desc = r.ReadLine().Substring(1).Trim();
            string humane = r.ReadLine().Substring(1).Trim();

            string[] plugin_det = header.Split('-');
            if (plugin_det[0].ToLower().StartsWith("skip"))
                return;
            else if (plugin_det[0].ToLower() != "plugin")
                throw new ArgumentException("Неизвестный тип DSL");


            switch (plugin_det[1].ToLower())
            {
                case "data":
                    ActionDataTupleVisualizerUI maction = new DslADTVUI();

                    switch (plugin_det[2].ToLower())
                    {
                        case "item_context_menu":
                            maction._action = ActionDataTupleVisualizerUI.Actions.TupleItemContextMenu;
                            break;
                        case "tuple_context_menu":
                            maction._action = ActionDataTupleVisualizerUI.Actions.DataTupleContextMenu;
                            break;
                        case "stream_context_menu":
                            maction._action = ActionDataTupleVisualizerUI.Actions.StreamContextMenu;
                            break;
                        case "item_changed":
                            maction._action = ActionDataTupleVisualizerUI.Actions.TupleItemCahnged;
                            break;
                        case "toolbar":
                            maction._action = ActionDataTupleVisualizerUI.Actions.ToolBarButton;
                            break;

                        default:
                            throw new ArgumentException("Неизвестный тип для tuple");
                    }
                    _action = maction;
                    break;

                default:
                    throw new ArgumentException("Неизвестный тип плагина");
            }

            _action._descr = desc;
            _action._humaneName = humane;
            _action._handler = EventHandler;
            _action._name = filename;
            _action._image = DefImages.ImageDslScript;

            string sname = System.IO.Path.GetFileNameWithoutExtension(filename);

            _plugin = DslBooPlugin.FromString("\n\n\n" + r.ReadToEnd(), typeof(AbstractPluginDslClass), filename, sname);
        }


        public ActionVisualizerUI ActionVisualizerUI
        {
            get
            {
                return _action;
            }
        }

        void EventHandler(object sender, EventArgs e)
        {
            IDataTupleVisualizerUI ui;

            if (_action is ActionDataTupleVisualizerUI)
            {
                ADTVEventArgs ev = (ADTVEventArgs)e;
                ui = ev._ui;

            }
            else if (_action is ActionDataCartogramVisualizerUI)
            {
                ADCVEventArgs ev = (ADCVEventArgs)e;
                IDataCartogramVisualizerUI cui = ev._ui;
                ui = cui.GetDataTupleVisualizer();
            }
            else
            {
                ui = null;
            }


            CompilerContext c = _plugin.CompilerContext;
            string dslErrors = c.Errors.ToString(true);

            Assembly a = c.GeneratedAssembly;
            if (a != null)
            {
                try
                {
                    AbstractPluginDslClass cls = (AbstractPluginDslClass)a.GetType(_plugin.Name)
                        .GetConstructor(new Type[2] { typeof(IDataTupleVisualizerUI), typeof(EventArgs) })
                        .Invoke(new object[2] { ui, e });

                    cls.onClick();
                }
                catch (Exception ex)
                {
                    dslErrors = String.Format("{0}\r\n{1}", dslErrors,
                        ex.ToString());
                }
            }

            if (ui != null)
                ui.SetDetailString(dslErrors);
        }

        static public System.Collections.Generic.List<DslBooPluginGen> GetScripts(string path)
        {
            System.Collections.Generic.List<DslBooPluginGen> lv = new System.Collections.Generic.List<DslBooPluginGen>();
            try
            {
                string[] files = System.IO.Directory.GetFiles(path, "*.dsl");
                foreach (string file in files)
                {
                    try
                    {
                        DslBooPluginGen z = new DslBooPluginGen(file);
                        if (z.ActionVisualizerUI != null)
                            lv.Add(z);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return lv;
        }

    }



    public abstract class AbstractPluginDslClass : AbstractDslClass
    {
        StringBuilder _log = new StringBuilder();

        public AbstractPluginDslClass(IDataTupleVisualizerUI obj, EventArgs e)
            : base(obj)
        {
            
        }


        public override void print(params object[] str)
        {
            foreach (object s in str)
            {
                _log.Append(s.ToString());
            }

            _obj.SetDetailString(_log.ToString());
        }

        public override void status(object s)
        {
            _obj.SetStatusString(s.ToString());
        }

        public override void log(object s)
        {
            _log.Append(s.ToString());
            _obj.SetDetailString(_log.ToString());
        }
    }
}
