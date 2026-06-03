using GLab6.Commands;
using GLab6.Models;
using GLab6.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GLab6.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private readonly AudioPlayerService _audioService;
        private List<Song> _queue = new List<Song>();
        private int _currentIndex = -1;

        private Song? _currentSong;
        private bool _isPlaying;
        private TimeSpan _position;
        private TimeSpan _duration;
        private double _volume = 0.5;

        public Song? CurrentSong { get => _currentSong; set => SetProperty(ref _currentSong, value); }
        public bool IsPlaying { get => _isPlaying; set => SetProperty(ref _isPlaying, value); }
        public double Volume
        {
            get => _volume;
            set { if (SetProperty(ref _volume, value)) _audioService.SetVolume(value); }
        }

        public TimeSpan Duration { get => _duration; set { if (SetProperty(ref _duration, value)) OnPropertyChanged(nameof(DurationSeconds)); } }
        public TimeSpan Position { get => _position; set { if (SetProperty(ref _position, value)) { OnPropertyChanged(nameof(PositionSeconds)); OnPropertyChanged(nameof(RemainingTime)); } } }
        public TimeSpan RemainingTime => Duration - Position;

        public double DurationSeconds => Duration.TotalSeconds;
        public double PositionSeconds
        {
            get => Position.TotalSeconds;
            set
            {
                if (Math.Abs(Position.TotalSeconds - value) > 1.0) 
                SeekCommand.Execute(value);
            }
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand SeekCommand { get; }

        public PlayerViewModel(AudioPlayerService audioService)
        {
            _audioService = audioService;
            _audioService.PositionChanged += p => Position = p;
            _audioService.MediaEnded += PlayNext;
            _audioService.SetVolume(Volume);

            PlayPauseCommand = new RelayCommand(TogglePlayPause, () => CurrentSong != null);
            PreviousCommand = new RelayCommand(PlayPrevious, () => _currentIndex > 0);
            NextCommand = new RelayCommand(PlayNext, () => _currentIndex >= 0 && _currentIndex < _queue.Count - 1);

            SeekCommand = new RelayCommand(param =>
            {
                if (param is double seconds)
                {
                    _audioService.SetPosition(TimeSpan.FromSeconds(seconds));
                    Position = TimeSpan.FromSeconds(seconds);
                }
            });
        }

        public void PlayQueue(List<Song> queue, int startIndex)
        {
            _queue = queue;
            _currentIndex = startIndex;
            PlayCurrentIndex();
        }

        private void PlayCurrentIndex()
        {
            if (_currentIndex >= 0 && _currentIndex < _queue.Count)
            {
                CurrentSong = _queue[_currentIndex];
                Duration = CurrentSong.Duration;
                Position = TimeSpan.Zero;

                if (CurrentSong.AudioData != null)
                {
                    _audioService.LoadAndPlay(CurrentSong.AudioData);
                    IsPlaying = true;
                }
                UpdateCommands();
            }
        }

        private void TogglePlayPause()
        {
            if (IsPlaying) { 
                _audioService.Pause(); 
                IsPlaying = false; 
            }
            else { 
                _audioService.Play(); 
                IsPlaying = true; 
            }
        }

        private void PlayPrevious() { 
            if (_currentIndex > 0) 
            { 
                _currentIndex--; 
                PlayCurrentIndex(); 
            } 
        }
        private void PlayNext() { 
            if (_currentIndex < _queue.Count - 1) 
            { 
                _currentIndex++; 
                PlayCurrentIndex(); 
            } 
        }

        private void UpdateCommands()
        {
            ((RelayCommand)PlayPauseCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
        }
    }
}