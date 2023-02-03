using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using System.Windows.Forms;

namespace corelib
{
    public interface IParameterVisualizer
    {
        void Init(AutoParameterInfo info);
        object Value
        {
            get;
            set;
        }
    }

    public class ParametersListVisualizerForm : Form
    {
        ParametersListVisualizer _vis;
        Button _buttonOk = new Button();
        Label _help = new Label();

        TableLayoutPanel _panel = new TableLayoutPanel();

        DataParamTable _result;

        public ParametersListVisualizerForm(AutoParameterInfo[] parameters, DataParamTable preInfo, bool showAll, string helpString)
        {
            SuspendLayout();

            _vis = new ParametersListVisualizer(parameters, preInfo, showAll);
            _vis.Dock = DockStyle.Fill;

            _help.Text = helpString;
			_panel.ColumnCount = 1;
			_panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _panel.RowCount = 3;
			_panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
			_panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			_panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            _panel.Controls.Add(_help, 0, 0);
            _panel.Controls.Add(_vis, 0, 1);
            _panel.Controls.Add(_buttonOk, 0, 2);

            _panel.Dock = DockStyle.Fill;

            _buttonOk.Text = "&Ok";
            _buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            
            _help.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _help.Dock = DockStyle.Fill;

            Controls.Add(_panel);
            ResumeLayout();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try 
            {
                _result = _vis.GetParameters();
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                MessageBox.Show(String.Format("Ошибка при проверке введенных данных:\r\n{0}", ex.Message));
            }
            base.OnClosing(e);
        }

        public DataParamTable GetParameters()
        {
            if (_result == null)
                throw new ArgumentException("Не были заданы параметры");
            return _result;
        }
    }

    public class ParametersListVisualizer : Panel
    {
        DataParamTable _init;
        AutoParameterInfo[] _actualParameters;

        TableLayoutPanel _panel = new TableLayoutPanel();        

        public static bool IsShowable(AutoParameterInfo p)
        {
            switch (p.InfoType)
            {
                case AutoParameterInfoType.Bool:
                case AutoParameterInfoType.FilePath:
                case AutoParameterInfoType.DirectoryPath:
                case AutoParameterInfoType.Password:
                case AutoParameterInfoType.String:
                case AutoParameterInfoType.List:
                    return true;

                default:
                    return false;
            }
        }

        void InitItem(int i, AutoParameterInfo par, object defValue)
        {
            Label l = new Label();
            l.Text = par.Help;
#if !DOTNET_V11
            l.MinimumSize = new System.Drawing.Size(100,20);
#endif
            l.Dock = DockStyle.Fill;
            l.AutoSize = true;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            _panel.Controls.Add(l, 0, i);

            IParameterVisualizer ctrl = CreateControl(par);
            ctrl.Init(par);
            if (defValue != null)
                ctrl.Value = defValue;

            _panel.Controls.Add((Control)ctrl, 1, i);
        }

        IParameterVisualizer CreateControl(AutoParameterInfo i)
        {
            switch (i.InfoType)
            {
                case AutoParameterInfoType.Bool:
                    return new ParameterVisualizerCheckBox();
                case AutoParameterInfoType.FilePath:
                    return new ParameterVisualizerFile();
                case AutoParameterInfoType.DirectoryPath:
                    return new ParameterVisualizerDilog();
                case AutoParameterInfoType.String:
                    return new ParameterVisualizerString();
                case AutoParameterInfoType.Password:
                    return new ParameterVisualizerPasswordString();
                case AutoParameterInfoType.List:
                    return new ParameterVisualizerListBox();

                default:
                    throw new ArgumentException();
            }
        }

        public ParametersListVisualizer(AutoParameterInfo[] parameters, DataParamTable preInfo, bool showAll)
        {
            SuspendLayout();

            int count = 0;
            _init = preInfo;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (preInfo.GetParamSafe(parameters[i].Name).IsNull)
                {
                    if (IsShowable(parameters[i]))
                        count++;
                    else
                        throw new ArgumentException("Параметер не может быть отображен");
                }
                else if ((showAll || !parameters[i].HideSatisfied) && IsShowable(parameters[i]))
                    count++;
            }

            _actualParameters = new AutoParameterInfo[count];
            _panel.RowCount = count + 1;
            _panel.ColumnCount = 2;

            int j = 0;
            foreach (AutoParameterInfo par in parameters)
            {
                if ((preInfo.GetParamSafe(par.Name).IsNull) && IsShowable(par))
                {
                    InitItem(j, par, par.DefValue);                    
                    _actualParameters[j++] = par;
                }
                else if ((showAll || !par.HideSatisfied) && IsShowable(par))
                {
                    InitItem(j, par, preInfo[par.Name]);
                    _actualParameters[j++] = par;                    
                }
            }

            //for (j = 0; j < count; j++)
            //    _panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

#if !DOTNET_V11
            _panel.Margin = new Padding(0);
            _panel.Padding = new Padding(5);
#endif
            _panel.Dock = DockStyle.Fill;
            Controls.Add(_panel);
            Dock = DockStyle.Fill;

            ResumeLayout(true);

            //AutoScroll = true;            
#if !DOTNET_V11
            _panel.MinimumSize = MinimumSize = new System.Drawing.Size(30 * count, 150);
#endif
        }

