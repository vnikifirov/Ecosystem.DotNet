namespace Snake.Views
{
    using System;
    using System.Windows.Forms;

    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void LoadGame(object sender, EventArgs e)
        {
            var board = new Board();
            
            // Hide current window
            this.Hide();

            // Show board of game 
            board.Show();
        }

        private void OpenRatingTable(object sender, EventArgs e)
        {
            var rating = new Rating();

            // Hide current window
            this.Hide();

            // Show board of game 
            rating.Show();
        }

        private void ColorSelector(object sender, EventArgs e)
        {
            var colorSelector = new ColorSelector();

            // Hide current window
            this.Hide();

            // Show board of game 
            colorSelector.Show();
        }
    }
}
