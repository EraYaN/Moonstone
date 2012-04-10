using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EMP
{
    class fileInfoParser
    {
        public string debugString;

        public string fileName;

        public DirectoryInfo fileDir;
        public string fileDirName;

        public string title = "Unknown";
        public string year = "Unknown";
        public string quality = "Unknown";
        public string source = "Unknown";
        public string filetype = "Unknown";
        public string encoding = "Unknown";

        public fileInfoParser(FileInfo fileInfo)
        {
            //Some obvious shit first
            fileName = fileInfo.Name;
            fileDir = fileInfo.Directory;
            fileDirName = fileDir.Name;

            parse(fileName);
            parse(fileDirName);

            debugString = "General:\n" + 
                "\nProcessed file:\t\t" + fileName +
                "\nIn directory:\t\t" + fileDirName + 
                "\n\nProperties:" + 
                "\nTitle:\t\t\t" + title +
                "\nQuality:\t\t" + quality +
                "\nSource:\t\t\t" + source +
                "\nFiletype:\t\t" + filetype +
                "\nEncoding:\t\t" + encoding;

        }

        private void parse(string inputString)
        {
            //First we split the string into an array of smaller, easier to handle substrings
            string[] split = inputString.Split(new Char[] { ' ', '.', '-', '_' });

            //Here we specify a couple of string arrays which could match certain substrings
            //If there is a match, it will give us some more info about the file's properties
            //For display, we use another array containing the pretty names for each property
            string[] qualities = new string[2] { "720p", "1080p" };
            string[] qualityNames = new string[2] { "720p HD", "1080p HD" };

            string[] sources = new string[3] { "brrip", "bluray", "dvdrip" };
            string[] sourceNames = new string[3] { "Blu-ray Rip", "Blu-ray Rip", "DVD Rip" };

            string[] filetypes = new string[3] { "mkv", "avi", "mp4" };
            string[] filetypeNames = new string[3] { "Matroschka Video (.mkv)", "Microsoft AVI (.avi)", "MPEG-4 (.mp4)" };

            string[] encodings = new string[4] { "x264", "h264", "xvid", "divx" };
            string[] encodingNames = new string[4] { "x264 Encoding", "H.264 Encoding", "Xvid Encoding", "DivX Encoding" };
            
            //This array is used to keep track of the location (index) at which certain info can be found
            //It will also be used to determine where the actual title of the movie ends and where the other crap like releasegroups starts
            //It is important to keep the indexes in the correct order! By default the indexes are in the same order as the above arrays, ending with the year index
            int[] indices = new int[5];

            //A little regex for recognizing the year
            Regex rgx = new Regex(@"\W(\d{4})\W");

            for (int i = 0; i < split.Count(); i++)
            {
                string s = split[i];
                if (Array.IndexOf(qualities, s.ToLower()) != -1 & indices[0] < 1)
                {
                    quality = qualityNames[Array.IndexOf(qualities, s.ToLower())];
                    indices[0] = i;
                    i = -1;
                }
                else if (Array.IndexOf(sources, s.ToLower()) != -1 & indices[1] < 1)
                {
                    source = sourceNames[Array.IndexOf(sources, s.ToLower())];
                    indices[1] = i;
                    i = -1;
                }
                else if (Array.IndexOf(filetypes, s.ToLower()) != -1 & indices[2] < 1)
                {
                    filetype = filetypeNames[Array.IndexOf(filetypes, s.ToLower())];
                    indices[2] = i;
                    i = -1;
                }
                else if (Array.IndexOf(encodings, s.ToLower()) != -1 & indices[3] < 1)
                {
                    encoding = encodingNames[Array.IndexOf(encodings, s.ToLower())];
                    indices[3] = i;
                    i = -1;
                }
                else if (rgx.IsMatch(s))
                {
                    for (int i2 = 0; i2 < rgx.Matches(inputString).Count;i2++)
                    {
                        Match match = rgx.Matches(inputString)[i2];
                        Console.WriteLine(match);
                    }
                }
            }
        }

        //Used for printing useful info stored in "debugString", can be called in another class by calling this constructor
        public override string ToString()
        {
            return "Debugging info:\n\n" + debugString + "\n";
        }
    }
}
