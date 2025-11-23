using Chhipa_Motors.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            public Image? Img;
        }

        Random rand = new Random();
        int minSpacing = 225;
        int moveSpeed = 15;

        int score = 0;
        const int pointsPerPass = 10;
        Font scoreFont = new Font("Segoe UI", 20, FontStyle.Bold);
        Brush scoreBrush = Brushes.White;

        int topScore = 0;
        Font topScoreFont = new Font("Segoe UI", 20, FontStyle.Bold);

        // Cached Images
        Image? carImage1, truckImage1, truckImage2, truckImage3, truckImage4, truckImage5;
        Image? playerImage, roadImage;

        // Active Game Entities
        GameEntity player = new GameEntity();
        GameEntity traffic1 = new GameEntity();
        GameEntity traffic2 = new GameEntity();
        GameEntity traffic3 = new GameEntity();
        
        Rectangle road1Rect, road2Rect;

        // Game Over UI
        Panel gameOverPanel = null!;
        Label lblOverTitle = null!;
        Label lblFinalScore = null!;
        Button btnPlayAgain = null!;
        Button btnQuit = null!;

        // Main Menu UI
        Panel menuPanel = null!;
        PictureBox pbQuickPlay = null!;
        PictureBox pbCredits = null!;
        PictureBox pbHowTo = null!;
        PictureBox pbQuit = null!;
        Label lblMenuTopScore = null!;

        // Car selection panel 
        Panel carSelectPanel = null!;
        Label lblSelectCar = null!;
        PictureBox[] carChoices = Array.Empty<PictureBox>();
        Button btnCarBack = null!;

        // Credits / HowTo panels
        Panel creditsPanel = null!;
        Label creditsLabel = null!;
        Button creditsClose = null!;
        Panel howToPanel = null!;
        Label howToLabel = null!;
        Button howToClose = null!;

        string TopScoreFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChhipaMotors", "topscore.txt");

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

            Image? tempPlayerImg = PlayerCar.Image ?? PlayerCar.BackgroundImage;
            if (tempPlayerImg != null)
                playerImage = FitImageToBox(tempPlayerImg, PlayerCar.Width, PlayerCar.Height);

            Image? tempRoadImg = Road1.Image ?? Road1.BackgroundImage;
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

            // top score
            LoadTopScore();

            CreateInFormGameOverUI();
            CreateMenuUI();
            CreateCarSelectUI();
            CreateCreditsPanel();
            CreateHowToPanel();

            spawnTraffic(traffic1);
            spawnTraffic(traffic2);
            spawnTraffic(traffic3);

            PauseForMenu();
        }

        private void LoadTopScore()
        {
            try
            {
                string dir = Path.GetDirectoryName(TopScoreFilePath) ?? "";
                if (!Directory.Exists(dir) && dir.Length > 0)
                    Directory.CreateDirectory(dir);

                if (File.Exists(TopScoreFilePath))
                {
                    var txt = File.ReadAllText(TopScoreFilePath).Trim();
                    if (int.TryParse(txt, out var val))
                        topScore = val;
                }
            }
            catch
            {
                topScore = 0;
            }
        }

        private void SaveTopScore()
        {
            try
            {
                string dir = Path.GetDirectoryName(TopScoreFilePath) ?? "";
                if (!Directory.Exists(dir) && dir.Length > 0)
                    Directory.CreateDirectory(dir);

                File.WriteAllText(TopScoreFilePath, topScore.ToString());
            }
            catch
            {
                // ignore write errors
            }
        }

        private void UpdateTopScoreIfNeeded()
        {
            if (score > topScore)
            {
                topScore = score;
                SaveTopScore();
            }
            UpdateMenuTopScoreLabel();
        }

        private void UpdateMenuTopScoreLabel()
        {
            if (lblMenuTopScore != null)
            {
                lblMenuTopScore.Text = $"Best: {topScore}";
            }
        }

        private void PauseForMenu()
        {
            if (gameTimer != null) gameTimer.Stop();

            menuPanel.Visible = true;
            menuPanel.BringToFront();

            if (gameOverPanel != null) gameOverPanel.Visible = false;
            carSelectPanel.Visible = false;

            UpdateMenuTopScoreLabel();
        }

        private void StartGameFromMenu()
        {
            // legacy start without explicit selection -> use current playerImage or default
            StartGameWithSelectedCar(playerImage);
        }

        private void StartGameWithSelectedCar(Image? selectedCar)
        {
            menuPanel.Visible = false;
            carSelectPanel.Visible = false;

            score = 0;

            if (selectedCar != null)
            {
                playerImage = FitImageToBox(selectedCar, PlayerCar.Width, PlayerCar.Height);
                player.Img = playerImage;
            }

            player.Rect = new Rectangle(PlayerCar.Location, PlayerCar.Size);
            road1Rect = new Rectangle(Road1.Left, Road1.Top, Road1.Width, Road1.Height);
            road2Rect = new Rectangle(Road1.Left, Road1.Top - Road1.Height, Road1.Width, Road1.Height);
            spawnTraffic(traffic1);
            spawnTraffic(traffic2);
            spawnTraffic(traffic3);

            gameTimer.Start();
            this.Focus();
        }

        private Image? FitImageToBox(Image original, int width, int height)
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
            if (traffic1.Img != null) 
                g.DrawImage(traffic1.Img, traffic1.Rect);
            if (traffic2.Img != null) 
                g.DrawImage(traffic2.Img, traffic2.Rect);
            if (traffic3.Img != null)
                g.DrawImage(traffic3.Img, traffic3.Rect);

            // 4. Draw Score (top-right)
            string scoreText = $"Score: {score}";
            SizeF textSize = g.MeasureString(scoreText, scoreFont);
            float padding = 20f;
            float x = this.ClientSize.Width - padding - textSize.Width;
            float y = padding;
            // Draw shadow
            g.DrawString(scoreText, scoreFont, Brushes.Black, x + 1, y + 1);
            g.DrawString(scoreText, scoreFont, scoreBrush, x, y);

            // Draw top (best) score under current score
            string bestText = $"Best: {topScore}";
            SizeF bestSize = g.MeasureString(bestText, topScoreFont);
            g.DrawString(bestText, topScoreFont, Brushes.Black, x + 1, y + textSize.Height + 4 + 1);
            g.DrawString(bestText, topScoreFont, Brushes.White, x, y + textSize.Height + 4);
        }

        private void spawnTraffic(GameEntity car)
        {
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

            int lanePosition = rand.Next(1, 4);
            switch (lanePosition)
            {
                case 1: car.Rect.X = 440; break;
                case 2: car.Rect.X = 560; break;
                case 3: car.Rect.X = 680; break;
            }

            int spawnY = -car.Rect.Height - rand.Next(100, 300);

            var others = new[] { traffic1, traffic2, traffic3 }.Where(t => !ReferenceEquals(t, car)).ToArray();
            int attempts = 0;
            while (attempts < 5)
            {
                bool conflict = false;
                foreach (var other in others)
                {
                    if (Math.Abs(car.Rect.X - other.Rect.X) < 70)
                    {
                        if (Math.Abs(spawnY - other.Rect.Y) < minSpacing)
                        {
                            conflict = true;
                            break;
                        }
                    }
                    else
                    {
                        if (Math.Abs(spawnY - other.Rect.Y) < minSpacing / 2)
                        {
                            conflict = true;
                            break;
                        }
                    }
                }

                if (!conflict) break;

                spawnY -= rand.Next(minSpacing, minSpacing + 200);
                if (attempts % 2 == 1)
                {
                    lanePosition = rand.Next(1, 4);
                    switch (lanePosition)
                    {
                        case 1: car.Rect.X = 440; break;
                        case 2: car.Rect.X = 560; break;
                        case 3: car.Rect.X = 680; break;
                    }
                }

                attempts++;
            }

            car.Rect.Y = spawnY;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
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

            if (traffic1.Rect.Y > this.Height)
            {
                score += pointsPerPass;
                spawnTraffic(traffic1);
            }
            if (traffic2.Rect.Y > this.Height)
            {
                score += pointsPerPass;
                spawnTraffic(traffic2);
            }
            if (traffic3.Rect.Y > this.Height)
            {
                score += pointsPerPass;
                spawnTraffic(traffic3);
            }

            // --- COLLISION LOGIC ---
            if (player.Rect.IntersectsWith(traffic1.Rect) ||
                player.Rect.IntersectsWith(traffic2.Rect) ||
                player.Rect.IntersectsWith(traffic3.Rect))
            {
                gameTimer.Stop();

                // update best
                UpdateTopScoreIfNeeded();

                lblFinalScore.Text = $"Score: {score}\nBest: {topScore}";
                gameOverPanel.Visible = true;
                gameOverPanel.BringToFront();
                this.Focus(); 

                return;
            }

            this.Invalidate();
        }

        private void CreateInFormGameOverUI()
        {
            // Panel
            gameOverPanel = new Panel
            {
                Size = new Size(340, 170),
                BackColor = Color.FromArgb(230, 30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            // Title
            lblOverTitle = new Label
            {
                Text = "GAME OVER",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };
            gameOverPanel.Controls.Add(lblOverTitle);

            // Score label
            lblFinalScore = new Label
            {
                Text = "Score: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };
            gameOverPanel.Controls.Add(lblFinalScore);

            // Buttons
            btnPlayAgain = new Button
            {
                Text = "Play Again",
                Size = new Size(120, 36),
                Location = new Point(40, 110)
            };
            btnPlayAgain.Click += (s, e) =>
            {
                gameOverPanel.Visible = false;
                ResetGame();
            };
            gameOverPanel.Controls.Add(btnPlayAgain);

            btnQuit = new Button
            {
                Text = "Quit",
                Size = new Size(120, 36),
                Location = new Point(gameOverPanel.Width - 40 - 120, 110)
            };
            btnQuit.Click += (s, e) => Application.Exit();
            gameOverPanel.Controls.Add(btnQuit);

            CenterOverlayPanel(gameOverPanel);
            this.Controls.Add(gameOverPanel);
            gameOverPanel.BringToFront();

            this.Resize += (s, e) => CenterOverlayPanel(gameOverPanel);
        }

        private void CreateMenuUI()
        {
            menuPanel = new Panel
            {
                Size = new Size(500, 400),
                BackColor = Color.FromArgb(200, 10, 10, 10),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            pbQuickPlay = new PictureBox
            {
                Image = Properties.Resources.b_QuickPlay,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            pbQuickPlay.Click += (s, e) =>
            {
                // show car selection panel
                menuPanel.Visible = false;
                carSelectPanel.Visible = true;
                carSelectPanel.BringToFront();
            };

            Image imgCredits = Properties.Resources.b_How2Play;
            double scaleFactor = 0.7;
            int credWidth = (int)(imgCredits.Width * scaleFactor);
            int credHeight = (int)(imgCredits.Height * scaleFactor + 4);

            pbCredits = new PictureBox
            {
                Image = Properties.Resources.b_Credits,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(credWidth, credHeight),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            pbCredits.Click += (s, e) => ShowCredits();

            pbHowTo = new PictureBox
            {
                Image = Properties.Resources.b_How2Play,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            pbHowTo.Click += (s, e) => ShowHowTo();

            pbQuit = new PictureBox
            {
                Image = Properties.Resources.b_Quit,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            pbQuit.Click += (s, e) => Application.Exit();

            int gap = 16;
            int totalHeight = pbQuickPlay.Height + pbCredits.Height + pbHowTo.Height + pbQuit.Height + (3 * gap);
            int currentY = (menuPanel.Height - totalHeight) / 2;

            pbQuickPlay.Location = new Point((menuPanel.Width - pbQuickPlay.Width) / 2, currentY);
            currentY = pbQuickPlay.Bottom + gap;
            pbCredits.Location = new Point((menuPanel.Width - pbCredits.Width) / 2, currentY);
            currentY = pbCredits.Bottom + gap;
            pbHowTo.Location = new Point((menuPanel.Width - pbHowTo.Width) / 2, currentY);
            currentY = pbHowTo.Bottom + gap;
            pbQuit.Location = new Point((menuPanel.Width - pbQuit.Width) / 2, currentY);

            // top score on menu
            lblMenuTopScore = new Label
            {
                Text = $"Best: {topScore}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 12),
                BackColor = Color.Transparent
            };
            menuPanel.Controls.Add(lblMenuTopScore);

            menuPanel.Controls.Add(pbQuickPlay);
            menuPanel.Controls.Add(pbCredits);
            menuPanel.Controls.Add(pbHowTo);
            menuPanel.Controls.Add(pbQuit);

            CenterOverlayPanel(menuPanel);
            this.Controls.Add(menuPanel);
            menuPanel.BringToFront();

            this.Resize += (s, e) => CenterOverlayPanel(menuPanel);
        }

        private void CreateCarSelectUI()
        {
            carSelectPanel = new Panel
            {
                Size = new Size(560, 300),
                BackColor = Color.FromArgb(220, 15, 15, 15),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            lblSelectCar = new Label
            {
                Text = "Select Your Car",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            carSelectPanel.Controls.Add(lblSelectCar);

            // resources to show - using the names you specified
            var resources = new Image?[]
            {
                Resources.chev,
                Resources.f1car,
                Resources.pointy,
                Resources.sportscar1,
                Resources.TopDownCar
            };

            carChoices = new PictureBox[resources.Length];
            int spacing = 12;
            int boxWidth = 96;
            int boxHeight = 96;
            int startX = (carSelectPanel.Width - (resources.Length * boxWidth + (resources.Length - 1) * spacing)) / 2;
            int y = 80;

            for (int i = 0; i < resources.Length; i++)
            {
                var pb = new PictureBox
                {
                    Size = new Size(boxWidth, boxHeight),
                    Location = new Point(startX + i * (boxWidth + spacing), y),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent,
                    BorderStyle = BorderStyle.FixedSingle,
                    Image = resources[i]
                };

                int idx = i; // capture
                pb.Click += (s, e) =>
                {
                    // on selection, set as player image and start game
                    Image? selected = resources[idx];
                    StartGameWithSelectedCar(selected);
                };

                carChoices[i] = pb;
                carSelectPanel.Controls.Add(pb);
            }

            btnCarBack = new Button
            {
                Text = "Back",
                Size = new Size(100, 36),
                Location = new Point((carSelectPanel.Width - 100) / 2, carSelectPanel.Height - 56)
            };
            btnCarBack.Click += (s, e) =>
            {
                carSelectPanel.Visible = false;
                menuPanel.Visible = true;
                menuPanel.BringToFront();
            };
            carSelectPanel.Controls.Add(btnCarBack);

            CenterOverlayPanel(carSelectPanel);
            this.Controls.Add(carSelectPanel);
            carSelectPanel.BringToFront();

            this.Resize += (s, e) => CenterOverlayPanel(carSelectPanel);
        }

        private void CreateCreditsPanel()
        {
            creditsPanel = new Panel
            {
                Size = new Size(420, 220),
                BackColor = Color.FromArgb(230, 20, 20, 20),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            creditsLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 150,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Text = "Creators:\n- Eesa Shoaib\n- Harris Tabussum\n- Husnain Barkat\n- Abdullah Mushtaq\nThank you for playing!"
            };
            creditsPanel.Controls.Add(creditsLabel);

            creditsClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 32),
                Location = new Point((creditsPanel.Width - 100) / 2, creditsPanel.Height - 50)
            };
            creditsClose.Click += (s, e) => creditsPanel.Visible = false;
            creditsPanel.Controls.Add(creditsClose);

            CenterOverlayPanel(creditsPanel);
            this.Controls.Add(creditsPanel);
            creditsPanel.BringToFront();

            this.Resize += (s, e) => CenterOverlayPanel(creditsPanel);
        }

        private void CreateHowToPanel()
        {
            howToPanel = new Panel
            {
                Size = new Size(480, 240),
                BackColor = Color.FromArgb(230, 20, 20, 20),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            howToLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 180,
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Padding(12),
                Text = "How to Play:\n\n- Use arrow keys or WASD to move your car.\n- Avoid incoming traffic.\n- Each car you pass gives +10 points.\n- If you crash, you'll see the Game Over overlay with your final score.\n\nGood luck!"
            };
            howToPanel.Controls.Add(howToLabel);

            howToClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 32),
                Location = new Point((howToPanel.Width - 100) / 2, howToPanel.Height - 50)
            };
            howToClose.Click += (s, e) => howToPanel.Visible = false;
            howToPanel.Controls.Add(howToClose);

            CenterOverlayPanel(howToPanel);
            this.Controls.Add(howToPanel);
            howToPanel.BringToFront();

            this.Resize += (s, e) => CenterOverlayPanel(howToPanel);
        }

        private void CenterOverlayPanel(Panel p)
        {
            if (p == null) return;
            int x = Math.Max(0, (this.ClientSize.Width - p.Width) / 2);
            int y = Math.Max(0, (this.ClientSize.Height - p.Height) / 2);
            p.Location = new Point(x, y);
        }

        private void ShowCredits()
        {
            creditsPanel.Visible = true;
            creditsPanel.BringToFront();
        }

        private void ShowHowTo()
        {
            howToPanel.Visible = true;
            howToPanel.BringToFront();
        }

        private void ResetGame()
        {
            score = 0;

            player.Rect = new Rectangle(PlayerCar.Location, PlayerCar.Size);

            road1Rect = new Rectangle(Road1.Left, Road1.Top, Road1.Width, Road1.Height);
            road2Rect = new Rectangle(Road1.Left, Road1.Top - Road1.Height, Road1.Width, Road1.Height);

            spawnTraffic(traffic1);
            spawnTraffic(traffic2);
            spawnTraffic(traffic3);

            if (gameOverPanel != null) gameOverPanel.Visible = false;
            if (menuPanel != null) menuPanel.Visible = false;
            if (creditsPanel != null) creditsPanel.Visible = false;
            if (howToPanel != null) howToPanel.Visible = false;

            gameTimer.Start();
            this.Invalidate();

            this.Focus();
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameTimer.Enabled == false) return;

            int speed = 25;

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
