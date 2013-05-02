using SpotiFire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWPF
{
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
        public PlayListViewData(Playlist pl, bool starred)
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
                        sb.Append(" & " + _track.Artists[i].Name);
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
}

