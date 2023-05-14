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

namespace WindowsApplication1
{
#if !DOTNET_V11
    using DataArrayDouble = DataArray<double>;
#endif

    public partial class DslManageWindow : Form
    {
        IDataTupleVisualizerUI _ui;
        bool _savedFlag = true;
        bool _closed = false;
        bool _indirectLogging = true;

        TextEditorControl dslText;

        public void SetUI(IDataTupleVisualizerUI ui)
        {
            _ui = ui;
        }

        public DslManageWindow()
        {
            InitializeComponent();

            dslText = new TextEditorControl();
            dslText.Dock = DockStyle.Fill;
            dslText.SetHighlighting("Boo");

            dslText.TextEditorProperties.ShowTabs = true;
            //dslText.TextEditorProperties.IsIconBarVisible = true;
            //dslText.TextEditorProperties.EnableFolding = true;
            

            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(dslText);
        }

        private void DslManageWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _closed = true;
        }
        private void DslManageWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !CheckSaved();
        }

        public void log(object s, bool indirect)
        {
            if (!_indirectLogging && indirect)
                return;

            dslErrors.Text = String.Format("{0}{1}\r\n", dslErrors.Text,
                        s.ToString());
        }

        public bool CheckSaved()
        {
            if (_closed)
                return true;

            if (_savedFlag)
                return true;

            DialogResult r = MessageBox.Show("Сценарий был изменен, сохранить?", this.Text, MessageBoxButtons.YesNoCancel);
            if (r == DialogResult.Yes)
            {
                btSave_Click(null, null);

                return _savedFlag;
            }
            else if (r == DialogResult.No)
                return true;

            return false;
        }

        private void dslText_TextChanged(object sender, EventArgs e)
        {
            _savedFlag = false;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            DialogResult r = saveFileDialog1.ShowDialog();
            if (r == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter s = new StreamWriter(saveFileDialog1.FileName))
                    {
                        s.Write(dslText.Text);
                    }

                    _savedFlag = true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка");
                }
            }
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            if (CheckSaved())
            {
                DialogResult r = openFileDialog1.ShowDialog();
                if (r == DialogResult.OK)
                {
                    try
                    {
                        using (StreamReader s = new StreamReader(openFileDialog1.FileName))
                        {
                            dslText.Text = s.ReadToEnd();
                        }

                        _savedFlag = true;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Ошибка");
                    }
                }
            }
        }


        private void btRun_Click(object sender, EventArgs e)
        {
            CompilerContext c = doDsl(dslText.Text);

            dslErrors.Text = c.Errors.ToString(true);

            Assembly a = c.GeneratedAssembly;
            if (a != null)
            {
                try
                {
                    AbstractDslClass cls = (AbstractDslClass)a.GetType("mydsl")
                        .GetConstructor(new Type[2] { typeof(IDataTupleVisualizerUI), typeof(DslManageWindow) })
                        .Invoke(new object[2] { _ui, this });

                    cls.onClick();
                }
                catch (Exception ex)
                {
                    dslErrors.Text = String.Format("{0}\r\n{1}", dslErrors.Text,
                        ex.ToString());
                }
            }

        }

        private CompilerContext doDsl(string text)
        {
            // DSL
            BooCompiler compiler = new BooCompiler();
            compiler.Parameters.Pipeline = new CompileToMemory();
            compiler.Parameters.OutputType = CompilerOutputType.Library;
            compiler.Parameters.GenerateInMemory = true;
            compiler.Parameters.Debug = true;
            compiler.Parameters.Ducky = true;

            compiler.Parameters.Input.Add(new StringInput("mydsl", text));

            compiler.Parameters.Pipeline.Insert(1,
                new ImplicitBaseClassCompilerStep(
                // the base type
                    typeof(AbstractScriptDslClass),
                // the method to override
                    "onClick",
                // import the following namespaces
                    "WindowsApplication1",
                    "corelib",
                    "System"));


            compiler.Parameters.Pipeline.Insert(2, new AutoReferenceFilesCompilerStep());
            compiler.Parameters.Pipeline.Insert(3, new UseCoordsSymbolsStep());

            //compiler.Parameters.References.Add(typeof(Form).Assembly);
            //compiler.Parameters.References.Add(typeof(IDataTuple).Assembly);
            //compiler.Parameters.References.Add(typeof(corelib.IDataTupleVisualizerUI).Assembly);

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                compiler.Parameters.References.Add(asm);
            }

            compiler.Parameters.Strict = false;

            return compiler.Run();
        }

    }

    public class UseCoordsSymbolsStep : AbstractTransformerCompilerStep
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            Visit(CompileUnit);
        }

        /// <summary>
        /// Called when visiting a reference expression.
        /// Will turn reference expressions with initial @ to Coords objects
        /// </summary>
        /// <param name="node">The node.</param>
        public override void OnReferenceExpression(ReferenceExpression node)
        {
            if (node.Name.StartsWith("@c") == false)
                return;

            string toReplace = node.Name.Substring(2);
            string coordsString = null;
            if (toReplace.Length == 4)
            {
                coordsString = toReplace.Substring(0, 2) + "-" + toReplace.Substring(2, 2);
            }
            else if (toReplace.Length == 5)
            {
                coordsString = toReplace;
            }

            if (toReplace != null)
            {
                ReplaceCurrentNode(new MethodInvocationExpression(
                    new MemberReferenceExpression(new ReferenceExpression("Coords"), "FromHumane"),
                    new StringLiteralExpression(node.LexicalInfo, coordsString)));
            }
        }

        public override void OnSpliceExpression(SpliceExpression node)
        {
            int i = 0;
            return;
        }
    }


    public abstract class AbstractScriptDslClass : AbstractDslClass
    {
        DslManageWindow _parentWnd;

        public AbstractScriptDslClass(IDataTupleVisualizerUI obj, DslManageWindow parentWnd) : base(obj)
        {
            _parentWnd = parentWnd;
        }


        public override void print(params object[] str)
        {
            StringBuilder b = new StringBuilder();

            foreach (object s in str)
            {
                b.Append(s.ToString());
            }

            _obj.SetDetailString(b.ToString());
            _parentWnd.log(b.ToString(), true);
        }

        public override void status(object s)
        {
            _obj.SetStatusString(s.ToString());
            _parentWnd.log(s, true);
        }

        public override void log(object s)
        {
            _parentWnd.log(s, false);
        }
    }
}