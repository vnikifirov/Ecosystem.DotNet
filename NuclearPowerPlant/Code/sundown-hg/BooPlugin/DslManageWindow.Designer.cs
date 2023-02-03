namespace WindowsApplication1
{
    partial class DslManageWindow
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btOpen = new System.Windows.Forms.ToolStripButton();
            this.btSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btRun = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dslErrors = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btOpen,
            this.btSave,
            this.toolStripSeparator1,
            this.btRun});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(486, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btOpen
            // 
            this.btOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btOpen.Image = corelib.DefImages.ImageOpen;
            this.btOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(28, 28);
            this.btOpen.Text = "Открыть сценарий";
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // btSave
            // 
            this.btSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btSave.Image = corelib.DefImages.ImageSaveGreen;
            this.btSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(28, 28);
            this.btSave.Text = "Сохранить сценарий";
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btRun
            // 
            this.btRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btRun.Image = corelib.DefImages.ImageRun;
            this.btRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btRun.Name = "btRun";
            this.btRun.Size = new System.Drawing.Size(28, 28);
            this.btRun.Text = "Запустить сценарий";
            this.btRun.Click += new System.EventHandler(this.btRun_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 31);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dslErrors);
            this.splitContainer1.Size = new System.Drawing.Size(486, 267);
            this.splitContainer1.SplitterDistance = 162;
            this.splitContainer1.TabIndex = 1;
            // 
            // dslErrors
            // 
            this.dslErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dslErrors.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dslErrors.Location = new System.Drawing.Point(0, 0);
            this.dslErrors.Multiline = true;
            this.dslErrors.Name = "dslErrors";
            this.dslErrors.ReadOnly = true;
            this.dslErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dslErrors.Size = new System.Drawing.Size(486, 101);
            this.dslErrors.TabIndex = 0;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "dsl";
            this.saveFileDialog1.Filter = "Файл сценария (*.dsl)|*.dsl|Все файлы (*.*)|*.*";
            this.saveFileDialog1.Title = "Сохранить файл сценария";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dsl";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Файл сценария (*.dsl)|*.dsl|Все файлы (*.*)|*.*";
            this.openFileDialog1.Title = "Открыть файл сценария";
            // 
            // DslManageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 298);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DslManageWindow";
            this.Text = "Редактор скриптов DSL";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DslManageWindow_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DslManageWindow_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btOpen;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox dslErrors;
        private System.Windows.Forms.ToolStripButton btSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btRun;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}