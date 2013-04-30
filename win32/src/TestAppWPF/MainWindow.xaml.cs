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

		public class PlayListViewData
		{
			Playlist _pl;
            Boolean _isStarred;
			public String Name
			{
				get
				{
                    if (_isStarred)
                    {
                        return "Starred";
                    }
                    else
                    {
                        return _pl.Name;
                    }
				}
			}

			public String PlaylistOwner
			{
				get
				{
                    return _pl.Owner.DisplayName;
				}
			}

			public Int32 NumberOfTracks
			{
				get
				{
					return _pl.Tracks.Count;
				}
			}

            public IList<Track> Tracks
            {
                get
                {
                    return _pl.Tracks;
                }
            }

			public PlayListViewData(Playlist pl)
			{
                _pl = pl;
                _isStarred = false;
			}
            public PlayListViewData(Playlist pl,bool starred)
            {
                _pl = pl;
                _isStarred = starred;
            }
		}

        public class TrackViewData
        {
            Track _track;            
            public String Name
            {
                get
                {
                    return _track.Name;
                }
            }

            public String MenuItemToggleStarText
            {
                get
                {
                    if (_track.IsStarred)
                    {
                        return "Unstar";
                    }
                    else
                    {
                        return "Star";
                    }
                }
            }
            public Track Track
            {
                get
                {
                    return _track;
                }
            }
            public Boolean IsEnabled
            {
                get
                {
                    return _track.IsAvailable;
                }
            }
            public Boolean IsStarred
            {
                get
                {
                    return _track.IsStarred;
                }
            }
            public String Artists
            {
                get
                {
                    int c = _track.Artists.Count();
                    if (c == 1)
                    {
                        return _track.Artists[0].Name;
                    }
                    else if (c > 1)
                    {
                        StringBuilder sb = new StringBuilder(_track.Artists[0].Name);
                        for (int i = 1; i < c; i++)
                        {
                            sb.Append(" & "+ _track.Artists[i].Name);
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            public TrackViewData(Track track)
            {
                _track = track;               
            }
        }
         
        private void SetupSession()
        {
            player = new NAudioPlayer(session);
            session.PrefferedBitrate = BitRate.Bitrate320k;
            player.Init();
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
            if (playlistsListView.SelectedItems.Count!=0)
            {
                tracksListView.Items.Clear();
                foreach (PlayListViewData item in playlistsListView.SelectedItems)
                {
                    List<Track> tracks = item.Tracks.ToList();

                    foreach (Track track in tracks)
                    {
                        tracksListView.Items.Add(new TrackViewData(await track));
                    }
                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (tracksListView.SelectedItem != null)
            {
                player.LoadTrack(((TrackViewData)tracksListView.SelectedItem).Track);
                player.Play();
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

        private void ListViewItemTrackDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TrackViewData track = ((ListViewItem)e.Source).DataContext as TrackViewData;
            player.LoadTrack(track.Track);
            player.Play();
        }

        private void trackMenuItemTStar_Click(object sender, RoutedEventArgs e)
        {
            TrackViewData trackviewdata = ((ListViewItem)e.Source).DataContext as TrackViewData;
            Track track = trackviewdata.Track;
            //TODO star/unstar
        }       
        
    }
}
