using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using corelib;

namespace corelib
{
    class RecoverySimple : Form
    {
        #region Automatic
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbModel = new System.Windows.Forms.ComboBox();
            this.cbRecovery = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btView = new System.Windows.Forms.Button();
            this.btRecover = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tbDetail = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tbFlow = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbCell = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbModel
            // 
            this.cbModel.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
#if !DOTNET_V11
            this.cbModel.FormattingEnabled = true;
#endif
            this.cbModel.Location = new System.Drawing.Point(3, 33);
            this.cbModel.Name = "cbModel";
            this.cbModel.Size = new System.Drawing.Size(237, 21);
            this.cbModel.TabIndex = 3;
            this.cbModel.SelectedIndexChanged += new System.EventHandler(this.cbModel_SelectedIndexChanged);
            // 
            // cbRecovery
            // 
            this.cbRecovery.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbRecovery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
#if !DOTNET_V11
            this.cbRecovery.FormattingEnabled = true;
#endif
            this.cbRecovery.Location = new System.Drawing.Point(246, 33);
            this.cbRecovery.Name = "cbRecovery";
            this.cbRecovery.Size = new System.Drawing.Size(237, 21);
            this.cbRecovery.TabIndex = 2;
            this.cbRecovery.SelectedIndexChanged += new System.EventHandler(this.cbRecovery_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Настройка модели за";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(246, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Восстановление за";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btView
            // 
            this.btView.Dock = System.Windows.Forms.DockStyle.Top;
            this.btView.Location = new System.Drawing.Point(3, 63);
            this.btView.Name = "btView";
            this.btView.Size = new System.Drawing.Size(237, 47);
            this.btView.TabIndex = 4;
            this.btView.Text = "Подробности восстановления...";
#if !DOTNET_V11
            this.btView.UseVisualStyleBackColor = true;
#endif
            this.btView.Click += new System.EventHandler(this.btView_Click);
            // 
            // btRecover
            // 
            this.btRecover.Dock = System.Windows.Forms.DockStyle.Top;
            this.btRecover.Location = new System.Drawing.Point(246, 63);
            this.btRecover.Name = "btRecover";
            this.btRecover.Size = new System.Drawing.Size(237, 47);
            this.btRecover.TabIndex = 1;
            this.btRecover.Text = "Восстановить";
#if !DOTNET_V11
            this.btRecover.UseVisualStyleBackColor = true;
#endif
            this.btRecover.Click += new System.EventHandler(this.btRecover_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(486, 218);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Быстрый просмотр";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.tbDetail, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(480, 199);
            this.tableLayoutPanel5.TabIndex = 9;
            // 
            // tbDetail
            // 
            this.tbDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDetail.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbDetail.Location = new System.Drawing.Point(3, 48);
            this.tbDetail.Multiline = true;
            this.tbDetail.Name = "tbDetail";
            this.tbDetail.ReadOnly = true;
            this.tbDetail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbDetail.Size = new System.Drawing.Size(474, 148);
            this.tbDetail.TabIndex = 3;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Controls.Add(this.tbFlow, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tbCell, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(474, 39);
            this.tableLayoutPanel4.TabIndex = 8;
            // 
            // tbFlow
            // 
            this.tbFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFlow.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbFlow.Location = new System.Drawing.Point(319, 3);
            this.tbFlow.Name = "tbFlow";
            this.tbFlow.ReadOnly = true;
            this.tbFlow.Size = new System.Drawing.Size(152, 29);
            this.tbFlow.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 39);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ячейка";
            // 
            // tbCell
            // 
            this.tbCell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbCell.Location = new System.Drawing.Point(161, 3);
            this.tbCell.Name = "tbCell";
            this.tbCell.Size = new System.Drawing.Size(152, 29);
            this.tbCell.TabIndex = 0;
            this.tbCell.TextChanged += new System.EventHandler(this.tbCell_TextChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.btView, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.cbModel, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btRecover, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.cbRecovery, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(486, 114);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(492, 344);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // RecoverySimple
            // 
#if !DOTNET_V11
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
#endif
            this.ClientSize = new System.Drawing.Size(492, 344);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Name = "RecoverySimple";
            this.Text = "Восстановление расходов";
            this.Load += new System.EventHandler(this.RecoverySimple_Load);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbRecovery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btView;
        private System.Windows.Forms.Button btRecover;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbDetail;
        private System.Windows.Forms.TextBox tbFlow;
        private System.Windows.Forms.TextBox tbCell;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbModel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;

        #endregion


        private SunEnv _env;
        DateTime[] _dates;
        IMultiDataTuple[] _recovered;

        public RecoverySimple(SunEnv env)
        {
            InitializeComponent();
            _env = env;
            _env.UpdateDates();

            DateTime[] dt = _env.GetDates();
            ArrayList lst = new ArrayList();
            for (int i = 0; i < dt.Length; i++)
                lst.Add(dt[i]);
            lst.Sort();
            lst.Reverse();

            _dates = new DateTime[dt.Length];
            lst.CopyTo(_dates);

            for (int i = 0; i < _dates.Length; i++)
            {
                cbRecovery.Items.Add(_dates[i]);
                cbModel.Items.Add(_dates[i]);
            }

            cbRecovery.SelectedIndex = 0;
            cbModel.SelectedIndex = (_dates.Length > 1) ? 1 : 0;
        }

        private void btRecover_Click(object sender, EventArgs e)
        {
            int irec = cbRecovery.SelectedIndex;
            int imod = cbModel.SelectedIndex;
            if ((irec < 0) || (imod < 0))
                return;

            btRecover.Enabled = false;
            // cbRecovery.Enabled = false;
            // cbModel.Enabled = false;

            Cursor = Cursors.WaitCursor;
            _recovered = _env.SimpleRecoveryWithCoeffRecoveryAndNewGamma(_env.GetDatabaseMainResource().GetMultiProvider(),
                _dates[irec], _dates[imod]);
            Cursor = Cursors.Arrow;

            btRecover.Enabled = true;
            // cbRecovery.Enabled = true;
            // cbModel.Enabled = true;

            btView.Enabled = true;
        }

        private void btView_Click(object sender, EventArgs e)
        {
            if (_recovered != null)
            {
                //_env.ViewMultiTuple(_recovered);
                ListMultiDataProvider p = new ListMultiDataProvider(_recovered);
                Form f = _env.ViewMultiTupleForm(p, false);

                f.Text = String.Format("Настройка {0} Восстановление {1}", _recovered[1].GetTimeDate(), _recovered[0].GetTimeDate());
                f.Width = 600;
                f.Height = 400;

                DataTupleVisualizer tv = f.Controls[0] as DataTupleVisualizer;
                if (tv != null)
                {
                    tv.RegisterAction(new RDTVPluginFindMaxErrors());
                    tv.RegisterAction(new RDTVPluginMakeVostReport());

                    tv.RegisterAction(new RDTVPluginFindMaxErrorsZrk());
                    tv.RegisterAction(new RDTVPluginFindBadCells());
                }

                f.ShowDialog();
            }
        }


        private void FillInfo(Coords c)
        {
            tbDetail.Text = null;

            StringBuilder sb = new StringBuilder();

            foreach (IDataTuple t in _recovered)
            {
                sb.AppendFormat("Ячейка {1}: Информация за {0}\r\n", t.GetTimeDate(), c.ToString());
                //foreach (ITupleItem i in t.GetData())
                for (int j = 0; j < t.ItemsCount; j++ )
                {
                    IDataCartogram cart = t[j] as IDataCartogram;
                    if ((cart != null) && (cart.IsValidCoord(c)))
                    {
                        IInfoFormatter nf = _env.GetFormatter(cart);
                        sb.AppendFormat("{0,-40} {1}\r\n", cart.GetHumanName(), //(float)cart[c, 0]);
                            nf.GetString(cart.GetObject(c, 0)));
                    }
                }
                sb.Append("\r\n");
            }

            tbDetail.Text = sb.ToString();
            
            IDataCartogram cg = (IDataCartogram)_recovered[0]["flow_az1_cart"];
            if (cg.IsValidCoord(c))
                tbFlow.Text = String.Format("{0:00.0}", cg[c,0]);
            else
                tbFlow.Text = "[нет]";
        }

        private void tbCell_TextChanged(object sender, EventArgs e)
        {
            if (_recovered == null)
            {
                tbCell.BackColor = SystemColors.Window; 
                return;
            }

            string tex = tbCell.Text;
            try
            {
                Coords c = Coords.FromHumane(tex);
                if ((c.IsOk) && (((IDataCartogram)_recovered[0]["flow_az1_cart"]).IsValidCoord(c)))
                {
                    FillInfo(c);

                    tbCell.BackColor = Color.FromArgb(128, 255, 128);
                    return;
                }
            }
            catch
            {               
            }
            tbCell.BackColor = Color.FromArgb(255, 128, 128);
        }

        private void cbRecovery_SelectedIndexChanged(object sender, EventArgs e)
        {
            _recovered = null;
            tbDetail.Text = null;
            btView.Enabled = false;
        }

        private void cbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _recovered = null;
            tbDetail.Text = null;
            btView.Enabled = false;
        }

        private void RecoverySimple_Load(object sender, EventArgs e)
        {
            btRecover.Focus();

        }
    }





}