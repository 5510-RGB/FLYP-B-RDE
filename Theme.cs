using System.Drawing;

namespace FlappyBird
{
    public enum Theme
    {
        Day,
        Night,
        Neon
    }

    public enum BirdSkin
    {
        Classic,
        Blue,
        Red,
        Neon
    }

    public struct ThemePalette
    {
        public Color Background;
        public Color PipeDark;
        public Color PipeLight;
        public Color PipeBorder;
        public Color TextColor;
    }

    public static class ThemeManager
    {
        public static ThemePalette GetPalette(Theme theme)
        {
            switch (theme)
            {
                case Theme.Night:
                    return new ThemePalette
                    {
                        Background = Color.FromArgb(255, 15, 20, 35),
                        PipeDark = Color.FromArgb(255, 30, 60, 80),
                        PipeLight = Color.FromArgb(255, 40, 90, 120),
                        PipeBorder = Color.FromArgb(255, 10, 30, 45),
                        TextColor = Color.White
                    };
                case Theme.Neon:
                    return new ThemePalette
                    {
                        Background = Color.FromArgb(255, 10, 10, 10),
                        PipeDark = Color.FromArgb(255, 0, 255, 170),
                        PipeLight = Color.FromArgb(255, 0, 200, 255),
                        PipeBorder = Color.FromArgb(255, 0, 140, 180),
                        TextColor = Color.FromArgb(255, 255, 255, 255)
                    };
                case Theme.Day:
                default:
                    return new ThemePalette
                    {
                        Background = Color.SkyBlue,
                        PipeDark = Color.FromArgb(255, 46, 125, 50),
                        PipeLight = Color.FromArgb(255, 34, 139, 34),
                        PipeBorder = Color.FromArgb(255, 0, 100, 0),
                        TextColor = Color.White
                    };
            }
        }
    }
}