        public DataParamTable GetParameters()
        {
            List<DataParamTableItem> items = new List<DataParamTableItem>();

            for (int i = 0; i < _actualParameters.Length; i++)
            {
                IParameterVisualizer c = (IParameterVisualizer)_panel.GetControlFromPosition(1, i);
                if (_actualParameters[i].TestType != null)
                {
                    items.Add(new DataParamTableItem(_actualParameters[i].Name,
                        AnyValue.FromBoxedValue(Component.CastObject(c.Value, _actualParameters[i].TestType))));
                }
                else
                {
                    items.Add(new DataParamTableItem(_actualParameters[i].Name, AnyValue.FromBoxedValue(c.Value)));
                }
            }

            return new DataParamTable(_init.Info, items);
        }


        class ParameterVisualizerString : TextBox, IParameterVisualizer
        {
            public ParameterVisualizerString()
            {
                Dock = DockStyle.Fill;
            }

            #region IParameterVisualizer Members

            public object Value
            {
                get
                {
                    return this.Text;
                }
                set
                {
                    this.Text = value.ToString();
                }
            }

            public void Init(AutoParameterInfo info)
            {
            
            }

            #endregion
        }

        class ParameterVisualizerPasswordString : ParameterVisualizerString
        {
            public ParameterVisualizerPasswordString()
            {
#if !DOTNET_V11
                this.UseSystemPasswordChar  = true;
#else
				this.PasswordChar = '*';
#endif
            }
        }

        class ParameterVisualizerCheckBox : CheckBox, IParameterVisualizer
        {
            public ParameterVisualizerCheckBox()
            {
                Dock = DockStyle.Fill;
            }

            #region IParameterVisualizer Members

            public void Init(AutoParameterInfo info)
            {
                
            }

            public object Value
            {
                get
                {
                    return Checked;
                }
                set
                {
                    Checked = Convert.ToBoolean(value);
                }
            }

            #endregion
        }


        class ParameterVisualizerListBox : ComboBox, IParameterVisualizer
        {
            public ParameterVisualizerListBox()
            {
                Dock = DockStyle.Fill;
                DropDownStyle = ComboBoxStyle.DropDownList;                
            }

            #region IParameterVisualizer Members

            public void Init(AutoParameterInfo info)
            {
                int sel = -1;
                Items.AddRange(info.VaidValues);
                SelectValue(info.DefValue);
                return;

                foreach (object o in info.VaidValues)
                {
                    int i = Items.Add(o);
                    if (o.Equals(info.DefValue))
                        sel = i;
                }
                if (sel > -1)
                    SelectedIndex = sel;
            }

            void SelectValue(object val)
            {
                int sel = -1;
                for (int i = 0; i < Items.Count; i++ )
                {
                    if (val.Equals(Items[i]))
                    { sel = i; break; }
                }
                if (sel > -1)
                    SelectedIndex = sel;
            }

            public object Value
            {
                get
                {
                    return SelectedIndex > -1 ?  Items[SelectedIndex] : null;
                }
                set
                {
                    SelectValue(value);
                }
            }

            #endregion
        }

        abstract class ParameterVisualizerChoiser : UserControl, IParameterVisualizer
        {
            protected string helpName;
            TextBox vale = new TextBox();
            Button bt = new Button();
            TableLayoutPanel pannel = new TableLayoutPanel();

            public ParameterVisualizerChoiser()
            {
#if !DOTNET_V11
                bt.Margin = new Padding(0, 3, 0, 0);
                vale.Margin = new Padding(0, 3, 0 ,0);
                pannel.Padding = new Padding(0);
                pannel.Margin = new Padding(0);

                MinimumSize = new System.Drawing.Size(100, vale.Height + 6);
#endif
                vale.Dock = DockStyle.Fill;

                Size = new System.Drawing.Size(0,0);

                pannel.RowCount = 1;
                pannel.ColumnCount = 2;
                pannel.Controls.Add(vale, 0, 0);
                pannel.Controls.Add(bt, 1, 0);

                pannel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                pannel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));

                bt.Text = "...";
                bt.Click += new EventHandler(bt_Click);

                pannel.Dock = DockStyle.Fill;
                Controls.Add(pannel);
                Dock = DockStyle.Fill;

                bt.Height = vale.Height;
                bt.Width = 40;
            }

            protected abstract void bt_Click(object sender, EventArgs e);

            #region IParameterVisualizer Members

            public void Init(AutoParameterInfo info)
            {
                helpName = info.Help;
            }

            public object Value
            {
                get
                {
                    return vale.Text;
                }
                set
                {
                    vale.Text = value.ToString();
                }
            }

            #endregion
        }


        class ParameterVisualizerFile : ParameterVisualizerChoiser 
        {
            OpenFileDialog fd = new OpenFileDialog();

            protected override void bt_Click(object sender, EventArgs e)
            {
                fd.Filter = helpName;
                fd.ShowReadOnly = false;
                fd.Multiselect = false;

                if (Value.ToString().Length > 0)
                    fd.FileName = Value.ToString();
                    
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    Value = fd.FileName;
                }
            }
        }

        class ParameterVisualizerDilog : ParameterVisualizerChoiser
        {
            FolderBrowserDialog bd = new FolderBrowserDialog();

            protected override void bt_Click(object sender, EventArgs e)
            {
                bd.Description = helpName;
                bd.ShowNewFolderButton = false;
                
                if (Value.ToString().Length > 0)
                    bd.SelectedPath = Value.ToString();

                if (bd.ShowDialog() == DialogResult.OK)
                {
                    Value = bd.SelectedPath;
                }
            }
        }

    }
}
