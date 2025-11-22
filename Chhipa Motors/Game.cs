using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chhipa_Motors
{
    public partial class Game : Form
    {
        // --- GAME OBJECTS ---
        class GameEntity
        {
            public Rectangle Rect;
            public Image Img;
        }

        Random rand = new Random();
        int minSpacing = 200;
        int moveSpeed = 10;

        // Cached Images
        Image carImage1, truckImage1, truckImage2, truckImage3, truckImage4, truckImage5;
        Image playerImage, roadImage;

        // Active Game Entities
        GameEntity player = new GameEntity();
        GameEntity traffic1 = new GameEntity();
        GameEntity traffic2 = new GameEntity();
        GameEntity traffic3 = new GameEntity();

        Rectangle road1Rect, road2Rect;

        public Game()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            rand = new Random();
            int carW = trafficVehicle1.Width;
            int carH = trafficVehicle1.Height;

            carImage1 = FitImageToBox(Properties.Resources.trafficCar1, carW, carH);
            truckImage1 = FitImageToBox(Properties.Resources.trafficTruck1, carW, carH);
            truckImage2 = FitImageToBox(Properties.Resources.trafficTruck2, carW, carH);
            truckImage3 = FitImageToBox(Properties.Resources.trafficTruck3, carW, carH);
            truckImage4 = FitImageToBox(Properties.Resources.trafficTruck4, carW, carH);
            truckImage5 = FitImageToBox(Properties.Resources.trafficTruck5, carW, carH);

            Image tempPlayerImg = PlayerCar.Image ?? PlayerCar.BackgroundImage;
            if (tempPlayerImg != null)
                playerImage = FitImageToBox(tempPlayerImg, PlayerCar.Width, PlayerCar.Height);

            Image tempRoadImg = Road1.Image ?? Road1.BackgroundImage;
            if (tempRoadImg != null)
            {
                roadImage = new Bitmap(tempRoadImg, Road1.Width, Road1.Height);
            }

            player.Rect = new Rectangle(PlayerCar.Location, PlayerCar.Size);
            player.Img = playerImage;
            PlayerCar.Visible = false;

            traffic1.Rect = new Rectangle(trafficVehicle1.Location, trafficVehicle1.Size);
            traffic1.Img = carImage1;
            trafficVehicle1.Visible = false;

            traffic2.Rect = new Rectangle(trafficVehicle2.Location, trafficVehicle2.Size);
            traffic2.Img = truckImage1;
            trafficVehicle2.Visible = false;

            traffic3.Rect = new Rectangle(trafficVehicle3.Location, trafficVehicle3.Size);
            traffic3.Img = truckImage2;
            trafficVehicle3.Visible = false;

            road1Rect = new Rectangle(Road1.Left, Road1.Top, Road1.Width, Road1.Height);
            road2Rect = new Rectangle(Road1.Left, Road1.Top - Road1.Height, Road1.Width, Road1.Height);

            Road1.Visible = false;
            Road2.Visible = false;
        }

        private Image FitImageToBox(Image original, int width, int height)
        {
            if (original == null) return null;
            return new Bitmap(original, width, height);
        }

        // --- DRAWING ENGINE ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // 1. Draw Roads
            if (roadImage != null)
            {
                g.DrawImage(roadImage, road1Rect);
                g.DrawImage(roadImage, road2Rect);
            }
            else
            {
                g.FillRectangle(Brushes.Gray, road1Rect);
                g.FillRectangle(Brushes.Gray, road2Rect);
                Pen lanePen = new Pen(Color.White, 5);
                g.DrawLine(lanePen, road1Rect.Left + road1Rect.Width / 2, road1Rect.Top, road1Rect.Left + road1Rect.Width / 2, road1Rect.Bottom);
                g.DrawLine(lanePen, road2Rect.Left + road2Rect.Width / 2, road2Rect.Top, road2Rect.Left + road2Rect.Width / 2, road2Rect.Bottom);
            }

            // 2. Draw Player
            if (player.Img != null)
                g.DrawImage(player.Img, player.Rect);
            else
                g.FillRectangle(Brushes.Blue, player.Rect);

            // 3. Draw Traffic
            g.DrawImage(traffic1.Img, traffic1.Rect);
            g.DrawImage(traffic2.Img, traffic2.Rect);
            g.DrawImage(traffic3.Img, traffic3.Rect);
        }

        private void spawnTraffic(GameEntity car)
        {
            // randomizing which traffic vehicle will be present
            int trafficCarNum = rand.Next(1, 7);

            switch (trafficCarNum)
            {
                case 1: car.Img = carImage1; break;
                case 2: car.Img = truckImage1; break;
                case 3: car.Img = truckImage2; break;
                case 4: car.Img = truckImage3; break;
                case 5: car.Img = truckImage4; break;
                case 6: car.Img = truckImage5; break;
                default: car.Img = carImage1; break;
            }

            // randomizing the lane of the vehicle
            int lanePosition = rand.Next(1, 4);
            switch (lanePosition)
            {
                case 1: car.Rect.X = 440; break;
                case 2: car.Rect.X = 560; break;
                case 3: car.Rect.X = 680; break;
            }

            int spawnY = -car.Rect.Height - rand.Next(100, 300);

            // logic for making sure that there is enough distance between the vehicles
            bool tooClose = true;
            int attempts = 0;

            while (tooClose && attempts < 5)
            {
                tooClose = false;

                if (car != traffic1 && traffic1.Rect.Y < 0)
                {
                    if (Math.Abs(car.Rect.X - traffic1.Rect.X) < 70 &&
                        Math.Abs(spawnY - traffic1.Rect.Y) < minSpacing) tooClose = true;
                }
                if (car != traffic2 && traffic2.Rect.Y < 0)
                {
                    if (Math.Abs(car.Rect.X - traffic2.Rect.X) < 70 &&
                        Math.Abs(spawnY - traffic2.Rect.Y) < minSpacing) tooClose = true;
                }
                if (car != traffic3 && traffic3.Rect.Y < 0)
                {
                    if (Math.Abs(car.Rect.X - traffic3.Rect.X) < 70 &&
                        Math.Abs(spawnY - traffic3.Rect.Y) < minSpacing) tooClose = true;
                }

                if (tooClose)
                {
                    lanePosition = rand.Next(1, 4);
                    switch (lanePosition)
                    {
                        case 1: car.Rect.X = 440; break;
                        case 2: car.Rect.X = 560; break;
                        case 3: car.Rect.X = 680; break;
                    }
                    spawnY -= rand.Next(100, 200);
                    attempts++;
                }
            }
            car.Rect.Y = spawnY;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move Roads
            road1Rect.Y += moveSpeed;
            road2Rect.Y += moveSpeed;

            if (road1Rect.Y >= this.Height)
            {
                road1Rect.Y = road2Rect.Y - road1Rect.Height;
            }
            if (road2Rect.Y >= this.Height)
            {
                road2Rect.Y = road1Rect.Y - road2Rect.Height;
            }

            // Move Traffic
            traffic1.Rect.Y += moveSpeed;
            traffic2.Rect.Y += moveSpeed;
            traffic3.Rect.Y += moveSpeed;

            if (traffic1.Rect.Y > this.Height) spawnTraffic(traffic1);
            if (traffic2.Rect.Y > this.Height) spawnTraffic(traffic2);
            if (traffic3.Rect.Y > this.Height) spawnTraffic(traffic3);

            // --- COLLISION LOGIC ---
            if (player.Rect.IntersectsWith(traffic1.Rect) ||
                player.Rect.IntersectsWith(traffic2.Rect) ||
                player.Rect.IntersectsWith(traffic3.Rect))
            {
                gameTimer.Stop();
                DialogResult result = MessageBox.Show("GAME OVER!", "Crash!", MessageBoxButtons.OK);

                if (result == DialogResult.OK)
                {
                    Application.Exit();
                }

                return;
            }

            this.Invalidate();
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            // disabling all controls once game is over
            if (gameTimer.Enabled == false) return;

            int speed = 15;

            // Handling movement of car
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                if (player.Rect.X - speed > road1Rect.X)
                    player.Rect.X -= speed;
            }

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                if (player.Rect.X + speed < road1Rect.X + road1Rect.Width - player.Rect.Width)
                    player.Rect.X += speed;
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                if (player.Rect.Y > 0)
                    player.Rect.Y -= speed;
            }

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                if (player.Rect.Y + player.Rect.Height < this.ClientSize.Height)
                    player.Rect.Y += speed;
            }
        }
    }
}