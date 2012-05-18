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

		private Boolean sample;
		public Boolean Sample
		{
			get
			{
				return sample;
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
			

		//Episode or movie?
			Regex ep = new Regex(@"([Ss]\d{1,3}[Ee]\d{1,3})|(\d{1,3}[Xx]\d{1,3})"); //Episode notation, s02e33 and 2x33 are supported
			Regex date = new Regex(@"[0-9]{2,4}-[0-9]{2}-[0-9]{2,4}"); //Episode by date is also supported

			if (ep.IsMatch(input) | date.IsMatch(input))
			{
				ParseEpisode(input, dir);
			}
			else
			{
				ParseMovie(input, dir);
			}
			ParseShared(inputCl, dir);

			Boolean match = ep.IsMatch("2x04");
		}

		private void ParseMovie(String input, Boolean dir)
		{
			#region Year
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
		}

		private void ParseEpisode(String input, Boolean dir)
		{

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
			#region cut
			if (cut == Cut.Final)
			{
				cut = helperDictionary.StrToCut(Check(input, helperDictionary.CutStrings));
			}
			#endregion
			#region sample
			//Check if our file is a sample
			if (!sample)
			{
				if (input.ToLower().Contains("sample") & (fileSize < 100 * 1024 * 1024))
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
			//TODO rewrite?
			String result = "";
			for (Int32 i = 0; i < props.Count(); i++)
			{
				String c = props[i];
				if (input.ToLower().Contains(c))
				{
					result = c;
				}
			}
			return result;
		}

		/// <summary>
		/// Provides a string that's useful for debugging when called from another class
		/// </summary>
		/// <returns>(string) Debugging info</returns>
		public override String ToString()
		{
			return "GENERAL:" +
				"\nProcessed file:\t\t" + fileName +
				"\nIn directory:\t\t" + fileDirName +
				"\n\nPROPERTIES:" +
				"\nTitle:\t\t\t" + title +
				"\nFallback title:\t\t" + titleFallback +
				"\nYear:\t\t\t" + year +
				"\nQuality:\t\t\t" + videoQuality.ToDisplayString() +
				"\nSource:\t\t\t" + videoSource.ToDisplayString() +
				"\nContainer:\t\t" + container.ToDisplayString() +
				"\nVideo Codec:\t\t" + videoCodec.ToDisplayString() +
				"\nAudio Codec:\t\t" + audioCodec.ToDisplayString() +
				"\nCut:\t\t\t" + cut.ToDisplayString() +
				"\nSample:\t\t\t" + sample +
				"\nOther info:\t\t" + other;
		}
	}
}
