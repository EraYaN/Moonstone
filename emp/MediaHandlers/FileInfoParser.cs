using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace EMP
{
	public class FileInfoParser
	{
		//Init	
		private String fileName;
		private String fileExt;
		private Int64 fileSize;
		private String fileDirName;

		private HelperDictionary helperDictionary;

		#region Properties
		private String title = "Unknown";
		public String Title
		{
			get
			{
				return title;
			}
		}

		private String titleFallback = "Unknown";
		public String TitleFallback
		{
			get
			{
				return titleFallback;
			}
		}

		private Int32 year;
		public Int32 Year
		{
			get
			{
				return year;
			}
		}

		private VideoQuality videoQuality;
		public VideoQuality VideoQuality
		{
			get
			{
				return videoQuality;
			}
		}

		private VideoSource videoSource;
		public VideoSource VideoSource
		{
			get
			{
				return videoSource;
			}
		}

		private Container container;
		public Container Container
		{
			get
			{
				return container;
			}
		}

		private VideoCodec videoCodec;
		public VideoCodec VideoCodec
		{
			get
			{
				return videoCodec;
			}
		}

		private AudioCodec audioCodec;
		public AudioCodec AudioCodec
		{
			get
			{
				return audioCodec;
			}
		}

		private Cut cut;
		public Cut Cut
		{
			get
			{
				return cut;
			}
		}

		private MediaKind mediaKind;
		public MediaKind MediaKind
		{
			get
			{
				return mediaKind;
			}
		}

		private Boolean sample;
		public String Sample
		{
			get
			{
				if (sample)
				{
					return "Yes";
				}
				else
				{
					return "No";
				}
			}
		}

		private String other = "None";
		public String Other
		{
			get
			{
				return other;
			}
		}

		private Int16 season;
		public Int16 Season
		{
			get
			{
				return season;
			}
		}

		private Int16 episode;
		public Int16 Episode
		{
			get
			{
				return episode;
			}
		}

		private String series;
		public String Series
		{
			get
			{
				return series;
			}
		}

		private DateTime airByDate;
		public DateTime AirByDate
		{
			get
			{
				return airByDate;
			}
		}
		#endregion
		/// <summary>
		/// Constructor for movie and/or show data.
		/// </summary>
		/// <param name="fileInfo">The file to process.</param>
		public FileInfoParser(FileInfo fileInfo, HelperDictionary dict = null)
		{
			if (dict == null)
			{
				helperDictionary = new HelperDictionary();
			}
			else
			{
				helperDictionary = dict;
			}
			//Some obvious shit first
			fileName = fileInfo.Name;
			fileSize = fileInfo.Length;
			fileExt = fileInfo.Extension;
			fileDirName = fileInfo.Directory.Name;			

			Parse(fileName, false);
			Parse(fileDirName, true);

		}

		/// <summary>
		/// Takes a string (file name or directory name) and extracts movie or show data from it.
		/// </summary>
		/// <param name="inputString">The string to process.</param>
		/// <param name="dir">The input type, False for filename, True for directory name</param>
		private void Parse(String input, Boolean dir)
		{
			String inputCl = helperDictionary.CleanFileName(input); //Clean dat shit for checking
			ParseShared(inputCl, dir);
			//Episode or movie?
			Regex ep = new Regex(@"[Ss](\d{1,3})[Ee](\d{1,3})|(\d{1,3})[Xx](\d{1,3})"); //Episode notation, ie s02e33 and 2x33 are supported, case-insensitive
			Regex date = new Regex(@"[0-9]{2,4}[-._][0-9]{2}[-._][0-9]{2,4}"); //Episode by date is also supported
			if (ep.IsMatch(input))
			{
				//Match will have 4 groups. Either the first 2 are populated or the last two.
				ParseEpisode(input, dir, ep.Match(input));
			}
			else if (date.IsMatch(input))
			{
				ParseEpisode(input, dir, date.Match(input), true);
			}
			else if (mediaKind == EMP.MediaKind.Movie || mediaKind == EMP.MediaKind.Unknown)
			{
				ParseMovie(input, dir);
			}
		}

		private void ParseMovie(String input, Boolean dir)
		{
			mediaKind = EMP.MediaKind.Movie;
			#region year
			//A little regex for recognizing the year
			Regex rgx = new Regex(@"((19|20)\d{2})");

			if (rgx.IsMatch(input))
			{
				if (year == 0)
				{
					String tmpyear = rgx.Matches(input)[rgx.Matches(input).Count - 1].ToString();
					Int32.TryParse(tmpyear, out year);
				}
			}
			#endregion
			#region cut
			if (cut == Cut.Final)
			{
				cut = helperDictionary.StrToCut(Check(input.ToLowerInvariant(), helperDictionary.CutStrings));
			}
			#endregion
		}

		private void ParseEpisode(String input, Boolean dir, Match match, Boolean AirDateInName = false)
		{
			mediaKind = EMP.MediaKind.Show;
			GroupCollection groups = match.Groups;
			if (AirDateInName)
			{
				if (airByDate == null)
				{
					DateTime.TryParse(match.ToString(), out airByDate);
				}
			}
			else
			{
				if (season == 0)
				{
					if (!Int16.TryParse(groups[1].ToString(), out season) || groups[1].ToString().Trim() == String.Empty)
					{
						Int16.TryParse(groups[3].ToString(), out season);
					}
				}
				if (episode == 0)
				{
					if (!Int16.TryParse(groups[2].ToString(), out episode) || groups[2].ToString().Trim() == String.Empty)
					{
						Int16.TryParse(groups[4].ToString(), out episode);
					}
				}
			}
		}

		private void ParseShared(String input, Boolean dir)
		{
			#region videoQuality
			if (videoQuality == VideoQuality.Unknown)
			{
				videoQuality = helperDictionary.StrToVideoQuality(Check(input, helperDictionary.VideoQualityStrings));
			}
			#endregion
			#region videoSource
			if (videoSource == VideoSource.Unknown)
			{
				videoSource = helperDictionary.StrToVideoSource(Check(input, helperDictionary.VideoSourceStrings));
			}
			#endregion
			#region container
			if (container == Container.Unknown & !dir)
			{
				container = helperDictionary.StrToContainer(Check(fileExt, helperDictionary.ContainerStrings));
			}
			#endregion
			#region videoCodec
			if (videoCodec == VideoCodec.Unknown)
			{
				videoCodec = helperDictionary.StrToVideoCodec(Check(input, helperDictionary.VideoCodecStrings));
			}
			#endregion
			#region audioCodec
			if (audioCodec == AudioCodec.Unknown)
			{
				audioCodec = helperDictionary.StrToAudioCodec(Check(input, helperDictionary.AudioCodecStrings));
			}
			#endregion
			#region sample
			//Check if our file is a sample
			if (!sample)
			{
				if (input.ToLower().Contains("sample") & ( fileSize < 100 * 1024 * 1024 ))
				{
					sample = true;
				}
			}
			#endregion
			#region title & other
			/*
			//Determine where the title ends
			Int32[] indicesSorted = indices; //Copy it
			Array.Sort(indicesSorted); //Sort it, adress 0 now contains the lowest value
			Int32 separation = 0;

			for (Int32 i = 0; i < indicesSorted.Count(); i++)
			{
				Int32 n = indicesSorted[i];
				if (n > 0)
				{
					separation = indicesSorted[i];
					break;
				}
			}

			//Getting the title itself
			String titletmp;
			if (iP == 0)
			{
				titletmp = input;
			}
			else
			{
				titletmp = input.Substring(0, separation - 1);
			}
			processed[iP] = titletmp;
			iP++;

			if (!dir)
			{
				title = titletmp.Replace('.', ' ');
			}
			else if (dir)
			{
				titleFallback = titletmp.Replace('.', ' ');
			}

			//A title starting with a capital letter is preferenced and mostly more correct
			if (dir & Char.IsUpper(titleFallback[0]) & Char.IsLower(title[0]))
			{
				String tmp = title;
				title = titleFallback;
				titleFallback = tmp;
			}

			//Extract any leftover shit into "other"
			if (other == "None")
			{
				other = input;
				foreach (String p in processed)
				{
					if (p != null)
					{
						other = other.Replace(p, null);
					}
				}
				String[] otherR = other.Split(new Char[] { '.', ' ', '_', '(', ')' });
				other = "";
				foreach (String o in otherR)
				{
					String s = o.Trim(new Char[] { '-', ' ' });
					if (s != "")
					{
						other = other + s + " ";
					}
				}
				if (other == "")
				{
					other = "None";
				}
			}
			 */
			#endregion
		}

		/// <summary>
		/// Checks for a preset string in the provided string and returns its "pretty name" (prop)
		/// </summary>
		/// <param name="input">The string to check</param>
		/// <param name="props">An array with preset strings to find in "input"</param>
		/// <returns>MatchedString</returns>
		private String Check(String input, List<String> props)
		{
			String result = "";
			for (Int32 i = 0; i < props.Count(); i++)
			{
				String c = props[i];
				if (input.Contains(c))
				{
					result = c;
				}
			}
			return result;
		}

		/// <summary>
		/// Provides a string that contains nearly all information contained in the class.
		/// </summary>
		/// <returns>String representation of the current class</returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("\nGENERAL:");
			sb.AppendFormat("Processed file: \t\t{0}/{1}\n", fileDirName, fileName);
			sb.AppendFormat("Media Kind: \t\t{0}\n", MediaKind.ToString());
			sb.AppendLine("\nPROPERTIES:");
			sb.AppendFormat("Title:\t\t\t{0}\n", Title);
			sb.AppendFormat("Cut:\t\t\t{0}\n", Cut.ToDisplayString());
			sb.AppendFormat("Is Sample:\t\t{0}\n", Sample);
			sb.AppendFormat("Fallback title:\t\t{0}\n", TitleFallback);
			sb.AppendFormat("Year:\t\t\t{0}\n", Year);
			if (mediaKind == EMP.MediaKind.Show)
			{
				sb.AppendFormat("Series:\t\t\t{0}\n", Series);
				sb.AppendFormat("Season:\t\t\t{0}\n", Season);
				sb.AppendFormat("Episode:\t\t\t{0}\n", Episode);
				//MessageBox.Show("SHOW!\n" + Series + "\nS" + Season + "E" + Episode);
			}
			sb.AppendFormat("Quality:\t\t\t{0}\n", VideoQuality.ToDisplayString());
			sb.AppendFormat("Source:\t\t\t{0}\n", VideoSource.ToDisplayString());
			sb.AppendFormat("Container:\t\t{0}\n", Container.ToDisplayString());
			sb.AppendFormat("Video Codec:\t\t{0}\n", VideoCodec.ToDisplayString());
			sb.AppendFormat("Audio Codec:\t\t{0}\n", AudioCodec.ToDisplayString());
			sb.AppendFormat("\nOTHER:\n{0}", Other);
			return sb.ToString();
		}
	}
}
