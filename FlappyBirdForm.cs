using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using static FlappyBird.ThemeManager;

namespace FlappyBird
{
    public partial class FlappyBirdForm : Form
    {
        private System.Windows.Forms.Timer gameTimer;
        private Bird bird;
        private List<Pipe> pipes;
        private int score;
        private bool gameStarted;
        private bool gameOver;
        private Random random;
        private int pipeCounter;
        private SoundManager soundManager;
        private List<Particle> particles;
        private float backgroundOffset = 0f;
        private Theme currentTheme = Theme.Day;
        private BirdSkin currentSkin = BirdSkin.Classic;
        private float globalOpacity = 1f; // Fade için
        private int screenShakeTimer = 0;
        private float screenShakeAmplitude = 0f;
        private const int PIPE_SPAWN_INTERVAL = 150; // Timer tick sayısı
        private const int PIPE_SPEED = 3;
        private const int GRAVITY = 2;
        private const int JUMP_FORCE = -15;

        public FlappyBirdForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Form ayarları
            this.Text = "Flappy Bird - C#";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.SkyBlue;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            // Oyun değişkenleri
            bird = new Bird(100, 200);
            pipes = new List<Pipe>();
            particles = new List<Particle>();
            soundManager = new SoundManager();
            score = 0;
            gameStarted = false;
            gameOver = false;
            random = new Random();
            pipeCounter = 0;
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;

            // Event handlers
            this.KeyDown += FlappyBirdForm_KeyDown;
            this.Paint += FlappyBirdForm_Paint;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted || gameOver) return;

            // Kuşu güncelle
            bird.Update();

            // Boruları güncelle
            UpdatePipes();

            // Parçacıkları güncelle
            UpdateParticles();

            // Arka plan animasyonu
            backgroundOffset += 0.5f;

            // Fade-in azalt
            if (globalOpacity < 1f)
            {
                globalOpacity = Math.Min(1f, globalOpacity + 0.05f);
            }

            // Ekran sarsıntısı süresi
            if (screenShakeTimer > 0)
            {
                screenShakeTimer--;
                if (screenShakeTimer == 0) screenShakeAmplitude = 0f;
            }

            // Çarpışma kontrolü
            CheckCollisions();

            // Skor güncelle
            UpdateScore();

