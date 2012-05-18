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

		private DirectoryInfo fileDir;
		private String fileDirName;

		private HelperDictionary helperDictionary;

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

		Int32[] indices = new Int32[10]; //must be equal or greater than the amount of properties we scan for
		String[] processed = new String[30]; //30 is the maximum amount of processable substrings, can be increased if necessary
		Int32 iP = 0; //iterator for processed array

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
			fileDir = fileInfo.Directory;
			fileDirName = fileDir.Name;

			parse(fileName, false);
			parse(fileDirName, true);

		}

		/// <summary>
		/// Takes a string (file name or directory name) and extracts movie or show data from it.
		/// </summary>
		/// <param name="inputString">The string to process.</param>
		/// <param name="dir">The input type, False for filename, True for directory name</param>
		private void parse(String inputString, Boolean dir)
		{
			String input_cl = helperDictionary.CleanFileName(inputString);
			//TODO check if needed
			Array.Clear(indices, 0, indices.Length);
			Array.Clear(processed, 0, processed.Length);
			iP = 0;
			//TODO check those elses necessary
			if (videoQuality == VideoQuality.Unknown)
			{
				videoQuality = helperDictionary.StrToVideoQuality(check(inputString, helperDictionary.VideoQualityStrings));
			}
			else
			{
				check(inputString, helperDictionary.VideoQualityStrings);
			}

			if (videoSource == VideoSource.Unknown)
			{
				videoSource = helperDictionary.StrToVideoSource(check(inputString, helperDictionary.VideoSourceStrings));
			}
			else
			{
				check(inputString, helperDictionary.VideoSourceStrings);
			}

			if (container == Container.Unknown & !dir)
			{
				//TODO (use filext)
				container = helperDictionary.StrToContainer(check(fileExt, helperDictionary.ContainerStrings));
				container = helperDictionary.StrToContainer(check(inputString, helperDictionary.ContainerStrings));
			}
			else
			{
				check(fileExt, helperDictionary.ContainerStrings);
			}

			if (videoCodec == VideoCodec.Unknown)
			{
				videoCodec = helperDictionary.StrToVideoCodec(check(inputString, helperDictionary.VideoCodecStrings));
			}
			else
			{
				check(inputString, helperDictionary.VideoCodecStrings);
			}

			if (audioCodec == AudioCodec.Unknown)
			{
				audioCodec = helperDictionary.StrToAudioCodec(check(inputString, helperDictionary.AudioCodecStrings));
			}
			else
			{
				check(inputString, helperDictionary.AudioCodecStrings);
			}
		}

		private void parse_movie(String input, Int32 type)
		{
			#region Year
			//A little regex for recognizing the year
			Regex rgx = new Regex(@"\b((19|20)\d{2})\b");

			if (rgx.IsMatch(input))
			{
				if (year == 0)
				{
					String tmpyear = rgx.Matches(input)[rgx.Matches(input).Count - 1].ToString();
					indices[iP] = input.IndexOf(tmpyear);
					processed[iP] = tmpyear;
					iP++;
					Int32.TryParse(tmpyear, out year);
				}
			}
			#endregion
		}

		private void parse_episode()
		{

		}

		private void parse_shared(String input, Boolean dir)
		{
			#region Sample
			//Check if our file is a sample
			if (!sample)
			{
				if (input.ToLower().Contains("sample") & (fileSize < 100 * 1024 * 1024))
				{
					sample = true;
					indices[iP] = input.IndexOf("sample");
					processed[iP] = "sample";
					iP++;
				}
			}
			#endregion
			#region Title & Other
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
			#endregion
		}

		/// <summary>
		/// Checks for a preset string in the provided string and returns its "pretty name" (prop)
		/// </summary>
		/// <param name="input">The string to check</param>
		/// <param name="props">An array with preset strings to find in "input"</param>        /// 
		/// <returns>MatchedString</returns>
		private String check(String input, List<String> props)
		{
			//TODO rewrite?
			String result = "";
			for (Int32 i = 0; i < props.Count(); i++)
			{
				String c = props[i];
				if (input.ToLower().Contains(c))
				{
					result = c;
					indices[iP] = input.IndexOf(c);
					processed[iP] = input.Substring(input.ToLower().IndexOf(c), c.Length);
					iP++;
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
				"\nContainer:\t\t\t" + container.ToDisplayString() +
				"\nVideo Codec:\t\t" + videoCodec.ToDisplayString() +
				"\nAudio Codec:\t\t" + audioCodec.ToDisplayString() +
				"\nCut:\t\t" + cut.ToDisplayString() +
				"\nSample:\t\t\t" + sample +
				"\nOther info:\t\t" + other;
		}
	}
}
