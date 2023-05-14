namespace EmulationComplex_vs90
{
    partial class ComplexForm
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
            this.OutFolderButton = new System.Windows.Forms.Button();
            this.OutFolder = new System.Windows.Forms.TextBox();
            this.Time = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CleanButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.InFolderButton = new System.Windows.Forms.Button();
            this.InFolder = new System.Windows.Forms.TextBox();
            Current_skala = new System.Windows.Forms.TextBox();
            Current_azot = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OutFolderButton
            // 
            this.OutFolderButton.Location = new System.Drawing.Point(12, 12);
            this.OutFolderButton.Name = "OutFolderButton";
            this.OutFolderButton.Size = new System.Drawing.Size(113, 23);
            this.OutFolderButton.TabIndex = 0;
            this.OutFolderButton.Text = "Источник данных";
            this.OutFolderButton.UseVisualStyleBackColor = true;
            this.OutFolderButton.Click += new System.EventHandler(this.OutFolderButton_Click);
            // 
            // OutFolder
            // 
            this.OutFolder.Location = new System.Drawing.Point(12, 41);
            this.OutFolder.Name = "OutFolder";
            this.OutFolder.Size = new System.Drawing.Size(241, 20);
            this.OutFolder.TabIndex = 1;
            // 
            // Time
            // 
            this.Time.Location = new System.Drawing.Point(153, 133);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(100, 20);
            this.Time.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Временные рамки, мин.";
            // 
            // CleanButton
            // 
            this.CleanButton.Location = new System.Drawing.Point(15, 152);
            this.CleanButton.Name = "CleanButton";
            this.CleanButton.Size = new System.Drawing.Size(98, 23);
            this.CleanButton.TabIndex = 4;
            this.CleanButton.Text = "Очистка базы";
            this.CleanButton.UseVisualStyleBackColor = true;
            this.CleanButton.Click += new System.EventHandler(this.CleanButton_Click);
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(15, 181);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(238, 33);
            this.StartButton.TabIndex = 5;
            this.StartButton.Text = "Запуск процессов";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(97, 220);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 30);
            this.StopButton.TabIndex = 6;
            this.StopButton.Text = "Остановка";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // InFolderButton
            // 
            this.InFolderButton.Location = new System.Drawing.Point(15, 67);
            this.InFolderButton.Name = "InFolderButton";
            this.InFolderButton.Size = new System.Drawing.Size(83, 23);
            this.InFolderButton.TabIndex = 7;
            this.InFolderButton.Text = "Хранилище";
            this.InFolderButton.UseVisualStyleBackColor = true;
            this.InFolderButton.Click += new System.EventHandler(this.InFolderButton_Click);
            // 
            // InFolder
            // 
            this.InFolder.Location = new System.Drawing.Point(15, 96);
            this.InFolder.Name = "InFolder";
            this.InFolder.Size = new System.Drawing.Size(238, 20);
            this.InFolder.TabIndex = 8;
            // 
            // Current_skala
            // 
            Current_skala.Location = new System.Drawing.Point(56, 263);
            Current_skala.Name = "Current_skala";
            Current_skala.Size = new System.Drawing.Size(192, 20);
            Current_skala.TabIndex = 9;
            // 
            // Current_azot
            // 
            Current_azot.Location = new System.Drawing.Point(56, 293);
            Current_azot.Name = "Current_azot";
            Current_azot.Size = new System.Drawing.Size(192, 20);
            Current_azot.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Скала";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 296);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Азот";
            // 
            // ComplexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 325);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(Current_azot);
            this.Controls.Add(Current_skala);
            this.Controls.Add(this.InFolder);
            this.Controls.Add(this.InFolderButton);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.CleanButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.OutFolder);
            this.Controls.Add(this.OutFolderButton);
            this.Name = "ComplexForm";
            this.Text = "Эмулятор обмена данных";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OutFolderButton;
        private System.Windows.Forms.TextBox OutFolder;
        private System.Windows.Forms.TextBox Time;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CleanButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button InFolderButton;
        private System.Windows.Forms.TextBox InFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public static System.Windows.Forms.TextBox Current_skala;
        public static System.Windows.Forms.TextBox Current_azot;
    }
}

