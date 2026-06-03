using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace GLab6.Services
{
    public class AudioPlayerService
    {
        private MediaPlayer _player;
        private DispatcherTimer _timer;
        private string? _tempFilePath;

        public event Action<TimeSpan>? PositionChanged;

        public event Action? MediaEnded;

        public AudioPlayerService()
        {
            _player = new MediaPlayer();

            _player.MediaEnded += (s, e) => MediaEnded?.Invoke();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };

            _timer.Tick += (s, e) => PositionChanged?.Invoke(_player.Position);
        }

        public void LoadAndPlay(byte[] audioData)
        {
            Stop();
            CleanUpTempFile();

            _tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
            File.WriteAllBytes(_tempFilePath, audioData);

            _player.Open(new Uri(_tempFilePath));
            Play();
        }

        public void Play()
        {
            _player.Play();
            _timer.Start();
        }

        public void Pause()
        {
            _player.Pause();
            _timer.Stop();
        }

        public void Stop()
        {
            _player.Stop();
            _timer.Stop();
        }

        public void SetPosition(TimeSpan position) => _player.Position = position;
        public void SetVolume(double volume) => _player.Volume = volume;

        private void CleanUpTempFile()
        {
            if (!string.IsNullOrEmpty(_tempFilePath) && File.Exists(_tempFilePath))
            {
                try { File.Delete(_tempFilePath); } catch { }
            }
        }
    }
}