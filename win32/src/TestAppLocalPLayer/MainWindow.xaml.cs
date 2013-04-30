using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestAppLocalPLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PathWindow pathWindow = new PathWindow();
        public static string musicPath;
        Player player = new Player();

        public MainWindow()
        {
            InitializeComponent();
            pathWindow.PathSet += pathWindow_PathSet;
            player.WaveOutDevice.PlaybackStopped += player_WaveOutDevice_PlaybackStopped;
            this.Closed += MainWindow_Closed;
        }

        #region Button handlers
        private void setPathButton_Click(object sender, RoutedEventArgs e)
        {
            pathWindow.Show();
        }

        private void playpauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.PlaybackState == NAudio.Wave.PlaybackState.Paused)
            {
                player.Resume();
                playpauseButton.Content = "Pause";
                playbackstatusLabel.Content = "Playback resumed";
            }
            else if (player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                player.Pause();
                playpauseButton.Content = "Play";
                playbackstatusLabel.Content = "Playback paused";
            }
            else if (player.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
            {
                player.Play((string)musiclistListView.SelectedValue);
                playpauseButton.Content = "Pause";
                playbackstatusLabel.Content = "Playback started";
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            player.Reset();
            playbackstatusLabel.Content = "Playback stopped";
        }

        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            player.Reset();

            player.Play((string)musiclistListView.SelectedValue);
        }
        #endregion

        #region Event handlers
        private void pathWindow_PathSet(object sender, EventArgs e)
        {
            MusicList musicList = new MusicList();
            foreach (string filepath in musicList.FilePaths)
            {
                musiclistListView.Items.Add(filepath);
            }
            entriesfoundLabel.Content = musicList.FilePaths.Count() + " entries found";
        }

        private void player_WaveOutDevice_PlaybackStopped(object sender, EventArgs e)
        {
            if (player != null)
            {
                player.Reset();
            }
            playpauseButton.Content = "Play";
            playbackstatusLabel.Content = "Playback stopped";
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            pathWindow.Close();

            if (player != null)
            {
                player.Dispose();
            }
        }
        #endregion
    }
}
