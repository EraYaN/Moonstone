using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using iTunesLib;

namespace EMP
{
	public class iTunesCOM : IDisposable
	{
		iTunesApp iT;
		IITLibraryPlaylist LP;
		IITPlaylistCollection PC;
		public event _IiTunesEvents_OnQuittingEventEventHandler iTunesQuit;
		public event _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler iTunesAboutToPromptUser;
		public String PlayerState
		{
			get { return iT.PlayerState.ToString(); }
		}
		public IITTrackCollection Tracks
		{
			get { return LP.Tracks; }
		}
		public IITPlaylistCollection Playlists
		{
			get { return PC; }
		}
		public IITPlaylist MoviePlaylist
		{
			get { return PC[3]; }
		}
		public IITPlaylist TVShowPlaylist
		{
			get { return PC[4]; }
		}
		public iTunesCOM()
		{
			iT = new iTunesApp();
			foreach (IITWindow Window in iT.Windows)
			{
				Window.Minimized = true;
			}
			LP = iT.LibraryPlaylist;
			PC = iT.LibrarySource.Playlists;
			iT.OnQuittingEvent += new _IiTunesEvents_OnQuittingEventEventHandler(iT_OnQuittingEvent);
			iT.OnAboutToPromptUserToQuitEvent += new _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler(iT_OnAboutToPromptUserToQuitEvent);
		}
		public void Dispose()
		{
			LP = null;
			PC = null;
			iT.OnQuittingEvent -= iT_OnQuittingEvent;
			iT.OnAboutToPromptUserToQuitEvent -= iT_OnAboutToPromptUserToQuitEvent;
			iT = null;
		}
		protected virtual void OniTunesQuit()
		{
			_IiTunesEvents_OnQuittingEventEventHandler handler = iTunesQuit;
			if (handler != null)
			{
				handler();
			}
		}
		protected virtual void OniTunesAboutToPromptUser()
		{
			_IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler handler = iTunesAboutToPromptUser;
			if (handler != null)
			{
				handler();
			}
		}
		private void iT_OnQuittingEvent()
		{
			OniTunesQuit();
		}
		private void iT_OnAboutToPromptUserToQuitEvent()
		{
			OniTunesAboutToPromptUser();
		}
	}
}
