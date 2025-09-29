using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyBird
{
    public class Particle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Life { get; set; }
        public float MaxLife { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public Particle(float x, float y, float velocityX, float velocityY, Color color, float size, float life)
        {
            X = x;
            Y = y;
            VelocityX = velocityX;
            VelocityY = velocityY;
            Color = color;
            Size = size;
            Life = life;
            MaxLife = life;
            Rotation = 0f;
            RotationSpeed = (float)(new Random().NextDouble() * 10 - 5);
        }

        public void Update()
        {
            X += VelocityX;
            Y += VelocityY;
            VelocityY += 0.2f; // Yerçekimi
            Life -= 1f;
            Rotation += RotationSpeed;
        }

        public bool IsAlive => Life > 0;

        public void Draw(Graphics g)
        {
            if (!IsAlive) return;

            float alpha = Life / MaxLife;
            Color currentColor = Color.FromArgb((int)(alpha * 255), Color);
            
            using (SolidBrush brush = new SolidBrush(currentColor))
            {
                GraphicsState state = g.Save();
                g.TranslateTransform(X, Y);
                g.RotateTransform(Rotation);
                
                RectangleF rect = new RectangleF(-Size/2, -Size/2, Size, Size);
                g.FillEllipse(brush, rect);
                
                g.Restore(state);
            }
        }
    }
}
