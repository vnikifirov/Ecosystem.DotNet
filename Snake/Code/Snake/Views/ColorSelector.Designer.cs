namespace Snake.Views
{
    partial class ColorSelector
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
            this.colorSeletor = new System.Windows.Forms.Label();
            this.colorPicker = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.colorPicker)).BeginInit();
            this.SuspendLayout();
            // 
            // colorSeletor
            // 
            this.colorSeletor.AutoSize = true;
            this.colorSeletor.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorSeletor.Location = new System.Drawing.Point(0, 0);
            this.colorSeletor.Name = "colorSeletor";
            this.colorSeletor.Size = new System.Drawing.Size(81, 15);
            this.colorSeletor.TabIndex = 0;
            this.colorSeletor.Text = "Color Selector";
            this.colorSeletor.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // colorPicker
            // 
            this.colorPicker.Location = new System.Drawing.Point(12, 28);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Size = new System.Drawing.Size(100, 50);
            this.colorPicker.TabIndex = 1;
            this.colorPicker.TabStop = false;
            // 
            // ColorSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 516);
            this.Controls.Add(this.colorPicker);
            this.Controls.Add(this.colorSeletor);
            this.Name = "ColorSelector";
            this.Text = "ColorSelector";
            ((System.ComponentModel.ISupportInitialize)(this.colorPicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label colorSeletor;
        private PictureBox colorPicker;
    }
}