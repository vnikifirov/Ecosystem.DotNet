namespace AirCraft
{
    partial class Board
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
            this.plane = new System.Windows.Forms.PictureBox();
            this.healthIndicator = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.indicatorAmmo = new System.Windows.Forms.Label();
            this.indicatorKilled = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.plane)).BeginInit();
            this.SuspendLayout();
            // 
            // plane
            // 
            this.plane.BackColor = System.Drawing.Color.Transparent;
            this.plane.BackgroundImage = global::AirCraft.Properties.Resources.F;
            this.plane.Location = new System.Drawing.Point(427, 297);
            this.plane.Name = "plane";
            this.plane.Size = new System.Drawing.Size(172, 262);
            this.plane.TabIndex = 0;
            this.plane.TabStop = false;
            // 
            // healthIndicator
            // 
            this.healthIndicator.Location = new System.Drawing.Point(837, 12);
            this.healthIndicator.Name = "healthIndicator";
            this.healthIndicator.Size = new System.Drawing.Size(152, 23);
            this.healthIndicator.TabIndex = 1;
            this.healthIndicator.Value = 100;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(767, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 22);
            this.label1.TabIndex = 2;
            this.label1.Text = "Health";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // indicatorAmmo
            // 
            this.indicatorAmmo.AutoSize = true;
            this.indicatorAmmo.BackColor = System.Drawing.Color.Transparent;
            this.indicatorAmmo.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.indicatorAmmo.Location = new System.Drawing.Point(12, 9);
            this.indicatorAmmo.Name = "indicatorAmmo";
            this.indicatorAmmo.Size = new System.Drawing.Size(93, 22);
            this.indicatorAmmo.TabIndex = 3;
            this.indicatorAmmo.Text = "Ammo: 32";
            this.indicatorAmmo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // indicatorKilled
            // 
            this.indicatorKilled.AutoSize = true;
            this.indicatorKilled.BackColor = System.Drawing.Color.Transparent;
            this.indicatorKilled.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.indicatorKilled.Location = new System.Drawing.Point(441, 9);
            this.indicatorKilled.Name = "indicatorKilled";
            this.indicatorKilled.Size = new System.Drawing.Size(73, 22);
            this.indicatorKilled.TabIndex = 4;
            this.indicatorKilled.Text = "Killed 0";
            this.indicatorKilled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Board
            // 
            this.BackgroundImage = global::AirCraft.Properties.Resources.backgound;
            this.ClientSize = new System.Drawing.Size(1001, 571);
            this.Controls.Add(this.indicatorKilled);
            this.Controls.Add(this.indicatorAmmo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.healthIndicator);
            this.Controls.Add(this.plane);
            this.Name = "Board";
            ((System.ComponentModel.ISupportInitialize)(this.plane)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox plane;
        private ProgressBar healthIndicator;
        private Label label1;
        private Label indicatorAmmo;
        private Label indicatorKilled;
    }
}