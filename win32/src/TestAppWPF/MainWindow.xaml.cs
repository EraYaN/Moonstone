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
namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		LoginWindow loginWindow = new LoginWindow();
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

			public PlayListViewData(PlaylistContainer.PlaylistInfo info)
			{
				_info = info;
				_pl = Playlist.Get(_info.Pointer);
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
        
    }
}
