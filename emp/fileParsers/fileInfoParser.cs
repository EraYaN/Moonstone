using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EMP
{
    public class fileInfoParser
    {
        //Init
        public string debugString;

        public string fileName;

        public DirectoryInfo fileDir;
        public string fileDirName;

        public string title = "Unknown";
        public string year = "Unknown";
        public string quality = "Unknown";
        public string source = "Unknown";
        public string filetype = "Unknown";
        public string codec = "Unknown";
        public string audioCodec = "Unknown";
        public string other = "None";

        /// <summary>
        /// Constructor for movie and/or show data.
        /// </summary>
        /// <param name="fileInfo">The file to process.</param>
        public fileInfoParser(FileInfo fileInfo)
        {
            //Some obvious shit first
            fileName = fileInfo.Name;
            fileDir = fileInfo.Directory;
            fileDirName = fileDir.Name;

            parse(fileName);
            //parse(fileDirName);

            debugString = "GENERAL:" +
                "\nProcessed file:\t\t" + fileName +
                "\nIn directory:\t\t" + fileDirName +
                "\n\nPROPERTIES:" +
                "\nTitle:\t\t\t" + title +
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
        private void parse(string inputString)
        {
            //First we split the string into an array of smaller, easier to handle substrings
            string[] split = inputString.Split(new Char[] { ' ', '.', '_' });

            //Here we specify a couple of string arrays which could match certain substrings
            //If there is a match, it will give us some more info about the file's properties
            //For display, we use another array containing the pretty names for each property
            string[] qualities = new string[2] { "720p", "1080p" };
            string[] qualityNames = new string[2] { "720p HD", "1080p HD" };

            string[] sources = new string[4] { "brrip", "bdrip", "bluray", "dvdrip" };
            string[] sourceNames = new string[4] { "Blu-ray Rip", "Blu-ray Rip", "Blu-ray Rip", "DVD Rip" };

            string[] filetypes = new string[4] { "mkv", "avi", "mp4", "m4a" };
            string[] filetypeNames = new string[4] { "Matroska Video (.mkv)", "Microsoft AVI (.avi)", "MPEG-4 (.mp4)", "MPEG-4 (.m4a)" };

            string[] codecs = new string[4] { "x264", "h264", "xvid", "divx" };
            string[] codecNames = new string[4] { "x264 Codec (x264)", "H.264 Codec (h264)", "Xvid Codec (xvid)", "DivX Codec (divx)" };

            string[] audioCodecs = new string[4] { "dts", "aac", "ac3", "mp3" };
            string[] audioCodecNames = new string[4] { "DTS Audio Codec (DTS)", "Advanced Audio Coding (AAC)", "Dolby Digital (AC3)", "MPEG-2 Audio Layer III (MP3)" };

            //This array is used to keep track of the location (index) at which certain info can be found
            //It will also be used to determine where the actual title of the movie ends and where the other crap like releasegroups starts
            //It is important to keep the indexes in the correct order! By default the indexes are in the same order as the above arrays, with the "year" at index 4
            int[] indices = new int[6];

            //A little regex for recognizing the year
            Regex rgx = new Regex(@"\b((19|20)\d{2})\b");

            //Array to store possible parts of the title with an extra integer to store the current index in the array
            string[] titleArray = new string[20];
            int iTitle = 0;

            string[] otherArray = new string[5];
            int iOther = 0;
            int iOtherRev = otherArray.Count() - 1; //Reverse index counter for storing values at the end

            //Processing dat shit.
            for (int i = 0; i < qualities.Count(); i++)
            {
                string c = qualities[i];

                if (inputString.ToLower().Contains(c))
                {
                    quality = qualityNames[i];
                    indices[0] = inputString.IndexOf(c);
                }
            }

            for (int i = 0; i < sources.Count(); i++)
            {
                string c = sources[i];

                if (inputString.ToLower().Contains(c))
                {
                    source = sourceNames[i];
                    indices[1] = inputString.IndexOf(c);
                }
            }

            for (int i = 0; i < filetypes.Count(); i++)
            {
                string c = filetypes[i];

                if (inputString.ToLower().Contains(c))
                {
                    filetype = filetypeNames[i];
                    indices[2] = inputString.IndexOf(c);

                }
            }

            for (int i = 0; i < codecs.Count(); i++)
            {
                string c = codecs[i];

                if (inputString.ToLower().Contains(c))
                {
                    codec = codecNames[i];
                    indices[3] = inputString.IndexOf(c);
                }
            }

            for (int i = 0; i < audioCodecs.Count(); i++)
            {
                string c = audioCodecs[i];

                if (inputString.ToLower().Contains(c))
                {
                    audioCodec = audioCodecNames[i];
                    indices[5] = inputString.IndexOf(c);
                }
            }

            if (rgx.IsMatch(inputString))
            {
                year = rgx.Matches(inputString)[rgx.Matches(inputString).Count - 1].ToString();
                indices[4] = inputString.IndexOf(year);
            }

            //The string is now processed, although we do not have a title yet
            //Following code is to remove all shit from the "titleArray" and, in normal circumstances, only keep the title itself
            //To determine where the title ends, we check where the other info starts
            //When this is done, we can use the index whereat the other info starts as a separator to cut off any leftover strings apart from the title

            //If no title could be found, we will assume the title is actually represented by a year (ie the movie "2012", which was released in 2009)
            if (titleArray[0] == null)
            {
                title = year;
                year = "Unknown";
            }
            else
            {
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

                for (int i = 0; i < titleArray.Count(); i++)
                {
                    string s = titleArray[i];
                    if (Array.IndexOf(split, s) > separation)
                    {
                        if (iOtherRev != otherArray.Count())
                        {
                            otherArray[iOther] = s + " ";
                        }
                        else
                        {
                            if (iOther == 0)
                            {
                                otherArray[iOther] = s;
                            }
                            else
                            {
                                otherArray[iOther] = " " + s;
                            }
                        }
                        iOther++;
                        titleArray[i] = "";
                    }
                    else if (i > 0)
                    {
                        titleArray[i] = " " + s;
                    }
                }
            }
            title = string.Concat(titleArray);
            other = string.Concat(otherArray);
        }

        //Used for printing useful info stored in "debugString", can be called in another class by calling this constructor
        public override string ToString()
        {
            return "Debugging info:\n\n" + debugString + "\n";
        }
    }
}
