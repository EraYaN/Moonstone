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
		private String debugString;

		private String fileName;
		private Int64 fileSize;

		private DirectoryInfo fileDir;
		private String fileDirName;

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

		private String quality = "Unknown";
		public String Quality
		{
			get
			{
				return quality;
			}
		}

		private String source = "Unknown";
		public String Source
		{
			get
			{
				return source;
			}
		}

		private String filetype = "Unknown";
		public String Filetype
		{
			get
			{
				return filetype;
			}
		}

		private String codec = "Unknown";
		public String Codec
		{
			get
			{
				return codec;
			}
		}

		private String audioCodec = "Unknown";
		public String AudioCodec
		{
			get
			{
				return audioCodec;
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
		public FileInfoParser(FileInfo fileInfo)
		{
			//Some obvious shit first
			fileName = fileInfo.Name;
			fileSize = fileInfo.Length;
			fileDir = fileInfo.Directory;
			fileDirName = fileDir.Name;

			parse(fileName, 0);
			parse(fileDirName, 1);

			debugString = "GENERAL:" +
				"\nProcessed file:\t\t" + fileName +
				"\nIn directory:\t\t" + fileDirName +
				"\n\nPROPERTIES:" +
				"\nTitle:\t\t\t" + title +
				"\nFallback title:\t\t" + titleFallback +
				"\nYear:\t\t\t" + year +
				"\nQuality:\t\t\t" + quality +
				"\nSource:\t\t\t" + source +
				"\nFiletype:\t\t\t" + filetype +
				"\nVideo Codec:\t\t" + codec +
				"\nAudio Codec:\t\t" + audioCodec +
				"\nSample:\t\t\t" + sample +
				"\nOther info:\t\t" + other;

		}

		/// <summary>
		/// Takes a string (file name or directory name) and extracts movie or show data from it.
		/// </summary>
		/// <param name="inputString">The string to process.</param>
		/// <param name="type">The input type, 0 for filename, 1 for directory name</param>
		private void parse(String inputString, Int32 type)
		{
			Array.Clear(indices, 0, indices.Length);
			Array.Clear(processed, 0, processed.Length);
			iP = 0;

			//Here we specify a couple of string arrays which could match certain subStrings
			//If there is a match, it will give us some more info about the file's properties
			//For display, we use another array containing the pretty names for each property
			String[] qualities = new String[2] { "720p", "1080p" };
			String[] qualityNames = new String[2] { "720p HD", "1080p HD" };
			if (quality == "Unknown")
			{
				quality = check(inputString, qualities, qualityNames);
			}
			else
			{
				check(inputString, qualities, qualityNames);
			}

			String[] sources = new String[4] { "brrip", "bdrip", "bluray", "dvdrip" };
			String[] sourceNames = new String[4] { "Blu-ray Rip", "Blu-ray Rip", "Blu-ray Rip", "DVD Rip" };
			if (source == "Unknown")
			{
				source = check(inputString, sources, sourceNames);
			}
			else
			{
				check(inputString, sources, sourceNames);
			}

			String[] filetypes = new String[4] { "mkv", "avi", "mp4", "m4a" };
			String[] filetypeNames = new String[4] { "Matroska Video (.mkv)", "Microsoft AVI (.avi)", "MPEG-4 (.mp4)", "MPEG-4 (.m4a)" };
			if (filetype == "Unknown" & type == 0)
			{
				filetype = check(inputString, filetypes, filetypeNames);
			}
			else
			{
				check(inputString, filetypes, filetypeNames);
			}

			String[] codecs = new String[4] { "x264", "h264", "xvid", "divx" };
			String[] codecNames = new String[4] { "x264 Codec (x264)", "H.264 Codec (h264)", "Xvid Codec (xvid)", "DivX Codec (divx)" };
			if (codec == "Unknown")
			{
				codec = check(inputString, codecs, codecNames);
			}
			else
			{
				check(inputString, codecs, codecNames);
			}

			String[] audioCodecs = new String[4] { "dts", "aac", "ac3", "mp3" };
			String[] audioCodecNames = new String[4] { "DTS Audio Codec (DTS)", "Advanced Audio Coding (AAC)", "Dolby Digital (AC3)", "MPEG-2 Audio Layer III (MP3)" };
			if (audioCodec == "Unknown")
			{
				audioCodec = check(inputString, audioCodecs, audioCodecNames);
			}
			else
			{
				check(inputString, audioCodecs, audioCodecNames);
			}

			//Check if our file is a sample
			if (!sample)
			{
				if (inputString.ToLower().Contains("sample") & (fileSize < 100 * 1024 * 1024))
				{
					sample = true;
					indices[iP] = inputString.IndexOf("sample");
					processed[iP] = "sample";
					iP++;
				}
			}

			//A little regex for recognizing the year
			Regex rgx = new Regex(@"\b((19|20)\d{2})\b");

			if (rgx.IsMatch(inputString))
			{
				if (year == 0)
				{
					String tmpyear = rgx.Matches(inputString)[rgx.Matches(inputString).Count - 1].ToString();
					indices[iP] = inputString.IndexOf(tmpyear);
					processed[iP] = tmpyear;
					iP++;
					Int32.TryParse(tmpyear, out year);
				}
			}
			
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
				titletmp = inputString;
			}
			else
			{
				titletmp = inputString.Substring(0, separation - 1);
			}
			processed[iP] = titletmp;
			iP++;

			if (type == 0)
			{
				title = titletmp.Replace('.', ' ');
			}
			else if (type == 1)
			{
				titleFallback = titletmp.Replace('.', ' ');
			}

			//A title starting with a capital letter is preferenced and mostly more correct
			if (type == 1 & Char.IsUpper(titleFallback[0]) & Char.IsLower(title[0]))
			{
				String tmp = title;
				title = titleFallback;
				titleFallback = tmp;
			}


			//Extract any leftover shit into "other"
			if (other == "None")
			{
				other = inputString;
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
		}

		/// <summary>
		/// Checks for a preset string in the provided string and returns its "pretty name" (prop)
		/// </summary>
		/// <param name="input">The string to check</param>
		/// <param name="props">An array with preset properties to find in "input"</param>
		/// <param name="propNames">An array with "pretty names", from whitch a match will be returned as "prop"</param>
		/// <returns>prop</returns>
		private String check(String input, String[] props, String[] propNames)
		{
			String prop = "Unknown";
			for (Int32 i = 0; i < props.Count(); i++)
			{
				String c = props[i];

				if (input.ToLower().Contains(c))
				{
					prop = propNames[i];
					indices[iP] = input.IndexOf(c);
					//MessageBox.Show(c.Length.ToString());
					processed[iP] = input.Substring(input.ToLower().IndexOf(c), c.Length);
					iP++;
				}
			}
			return prop;
		}

		/// <summary>
		/// Provides a string that's useful for debugging when called from another class
		/// </summary>
		/// <returns>(string) Debugging info</returns>
		public override String ToString()
		{
			return "Debugging info:\n\n" + debugString + "\n";
		}
	}
}
