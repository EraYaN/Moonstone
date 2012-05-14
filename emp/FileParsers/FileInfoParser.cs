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
        public string debugString;

        public string fileName;

        public DirectoryInfo fileDir;
        public string fileDirName;

        public string title = "Unknown";
        public string titleFallback = "Unknown";
        public string year = "Unknown";
        public string quality = "Unknown";
        public string source = "Unknown";
        public string filetype = "Unknown";
        public string codec = "Unknown";
        public string audioCodec = "Unknown";
        public string cut = "Unknown";
        public string other = "None";

        int[] indices = new int[6];
        string[] processed = new string[20];
        int iP = 0; //index for processed array

        /// <summary>
        /// Constructor for movie and/or show data.
        /// </summary>
        /// <param name="fileInfo">The file to process.</param>
        public FileInfoParser(FileInfo fileInfo)
        {
            //Some obvious shit first
            fileName = fileInfo.Name;
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
                "\nOther info:\t\t" + other;

        }

        /// <summary>
        /// Takes a string (file name or directory name) and extracts movie or show data from it.
        /// </summary>
        /// <param name="inputString">The string to process.</param>
        /// <param name="type">The input type, 0 for filename, 1 for directory name</param>
        private void parse(string inputString, int type)
        {
            Array.Clear(indices, 0, indices.Length);
            Array.Clear(processed, 0, processed.Length);
            iP = 0;

            //Here we specify a couple of string arrays which could match certain substrings
            //If there is a match, it will give us some more info about the file's properties
            //For display, we use another array containing the pretty names for each property
            string[] qualities = new string[2] { "720p", "1080p" };
            string[] qualityNames = new string[2] { "720p HD", "1080p HD" };
            if (quality == "Unknown")
            {
                quality = check(inputString, qualities, qualityNames);
            }
            else
            {
                check(inputString, qualities, qualityNames);
            }

            string[] sources = new string[4] { "brrip", "bdrip", "bluray", "dvdrip" };
            string[] sourceNames = new string[4] { "Blu-ray Rip", "Blu-ray Rip", "Blu-ray Rip", "DVD Rip" };
            if (source == "Unknown")
            {
                source = check(inputString, sources, sourceNames);
            }
            else
            {
                check(inputString, sources, sourceNames);
            }

            string[] filetypes = new string[4] { "mkv", "avi", "mp4", "m4a" };
            string[] filetypeNames = new string[4] { "Matroska Video (.mkv)", "Microsoft AVI (.avi)", "MPEG-4 (.mp4)", "MPEG-4 (.m4a)" };
            if (filetype == "Unknown" & type == 0)
            {
                filetype = check(inputString, filetypes, filetypeNames);
            }
            else
            {
                check(inputString, filetypes, filetypeNames);
            }

            string[] codecs = new string[4] { "x264", "h264", "xvid", "divx" };
            string[] codecNames = new string[4] { "x264 Codec (x264)", "H.264 Codec (h264)", "Xvid Codec (xvid)", "DivX Codec (divx)" };
            if (codec == "Unknown")
            {
                codec = check(inputString, codecs, codecNames);
            }
            else
            {
                check(inputString, codecs, codecNames);
            }

            string[] audioCodecs = new string[4] { "dts", "aac", "ac3", "mp3" };
            string[] audioCodecNames = new string[4] { "DTS Audio Codec (DTS)", "Advanced Audio Coding (AAC)", "Dolby Digital (AC3)", "MPEG-2 Audio Layer III (MP3)" };
            if (audioCodec == "Unknown")
            {
                audioCodec = check(inputString, audioCodecs, audioCodecNames);
            }
            else
            {
                check(inputString, audioCodecs, audioCodecNames);
            }
/*
 * Not sure whether I want this
            string[] cuts = new string[4] { "dc", "extended", "theatrical", "directors" };
            string[] cutNames = new string[4] { "DTS Audio Codec (DTS)", "Advanced Audio Coding (AAC)", "Dolby Digital (AC3)", "MPEG-2 Audio Layer III (MP3)" };
            if (cut == "Unknown")
            {
                cut = check(inputString, cuts, cutNames);
            }
            else
            {
                check(inputString, cuts, cutNames);
            }
*/
            //A little regex for recognizing the year
            Regex rgx = new Regex(@"\b((19|20)\d{2})\b");

            if (rgx.IsMatch(inputString))
            {
                if (year == "Unknown")
                {
                    year = rgx.Matches(inputString)[rgx.Matches(inputString).Count - 1].ToString();
                }
                indices[4] = inputString.IndexOf(year);
                processed[iP] = year;
                iP++;
            }

            //Determine where the title ends
            int[] indicesSorted = indices; //Copy it
            Array.Sort(indicesSorted); //Sort it, adress 0 now contains the lowest value
            int separation = 0;

            for (int i = 0; i < indicesSorted.Count(); i++)
            {
                int n = indicesSorted[i];
                if (n > 0)
                {
                    separation = indicesSorted[i];
                    break;
                }
            }

            //Getting the title itself
            string titletmp;
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
            if(type == 1 & char.IsUpper(titleFallback[0]) & char.IsLower(title[0]))
            {
                string tmp = title;
                title = titleFallback;
                titleFallback = tmp;
            }


            //Extract any leftover shit into "other"
            if (other == "None")
            {
                other = inputString;
                foreach (string p in processed)
                {
                    if (p != null)
                    {
                        other = other.Replace(p, null);
                    }
                }
                string[] otherR = other.Split(new char[] { '.', ' ', '_', '(', ')' });
                other = "";
                foreach (string o in otherR)
                {
                    string s = o.Trim(new char[] { '-', ' ' });
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
        private string check(string input, string[] props, string[] propNames)
        {
            string prop = "Unknown";
                for (int i = 0; i < props.Count(); i++)
                {
                    string c = props[i];

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

        //Used for printing useful info stored in "debugString". Can be called in another class by calling this constructor
        public override string ToString()
        {
            return "Debugging info:\n\n" + debugString + "\n";
        }
    }
}
