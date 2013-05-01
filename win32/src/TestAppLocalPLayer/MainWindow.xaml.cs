using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using System.Runtime.CompilerServices;

namespace TestAppLocalPLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        PathWindow pathWindow = new PathWindow();
        public static string musicPath;
        public static Player player = new Player();
        public static TrackList trackList;

        public MainWindow()
        {
            InitializeComponent();

			#region Event hooks
			pathWindow.PathSet += pathWindow_PathSet;
            player.WaveOutDevice.PlaybackStopped += player_WaveOutDevice_PlaybackStopped;
			player.PropertyChanged += player_PropertyChanged;
            this.Closed += MainWindow_Closed;
			#endregion
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
                if (tracklistListView.SelectedValue != null)
                {
                    player.Play((string)tracklistListView.SelectedValue);
                    playpauseButton.Content = "Pause";
                    playbackstatusLabel.Content = "Playback started";
                }
                else
                {
                    return;
                }
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
            player.Stop();
            playbackstatusLabel.Content = "Playback stopped";
        }

        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            player.Reset();
            player.Play((string)tracklistListView.SelectedValue);
            playpauseButton.Content = "Pause";
            playbackstatusLabel.Content = "Playback started";
        }
        #endregion

        #region Other event handlers
        private void pathWindow_PathSet(object sender, EventArgs e)
        {
			update_trackList();
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

		private void player_PropertyChanged(object sender, EventArgs e)
		{
			MessageBox.Show("PlaybackState changed to" + player.PlaybackState);
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

		#region UI updaters
		private void update_trackList()
		{
			tracklistListView.Items.Clear();
			if (trackList != null)
			{
				trackList.Dispose();
			}

			trackList = new TrackList();

			foreach (string filepath in trackList.FilePaths)
			{
				tracklistListView.Items.Add(filepath);
			}
			entriesfoundLabel.Content = trackList.FilePaths.Count() + " entries found";
		}

		private void update_playpauseButton()
		{

		}
		#endregion
	}
}
