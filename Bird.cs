using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static FlappyBird.ThemeManager;

namespace FlappyBird
{
    public class Bird
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Velocity { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Rectangle => new Rectangle((int)X, (int)Y, Width, Height);

        private const float GRAVITY = 0.8f;
        private const float JUMP_FORCE = -12f;
        private const float MAX_VELOCITY = 15f;
        
        // Animasyon değişkenleri
        private float wingAnimation = 0f;
        private float rotation = 0f;
        private bool isFlapping = false;
        private int flapTimer = 0;

        public Bird(int x, int y)
        {
            X = x;
            Y = y;
            Velocity = 0;
            Width = 30;
            Height = 25;
        }

        public void Update()
        {
            // Yerçekimi uygula
            Velocity += GRAVITY;
            
            // Maksimum hız sınırı
            if (Velocity > MAX_VELOCITY)
                Velocity = MAX_VELOCITY;
            
            // Pozisyonu güncelle
            Y += Velocity;
            
            // Animasyonları güncelle
            wingAnimation += 0.3f;
            rotation = Math.Max(-30, Math.Min(30, Velocity * 2));
            
            // Kanat çırpma animasyonu
            if (isFlapping)
            {
                flapTimer--;
                if (flapTimer <= 0)
                    isFlapping = false;
            }
        }

        public void Jump()
        {
            Velocity = JUMP_FORCE;
            isFlapping = true;
            flapTimer = 10;
        }

        public void Draw(Graphics g)
        {
            // Anti-aliasing için yüksek kalite ayarları
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Kuş merkez noktası
            float centerX = X + Width / 2f;
            float centerY = Y + Height / 2f;
            
            // Dönüş matrisi kaydet
            GraphicsState state = g.Save();
            
            // Kuşu döndür
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(rotation);
            g.TranslateTransform(-centerX, -centerY);
            
            // Ana gövde gradyanı
            using (LinearGradientBrush bodyBrush = new LinearGradientBrush(
                new RectangleF(X, Y, Width, Height),
                Color.FromArgb(255, 255, 165, 0), // Altın sarısı
                Color.FromArgb(255, 255, 140, 0), // Koyu turuncu
                LinearGradientMode.Vertical))
            {
                // Ana gövde (oval)
                RectangleF bodyRect = new RectangleF(X, Y, Width, Height);
                g.FillEllipse(bodyBrush, bodyRect);
                
                // Gövde kenarı
                using (Pen bodyPen = new Pen(Color.FromArgb(200, 255, 100, 0), 2))
                {
                    g.DrawEllipse(bodyPen, bodyRect);
                }
            }
            
            // Kanat animasyonu
            float wingOffset = isFlapping ? (float)Math.Sin(wingAnimation * 2) * 3 : (float)Math.Sin(wingAnimation) * 1;
            
            // Kanat gradyanı
            using (LinearGradientBrush wingBrush = new LinearGradientBrush(
                new RectangleF(X + 3, Y + 6 + wingOffset, Width - 6, Height - 10),
                Color.FromArgb(255, 255, 69, 0), // Kırmızı-turuncu
                Color.FromArgb(255, 255, 140, 0), // Turuncu
                LinearGradientMode.Vertical))
            {
                RectangleF wingRect = new RectangleF(X + 3, Y + 6 + wingOffset, Width - 6, Height - 10);
                g.FillEllipse(wingBrush, wingRect);
                
                // Kanat detayları
                using (Pen wingPen = new Pen(Color.FromArgb(150, 255, 50, 0), 1))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        float y = wingRect.Y + wingRect.Height * (i + 1) / 4;
                        g.DrawLine(wingPen, wingRect.X + 2, y, wingRect.Right - 2, y);
                    }
                }
            }
            
            // Göz beyazı
            using (SolidBrush eyeBrush = new SolidBrush(Color.White))
            {
                RectangleF eyeRect = new RectangleF(X + Width - 10, Y + 4, 8, 8);
                g.FillEllipse(eyeBrush, eyeRect);
                
                // Göz kenarı
                using (Pen eyePen = new Pen(Color.FromArgb(100, Color.Black), 1))
                {
                    g.DrawEllipse(eyePen, eyeRect);
                }
            }
            
            // Göz bebeği
            using (SolidBrush pupilBrush = new SolidBrush(Color.Black))
            {
                RectangleF pupilRect = new RectangleF(X + Width - 8, Y + 5, 5, 5);
                g.FillEllipse(pupilBrush, pupilRect);
            }
            
