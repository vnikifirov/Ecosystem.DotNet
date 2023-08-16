namespace DemoChart
{
    partial class Form
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart_files = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
            this.label_disk = new System.Windows.Forms.Label();
            this.select_disk = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.old_files = new System.Windows.Forms.Label();
            this.new_files = new System.Windows.Forms.Label();
            this.total = new System.Windows.Forms.Label();
            this.save_label = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart_files)).BeginInit();
            this.SuspendLayout();
            // 
            // chart_files
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_files.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart_files.Legends.Add(legend2);
            this.chart_files.Location = new System.Drawing.Point(186, 13);
            this.chart_files.Margin = new System.Windows.Forms.Padding(4);
            this.chart_files.Name = "chart_files";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series2.Legend = "Legend1";
            series2.Name = "Files";
            this.chart_files.Series.Add(series2);
            this.chart_files.Size = new System.Drawing.Size(480, 318);
            this.chart_files.TabIndex = 0;
            this.chart_files.Text = "New Files";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(96, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Scan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button_Scan);
            // 
            // label_disk
            // 
            this.label_disk.AutoSize = true;
            this.label_disk.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_disk.Location = new System.Drawing.Point(12, 14);
            this.label_disk.Name = "label_disk";
            this.label_disk.Size = new System.Drawing.Size(175, 16);
            this.label_disk.TabIndex = 2;
            this.label_disk.Text = "Current disk [disk-name]";
            // 
            // select_disk
            // 
            this.select_disk.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.select_disk.Location = new System.Drawing.Point(15, 44);
            this.select_disk.Name = "select_disk";
            this.select_disk.Size = new System.Drawing.Size(75, 23);
            this.select_disk.TabIndex = 3;
            this.select_disk.Text = "Browse";
            this.select_disk.UseVisualStyleBackColor = true;
            this.select_disk.Click += new System.EventHandler(this.Button_BrowseForScan_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(12, 312);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Exit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // old_files
            // 
            this.old_files.AutoSize = true;
            this.old_files.Location = new System.Drawing.Point(22, 84);
            this.old_files.Name = "old_files";
            this.old_files.Size = new System.Drawing.Size(68, 16);
            this.old_files.TabIndex = 5;
            this.old_files.Text = "Old files: 0";
            // 
            // new_files
            // 
            this.new_files.AutoSize = true;
            this.new_files.Location = new System.Drawing.Point(22, 117);
            this.new_files.Name = "new_files";
            this.new_files.Size = new System.Drawing.Size(79, 16);
            this.new_files.TabIndex = 6;
            this.new_files.Text = "New Files: 0";
            // 
            // total
            // 
            this.total.AutoSize = true;
            this.total.Location = new System.Drawing.Point(22, 147);
            this.total.Name = "total";
            this.total.Size = new System.Drawing.Size(51, 16);
            this.total.TabIndex = 7;
            this.total.Text = "Total: 0";
            // 
            // save_label
            // 
            this.save_label.AutoSize = true;
            this.save_label.Location = new System.Drawing.Point(13, 191);
            this.save_label.Name = "save_label";
            this.save_label.Size = new System.Drawing.Size(74, 16);
            this.save_label.TabIndex = 8;
            this.save_label.Text = "Save result";
            // 
            // button_save
            // 
            this.button_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_save.Location = new System.Drawing.Point(12, 222);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 9;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 347);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.save_label);
            this.Controls.Add(this.total);
            this.Controls.Add(this.new_files);
            this.Controls.Add(this.old_files);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.select_disk);
            this.Controls.Add(this.label_disk);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chart_files);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form";
            this.Text = "File tracking system (Demo)";
            ((System.ComponentModel.ISupportInitialize)(this.chart_files)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_files;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label_disk;
        private System.Windows.Forms.Button select_disk;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label old_files;
        private System.Windows.Forms.Label new_files;
        private System.Windows.Forms.Label total;
        private System.Windows.Forms.Label save_label;
        private System.Windows.Forms.Button button_save;
    }
}

