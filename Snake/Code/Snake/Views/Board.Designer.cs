namespace Snake.Views
{
    partial class Board
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
            this.btnMenu = new System.Windows.Forms.Button();
            this.snake = new System.Windows.Forms.PictureBox();
            this.healthIndicator = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.snake)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMenu
            // 
            this.btnMenu.Location = new System.Drawing.Point(12, 12);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(85, 38);
            this.btnMenu.TabIndex = 1;
            this.btnMenu.Text = "Menu";
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.BackToMenu);
            // 
            // snake
            // 
            this.snake.Image = global::Snake.Properties.Resources.snake;
            this.snake.Location = new System.Drawing.Point(390, 200);
            this.snake.Name = "snake";
            this.snake.Size = new System.Drawing.Size(42, 37);
            this.snake.TabIndex = 2;
            this.snake.TabStop = false;
            // 
            // healthIndicator
            // 
            this.healthIndicator.AutoSize = true;
            this.healthIndicator.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.healthIndicator.Location = new System.Drawing.Point(674, 9);
            this.healthIndicator.Name = "healthIndicator";
            this.healthIndicator.Size = new System.Drawing.Size(114, 30);
            this.healthIndicator.TabIndex = 3;
            this.healthIndicator.Text = "Health: 10";
            // 
            // Board
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Snake.Properties.Resources.board_cave_background;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.healthIndicator);
            this.Controls.Add(this.snake);
            this.Controls.Add(this.btnMenu);
            this.Name = "Board";
            this.Text = "Board";
            ((System.ComponentModel.ISupportInitialize)(this.snake)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private Button btnMenu;
        private PictureBox snake;
        private Label healthIndicator;
    }
}