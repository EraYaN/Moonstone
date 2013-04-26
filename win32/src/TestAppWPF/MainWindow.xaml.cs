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
using TestApp.Spotify;
using libspotifydotnet;
using NAudio.Wave;
using System.Threading;
namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		LoginWindow loginWindow = new LoginWindow();
        NAudioPlayer player = new NAudioPlayer();
        public MainWindow()
        {
            InitializeComponent();
            Spotify.Initialize();
            Application.Current.Exit += Current_Exit;
			loginWindow.LoggedIn += loginWindow_LoggedIn;                 
        }

		public class PlayListViewData
		{
			PlaylistContainer.PlaylistInfo _info;
			Playlist _pl;
			public String Name
			{
				get
				{
					return _info.Name;
				}
			}

			public String PlaylistType
			{
				get
				{
					return _info.PlaylistType.ToString();
				}
			}

			public Int32 NumberOfTracks
			{
				get
				{
					return _pl.TrackCount;
				}
			}

            public List<Track> Tracks
            {
                get
                {
                    return _pl.GetTracks();
                }
            }

			public PlayListViewData(PlaylistContainer.PlaylistInfo info)
			{
				_info = info;
				_pl = Playlist.Get(_info.Pointer);
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

            public IntPtr TrackPtr
            {
                get
                {
                    return _track.TrackPtr;
                }
            }

            public String Artists
            {
                get
                {
                    int c = _track.Artists.Count();
                    if (c == 1)
                    {
                        return _track.Artists[0];
                    }
                    else if (c > 1)
                    {
                        StringBuilder sb = new StringBuilder(_track.Artists[0]);
                        for (int i = 1; i < c; i++)
                        {
                            sb.Append(" & "+ _track.Artists[i]);
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

		void loginWindow_LoggedIn(object sender, EventArgs e)
		{
			playlistsListView.Items.Clear();
			IntPtr sessionPtr = Session.GetSessionPtr();
			IntPtr userPtr = Session.GetUserPtr();
			PlaylistContainer playlistContainer = Spotify.GetUserPlaylists(userPtr);
			List<PlaylistContainer.PlaylistInfo> infos = playlistContainer.GetAllPlaylists();
			foreach (PlaylistContainer.PlaylistInfo info in infos)
			{
				playlistsListView.Items.Add(new PlayListViewData(info));
			}  
		}
        void Current_Exit(object sender, ExitEventArgs e)
        {
			if (Spotify.IsRunning)
			{
				Spotify.ShutDown();
			}
			
        }

		private void mainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			loginWindow.Show();
		}

		private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            player.Dispose();
            loginWindow.Close();
		}

		private void refreshButton_Click(object sender, RoutedEventArgs e)
		{
			playlistsListView.Items.Clear();
			IntPtr sessionPtr = Session.GetSessionPtr();
			IntPtr userPtr = Session.GetUserPtr();
			User u = new User(userPtr);
			PlaylistContainer playlistContainer = Spotify.GetUserPlaylists(userPtr);
			if(playlistContainer.PlaylistsAreLoaded){
				List<PlaylistContainer.PlaylistInfo> infos = playlistContainer.GetAllPlaylists();
				foreach (PlaylistContainer.PlaylistInfo info in infos)
				{
					playlistsListView.Items.Add(new PlayListViewData(info));
				}
			}
			
		}

        private void playlistsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistsListView.SelectedItems.Count!=0)
            {
                tracksListView.Items.Clear();
                foreach (PlayListViewData item in playlistsListView.SelectedItems)
                {
                    List<Track> tracks = item.Tracks;

                    foreach (Track track in tracks)
                    {
                        tracksListView.Items.Add(new TrackViewData(track));
                    }
                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            player.Init();
            player.LoadTrack(((TrackViewData)tracksListView.SelectedItem).TrackPtr);
            player.Play();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((String)pauseButton.Content == "Pause")
            {
                player.Pause();
                pauseButton.Content = "Resume";
            }
            else if ((String)pauseButton.Content == "Resume")
            {
                player.Play();
                pauseButton.Content = "Pause";
            }
        }	
        
    }
}
