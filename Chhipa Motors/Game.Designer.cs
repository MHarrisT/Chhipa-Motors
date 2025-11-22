namespace Chhipa_Motors
{
    partial class Game
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
            components = new System.ComponentModel.Container();
            gameTimer = new System.Windows.Forms.Timer(components);
            Road1 = new PictureBox();
            Road2 = new PictureBox();
            PlayerCar = new PictureBox();
            trafficVehicle1 = new PictureBox();
            trafficVehicle2 = new PictureBox();
            trafficVehicle3 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)Road1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Road2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PlayerCar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trafficVehicle1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trafficVehicle2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trafficVehicle3).BeginInit();
            SuspendLayout();
            // 
            // gameTimer
            // 
            gameTimer.Enabled = true;
            gameTimer.Interval = 20;
            gameTimer.Tick += gameTimer_Tick;
            // 
            // Road1
            // 
            Road1.BackColor = Color.Transparent;
            Road1.BackgroundImage = Properties.Resources.RoadImg;
            Road1.BackgroundImageLayout = ImageLayout.Stretch;
            Road1.Location = new Point(400, 0);
            Road1.Name = "Road1";
            Road1.Size = new Size(385, 800);
            Road1.TabIndex = 0;
            Road1.TabStop = false;
            // 
            // Road2
            // 
            Road2.BackColor = Color.Transparent;
            Road2.BackgroundImage = Properties.Resources.RoadImg;
            Road2.BackgroundImageLayout = ImageLayout.Stretch;
            Road2.Location = new Point(400, -800);
            Road2.Name = "Road2";
            Road2.Size = new Size(385, 800);
            Road2.TabIndex = 1;
            Road2.TabStop = false;
            // 
            // PlayerCar
            // 
            PlayerCar.BackColor = Color.Gray;
            PlayerCar.BackgroundImage = Properties.Resources.TopDownCar;
            PlayerCar.BackgroundImageLayout = ImageLayout.Stretch;
            PlayerCar.Location = new Point(560, 641);
            PlayerCar.Name = "PlayerCar";
            PlayerCar.Size = new Size(50, 100);
            PlayerCar.TabIndex = 2;
            PlayerCar.TabStop = false;
            // 
            // trafficVehicle1
            // 
            trafficVehicle1.BackColor = Color.DimGray;
            trafficVehicle1.BackgroundImageLayout = ImageLayout.Stretch;
            trafficVehicle1.Location = new Point(444, 408);
            trafficVehicle1.Name = "trafficVehicle1";
            trafficVehicle1.Size = new Size(50, 100);
            trafficVehicle1.TabIndex = 3;
            trafficVehicle1.TabStop = false;
            // 
            // trafficVehicle2
            // 
            trafficVehicle2.BackColor = Color.DimGray;
            trafficVehicle2.BackgroundImageLayout = ImageLayout.Stretch;
            trafficVehicle2.Location = new Point(560, 29);
            trafficVehicle2.Name = "trafficVehicle2";
            trafficVehicle2.Size = new Size(50, 100);
            trafficVehicle2.TabIndex = 4;
            trafficVehicle2.TabStop = false;
            // 
            // trafficVehicle3
            // 
            trafficVehicle3.BackColor = Color.DimGray;
            trafficVehicle3.BackgroundImageLayout = ImageLayout.Stretch;
            trafficVehicle3.Location = new Point(684, 266);
            trafficVehicle3.Name = "trafficVehicle3";
            trafficVehicle3.Size = new Size(50, 100);
            trafficVehicle3.TabIndex = 5;
            trafficVehicle3.TabStop = false;
            // 
            // Game
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Back;
            ClientSize = new Size(1182, 753);
            Controls.Add(trafficVehicle3);
            Controls.Add(trafficVehicle2);
            Controls.Add(trafficVehicle1);
            Controls.Add(PlayerCar);
            Controls.Add(Road2);
            Controls.Add(Road1);
            Name = "Game";
            Text = "Game";
            KeyDown += Game_KeyDown;
            ((System.ComponentModel.ISupportInitialize)Road1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Road2).EndInit();
            ((System.ComponentModel.ISupportInitialize)PlayerCar).EndInit();
            ((System.ComponentModel.ISupportInitialize)trafficVehicle1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trafficVehicle2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trafficVehicle3).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer gameTimer;
        private PictureBox Road1;
        private PictureBox Road2;
        private PictureBox PlayerCar;
        private PictureBox trafficVehicle1;
        private PictureBox trafficVehicle2;
        private PictureBox trafficVehicle3;
    }
}