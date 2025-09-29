using System;
using System.Media;
using System.IO;
using System.Reflection;

namespace FlappyBird
{
    public class SoundManager
    {
        private SoundPlayer jumpSound;
        private SoundPlayer scoreSound;
        private SoundPlayer gameOverSound;
        private SoundPlayer backgroundMusic;
        
        private bool soundEnabled = true;
        private bool musicEnabled = true;

        public SoundManager()
        {
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            try
            {
                // Basit ses efektleri oluştur (gerçek ses dosyaları yerine)
                jumpSound = CreateBeepSound(800, 100); // Yüksek ton - zıplama
                scoreSound = CreateBeepSound(1200, 150); // Daha yüksek ton - puan
                gameOverSound = CreateBeepSound(400, 300); // Düşük ton - oyun bitti
                backgroundMusic = CreateBackgroundMusic();
            }
            catch (Exception ex)
            {
                // Ses yüklenemezse sessiz moda geç
                soundEnabled = false;
                musicEnabled = false;
            }
        }

        private SoundPlayer CreateBeepSound(int frequency, int duration)
        {
            // Basit beep sesi oluştur
            return new SoundPlayer();
        }

        private SoundPlayer CreateBackgroundMusic()
        {
            // Arka plan müziği için basit ses
            return new SoundPlayer();
        }

        public void PlayJumpSound()
        {
            if (soundEnabled && jumpSound != null)
            {
                try
                {
                    // Console.Beep ile basit ses efekti
                    Console.Beep(800, 100);
                }
                catch
                {
                    // Ses çalınamazsa sessizce devam et
                }
            }
        }

        public void PlayScoreSound()
        {
            if (soundEnabled && scoreSound != null)
            {
                try
                {
                    Console.Beep(1200, 150);
                }
                catch
                {
                    // Ses çalınamazsa sessizce devam et
                }
            }
        }

        public void PlayGameOverSound()
        {
            if (soundEnabled && gameOverSound != null)
            {
                try
                {
                    // Oyun bitti sesi - düşük ton
                    Console.Beep(400, 200);
                    System.Threading.Thread.Sleep(50);
                    Console.Beep(300, 200);
                }
                catch
                {
                    // Ses çalınamazsa sessizce devam et
                }
            }
        }

        public void PlayBackgroundMusic()
        {
            if (musicEnabled && backgroundMusic != null)
            {
                try
                {
                    // Arka plan müziği için basit melodi
                    PlayMelody();
                }
                catch
                {
                    // Müzik çalınamazsa sessizce devam et
                }
            }
        }

        private void PlayMelody()
        {
            // Basit melodi
            int[] notes = { 523, 587, 659, 698, 784, 880, 988 }; // C major scale
            int[] durations = { 200, 200, 200, 200, 200, 200, 400 };
            
            for (int i = 0; i < notes.Length; i++)
            {
                try
                {
                    Console.Beep(notes[i], durations[i]);
                }
                catch
                {
                    break;
                }
            }
        }

        public void ToggleSound()
        {
            soundEnabled = !soundEnabled;
        }

        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
        }

        public bool IsSoundEnabled => soundEnabled;
        public bool IsMusicEnabled => musicEnabled;
    }
}