            // Ekranı yenile
            this.Invalidate();
        }

        private void UpdatePipes()
        {
            // Yeni borular oluştur
            pipeCounter++;
            if (pipeCounter >= PIPE_SPAWN_INTERVAL)
            {
                CreatePipe();
                pipeCounter = 0;
            }

            // Mevcut boruları hareket ettir
            for (int i = pipes.Count - 1; i >= 0; i--)
            {
                pipes[i].Update();
                
                // Ekrandan çıkan boruları kaldır
                if (pipes[i].X + pipes[i].Width < 0)
                {
                    pipes.RemoveAt(i);
                }
            }
        }

        private void CreatePipe()
        {
            int gap = 150; // Borular arası boşluk
            int pipeHeight = random.Next(100, this.Height - gap - 100);
            
            pipes.Add(new Pipe(this.Width, pipeHeight, gap));
        }

        private void CheckCollisions()
        {
            // Zemin ve tavan çarpışması
            if (bird.Y <= 0 || bird.Y + bird.Height >= this.Height)
            {
                GameOver();
                return;
            }

            // Boru çarpışması
            foreach (Pipe pipe in pipes)
            {
                if (bird.Rectangle.IntersectsWith(pipe.TopRectangle) ||
                    bird.Rectangle.IntersectsWith(pipe.BottomRectangle))
                {
                    GameOver();
                    return;
                }
            }
        }

        private void UpdateParticles()
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update();
                if (!particles[i].IsAlive)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        private void UpdateScore()
        {
            foreach (Pipe pipe in pipes)
            {
                if (!pipe.ScoreCounted && pipe.X + pipe.Width < bird.X)
                {
                    score++;
                    pipe.ScoreCounted = true;
                    soundManager.PlayScoreSound();
                    StartScreenShake(6, 10);
                    
                    // Puan alma parçacıkları
                    CreateScoreParticles(pipe.X + pipe.Width, pipe.TopHeight + pipe.Gap / 2);
                }
            }
        }

        private void CreateScoreParticles(float x, float y)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = (float)(i * Math.PI * 2 / 8);
                float speed = 2f + (float)(random.NextDouble() * 3);
                float velocityX = (float)(Math.Cos(angle) * speed);
                float velocityY = (float)(Math.Sin(angle) * speed);
                
                Color color = Color.FromArgb(255, 255, 215, 0); // Altın renk
                particles.Add(new Particle(x, y, velocityX, velocityY, color, 4f, 30f));
            }
        }

        private void CreateExplosionParticles(float x, float y)
        {
            for (int i = 0; i < 15; i++)
            {
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float speed = 1f + (float)(random.NextDouble() * 4);
                float velocityX = (float)(Math.Cos(angle) * speed);
                float velocityY = (float)(Math.Sin(angle) * speed);
                
                Color color = Color.FromArgb(255, random.Next(200, 256), random.Next(0, 100), 0); // Kırmızı-turuncu
                particles.Add(new Particle(x, y, velocityX, velocityY, color, 6f, 40f));
            }
        }

        private void GameOver()
        {
            gameOver = true;
            gameTimer.Stop();
            soundManager.PlayGameOverSound();
            StartScreenShake(10, 20);
            
            // Patlama parçacıkları
            CreateExplosionParticles(bird.X + bird.Width / 2, bird.Y + bird.Height / 2);
        }

        private void FlappyBirdForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (!gameStarted)
                {
                    StartGame();
                }
                else if (!gameOver)
                {
                    bird.Jump();
                    soundManager.PlayJumpSound();
                }
            }
            else if (e.KeyCode == Keys.T)
            {
                // Tema değiştir: Day -> Night -> Neon -> Day
                currentTheme = currentTheme == Theme.Day ? Theme.Night : currentTheme == Theme.Night ? Theme.Neon : Theme.Day;
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.S)
            {
                // Skin değiştir
                currentSkin = currentSkin == BirdSkin.Classic ? BirdSkin.Blue :
                              currentSkin == BirdSkin.Blue ? BirdSkin.Red :
                              currentSkin == BirdSkin.Red ? BirdSkin.Neon : BirdSkin.Classic;
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.R && gameOver)
            {
                RestartGame();
            }
        }

        private void StartGame()
        {
            gameStarted = true;
            gameTimer.Start();
            globalOpacity = 0f; // Yeni oyun için fade-in
        }

        private void RestartGame()
        {
            bird = new Bird(100, 200);
            pipes.Clear();
            particles.Clear();
            score = 0;
            gameStarted = false;
            gameOver = false;
            pipeCounter = 0;
            gameTimer.Stop();
            this.Invalidate();
        }

        private void FlappyBirdForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Tema paleti
            var palette = GetPalette(currentTheme);

            // Ekran sarsıntısı ofseti
            float shakeX = 0f, shakeY = 0f;
            if (screenShakeTimer > 0 && screenShakeAmplitude > 0f)
            {
                var rnd = new Random();
                shakeX = ((float)rnd.NextDouble() * 2 - 1) * screenShakeAmplitude;
                shakeY = ((float)rnd.NextDouble() * 2 - 1) * screenShakeAmplitude;
            }

            // Arka plan
            using (SolidBrush bg = new SolidBrush(palette.Background))
                g.FillRectangle(bg, this.ClientRectangle);

            // Boruları çiz (tema ve fade ile)
            foreach (Pipe pipe in pipes)
            {
                pipe.DrawThemed(g, palette, globalOpacity);
            }

            // Kuşu çiz (skin ile)
            bird.DrawSkinned(g, currentSkin);

            // Skor
            DrawScore(g, palette.TextColor);

            // Oyun durumu mesajları
            if (!gameStarted)
            {
                DrawStartMessage(g, palette.TextColor);
            }
            else if (gameOver)
            {
                DrawGameOverMessage(g, palette.TextColor);
            }
        }

        private void DrawScore(Graphics g, Color textColor)
        {
            Font scoreFont = new Font("Arial", 24, FontStyle.Bold);
            string scoreText = $"Skor: {score}";
            SizeF textSize = g.MeasureString(scoreText, scoreFont);
            
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(scoreText, scoreFont, brush, new PointF(10, 10));
            }
        }

        private void DrawStartMessage(Graphics g, Color textColor)
        {
            Font messageFont = new Font("Arial", 20, FontStyle.Bold);
            string message = "SPACE tuşuna basarak başlayın!";
            SizeF textSize = g.MeasureString(message, messageFont);
            
            PointF position = new PointF(
                (this.Width - textSize.Width) / 2,
                (this.Height - textSize.Height) / 2);
            
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(message, messageFont, brush, position);
            }
        }

        private void DrawGameOverMessage(Graphics g, Color textColor)
        {
            Font messageFont = new Font("Arial", 20, FontStyle.Bold);
            string message = $"Oyun Bitti! Skor: {score}";
            string restartMessage = "R tuşuna basarak yeniden başlayın";
            
            SizeF textSize = g.MeasureString(message, messageFont);
            SizeF restartSize = g.MeasureString(restartMessage, messageFont);
            
            PointF messagePos = new PointF(
                (this.Width - textSize.Width) / 2,
                (this.Height - textSize.Height) / 2 - 30);
            
            PointF restartPos = new PointF(
                (this.Width - restartSize.Width) / 2,
                messagePos.Y + 40);
            
            using (var red = new SolidBrush(Color.Red))
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(message, messageFont, red, messagePos);
                g.DrawString(restartMessage, messageFont, brush, restartPos);
            }
        }

        private void StartScreenShake(int durationTicks, float amplitude)
        {
            screenShakeTimer = durationTicks;
            screenShakeAmplitude = amplitude;
        }
    }
}
