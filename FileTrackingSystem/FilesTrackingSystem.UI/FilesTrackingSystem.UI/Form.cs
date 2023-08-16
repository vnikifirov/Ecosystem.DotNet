using System;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace DemoChart
{
    public partial class Form : System.Windows.Forms.Form
    {
        public string _directoryPath { get; set; }
        private string _dataFile { get; set; } = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "\\data.txt";
        public int _old_file_count { get; set; }
        public int _all_file_count { get; set; }
        public bool _scanned { get; set; } = false;

        public Form()
        {
            InitializeComponent();
        }

        private void Button_Scan(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._directoryPath))
                MessageBox.Show("You need select directory first", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                fillChart();
                this._scanned = true;
            }
        }

        private void Button_BrowseForScan_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this._directoryPath = folderBrowserDialog1.SelectedPath;
                this.label_disk.Text = string.Format("Current disk {0}", this._directoryPath);
            }
        }

        //fillChart method
        private void fillChart()
        {
            //var path = "C:/";
            // searches the current directory and sub directory
            this._all_file_count = Directory.GetFiles(this._directoryPath, "*", SearchOption.AllDirectories).Length;
            this._old_file_count = this.ReadOldFiles(this._dataFile);

            chart_files.Titles.Clear();
            chart_files.Series["Files"].Points.Clear();

            chart_files.Titles.Add("Files");
            if (this._old_file_count > 0)
            {
                int new_file_count = this._all_file_count - this._old_file_count;

                chart_files.Series["Files"].Points.AddXY("Old file", this._old_file_count);
                chart_files.Series["Files"].Points.AddXY("New files", new_file_count);

                new_files.Text = string.Format("New Files: {0}", new_file_count);
                old_files.Text = string.Format("Old Files: {0}", this._old_file_count);
                total.Text = string.Format("Total: {0}", this._all_file_count);
            }
            else 
            {
                chart_files.Series["Files"].Points.AddXY("Old file", this._all_file_count);

                new_files.Text = string.Format("New Files: {0}", this._all_file_count);
                total.Text = string.Format("Total: {0}", this._all_file_count);
            }
        }

        private int ReadOldFiles(string path)
        {          
            var sr = new StreamReader(path);

            var data = sr.ReadToEnd();            
            var row = data.Split('\n');

            for (var i = 0; i < row.Length; i++)
            {
                var columns = row[i].Split(',');

                if (int.TryParse(columns[0], out int result))
                    this._old_file_count = result;
            }
            
            sr.Close();
            sr.Dispose();

            return this._old_file_count;
        }

        private void WriteFiles(string path)
        {
            //before your loop
            var csv = new StringBuilder();

            //in your loop            
            //Suggestion made by KyleMit
            var newLine = string.Format("{0}", this._all_file_count);
            csv.AppendLine(newLine);

            //after your loop
            File.WriteAllText(path, csv.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to really exit and safe result? ", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                string project_dir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                this.WriteFiles(project_dir + "\\data.txt");

                System.Windows.Forms.Application.Exit();
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to really safe result? ", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes && this._scanned == true)
            {
                string project_dir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                this.WriteFiles(project_dir + "\\data.txt");                
            }

            if (this._scanned == false) 
            {
                MessageBox.Show("You must scan directory first? ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
