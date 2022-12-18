namespace Snake.Views
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class ColorSelector : Form
    {
        public ColorSelector()
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

        private void EnterColor(object sender, EventArgs e)
        {
            /*
            var color = Color.White;

            if (!string.IsNullOrWhiteSpace(red.Text))
            {
                var red = red.Text as byte;
                color.R = red;
            }

            var green = (byte)green.Text;
            if (!string.IsNullOrWhiteSpace(green.Text))
            {

            }

            var blue = (byte)green.Text;
            if (!string.IsNullOrWhiteSpace(blue.Text))
            {

            }
            */
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            var pixelData = colorPicker.Image as Bitmap;
            if (pixelData is not null)
            {
                var color = pixelData.GetPixel(e.X, e.Y);
                panel.BackColor = color;

                // Fill RGB or Red, Green, Blue feilds by color 
                red.Text = color.R.ToString();
                green.Text = color.G.ToString();
                blue.Text = color.B.ToString();

                panel.BackColor = color;
            }
        }
    }
}
