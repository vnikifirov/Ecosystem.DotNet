namespace Snake.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Rating : Form
    {
        public Rating()
        {
            InitializeComponent();
        }

        private void BackToMenu(object sender, EventArgs e)
        {
            var menu = new Menu();

            // Hide current window
            this.Hide();

            // Show board of game 
            menu.Show();
        }
    }
}