            // Göz parıltısı
            using (SolidBrush shineBrush = new SolidBrush(Color.White))
            {
                RectangleF shineRect = new RectangleF(X + Width - 7, Y + 5.5f, 2, 2);
                g.FillEllipse(shineBrush, shineRect);
            }
            
            // Gaga gradyanı
            using (LinearGradientBrush beakBrush = new LinearGradientBrush(
                new RectangleF(X + Width, Y + Height / 2 - 3, 10, 6),
                Color.FromArgb(255, 255, 215, 0), // Altın
                Color.FromArgb(255, 255, 165, 0), // Turuncu-altın
                LinearGradientMode.Horizontal))
            {
                PointF[] beakPoints = {
                    new PointF(X + Width, Y + Height / 2),
                    new PointF(X + Width + 10, Y + Height / 2 - 3),
                    new PointF(X + Width + 10, Y + Height / 2 + 3)
                };
                g.FillPolygon(beakBrush, beakPoints);
                
                // Gaga kenarı
                using (Pen beakPen = new Pen(Color.FromArgb(200, 255, 140, 0), 1))
                {
                    g.DrawPolygon(beakPen, beakPoints);
                }
            }
            
            // Kuyruk
            using (LinearGradientBrush tailBrush = new LinearGradientBrush(
                new RectangleF(X - 5, Y + Height / 2 - 2, 8, 4),
                Color.FromArgb(255, 255, 69, 0),
                Color.FromArgb(255, 255, 140, 0),
                LinearGradientMode.Horizontal))
            {
                PointF[] tailPoints = {
                    new PointF(X - 5, Y + Height / 2),
                    new PointF(X - 8, Y + Height / 2 - 3),
                    new PointF(X - 8, Y + Height / 2 + 3)
                };
                g.FillPolygon(tailBrush, tailPoints);
            }
            
            // Ayaklar
            using (SolidBrush footBrush = new SolidBrush(Color.FromArgb(255, 255, 69, 0)))
            {
                RectangleF leftFoot = new RectangleF(X + Width / 2 - 3, Y + Height - 2, 2, 4);
                RectangleF rightFoot = new RectangleF(X + Width / 2 + 1, Y + Height - 2, 2, 4);
                g.FillEllipse(footBrush, leftFoot);
                g.FillEllipse(footBrush, rightFoot);
            }
            
            // Dönüş matrisini geri yükle
            g.Restore(state);
            
            // Kuş gölgesi (daha gerçekçi)
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
            {
                RectangleF shadowRect = new RectangleF(X + 3, Y + Height + 1, Width - 6, Height / 4);
                g.FillEllipse(shadowBrush, shadowRect);
            }
            
