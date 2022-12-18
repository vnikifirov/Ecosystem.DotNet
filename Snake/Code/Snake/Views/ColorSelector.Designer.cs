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
            this.red = new System.Windows.Forms.TextBox();
            this.green = new System.Windows.Forms.TextBox();
            this.blue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
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
            this.colorPicker.Image = global::Snake.Properties.Resources.color_picker;
            this.colorPicker.Location = new System.Drawing.Point(12, 28);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Size = new System.Drawing.Size(455, 430);
            this.colorPicker.TabIndex = 1;
            this.colorPicker.TabStop = false;
            this.colorPicker.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove);
            // 
            // red
            // 
            this.red.Location = new System.Drawing.Point(65, 464);
            this.red.Name = "red";
            this.red.Size = new System.Drawing.Size(100, 23);
            this.red.TabIndex = 2;
            this.red.Enter += new System.EventHandler(this.EnterColor);
            // 
            // green
            // 
            this.green.Location = new System.Drawing.Point(65, 493);
            this.green.Name = "green";
            this.green.Size = new System.Drawing.Size(100, 23);
            this.green.TabIndex = 3;
            this.green.Move += new System.EventHandler(this.EnterColor);
            // 
            // blue
            // 
            this.blue.Location = new System.Drawing.Point(65, 522);
            this.blue.Name = "blue";
            this.blue.Size = new System.Drawing.Size(100, 23);
            this.blue.TabIndex = 4;
            this.blue.Enter += new System.EventHandler(this.EnterColor);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 464);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 493);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 522);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Blue";
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(171, 464);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(306, 133);
            this.panel.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(171, 603);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 50);
            this.button1.TabIndex = 9;
            this.button1.Text = "Menu";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BackToMenu);
            // 
            // ColorSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 675);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.blue);
            this.Controls.Add(this.green);
            this.Controls.Add(this.red);
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
        private TextBox red;
        private TextBox green;
        private TextBox blue;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel;
        private Button button1;
    }
}