using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static FlappyBird.ThemeManager;

namespace FlappyBird
{
    public class Pipe
    {
        public float X { get; set; }
        public int TopHeight { get; set; }
        public int BottomY { get; set; }
        public int Width { get; set; }
        public int Gap { get; set; }
        public bool ScoreCounted { get; set; }
        public Rectangle TopRectangle => new Rectangle((int)X, 0, Width, TopHeight);
        public Rectangle BottomRectangle => new Rectangle((int)X, BottomY, Width, 600 - BottomY);

        private const int PIPE_SPEED = 3;

        public Pipe(int x, int topHeight, int gap)
        {
            X = x;
            TopHeight = topHeight;
            Gap = gap;
            BottomY = topHeight + gap;
            Width = 60;
            ScoreCounted = false;
        }

        public void Update()
        {
            X -= PIPE_SPEED;
        }

        public void Draw(Graphics g)
        {
            // Anti-aliasing için yüksek kalite ayarları
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Üst boru gradyanı
            using (LinearGradientBrush topPipeBrush = new LinearGradientBrush(
                new RectangleF(X, 0, Width, TopHeight),
                Color.FromArgb(255, 46, 125, 50), // Koyu yeşil
                Color.FromArgb(255, 34, 139, 34), // Orta yeşil
                LinearGradientMode.Vertical))
            {
                RectangleF topPipe = new RectangleF(X, 0, Width, TopHeight);
                g.FillRectangle(topPipeBrush, topPipe);
            }
            
            // Alt boru gradyanı
            using (LinearGradientBrush bottomPipeBrush = new LinearGradientBrush(
                new RectangleF(X, BottomY, Width, 600 - BottomY),
                Color.FromArgb(255, 46, 125, 50), // Koyu yeşil
                Color.FromArgb(255, 34, 139, 34), // Orta yeşil
                LinearGradientMode.Vertical))
            {
                RectangleF bottomPipe = new RectangleF(X, BottomY, Width, 600 - BottomY);
                g.FillRectangle(bottomPipeBrush, bottomPipe);
            }
            
            // Boru kenarları (3D efekt)
            using (Pen borderPen = new Pen(Color.FromArgb(255, 0, 100, 0), 4))
            {
                // Üst boru kenarları
                g.DrawRectangle(borderPen, X, 0, Width, TopHeight);
                
                // Alt boru kenarları
                g.DrawRectangle(borderPen, X, BottomY, Width, 600 - BottomY);
            }
            
            // İç gölge efekti
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, Color.Black)))
            {
                // Üst boru iç gölgesi
                RectangleF topShadow = new RectangleF(X + 2, 2, Width - 4, TopHeight - 4);
                g.FillRectangle(shadowBrush, topShadow);
                
                // Alt boru iç gölgesi
                RectangleF bottomShadow = new RectangleF(X + 2, BottomY + 2, Width - 4, 600 - BottomY - 4);
                g.FillRectangle(shadowBrush, bottomShadow);
            }
            
            // Boru içi detayları (daha ince)
            using (SolidBrush detailBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
            {
                // Üst boru içi çizgiler
                for (int i = 10; i < TopHeight - 10; i += 15)
                {
                    g.FillRectangle(detailBrush, X + 8, i, Width - 16, 1);
                }
                
                // Alt boru içi çizgiler
                for (int i = BottomY + 10; i < 590; i += 15)
                {
                    g.FillRectangle(detailBrush, X + 8, i, Width - 16, 1);
                }
            }
            
            // Boru başlıkları (3D efekt)
            using (LinearGradientBrush topCapBrush = new LinearGradientBrush(
                new RectangleF(X - 5, TopHeight - 25, Width + 10, 25),
                Color.FromArgb(255, 0, 100, 0), // Koyu yeşil
                Color.FromArgb(255, 34, 139, 34), // Açık yeşil
                LinearGradientMode.Vertical))
            {
                RectangleF topCap = new RectangleF(X - 5, TopHeight - 25, Width + 10, 25);
                g.FillRectangle(topCapBrush, topCap);
            }
            
            using (LinearGradientBrush bottomCapBrush = new LinearGradientBrush(
                new RectangleF(X - 5, BottomY, Width + 10, 25),
                Color.FromArgb(255, 0, 100, 0), // Koyu yeşil
                Color.FromArgb(255, 34, 139, 34), // Açık yeşil
                LinearGradientMode.Vertical))
            {
                RectangleF bottomCap = new RectangleF(X - 5, BottomY, Width + 10, 25);
                g.FillRectangle(bottomCapBrush, bottomCap);
            }
            
            // Başlık kenarları (3D efekt)
            using (Pen capBorderPen = new Pen(Color.FromArgb(255, 0, 80, 0), 3))
            {
                // Üst başlık kenarı
                g.DrawRectangle(capBorderPen, X - 5, TopHeight - 25, Width + 10, 25);
                
                // Alt başlık kenarı
                g.DrawRectangle(capBorderPen, X - 5, BottomY, Width + 10, 25);
            }
            
            // Başlık parıltısı
            using (SolidBrush shineBrush = new SolidBrush(Color.FromArgb(100, Color.White)))
            {
                // Üst başlık parıltısı
                RectangleF topShine = new RectangleF(X - 3, TopHeight - 23, Width + 6, 3);
                g.FillRectangle(shineBrush, topShine);
                
                // Alt başlık parıltısı
                RectangleF bottomShine = new RectangleF(X - 3, BottomY + 2, Width + 6, 3);
                g.FillRectangle(shineBrush, bottomShine);
            }
            
            // Boru gölgesi (alt kısım)
            using (SolidBrush pipeShadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
            {
                RectangleF topShadow = new RectangleF(X + 3, TopHeight + 1, Width - 6, 5);
                RectangleF bottomShadow = new RectangleF(X + 3, BottomY - 6, Width - 6, 5);
                g.FillRectangle(pipeShadowBrush, topShadow);
                g.FillRectangle(pipeShadowBrush, bottomShadow);
            }
        }

        public void DrawThemed(Graphics g, ThemePalette palette, float globalOpacity)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int alpha = (int)(Math.Min(1f, Math.Max(0f, globalOpacity)) * 255);
            Color dark = Color.FromArgb(alpha, palette.PipeDark);
            Color light = Color.FromArgb(alpha, palette.PipeLight);
            Color border = Color.FromArgb(alpha, palette.PipeBorder);

            using (LinearGradientBrush topPipeBrush = new LinearGradientBrush(
                new RectangleF(X, 0, Width, TopHeight),
                dark,
                light,
                LinearGradientMode.Vertical))
            {
                RectangleF topPipe = new RectangleF(X, 0, Width, TopHeight);
                g.FillRectangle(topPipeBrush, topPipe);
            }

            using (LinearGradientBrush bottomPipeBrush = new LinearGradientBrush(
                new RectangleF(X, BottomY, Width, 600 - BottomY),
                dark,
                light,
                LinearGradientMode.Vertical))
            {
                RectangleF bottomPipe = new RectangleF(X, BottomY, Width, 600 - BottomY);
                g.FillRectangle(bottomPipeBrush, bottomPipe);
            }

            using (Pen borderPen = new Pen(border, 4))
            {
                g.DrawRectangle(borderPen, X, 0, Width, TopHeight);
                g.DrawRectangle(borderPen, X, BottomY, Width, 600 - BottomY);
            }
        }
    }
}
