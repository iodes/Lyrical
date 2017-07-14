using Lyrical.Controls.Base;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Lyrical.Sample
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        class LyricsProvider : ILyricsProvider
        {
            private MediaPlayer player;

            public LyricsProvider(MediaPlayer target)
            {
                player = target;
            }

            public TimeSpan GetPosition()
            {
                return player.Position;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "MP3 (*.mp3)|*.mp3|모든 파일 (*.*)|*.*"
            };

            if (openDialog.ShowDialog().Value)
            {
                var player = new MediaPlayer();
                var provider = new LyricsProvider(player);
                var document = new LyricsDocument($@"{Path.GetDirectoryName(openDialog.FileName)}\{Path.GetFileNameWithoutExtension(openDialog.FileName)}.slr");

                player.Open(new Uri(openDialog.FileName));
                lyricsView.Provider = provider;
                lyricsView.Document = document;

                player.Play();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
