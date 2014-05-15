using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using SpotiFire;
using NAudio.Wave;
using System.Threading;
using System.Threading.Tasks;
namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NAudioPlayer player;
        public Session session;
        private PlaylistContainer pc;
        public MainWindow()
        {
            InitializeComponent();	                
        }		
         
        private void SetupSession()
        {
            player = new NAudioPlayer(session);
            session.PrefferedBitrate = BitRate.Bitrate320k;
            player.Init();
            noPlayingLabel.DataContext = player;
        }        
        private async void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            session = await Spotify.Task;
            SetupSession();
            Error err = await session.Relogin();
            if (err == Error.OK && (session.ConnectionState == ConnectionState.LoggedIn || session.ConnectionState == ConnectionState.Offline))
            {
                updatePlaylists();
                loginButtonSpotify.IsEnabled = false;
            }
        } 
       
        private void loginButtonSpotify_Click(object sender, RoutedEventArgs e)
        {
            if (session.ConnectionState != ConnectionState.LoggedIn && session.ConnectionState != ConnectionState.Offline)
            {
                while (true)
                {
                    LoginDialog login = new LoginDialog(session);
                    try
                    {
                        bool? result = login.ShowDialog();
                        if (result.HasValue && result.Value)
                            break;
                        else if (result.HasValue)
                        {
                            Application.Current.Shutdown();
                            return;
                        }
                    }
                    catch (Exception err)
                    {
                        //
                    }
                }
                updatePlaylists();
            }
        }
        private async void updatePlaylists()
        {
            pc = await session.PlaylistContainer;
            playlistsListView.Items.Clear();
            playlistsListView.Items.Add(new PlayListViewData(await session.Starred,true));
            foreach (Playlist playlist in pc.Playlists)
            {
                playlistsListView.Items.Add(new PlayListViewData(await playlist));
            }
        }
		private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            player.Dispose();            
		}
        private async void playlistsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (playlistsListView.SelectedItems.Count!=0)
            {
                tracksListView.Items.Clear();
                foreach (PlayListViewData item in playlistsListView.SelectedItems)
                {
                    List<Track> tracks = item.Tracks.ToList();

                    foreach (Track track in tracks)
                    {
                        tracksListView.Items.Add(new TrackViewData(await track));
                    }
                *}
            }*/
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlistsListView.SelectedItems.Count>0)
            {
                List<Track> tracks = new List<Track>();
                foreach(PlayListViewData pl in playlistsListView.SelectedItems){
                    tracks.AddRange(pl.Tracks);
                }
                player.SetSeed(tracks);
                if (tracksListView.SelectedItem == null)
                    player.NextTrack();
            }

            if (tracksListView.SelectedItem != null)
            {
                player.Enqueue(((TrackViewData)tracksListView.SelectedItem).Track);
                player.NextTrack();
            }            
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.IsPlaying)
            {
                player.Pause();
            }
            else
            {
                player.Play();
            }
        }

        private async void ListViewItemTrackDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TrackViewData track = ((ListViewItem)e.Source).DataContext as TrackViewData;
            player.pq.Enqueue(await track.Track);
            
            if (playlistsListView.SelectedItems.Count > 0)
            {
                List<Track> tracks = new List<Track>();
                foreach (PlayListViewData pl in playlistsListView.SelectedItems)
                {
                    tracks.AddRange(pl.Tracks);
                }
                player.SetSeed(tracks);                
            }
            player.NextTrack();           
        }

        private void trackMenuItemTStar_Click(object sender, RoutedEventArgs e)
        {
            TrackViewData trackviewdata = ((ListViewItem)e.Source).DataContext as TrackViewData;
            Track track = trackviewdata.Track;
            //TODO star/unstar
        }

        private void queueListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            queueListView.Items.Clear();
            Track[] snapshot = new Track[player.pq.Count];
            player.pq.CopyTo(snapshot, 0);
            foreach (Track track in snapshot)
            {
                queueListView.Items.Add(track);
            }
            
        }

        private void nextTrackButton_Click(object sender, RoutedEventArgs e)
        {
            player.NextTrack();
        }       
        
    }
}
