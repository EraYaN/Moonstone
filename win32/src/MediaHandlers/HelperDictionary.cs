using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EMP
{
	public class HelperDictionary
	{
		private Dictionary<String, Container> ContainerDictionary = new Dictionary<String, Container>();
		private Dictionary<String, VideoSource> VideoSourceDictionary = new Dictionary<String, VideoSource>();
		private Dictionary<String, VideoQuality> VideoQualityDictionary = new Dictionary<String, VideoQuality>();
		private Dictionary<String, VideoCodec> VideoCodecDictionary = new Dictionary<String, VideoCodec>();
		private Dictionary<String, AudioCodec> AudioCodecDictionary = new Dictionary<String, AudioCodec>();
		private Dictionary<String, Cut> CutDictionary = new Dictionary<String, Cut>();

		public List<String> ContainerStrings
		{
			get { return ContainerDictionary.Keys.ToList<String>();}
		}

		public List<String> VideoSourceStrings
		{
			get { return VideoSourceDictionary.Keys.ToList<String>(); }
		}

		public List<String> VideoQualityStrings
		{
			get { return VideoQualityDictionary.Keys.ToList<String>(); }
		}

		public List<String> VideoCodecStrings
		{
			get { return VideoCodecDictionary.Keys.ToList<String>(); }
		}

		public List<String> AudioCodecStrings
		{
			get { return AudioCodecDictionary.Keys.ToList<String>(); }
		}

		public List<String> CutStrings
		{
			get { return CutDictionary.Keys.ToList<String>(); }
		}
		
		public HelperDictionary()
		{
			#region ContainerDictionary
			//Containers (FileExt->Contianer)
			ContainerDictionary.Add(".mkv", Container.Matroska);
			ContainerDictionary.Add(".mk3d", Container.Matroska);
			ContainerDictionary.Add(".mka", Container.Matroska);
			ContainerDictionary.Add(".mks", Container.Matroska);

			ContainerDictionary.Add(".avi", Container.AVI);

			ContainerDictionary.Add(".mov", Container.QuickTime);

			ContainerDictionary.Add(".mp4", Container.MPEG4);
			ContainerDictionary.Add(".m4a", Container.MPEG4);
			ContainerDictionary.Add(".m4v", Container.MPEG4);

			ContainerDictionary.Add(".flv", Container.Flash);
			ContainerDictionary.Add(".f4p", Container.Flash);
			ContainerDictionary.Add(".f4v", Container.Flash);
			ContainerDictionary.Add(".swf", Container.Flash);

			ContainerDictionary.Add(".iff", Container.IFF);

			ContainerDictionary.Add(".mpg", Container.MPEGPS);
			ContainerDictionary.Add(".mpeg", Container.MPEGPS);
			ContainerDictionary.Add(".ps", Container.MPEGPS);

			ContainerDictionary.Add(".mts", Container.MPEGTS);
			ContainerDictionary.Add(".ts", Container.MPEGTS);

			ContainerDictionary.Add(".ogv", Container.Ogg);
			ContainerDictionary.Add(".oga", Container.Ogg);
			ContainerDictionary.Add(".ogx", Container.Ogg);
			ContainerDictionary.Add(".ogg", Container.Ogg);
			ContainerDictionary.Add(".spx", Container.Ogg);

			ContainerDictionary.Add(".webm", Container.WebM);

			ContainerDictionary.Add(".rm", Container.RM);
			#endregion
			#region VideoSourceDictionary
			//VideoSources (Matching Str->Contianer)			

			VideoSourceDictionary.Add("cam", VideoSource.Cam);
			VideoSourceDictionary.Add("camrip", VideoSource.Cam);

			VideoSourceDictionary.Add("ts", VideoSource.Telesync);
			VideoSourceDictionary.Add("telesync", VideoSource.Telesync);
			VideoSourceDictionary.Add("pdvd", VideoSource.Telesync);

			VideoSourceDictionary.Add("workprint", VideoSource.Workprint);
			VideoSourceDictionary.Add("wp", VideoSource.Workprint);

			VideoSourceDictionary.Add("telecine", VideoSource.Telecine);
			//VideoSourceDictionary.Add("tc", VideoSource.Telecine);//gave problems

			VideoSourceDictionary.Add("ppv", VideoSource.PayPerView);
			VideoSourceDictionary.Add("ppvrip", VideoSource.PayPerView);

			VideoSourceDictionary.Add("scr", VideoSource.Screener);
			VideoSourceDictionary.Add("screener", VideoSource.Screener);
			VideoSourceDictionary.Add("dvdscreener", VideoSource.Screener);
			VideoSourceDictionary.Add("dvdscr", VideoSource.Screener);
			VideoSourceDictionary.Add("bdscr", VideoSource.Screener);

			VideoSourceDictionary.Add("ddc", VideoSource.DDC);

			VideoSourceDictionary.Add("r0", VideoSource.RX);
			VideoSourceDictionary.Add("r1", VideoSource.RX);
			VideoSourceDictionary.Add("r2", VideoSource.RX);
			VideoSourceDictionary.Add("r3", VideoSource.RX);
			VideoSourceDictionary.Add("r4", VideoSource.RX);
			VideoSourceDictionary.Add("r5", VideoSource.RX);
			VideoSourceDictionary.Add("r6", VideoSource.RX);
			VideoSourceDictionary.Add("r7", VideoSource.RX);
			VideoSourceDictionary.Add("r8", VideoSource.RX);
			VideoSourceDictionary.Add("r9", VideoSource.RX);
			
			VideoSourceDictionary.Add("dvdrip", VideoSource.DVDRip);

			VideoSourceDictionary.Add("dvdr", VideoSource.DVDR);
			VideoSourceDictionary.Add("isorip", VideoSource.DVDR);
			VideoSourceDictionary.Add("iso", VideoSource.DVDR);			
			VideoSourceDictionary.Add("dvdfull", VideoSource.DVDR);
			VideoSourceDictionary.Add("dvd5", VideoSource.DVDR);
			VideoSourceDictionary.Add("dvd9", VideoSource.DVDR);

			VideoSourceDictionary.Add("hdtv", VideoSource.TVRip);
			VideoSourceDictionary.Add("hd tv", VideoSource.TVRip);
			VideoSourceDictionary.Add("dsr", VideoSource.TVRip);
			VideoSourceDictionary.Add("dsrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("dthrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("dvbrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("pdtv", VideoSource.TVRip);
			VideoSourceDictionary.Add("tvrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("hdtvrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("sdtv", VideoSource.TVRip);
			VideoSourceDictionary.Add("sd tv", VideoSource.TVRip);

			VideoSourceDictionary.Add("vodrip", VideoSource.VODRip);
			VideoSourceDictionary.Add("vodr", VideoSource.VODRip);

			VideoSourceDictionary.Add("bdr", VideoSource.BluRay);
			VideoSourceDictionary.Add("bd25", VideoSource.BluRay);
			VideoSourceDictionary.Add("bd50", VideoSource.BluRay);

			VideoSourceDictionary.Add("bluray", VideoSource.BluRayRip);
			VideoSourceDictionary.Add("bd5", VideoSource.BluRayRip);
			VideoSourceDictionary.Add("bd9", VideoSource.BluRayRip);
			VideoSourceDictionary.Add("blurayrip", VideoSource.BluRayRip);
			VideoSourceDictionary.Add("bdrip", VideoSource.BluRayRip);
			VideoSourceDictionary.Add("brrip", VideoSource.BluRayRip);
			#endregion
			#region VideoQualityDictionary
			//VideoQuality (Matching Str->VideoQuality)
			VideoQualityDictionary.Add("1080p", VideoQuality.FullHD);
			VideoQualityDictionary.Add("1080i", VideoQuality.FullHD);

			VideoQualityDictionary.Add("720p", VideoQuality.HDReady);
			VideoQualityDictionary.Add("720i", VideoQuality.HDReady);

			VideoQualityDictionary.Add("pal", VideoQuality.PAL);

			VideoQualityDictionary.Add("ntsc", VideoQuality.NTSC);

			VideoQualityDictionary.Add("secam", VideoQuality.NTSC);

			VideoQualityDictionary.Add("480p", VideoQuality.SD);
			VideoQualityDictionary.Add("360p", VideoQuality.SD);
			VideoQualityDictionary.Add("240p", VideoQuality.SD);			
			#endregion
			#region VideoCodecDictionary
			//VideoCodec (Matching Str->VideoQuality)
			VideoCodecDictionary.Add("h264", VideoCodec.H264);
			VideoCodecDictionary.Add("x264", VideoCodec.H264);
            VideoCodecDictionary.Add("qebs", VideoCodec.H264);

			VideoCodecDictionary.Add("divx", VideoCodec.DivX);

			VideoCodecDictionary.Add("xvid", VideoCodec.Xvid);

			VideoCodecDictionary.Add("mpeg4", VideoCodec.MPEG4);

			VideoCodecDictionary.Add("nero", VideoCodec.Nero);

			VideoCodecDictionary.Add("sorenson", VideoCodec.QuickTime);

			VideoCodecDictionary.Add("wmv", VideoCodec.WMV);
			VideoCodecDictionary.Add("windows media", VideoCodec.WMV);

			VideoCodecDictionary.Add("vp3", VideoCodec.VPX);
			VideoCodecDictionary.Add("vp4", VideoCodec.VPX);
			VideoCodecDictionary.Add("vp5", VideoCodec.VPX);
			VideoCodecDictionary.Add("vp6", VideoCodec.VPX);
			VideoCodecDictionary.Add("vp7", VideoCodec.VPX);
			VideoCodecDictionary.Add("vp8", VideoCodec.VPX);

			VideoCodecDictionary.Add("theora", VideoCodec.Theora);

			VideoCodecDictionary.Add("real", VideoCodec.RealVideo);

			VideoCodecDictionary.Add("dirac", VideoCodec.Dirac);

			VideoCodecDictionary.Add("indeo", VideoCodec.Indeo);

			VideoCodecDictionary.Add("cinepak", VideoCodec.Cinepak);

			VideoCodecDictionary.Add("dv", VideoCodec.DV);
			VideoCodecDictionary.Add("hdv", VideoCodec.DV);

			VideoCodecDictionary.Add("lossless", VideoCodec.Lossless);
			#endregion
			#region AudioCodecDictionary
			//AudioCodec (Matching Str->VideoQuality)
			AudioCodecDictionary.Add("mp3", AudioCodec.MP3);

			AudioCodecDictionary.Add("aac", AudioCodec.AAC);

			AudioCodecDictionary.Add("dts", AudioCodec.DTS);

			AudioCodecDictionary.Add("dolby", AudioCodec.AC3);
			AudioCodecDictionary.Add("dolbydigital", AudioCodec.AC3);
			AudioCodecDictionary.Add("ac3", AudioCodec.AC3);

			AudioCodecDictionary.Add("dtshd", AudioCodec.DTSHD);

			AudioCodecDictionary.Add("wma", AudioCodec.WMA);

			AudioCodecDictionary.Add("wav", AudioCodec.Wave);
			AudioCodecDictionary.Add("pcm", AudioCodec.Wave);

			AudioCodecDictionary.Add("flac", AudioCodec.FLAC);

			AudioCodecDictionary.Add("alac", AudioCodec.ALAC);
			AudioCodecDictionary.Add("apple", AudioCodec.ALAC);

			AudioCodecDictionary.Add("truehd", AudioCodec.TrueHD);

			//AudioCodecDictionary.Add("ape", AudioCodec.APE);//gave problems with title
			AudioCodecDictionary.Add("monkey", AudioCodec.APE);

			AudioCodecDictionary.Add("realaudio", AudioCodec.RealAudio);

			AudioCodecDictionary.Add("mp1", AudioCodec.MP1);

			AudioCodecDictionary.Add("mp2", AudioCodec.MP2);

			AudioCodecDictionary.Add("heaac", AudioCodec.HEAAC);

			AudioCodecDictionary.Add("vorbis", AudioCodec.Vorbis);

			AudioCodecDictionary.Add("lossless", AudioCodec.Vorbis);
			#endregion			
			#region CutDictionary
			//Cut (Matching Str->VideoQuality)
			CutDictionary.Add("clean", Cut.Clean);

			CutDictionary.Add("explicit", Cut.Explicit);

			CutDictionary.Add("unrated", Cut.Unrated);

			CutDictionary.Add("director", Cut.Directors);
			CutDictionary.Add("directors", Cut.Directors);
			CutDictionary.Add("dir cut", Cut.Directors);
			CutDictionary.Add("dc", Cut.Directors);
			CutDictionary.Add("dircut", Cut.Extended);

			CutDictionary.Add("editor", Cut.Editors);
			CutDictionary.Add("editors", Cut.Editors);

			CutDictionary.Add("extended", Cut.Extended);
			CutDictionary.Add("ee", Cut.Extended);
			CutDictionary.Add("exted", Cut.Extended);

			CutDictionary.Add("final", Cut.Final);
			#endregion			
		}
		/// <summary>
		/// Looks up the container for the given string
		/// </summary>
		/// <param name="str">Lookup string</param>
		/// <returns>Container</returns>
		public Container StrToContainer(String str)
		{
			try
			{
				return ContainerDictionary[str];
			}
			catch
			{
				return Container.Unknown;
			}
		}
		/// <summary>
		/// Looks up the videosource for the given string
		/// </summary>
		/// <param name="str">Lookup String</param>
		/// <returns>VideoSource</returns>
		public VideoSource StrToVideoSource(String str)
		{
			try
			{
				return VideoSourceDictionary[CleanLookupString(str)];
			}
			catch
			{
				return VideoSource.Unknown;
			}
		}
		/// <summary>
		/// Looks up the videoquality for the given string
		/// </summary>
		/// <param name="str">Lookup String</param>
		/// <returns>VideoQuality</returns>
		public VideoQuality StrToVideoQuality(String str)
		{
			try
			{
				return VideoQualityDictionary[CleanLookupString(str)];
			}
			catch
			{
				return VideoQuality.Unknown;
			}
		}
		/// <summary>
		/// Looks up the videocodec for the given string
		/// </summary>
		/// <param name="str">Lookup String</param>
		/// <returns>VideoCodec</returns>
		public VideoCodec StrToVideoCodec(String str)
		{
			try
			{
				return VideoCodecDictionary[CleanLookupString(str)];
			}
			catch
			{
				return VideoCodec.Unknown;
			}
		}
		/// <summary>
		/// Looks up the audiocodec for the given string
		/// </summary>
		/// <param name="str">Lookup String</param>
		/// <returns>AudioCodec</returns>
		public AudioCodec StrToAudioCodec(String str)
		{
			try
			{
				return AudioCodecDictionary[CleanLookupString(str)];
			}
			catch
			{
				return AudioCodec.Unknown;
			}
		}
		/// <summary>
		/// Looks up the cut for the given string
		/// </summary>
		/// <param name="str">Lookup String</param>
		/// <returns>Cut</returns>
		public Cut StrToCut(String str)
		{
			try
			{
				return CutDictionary[CleanLookupString(str)];
			}
			catch
			{
				return Cut.Final;
			}
		}
		private String CleanLookupString(String str)
		{
			str = str.Replace("-", "");
			str = str.Replace("_", "");
			//str = str.Replace(".", "");
			//str = str.Replace(" ", "");	
			return str.ToLowerInvariant();
		}
		public String CleanFileName(String str)
		{
			return CleanLookupString(str);
		}
	}
}
