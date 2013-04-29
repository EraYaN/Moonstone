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
        private List<StringLocation> Index;

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
        private String imdbid;
        public String IMDBid
        {
            get
            {
                if (imdbid != null && imdbid != String.Empty)
                {
                    return imdbid;
                }
                else
                {
                    return "Unknown";
                }
            }
        }
        private String other;
        public String Other
        {
            get
            {
                if (String.IsNullOrEmpty(other))
                {
                    return "None";
                }
                else
                {
                    return other;
                }
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

        private DateTime airDate;
        public DateTime AirDate
        {
            get
            {
                return airDate;
            }
        }
        private String TmpTitle;
        private String TmpDirTitle;
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


            Index = new List<StringLocation>();
            //Some obvious shit first
            fileName = fileInfo.Name;
            fileSize = fileInfo.Length;
            fileExt = fileInfo.Extension;
            fileDirName = fileInfo.Directory.Name;

            //TODO rewrite to 1 function call
            Parse(fileName, false);
            Parse(fileDirName, true);

            String[] tmptitles = GetStringInfo();
            TmpTitle = tmptitles[0];
            TmpDirTitle = tmptitles[1];
        }

        /// <summary>
        /// Takes a string (file name or directory name) and extracts movie or show data from it.
        /// </summary>
        /// <param name="inputString">The string to process.</param>
        /// <param name="dir">The input type, False for filename, True for directory name</param>
        private void Parse(String input, Boolean dir)
        {
            ParseShared(input, dir);
            //Episode or movie?
            Regex ep = new Regex(@"[Ss](\d{1,3})\s?[Ee](\d{1,3})|(\d{1,3})[Xx](\d{1,3})"); //Episode notation, ie s02e33 and 2x33 are supported, case-insensitive
            Regex date = new Regex(@"[0-9]{2,4}[-._][0-9]{2}[-._][0-9]{2,4}"); //Episode by date is also supported
            if (ep.IsMatch(input))
            {
                //Match will have 4 groups. Either the first 2 are populated or the last two.
                ParseShow(input, ep.Match(input), dir);
            }
            else if (date.IsMatch(input))
            {
                ParseShow(input, date.Match(input), dir, true);
            }
            else if (mediaKind == EMP.MediaKind.Movie || mediaKind == EMP.MediaKind.Unknown)
            {
                ParseMovie(input, dir);
            }
        }
        private String MaskPartOfString(String input, Int32 Start, Int32 Length)
        {
            Char[] chars = input.ToCharArray();
            for (int I = Start; I < Start + Length; I++)
            {
                chars[I] = '#';
            }
            return new String(chars);
        }
        private Int32 CleanToNormal(String input, Int32 pos)
        {
            input = input.ToLowerInvariant();
            String inputCl = helperDictionary.CleanFileName(input);
            String searchStr = inputCl.Substring(pos, Math.Min(3, inputCl.Length - pos));
            if (searchStr[0] == '.')
            {
                MessageBox.Show("Fail");
            }
            List<Int32> list = new List<int>();
            Int32 tmpI = 0;
            Int32 result = pos;
            do
            {
                tmpI = input.IndexOf(searchStr, Math.Max(0, tmpI + 1));
                if (tmpI > -1)
                    list.Add(tmpI);
            } while (tmpI > -1);

            foreach (Int32 i in list)
            {
                if (i >= pos)
                {
                    return i;
                }
            }
            return pos;

        }
        private String[] GetStringInfo()
        {
            String dir = fileDirName;
            String file = Path.GetFileNameWithoutExtension(fileName);
            String dirCl = helperDictionary.CleanFileName(fileDirName);
            String fileCl = helperDictionary.CleanFileName(fileName);
            StringBuilder sb = new StringBuilder();
            //match strings (algorithm of exclusion)
            String tmptitledir = dir;
            String tmptitle = file;
            foreach (StringLocation sl in Index)
            {
                String result = "";
                String source = "";
                Int32 Start = sl.Start;

                if (sl.InDirectoryName)
                {
                    if (sl.InCleanString)
                    {
                        Start = CleanToNormal(dir, Start);
                    }
                    result = dir.Substring(Start, sl.Length);
                    source = MaskPartOfString(dir, Start, sl.Length);
                    tmptitledir = MaskPartOfString(tmptitledir, Start, sl.Length);
                }
                else
                {
                    if (sl.InCleanString)
                    {
                        Start = CleanToNormal(file, Start);
                    }
                    result = file.Substring(Start, sl.Length);
                    source = MaskPartOfString(file, Start, sl.Length);
                    tmptitle = MaskPartOfString(tmptitle, Start, sl.Length);
                }
                sb.AppendLine(sl.String + " = " + result + " - Start: " + Start + " (" + sl.Start + ") - Length: " + sl.Length + "\n" + source);
            }
            sb.AppendLine("\n\n" + tmptitle + "\n" + tmptitledir);
            //MessageBox.Show(sb.ToString());
            //tmptitle = tmptitle.Replace(" - ", " ").Trim();
            //tmptitledir = tmptitledir.Replace(" - ", " ").Trim();
            Regex regexSpaces = new Regex(@"(\s*-|-\s*)");
            tmptitle = regexSpaces.Replace(tmptitle, @" ");
            tmptitledir = regexSpaces.Replace(tmptitledir, @" ");

            Regex regex = new Regex(@"([#]+[-_\.]+)?([\.\w\d\s',_-]+)((\(?[#]+[-_)\.]*)+([\.\w\d\s'_-]+)((\(?\{?\[?[#]+[-_)}\.]*)+(.+))?)?");
            Match fnMatch = regex.Match(tmptitle);
            Match dnMatch = regex.Match(tmptitledir);
            if (mediaKind == EMP.MediaKind.Movie)
            {
                if (fnMatch.Groups[2].Length > dnMatch.Groups[2].Length || fnMatch.Groups[2].Length > 4)
                {
                    title = fnMatch.Groups[2].ToString().Replace(".", " ").Replace("_", " ").Trim();
                    titleFallback = dnMatch.Groups[2].ToString().Replace(".", " ").Replace("_", " ").Trim();
                }
                else
                {
                    title = dnMatch.Groups[2].ToString().Replace(".", " ").Replace("_", " ").Trim();
                    titleFallback = fnMatch.Groups[2].ToString().Replace(".", " ").Replace("_", " ").Trim();
                }
                if (fnMatch.Groups[5].Length + fnMatch.Groups[7].Length + fnMatch.Groups[1].Length > dnMatch.Groups[5].Length + dnMatch.Groups[7].Length + dnMatch.Groups[1].Length)
                {
                    other = (fnMatch.Groups[1].ToString() + " " + fnMatch.Groups[5].ToString() + " " + fnMatch.Groups[7].ToString()).Replace(".", " ").Replace("_", " ").Trim();
                }
                else
                {
                    other = (dnMatch.Groups[1].ToString() + " " + dnMatch.Groups[5].ToString() + " " + dnMatch.Groups[7].ToString()).Replace(".", " ").Replace("_", " ").Trim();
                }
                series = "N/A";
            }
            else if (mediaKind == EMP.MediaKind.Show)
            {
                if (fnMatch.Groups[5].Length > dnMatch.Groups[5].Length || fnMatch.Groups[5].Length > 4)
                {
                    title = fnMatch.Groups[5].ToString().Replace(".", " ").Replace(" Unknown", "").Replace("_", " ").Trim();
                    titleFallback = dnMatch.Groups[5].ToString().Replace(".", " ").Replace(" Unknown", "").Replace("_", " ").Trim();
                }
                else
                {
                    title = dnMatch.Groups[5].ToString().Replace(".", " ").Replace("_", " ").Trim();
                    titleFallback = fnMatch.Groups[5].ToString().Replace(".", " ").Replace("_", " ").Trim();
                }
                if (fnMatch.Groups[2].Length > dnMatch.Groups[2].Length || fnMatch.Groups[2].Length > 4)
                {
                    series = fnMatch.Groups[2].ToString().Replace(".", " ").Replace("_", " ").Replace("-", " ").Trim();
                }
                else
                {
                    series = dnMatch.Groups[2].ToString().Replace(".", " ").Replace("_", " ").Replace("-", " ").Trim();
                }
                if (fnMatch.Groups[7].Length + fnMatch.Groups[1].Length > dnMatch.Groups[7].Length + dnMatch.Groups[1].Length)
                {
                    other = (fnMatch.Groups[1].ToString() + " " + fnMatch.Groups[7].ToString()).Replace(".", " ").Replace("_", " ").Trim();
                }
                else
                {
                    other = (dnMatch.Groups[1].ToString() + " " + dnMatch.Groups[7].ToString()).Replace(".", " ").Replace("_", " ").Trim();
                }
            }
            return new String[] { tmptitle, tmptitledir };
        }
        /// <summary>
        /// Parses the Movie specific data from the input string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="dir">Switch whenever it is a directory name.</param>
        private void ParseMovie(String input, Boolean dir = false)
        {
            mediaKind = EMP.MediaKind.Movie;
            String inputCl = helperDictionary.CleanFileName(input);
            Int32 TmpStart;
            String TmpString;
            #region year
            //A little regex for recognizing the year
            Regex rgx = new Regex(@"((19|20)\d{2})");

            if (rgx.IsMatch(input))
            {
                if (year == 0)
                {
                    Match match = rgx.Matches(inputCl)[rgx.Matches(input).Count - 1];
                    TmpString = match.ToString();
                    if (match.Length > 0)
                        Index.Add(new StringLocation(TmpString, match.Index, match.Length, true, dir));
                    Int32.TryParse(TmpString, out year);
                }
            }
            #endregion
            #region cut
            if (cut == Cut.Final)
            {
                TmpString = Check(inputCl, helperDictionary.CutStrings, out TmpStart);
                cut = helperDictionary.StrToCut(TmpString);
                if (TmpString.Length > 0)
                    Index.Add(new StringLocation(TmpString, TmpStart, TmpString.Length, true, dir));
            }
            #endregion
            #region IMDBid
            if (imdbid == String.Empty || imdbid == null)
            {
                // Here we call Regex.Match.
                Match match = Regex.Match(input, @"tt[0-9]{7}",
                    RegexOptions.IgnoreCase);

                // Here we check the Match instance.
                if (match.Success)
                {
                    // Finally, we get the Group value and display it.
                    imdbid = match.Value;
                    Index.Add(new StringLocation(match.Value, match.Index, match.Length, false, dir));
                }
            }
            #endregion
        }
        /// <summary>
        /// Parses the Show specific data from the input string and the match.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="match">The returned matches for the regexes.</param>
        /// <param name="dir">Switch whenever it is a directory name.</param>
        /// <param name="AirDateInName">Switch whenever the date regex matched.</param>
        private void ParseShow(String input, Match match, Boolean dir = false, Boolean AirDateInName = false)
        {
            mediaKind = EMP.MediaKind.Show;
            //String inputCl = helperDictionary.CleanFileName(input);//unnneeded
            GroupCollection groups = match.Groups;
            #region Season, Episode or AirDate
            if (match.Length > 0)
                Index.Add(new StringLocation(match.ToString(), match.Index, match.Length, false, dir));
            if (AirDateInName)
            {
                if (airDate == null)
                {
                    DateTime.TryParse(match.ToString(), out airDate);
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
            #endregion
            //title is series and the shit in between episode number and the rest is series title.
            #region Series, Title & Other

            #endregion
        }
        /// <summary>
        /// Parse all the values that are shared between shows and movies
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="dir">Switch whenever it is a directory name.</param>
        private void ParseShared(String input, Boolean dir = false)
        {
            String inputCl = helperDictionary.CleanFileName(input);
            Int32 TmpStart;
            String TmpString;
            #region videoQuality
            if (videoQuality == VideoQuality.Unknown)
            {
                TmpString = Check(inputCl, helperDictionary.VideoQualityStrings, out TmpStart);
                videoQuality = helperDictionary.StrToVideoQuality(TmpString);
                if (TmpString.Length > 0)
                    Index.Add(new StringLocation(TmpString, TmpStart, TmpString.Length, true, dir));
            }
            #endregion
            #region videoSource
            if (videoSource == VideoSource.Unknown)
            {
                TmpString = Check(inputCl, helperDictionary.VideoSourceStrings, out TmpStart);
                videoSource = helperDictionary.StrToVideoSource(TmpString);
                if (TmpString.Length > 0)
                    Index.Add(new StringLocation(TmpString, TmpStart, TmpString.Length, true, dir));
            }
            #endregion
            #region container
            if (container == Container.Unknown & !dir)
            {
                //TmpString = Check(fileExt, helperDictionary.ContainerStrings, out TmpStart);
                container = helperDictionary.StrToContainer(fileExt);
                //if (TmpString.Length > 0)
                //Index.Add(new StringLocation(TmpString, TmpStart, TmpString.Length, true, dir));
            }
            #endregion
            #region videoCodec
            if (videoCodec == VideoCodec.Unknown)
            {
                TmpString = Check(inputCl, helperDictionary.VideoCodecStrings, out TmpStart);
                videoCodec = helperDictionary.StrToVideoCodec(TmpString);
                if (TmpString.Length > 0)
                    Index.Add(new StringLocation(TmpString, TmpStart, TmpString.Length, true, dir));
            }
            #endregion
            #region audioCodec
            if (audioCodec == AudioCodec.Unknown)
            {
                TmpString = Check(inputCl, helperDictionary.AudioCodecStrings, out TmpStart);
                audioCodec = helperDictionary.StrToAudioCodec(TmpString);
                if (TmpString.Length > 0)
                    Index.Add(new StringLocation(TmpString, TmpStart, TmpString.Length, true, dir));
            }
            #endregion
            #region sample
            //Check if our file is a sample
            if (!sample)
            {
                TmpStart = inputCl.IndexOf("sample");
                if (TmpStart > -1 & (fileSize < 1024 * 1024 * 1024))
                {
                    sample = true;
                    Index.Add(new StringLocation("sample", TmpStart, 6, true, dir));
                }
            }
            #endregion
        }

        /// <summary>
        /// Checks for a preset string in the provided string and returns its "pretty name" (prop)
        /// </summary>
        /// <param name="input">The string to check</param>
        /// <param name="props">An array with preset strings to find in "input"</param>
        /// <returns>MatchedString</returns>
        private String Check(String input, List<String> props, out Int32 Start)
        {
            String result = "";
            Int32 TmpStart = -1;
            Start = -1;
            for (Int32 i = 0; i < props.Count(); i++)
            {
                String c = props[i];
                TmpStart = input.IndexOf(c);
                if (TmpStart > -1)
                {
                    result = c;
                    Start = TmpStart;
                }
            }
            return result;
        }

        private String Check(List<String> props, out Int32 Start)
        {
            return Check(helperDictionary.CleanFileName(fileDirName + "\\" + fileName), props, out Start);
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
            sb.AppendFormat("Is Sample:\t\t{0}\n", Sample);
            sb.AppendFormat("Fallback title:\t\t{0}\n", TitleFallback);
            if (mediaKind == EMP.MediaKind.Movie)
            {
                sb.AppendFormat("Year:\t\t\t{0}\n", Year);
                sb.AppendFormat("Cut:\t\t\t{0}\n", Cut.ToDisplayString());
                sb.AppendFormat("IMDB id:\t\t\t{0}\n", IMDBid);
                //MessageBox.Show("SHOW!\n" + Series + "\nS" + Season + "E" + Episode);
            }
            else if (mediaKind == EMP.MediaKind.Show)
            {
                sb.AppendFormat("Series:\t\t\t{0}\n", Series);
                sb.AppendFormat("Season:\t\t\t{0}\n", Season);
                sb.AppendFormat("Episode:\t\t\t{0}\n", Episode);
                sb.AppendFormat("Aired:\t\t\t{0}\n", AirDate);
                //MessageBox.Show("SHOW!\n" + Series + "\nS" + Season + "E" + Episode);
            }
            sb.AppendFormat("Quality:\t\t\t{0}\n", VideoQuality.ToDisplayString());
            sb.AppendFormat("Source:\t\t\t{0}\n", VideoSource.ToDisplayString());
            sb.AppendFormat("Container:\t\t{0}\n", Container.ToDisplayString());
            sb.AppendFormat("Video Codec:\t\t{0}\n", VideoCodec.ToDisplayString());
            sb.AppendFormat("Audio Codec:\t\t{0}\n", AudioCodec.ToDisplayString());
            sb.AppendFormat("\nOTHER:\n{0}\n", Other);
            sb.AppendFormat("Temp Title:\t\t{0}\n", TmpTitle);
            sb.AppendFormat("Temp Directory Title:\t{0}\n", TmpDirTitle);
            return sb.ToString();
        }

        public class StringLocation
        {
            public String String;
            public Int32 Start;
            public Int32 Length;
            public Boolean InCleanString;
            public Boolean InDirectoryName;
            public StringLocation(String _string, Int32 _start, Int32 _length, Boolean _inCleanString, Boolean _inDirectoryName = false)
            {
                String = _string;
                Start = _start;
                Length = _length;
                InCleanString = _inCleanString;
                InDirectoryName = _inDirectoryName;
            }
        }
    }
}
