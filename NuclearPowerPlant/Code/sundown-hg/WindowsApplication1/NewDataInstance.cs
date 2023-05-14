using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;


namespace corelib
{
    public class NewDataInstance : Form
    {
        private Label label;
        private ListBox components;

        private TableLayoutPanel tableLayoutPanel0;
#if !DOTNET_V11
        private SplitContainer tableLayoutPanel1;
#else
        private TableLayoutPanel tableLayoutPanel1;
#endif
        private TableLayoutPanel tableLayoutPanel2;
        private Button btOk;
        private Button btCancel;
        private TableLayoutPanel tableLayoutPanel3;

        private DataParamTable _parameters;
        private IDataComponent _component;
        private ParametersListVisualizer _lv;

        public NewDataInstance()
        {
            InitInternals();
        }

        void InitInternals()
        {
            components = new ListBox();
            components.Dock = DockStyle.Fill;
            components.SelectedIndexChanged += new EventHandler(components_SelectedIndexChanged);

            tableLayoutPanel0 = new TableLayoutPanel();

#if !DOTNET_V11
            tableLayoutPanel1 = new SplitContainer();
#else
            tableLayoutPanel1 = new TableLayoutPanel();
#endif
            tableLayoutPanel2 = new TableLayoutPanel();

            btOk = new Button();
            btCancel = new Button();
            label = new Label();

            tableLayoutPanel3 = new TableLayoutPanel();

            tableLayoutPanel0.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();

            this.Text = "Создание источника данных";


            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Name = "tableLayoutPanel3";

            tableLayoutPanel3.Controls.Add(this.label, 0, 0);

#if !DOTNET_V11
            tableLayoutPanel1.Panel1.Padding = new Padding(3);
            tableLayoutPanel1.Panel2.Padding = new Padding(3);
            tableLayoutPanel1.Panel1.Controls.Add(this.components);
            tableLayoutPanel1.Panel2.Controls.Add(this.tableLayoutPanel3);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Orientation = Orientation.Vertical;
            tableLayoutPanel1.SplitterDistance = 50;
#else
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(this.components, 0, 0);
            tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            tableLayoutPanel1.TabIndex = 0;
#endif

            btCancel.Text = "&Отмена";
            btCancel.Dock = DockStyle.Fill;
            btOk.Text = "&Ок";
            btOk.Dock = DockStyle.Fill;
            label.Dock = DockStyle.Fill;
            

            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Name = "tableLayoutPanel2";

            tableLayoutPanel2.Controls.Add(this.btCancel, 1, 0);
            tableLayoutPanel2.Controls.Add(this.btOk, 2, 0);


            tableLayoutPanel0.ColumnCount = 1;
            tableLayoutPanel0.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel0.RowCount = 2;
            tableLayoutPanel0.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel0.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            tableLayoutPanel0.Dock = DockStyle.Fill;
            tableLayoutPanel0.Name = "tableLayoutPanel0";
            
            tableLayoutPanel0.Controls.Add(this.tableLayoutPanel1, 0, 0);
            tableLayoutPanel0.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.Controls.Add(this.tableLayoutPanel0);

            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel0.ResumeLayout(false);
            this.ResumeLayout(false);

            this.CancelButton = btCancel;
            this.AcceptButton = btOk;

            this.btOk.Click += new EventHandler(btOk_Click);
            //this.btOk.DialogResult = DialogResult.OK;
        }

        void btOk_Click(object sender, EventArgs e)
        {
            _component = SelectedComponent;
            _parameters = _lv.GetParameters();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        void components_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDataComponent component = SelectedComponent;

            if (component == null)
                return;

            DataParamTable t = new DataParamTable(new TupleMetaData("settings", "settings", DateTime.Now, "settings"),
                new DataParamTableItem("enviromentObject", 1));

            try
            {
                ParametersListVisualizer lv = new ParametersListVisualizer(component.Info.GetParametrsInfo(), t, true);
                if (tableLayoutPanel3.Controls.Count > 1)
                    tableLayoutPanel3.Controls.RemoveAt(1);
                tableLayoutPanel3.Controls.Add(lv, 0, 1);

                label.Text = component.Info.HumanDescribe;

                _lv = lv;
            }
            catch
            {
                components.Items.Remove(component);
            }
        }

        public void AddComponent(IDataComponent component)
        {
            components.Items.Add(component);
        }

        public IDataComponent SelectedComponent
        {
            get { return (IDataComponent)components.SelectedItem; }
            set { components.SelectedItem = value; }
        }

        public IDataComponent Component
        {
            get { return _component; }
        }

        public DataParamTable Parameters
        {
            get { return _parameters; }
        }

        public static IDataResource SelectComponent(BasicEnv env, string name)
        {
            NewDataInstance n = new NewDataInstance();

            foreach (IDataComponent c in env.Data.Components)
            {
                AutoParameterInfo[] nfo = c.Info.GetParametrsInfo();
                if (nfo != null)
                    n.AddComponent(c);
            }

            n.Size = new System.Drawing.Size(600, 300);
            
            if (n.ShowDialog() == DialogResult.OK)
                return env.CreateData(n._component, name, n._parameters);

            return null;
        }
    }
}
