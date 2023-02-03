namespace ServiceEmulation
{
    partial class ServiceForm
    {
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            TB_skala = new System.Windows.Forms.TextBox();
            TB_azot = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            CoordsGridView = new System.Windows.Forms.DataGridView();
            CoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Date1Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Date2Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(CoordsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(130, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(240, 50);
            this.button1.TabIndex = 0;
            this.button1.Text = "Запуск службы";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(130, 286);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(240, 50);
            this.button2.TabIndex = 1;
            this.button2.Text = "Остановка";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TB_skala
            // 
            TB_skala.Dock = System.Windows.Forms.DockStyle.Fill;
            TB_skala.Location = new System.Drawing.Point(3, 16);
            TB_skala.Name = "TB_skala";
            TB_skala.Size = new System.Drawing.Size(224, 20);
            TB_skala.TabIndex = 2;
            // 
            // TB_azot
            // 
            TB_azot.Dock = System.Windows.Forms.DockStyle.Fill;
            TB_azot.Location = new System.Drawing.Point(3, 16);
            TB_azot.Name = "TB_azot";
            TB_azot.Size = new System.Drawing.Size(224, 20);
            TB_azot.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(TB_skala);
            this.groupBox1.Location = new System.Drawing.Point(12, 232);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 48);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Последний срез Скалы-микро";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(TB_azot);
            this.groupBox2.Location = new System.Drawing.Point(256, 232);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 49);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Последняя прописка";
            // 
            // CoordsGridView
            // 
            CoordsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            CoordsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            CoordColumn,
            Date1Column,
            Date2Column});
            CoordsGridView.Location = new System.Drawing.Point(12, 75);
            CoordsGridView.Name = "CoordsGridView";
            CoordsGridView.RowHeadersVisible = false;
            CoordsGridView.Size = new System.Drawing.Size(474, 151);
            CoordsGridView.TabIndex = 6;
            // 
            // CoordColumn
            // 
            CoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            CoordColumn.FillWeight = 91.37056F;
            CoordColumn.HeaderText = "Ячейка";
            CoordColumn.Name = "CoordColumn";
            CoordColumn.Width = 69;
            // 
            // Date1Column
            // 
            Date1Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Date1Column.FillWeight = 104.3147F;
            Date1Column.HeaderText = "Дата начала запрещения";
            Date1Column.Name = "Date1Column";
            // 
            // Date2Column
            // 
            Date2Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Date2Column.FillWeight = 104.3147F;
            Date2Column.HeaderText = "Дата окончания запрещения";
            Date2Column.Name = "Date2Column";
            // 
            // ServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 348);
            this.Controls.Add(CoordsGridView);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "ServiceForm";
            this.Text = "П1_Service";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ServiceForm_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(CoordsGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        public static System.Windows.Forms.TextBox TB_skala;
        public static System.Windows.Forms.TextBox TB_azot;
        public static System.Windows.Forms.DataGridView CoordsGridView;
        public static System.Windows.Forms.DataGridViewTextBoxColumn CoordColumn;
        public static System.Windows.Forms.DataGridViewTextBoxColumn Date1Column;
        public static System.Windows.Forms.DataGridViewTextBoxColumn Date2Column;
    }
}

