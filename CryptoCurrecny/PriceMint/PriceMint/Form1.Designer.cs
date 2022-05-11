namespace PriceMint
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.gst = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gmt = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Solana = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gst exchange rate$";
            // 
            // gst
            // 
            this.gst.AutoSize = true;
            this.gst.Location = new System.Drawing.Point(35, 24);
            this.gst.Name = "gst";
            this.gst.Size = new System.Drawing.Size(29, 15);
            this.gst.TabIndex = 1;
            this.gst.Text = "gst$";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(125, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Gmt exchange rate$";
            // 
            // gmt
            // 
            this.gmt.AutoSize = true;
            this.gmt.Location = new System.Drawing.Point(154, 24);
            this.gmt.Name = "gmt";
            this.gmt.Size = new System.Drawing.Size(35, 15);
            this.gmt.TabIndex = 3;
            this.gmt.Text = "gmt$";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(244, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Solana exchange rate$";
            // 
            // Solana
            // 
            this.Solana.AutoSize = true;
            this.Solana.Location = new System.Drawing.Point(284, 24);
            this.Solana.Name = "Solana";
            this.Solana.Size = new System.Drawing.Size(48, 15);
            this.Solana.TabIndex = 5;
            this.Solana.Text = "Solana$";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 229);
            this.Controls.Add(this.Solana);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.gmt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gst);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label gst;
        private Label label3;
        private Label gmt;
        private Label label5;
        private Label Solana;
    }
}