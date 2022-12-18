namespace Snake.Views
{
    using System.Windows.Forms;

    #region BLL or Bussines logic layer

    // Code ...

    #endregion

    public partial class Board : Form
    {
        /// <summary>
        /// .This is contsant velocity of Snake to make it better you can add time. 
        ///  The speed * time is distance
        /// </summary>
        public int SnakeVelocity { get; set; } = 8;

        public Board()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.OnKeyDown);
        }
        
        /// <summary>
        /// Menu control event handler
        /// </summary>

        private void BackToMenu(object sender, EventArgs e)
        {
            var menu = new Menu();
            
            // Hide current window
            this.Hide();

            // Show board of game 
            menu.Show();
        }

        /// <summary>
        /// Snake movements on grid event handler 
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {       
            var location = snake.Location;
            if (e.KeyCode == Keys.W)
                location.Y = snake.Location.Y - SnakeVelocity; 

            if (e.KeyCode == Keys.S)  
                location.Y = snake.Location.Y + SnakeVelocity;
            
            if (e.KeyCode == Keys.D)          
                location.X = snake.Location.X + SnakeVelocity;

            if (e.KeyCode == Keys.A)
                location.X = snake.Location.X - SnakeVelocity;

            if (snake.Location.X > ClientSize.Width)
                return;

            if (snake.Location.X > ClientSize.Height)
                return;

            if (location.Y < 56)
                return;

            snake.Location = location;
        }
    }
}