            // Parıltı efekti
            if (isFlapping)
            {
                using (SolidBrush sparkleBrush = new SolidBrush(Color.FromArgb(100, Color.White)))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float sparkleX = X + (float)(new Random().NextDouble() * Width);
                        float sparkleY = Y + (float)(new Random().NextDouble() * Height);
                        RectangleF sparkleRect = new RectangleF(sparkleX, sparkleY, 2, 2);
                        g.FillEllipse(sparkleBrush, sparkleRect);
                    }
                }
            }
        }

        public void DrawSkinned(Graphics g, BirdSkin skin)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color bodyStart;
            Color bodyEnd;
            Color wingStart;
            Color wingEnd;

            switch (skin)
            {
                case BirdSkin.Blue:
                    bodyStart = Color.FromArgb(255, 0, 170, 255);
                    bodyEnd = Color.FromArgb(255, 0, 120, 220);
                    wingStart = Color.FromArgb(255, 0, 140, 255);
                    wingEnd = Color.FromArgb(255, 0, 200, 255);
                    break;
                case BirdSkin.Red:
                    bodyStart = Color.FromArgb(255, 255, 80, 80);
                    bodyEnd = Color.FromArgb(255, 220, 40, 40);
                    wingStart = Color.FromArgb(255, 255, 100, 100);
                    wingEnd = Color.FromArgb(255, 255, 160, 160);
                    break;
                case BirdSkin.Neon:
                    bodyStart = Color.FromArgb(255, 0, 255, 170);
                    bodyEnd = Color.FromArgb(255, 0, 200, 255);
                    wingStart = Color.FromArgb(255, 255, 0, 200);
                    wingEnd = Color.FromArgb(255, 0, 255, 255);
                    break;
                case BirdSkin.Classic:
                default:
                    bodyStart = Color.FromArgb(255, 255, 165, 0);
                    bodyEnd = Color.FromArgb(255, 255, 140, 0);
                    wingStart = Color.FromArgb(255, 255, 69, 0);
                    wingEnd = Color.FromArgb(255, 255, 140, 0);
                    break;
            }

            float centerX = X + Width / 2f;
            float centerY = Y + Height / 2f;
            GraphicsState state = g.Save();
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(rotation);
            g.TranslateTransform(-centerX, -centerY);

            using (LinearGradientBrush bodyBrush = new LinearGradientBrush(
                new RectangleF(X, Y, Width, Height),
                bodyStart,
                bodyEnd,
                LinearGradientMode.Vertical))
            {
                RectangleF bodyRect = new RectangleF(X, Y, Width, Height);
                g.FillEllipse(bodyBrush, bodyRect);
                using (Pen bodyPen = new Pen(Color.FromArgb(200, bodyEnd), 2))
                {
                    g.DrawEllipse(bodyPen, bodyRect);
                }
            }

            float wingOffset = isFlapping ? (float)Math.Sin(wingAnimation * 2) * 3 : (float)Math.Sin(wingAnimation) * 1;
            using (LinearGradientBrush wingBrush = new LinearGradientBrush(
                new RectangleF(X + 3, Y + 6 + wingOffset, Width - 6, Height - 10),
                wingStart,
                wingEnd,
                LinearGradientMode.Vertical))
            {
                RectangleF wingRect = new RectangleF(X + 3, Y + 6 + wingOffset, Width - 6, Height - 10);
                g.FillEllipse(wingBrush, wingRect);
            }

            using (SolidBrush eyeBrush = new SolidBrush(Color.White))
            {
                RectangleF eyeRect = new RectangleF(X + Width - 10, Y + 4, 8, 8);
                g.FillEllipse(eyeBrush, eyeRect);
                using (Pen eyePen = new Pen(Color.FromArgb(100, Color.Black), 1))
                {
                    g.DrawEllipse(eyePen, eyeRect);
                }
            }

            using (SolidBrush pupilBrush = new SolidBrush(Color.Black))
            {
                RectangleF pupilRect = new RectangleF(X + Width - 8, Y + 5, 5, 5);
                g.FillEllipse(pupilBrush, pupilRect);
            }

            using (SolidBrush shineBrush = new SolidBrush(Color.White))
            {
                RectangleF shineRect = new RectangleF(X + Width - 7, Y + 5.5f, 2, 2);
                g.FillEllipse(shineBrush, shineRect);
            }

            using (LinearGradientBrush beakBrush = new LinearGradientBrush(
                new RectangleF(X + Width, Y + Height / 2 - 3, 10, 6),
                Color.FromArgb(255, 255, 215, 0),
                Color.FromArgb(255, 255, 165, 0),
                LinearGradientMode.Horizontal))
            {
                PointF[] beakPoints = {
                    new PointF(X + Width, Y + Height / 2),
                    new PointF(X + Width + 10, Y + Height / 2 - 3),
                    new PointF(X + Width + 10, Y + Height / 2 + 3)
                };
                g.FillPolygon(beakBrush, beakPoints);
                using (Pen beakPen = new Pen(Color.FromArgb(200, 255, 140, 0), 1))
                {
                    g.DrawPolygon(beakPen, beakPoints);
                }
            }

            using (LinearGradientBrush tailBrush = new LinearGradientBrush(
                new RectangleF(X - 5, Y + Height / 2 - 2, 8, 4),
                wingStart,
                wingEnd,
                LinearGradientMode.Horizontal))
            {
                PointF[] tailPoints = {
                    new PointF(X - 5, Y + Height / 2),
                    new PointF(X - 8, Y + Height / 2 - 3),
                    new PointF(X - 8, Y + Height / 2 + 3)
                };
                g.FillPolygon(tailBrush, tailPoints);
            }

            using (SolidBrush footBrush = new SolidBrush(Color.FromArgb(255, 255, 69, 0)))
            {
                RectangleF leftFoot = new RectangleF(X + Width / 2 - 3, Y + Height - 2, 2, 4);
                RectangleF rightFoot = new RectangleF(X + Width / 2 + 1, Y + Height - 2, 2, 4);
                g.FillEllipse(footBrush, leftFoot);
                g.FillEllipse(footBrush, rightFoot);
            }

            g.Restore(state);

            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
            {
                RectangleF shadowRect = new RectangleF(X + 3, Y + Height + 1, Width - 6, Height / 4);
                g.FillEllipse(shadowBrush, shadowRect);
            }
        }
    }
}
