using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using iTunesLib;

namespace EMP
{
    public class iTunesCOM
    {
        iTunesApp iT = new iTunesApp();
        IITLibraryPlaylist LP;
        IITSourceCollection SC;
        IITPlaylistCollection PC;
        int currentTrack = 1;
        int currentSource = 1;
        int currentPlaylist = 1;

        public String PlayerState
        {
            get { return iT.PlayerState.ToString(); }
        }
        public Int32 TrackCount
        {
            get { return LP.Tracks.Count; }
        }
        public Int32 SourceCount
        {
            get { return SC.Count; }
        }
        public Int32 PlaylistCount
        {
            get { return PC.Count; }
        }
        public Int32 SourceProgress
        {
            get { return (Int32)Math.Round((double)currentSource / (double)SourceCount * 100); }
        }
        public Int32 TrackProgress
        {
            get { return (Int32)Math.Round((double)currentTrack / (double)TrackCount * 100); }
        }
        public Int32 PlaylistProgress
        {
            get { return (Int32)Math.Round((double)currentPlaylist / (double)PlaylistCount * 100); }
        }
        public Boolean EndOfTracks
        {
            get { return currentTrack > TrackCount; }
        }
        public Boolean EndOfSources
        {
            get { return currentSource > SourceCount; }
        }
        public Boolean EndOfPlaylists
        {
            get { return currentPlaylist > PlaylistCount; }
        }
        public iTunesCOM()
        {
            LP = iT.LibraryPlaylist;
            SC = iT.Sources;
            PC = iT.LibrarySource.Playlists;
        }
        public String GetNextTrack()
        {
            if (currentTrack > LP.Tracks.Count)
            {
                return "";
            }
            String value = LP.Tracks[currentTrack].Name + " - " + LP.Tracks[currentTrack].Artist + " (" + LP.Tracks[currentTrack].KindAsString + ")";
            currentTrack++;
            return value;
        }
        public String GetNextSource()
        {
            if (currentSource > SC.Count)
            {
                return "";
            }
            String value = currentSource + "/" + SC[currentSource].Index + " - " + SC[currentSource].Name + " (" + SC[currentSource].Kind.ToString() + ")";
            currentSource++;
            return value;
        }
        public String GetNextPlaylist()
        {
            if (currentPlaylist > PC.Count)
            {
                return "";
            }
            String value = currentPlaylist + "/" + PC[currentPlaylist].Index + " - " + PC[currentPlaylist].Name + "; " + PC[currentPlaylist].Tracks.Count + " items (" + PC[currentPlaylist].Kind.ToString() + ")";
            currentPlaylist++;
            return value;
        }
        public IITPlaylist GetMoviePlaylist()
        {
            return PC[3];
        }
        public String GetMoviePlaylistStr()
        {
            return PC[3].Index + " - " + PC[3].Name;
        }
    }
}
