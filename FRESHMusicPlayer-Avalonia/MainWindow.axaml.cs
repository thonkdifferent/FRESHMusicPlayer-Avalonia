using ATL;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FRESHMusicPlayer;
using System;
using System.IO;
using System.Timers;

namespace FRESHMusicPlayer_Avalonia
{
    public partial class MainWindow : Window
    {
        public Player Player = new Player();
        public Track? CurrentTrack;

        private Timer progressTimer = new Timer(1000);

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Player_SongException(object? sender, FRESHMusicPlayer.Handlers.PlaybackExceptionEventArgs e)
        {
            // ignored for now
        }

        private void Player_SongStopped(object? sender, System.EventArgs e)
        {
            Title = "FRESHMusicPlayer";
            TrackTitleTextBlock.Text = "Nothing Playing";
            TrackArtistTextBlock.Text = "Nothing Playing";
            ProgressIndicator1.Text = "--:--";
            ProgressIndicator2.Text = "--:--";
            progressTimer.Stop();
        }

        private void Player_SongChanged(object? sender, System.EventArgs e)
        {
            CurrentTrack = new Track(Player.FilePath);
            Title = $"{CurrentTrack.Artist} - {CurrentTrack.Title} | FRESHMusicPlayer";
            TrackTitleTextBlock.Text = string.IsNullOrWhiteSpace(CurrentTrack.Title) ? "Unknown Title" : CurrentTrack.Title;
            TrackArtistTextBlock.Text = string.IsNullOrWhiteSpace(CurrentTrack.Artist) ? "Unknown Artist" : CurrentTrack.Artist;
            CoverArtImage.Source = new Bitmap(new MemoryStream(CurrentTrack.EmbeddedPictures[0].PictureData));
            ProgressIndicator2.Text = Player.CurrentBackend.TotalTime.ToString(@"mm\:ss");
            ProgressSlider.Maximum = Player.CurrentBackend.TotalTime.TotalSeconds;
            progressTimer.Start();
        }
        private void ProgressTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var time = TimeSpan.FromSeconds(Math.Floor(Player.CurrentBackend.CurrentTime.TotalSeconds));
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ProgressIndicator1.Text = time.ToString(@"mm\:ss");
                ProgressSlider.Value = time.TotalSeconds;
            });
        }
        public void PlayPauseMethod()
        {
            if (!Player.Playing) return;
            if (Player.Paused)
            {
                Player.ResumeMusic();
                progressTimer.Start();
            }
            else
            {
                Player.PauseMusic();
                progressTimer.Stop();
            }
        }
        public void StopMethod() => Player.StopMusic();
        public void NextTrackMethod() => Player.NextSong();
        public void PreviousTrackMethod() => Player.PreviousSong();
        public void UpdatePlayButtonState()
        {
            if (!Player.Paused) PlayPauseButton.Content = "Pause";
            else PlayPauseButton.Content = "Resume";
        }
        private void NextTrackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => NextTrackMethod();

        private void RepeatOneButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Player.RepeatOnce = RepeatOneButton.IsPressed;

        private void PlayPauseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => PlayPauseMethod();

        private void ShuffleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Player.Shuffle = ShuffleButton.IsPressed;

        private void PreviousTrackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => PreviousTrackMethod();

        private void Import_PlaySongButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Player.AddQueue(Import_PathTextBox.Text);
            Player.CurrentVolume = 0.5f;
            Player.PlayMusic();
        }
    }
}