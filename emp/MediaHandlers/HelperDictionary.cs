using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EMP
{
	class HelperDictionary
	{
		private Dictionary<String, Container> ContainerDictionary = new Dictionary<String, Container>();
		private Dictionary<String, VideoSource> VideoSourceDictionary = new Dictionary<String, VideoSource>();
		private Dictionary<String, VideoQuality> VideoQualityDictionary = new Dictionary<String, VideoQuality>();
		private Dictionary<String, VideoCodec> VideoCodecDictionary = new Dictionary<String, VideoCodec>();
		private Dictionary<String, AudioCodec> AudioCodecDictionary = new Dictionary<String, AudioCodec>();

		public HelperDictionary()
		{
			#region Hardcoded Lookup Dicts

			//Containers (FileExt->Contianer)
			ContainerDictionary.Add("mkv", Container.Matroska);
			ContainerDictionary.Add("mk3d", Container.Matroska);
			ContainerDictionary.Add("mka", Container.Matroska);
			ContainerDictionary.Add("mks", Container.Matroska);

			//VideoSources (Matching Str->Contianer)
			VideoSourceDictionary.Add("cam", VideoSource.Cam);
			VideoSourceDictionary.Add("camrip", VideoSource.Cam);

			VideoSourceDictionary.Add("ts", VideoSource.Telesync);
			VideoSourceDictionary.Add("telesync", VideoSource.Telesync);
			VideoSourceDictionary.Add("pdvd", VideoSource.Telesync);

			VideoSourceDictionary.Add("workprint", VideoSource.Workprint);
			VideoSourceDictionary.Add("wp", VideoSource.Workprint);

			VideoSourceDictionary.Add("telecine", VideoSource.Telecine);
			VideoSourceDictionary.Add("tc", VideoSource.Telecine);

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
			VideoSourceDictionary.Add("dsr", VideoSource.TVRip);
			VideoSourceDictionary.Add("dsrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("dthrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("dvbrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("pdtv", VideoSource.TVRip);
			VideoSourceDictionary.Add("tvrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("hdtvrip", VideoSource.TVRip);
			VideoSourceDictionary.Add("sdtv", VideoSource.TVRip);

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
		}
		/// <summary>
		/// Looks up the container for the given string
		/// </summary>
		/// <param name="str">Lookup string</param>
		/// <returns>The corresponding Container for the string or Unknown if the string didn't match anything.</returns>
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
		/// <returns></returns>
		public VideoSource StrToVideoSource(String str)
		{
			try
			{
				return VideoSourceDictionary[CleanLookupStrings(str)];
			}
			catch
			{
				return VideoSource.Unknown;
			}
		}
		private String CleanLookupStrings(String str)
		{
			str = str.Replace("-", "");
			str = str.Replace("_", "");
			return str.ToLowerInvariant();
		}
	}
}
