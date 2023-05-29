using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using System.Drawing;
using System.Windows.Forms;
using System.Numerics;

namespace AirCraft
{
    public enum Direction
    {
        Top,
        Down,
        Left,
        Right
    }
    public class Bullet 
    {
        public int _Speed { get; set; } = 32;
        public PictureBox _Bullet { get; set; } = new PictureBox();
        public System.Windows.Forms.Timer BulletTimer { get; set; } = new System.Windows.Forms.Timer();
        public int _BulletY { get; set; }
        public int _BulletX { get; set; }
        public Direction Direction { get; set; }

        public void MakeBullet(Form form)
        {
            _Bullet.BackColor = Color.Black;
            _Bullet.Size = new Size(5, 5);
            _Bullet.Tag = "Bullet";
            _Bullet.Top = _BulletY;
            _Bullet.Left = _BulletX;
            _Bullet.BringToFront();
            
            form.Controls.Add(_Bullet);

            BulletTimer.Interval = _Speed;
            BulletTimer.Tick += new EventHandler(bulletTimerEvent);
            BulletTimer.Start();
        }

        /// <summary>
        /// Aircraft movements on grid event handler 
        /// </summary>
        private void bulletTimerEvent(object sender, EventArgs e)
        {
            var location = _Bullet.Location;
            if (Direction == Direction.Top)
                location.Y -= _Speed;

            if (Direction == Direction.Down)
                location.Y += _Speed;

            if (_Bullet.Location.Y < 10 || _Bullet.Location.Y > 512 || _Bullet.Location.X > 1024 || _Bullet.Location.X > 10)
            {
                BulletTimer.Stop();
                BulletTimer.Dispose();
                _Bullet.Dispose();
                BulletTimer = null;
                _Bullet = null;
;            }
        }
    }

    public partial class Board : Form
    {
        /// <summary>
        /// This is contsant velocity of aircraft to make it better you can add time. 
        ///  The speed * time is distance
        /// </summary>
        public int velocity { get; set; } = 8;

        /// <summary>
        /// This is indicator shows game is over or not
        /// </summary>
        public bool GameOver { get; set; } = false;

        public int _ammo = 32;
        /// <summary>
        /// Ammunition of aircraft
        /// </summary>
        public int Ammo { 
            get 
            {
                return this._ammo;
            } 
            set 
            {
                if (0 >= _ammo)
                    return;

                this._ammo = value;
                this.indicatorAmmo.Text =  String.Format("Ammo {0}", this._ammo);
            } 
        }

        /// <summary>
        /// Player health
        /// </summary>
        public int Health { get; set; } = 100;

        public Board() 
        {
            InitializeComponent();
            this.KeyDown += (s, e) => this.OnKeyDown(s, (KeyEventArgs)e);           
            this.plane.Focus();

            //this.Click += (s, e) => this.Click(s, (MouseEventArgs)e);
        }

        private void MainTimeEvent()
        {

        }

        /// <summary>
        /// Aircraft movements on grid event handler 
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //var _event = e as KeyEventArgs;
            var location = plane.Location;
            if (e.KeyCode == Keys.W)
                location.Y = this.plane.Location.Y - velocity;

            if (e.KeyCode == Keys.S)
                location.Y = this.plane.Location.Y + velocity;

            if (e.KeyCode == Keys.D)
                location.X = this.plane.Location.X + velocity;

            if (e.KeyCode == Keys.A)
                location.X = this.plane.Location.X - velocity;

            if (e.KeyCode == Keys.Space)
            {
                Ammo -= 1;
                ShootBullet(Direction.Top);
            }

            this.plane.Location = location;
        }

        private void ShootBullet(Direction direction) 
        {
            var bullet = new Bullet();
            bullet.Direction = direction;
            bullet._BulletY = this.plane.Height;
            bullet._BulletX = this.plane.Location.X + (this.plane.Width / 2) ;
            bullet.MakeBullet(this);
        }

        private void MakePlane()
        {

        }

        private void RestartGame()
        {

        }
    }
}