using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Dictation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool m_nowPlaying = false;
        private String m_currentFileDurationInString;

        public MainPage()
        {
            this.InitializeComponent();
            var curCoreWindow = CoreWindow.GetForCurrentThread();

            curCoreWindow.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            var pressedKey = args.VirtualKey;

            if (pressedKey == Windows.System.VirtualKey.Left)
            {
                _backward();
            }
            else if (pressedKey == Windows.System.VirtualKey.Right)
            {
                _forward();
            }
            else if (pressedKey == Windows.System.VirtualKey.Space)
            {
                _pause();
            }
        }

        public async System.Threading.Tasks.Task SetLocalMedia()
        {
            var openFilePicker = new FileOpenPicker();

            openFilePicker.FileTypeFilter.Add(".mp3");
            openFilePicker.FileTypeFilter.Add(".wma");

            var file = await openFilePicker.PickSingleFileAsync();

            if (file != null)
            {
                statusText.Text = "재생전!";
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                mediaPlayer.MediaPlayer.PlaybackSession.BufferingEnded += _onBufferingEnded;
                mediaPlayer.MediaPlayer.PlaybackSession.PositionChanged += _onPositionChanged;
                var fileDuration = mediaPlayer.MediaPlayer.PlaybackSession.NaturalDuration;
                m_currentFileDurationInString = fileDuration.ToString();
                mediaPlayer.MediaPlayer.Play();
                statusText.Text = "재생중..";
                m_nowPlaying = true;

                songName.Text = file.Name;
            }
        }

        private void _onPositionChanged(Windows.Media.Playback.MediaPlaybackSession sender, object args)
        {
        }


        private void _onBufferingEnded(Windows.Media.Playback.MediaPlaybackSession sender, object args)
        {
            statusText.Text = "재생중!";
        }

        private async void pickButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_nowPlaying == false)
            {
                m_nowPlaying = true;
            }
            else
            {
                // 지금 재생하고 있는 거 일단 중지
                mediaPlayer.MediaPlayer.Pause();
                m_nowPlaying = false;
                statusText.Text = "새로운파일로!";
            }
            await SetLocalMedia();
        }

        private void backwardButton_Click(object sender, RoutedEventArgs e)
        {
            _backward();
        }

        private void _backward()
        {
            if (m_nowPlaying)
            {
                var curPos = mediaPlayer.MediaPlayer.PlaybackSession.Position;
                TimeSpan ts = mediaPlayer.MediaPlayer.PlaybackSession.Position.Subtract(new TimeSpan(0, 0, 3));

                mediaPlayer.MediaPlayer.PlaybackSession.Position = ts;
            }
        }

        private void pausePlayButton_Click(object sender, RoutedEventArgs e)
        {
            _pause();
        }

        private void _pause()
        {
            if (m_nowPlaying)
            {
                mediaPlayer.MediaPlayer.Pause();
                pausePlayButton.Content = "PLAY";
                m_nowPlaying = false;
            }
            else
            {
                mediaPlayer.MediaPlayer.Play();
                pausePlayButton.Content = "PAUSE";
                m_nowPlaying = true;
            }
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            _forward();
        }

        private void _forward()
        {
            if (m_nowPlaying)
            {
                var curPos = mediaPlayer.MediaPlayer.PlaybackSession.Position;
                TimeSpan ts = mediaPlayer.MediaPlayer.PlaybackSession.Position.Add(new TimeSpan(0, 0, 3));

                mediaPlayer.MediaPlayer.PlaybackSession.Position = ts;
            }
        }
    }
}
