using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppLocalPLayer
{
	class Track<Provider>
	{		
		#region Properties
		public enum Sources { local, sportify, soundcloud };
		public enum Channels { mono, stereo };
		public enum EqualizerPresets { };

		//Track info
		public int ID { get; set; }
		public int Source { get; set; }
		public string Title { get; set; }
		public string Artist { get; set; }
		public int Year { get; set; }
		public string Album { get; set; }
		public string AlbumArtist { get; set; }
		public int AlbumTrackNo { get; set; }
		public int AlbumNumTracks { get; set; }
		public int AlbumDiskNo { get; set; }
		public int AlbumNumDisks { get; set; }
		public string AlbumArtist { get; set; }
		public string Remarks { get; set; }
		public string Group { get; set; }
		public string Composer { get; set; }
		public string Genre { get; set; }
		public int Length { get; set; } //seconds

		//File info
		public string URI { get; set; }
		public string Codec { get; set; }
		public string Encoder { get; set; }
		public int Channel { get; set; }
		public int Bitrate { get; set; } //kbit/s
		public long Filesize { get; set; } //bytes
		public int SampleRate { get; set; } //Hz
		public Version TagVersion { get; set; } //ID3-tag version
		public DateTime AddedOn { get; set; }
		public DateTime LastPlayedOn { get; set; }
		public int TimesPlayed { get; set; }

		//Settings
		public int VolumeAdjust { get; set; } // -100 to 100
		public int EqualizerPreset { get; set; }
		public int Rating { get; set; } //0 (for none) to 5
		public int StartTime { get; set; } //seconds
		public int EndTime { get; set; } //seconds
		public bool RememberPlayPos { get; set; }
		public int RememberedPlayPos { get; set; }
		public bool SkipWhenShuffle { get; set; }
		#endregion

		public void Track(string _source, string _uri)
		{

		}

		public interface IProvider
		{

		}
	}
}
