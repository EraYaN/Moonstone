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
        public static TrackList trackList = new TrackList();

        public MainWindow()
        {
            InitializeComponent();

			#region UI bindings
			playbackstatusLabel.DataContext = player;
			playpauseButton.DataContext = player;
			entriesfoundLabel.DataContext = trackList;
			tracklistListView.ItemsSource = trackList.FilePaths;
			#endregion

			#region Event hooks
			pathWindow.PathSet += pathWindow_PathSet;
            player.WaveOutDevice.PlaybackStopped += player_WaveOutDevice_PlaybackStopped;
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
            }
            else if (player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                player.Pause();
            }
            else if (player.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
            {
                if (tracklistListView.SelectedValue != null)
                {
                    player.Play((string)tracklistListView.SelectedValue);
                }
                else
                {
                    return;
                }
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
			if (tracklistListView.SelectedValue == null)
			{
				tracklistListView.SelectedIndex = 0;
			}
			else if (tracklistListView.SelectedIndex < (trackList.FilePaths.Count() - 1))
			{
				tracklistListView.SelectedIndex++;
			}
			else 
			{
				return;
			}
			tracklistListView.ScrollIntoView(tracklistListView.SelectedItem);
			player.Play((string)tracklistListView.SelectedValue);
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
			if (tracklistListView.SelectedValue == null)
			{
				tracklistListView.SelectedIndex = (trackList.FilePaths.Count() - 1);
			}
			else if (tracklistListView.SelectedIndex > 0)
			{
				tracklistListView.SelectedIndex--;
			}
			else
			{
				return;
			}
			tracklistListView.ScrollIntoView(tracklistListView.SelectedItem);
			player.Play((string)tracklistListView.SelectedValue);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            player.Reset();
            player.Play((string)tracklistListView.SelectedValue);
        }
        #endregion

        #region Other event handlers
        private void pathWindow_PathSet(object sender, EventArgs e)
        {
			trackList.refreshTrackList();
        }

        private void player_WaveOutDevice_PlaybackStopped(object sender, EventArgs e)
        {
            if (player != null)
            {
                player.Reset();
            }
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
