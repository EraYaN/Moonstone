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
        public string encoding = "Unknown";
        public string other = "None";

        public fileInfoParser(FileInfo fileInfo)
        {
            //Some obvious shit first
            fileName = fileInfo.Name;
            fileDir = fileInfo.Directory;
            fileDirName = fileDir.Name;

            parse(fileName);
            //parse(fileDirName);

            debugString = "General:\n" + 
                "\nProcessed file:\t\t" + fileName +
                "\nIn directory:\t\t" + fileDirName + 
                "\n\nProperties:" + 
                "\nTitle:\t\t\t" + title +
                "\nYear:\t\t\t" + year +
                "\nQuality:\t\t" + quality +
                "\nSource:\t\t\t" + source +
                "\nFiletype:\t\t" + filetype +
                "\nEncoding:\t\t" + encoding +
                "\nOther info:\t\t" + other;

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
            Regex rgx = new Regex(@"\d{4}");

            //Array to store possible parts of the title with an extra int to store the current index in the array
            string[] titleArray = new string[20];
            int iTitle = 0;

            string[] otherArray = new string[20];
            int iOther = 0;

            //Processing the input!
            for (int i = 0; i < split.Count(); i++)
            {
                string s = split[i];
                if (Array.IndexOf(qualities, s.ToLower()) != -1)
                {
                    if (indices[0] < 1)
                    {
                        quality = qualityNames[Array.IndexOf(qualities, s.ToLower())];
                        indices[0] = i;
                        i = -1;
                    }
                }
                else if (Array.IndexOf(sources, s.ToLower()) != -1)
                {
                    if (indices[1] < 1)
                    {
                        source = sourceNames[Array.IndexOf(sources, s.ToLower())];
                        indices[1] = i;
                        i = -1;
                    }
                }
                else if (Array.IndexOf(filetypes, s.ToLower()) != -1)
                {
                    if (indices[2] < 1)
                    {
                        filetype = filetypeNames[Array.IndexOf(filetypes, s.ToLower())];
                        indices[2] = i;
                        i = -1;
                    }
                }
                else if (Array.IndexOf(encodings, s.ToLower()) != -1)
                {
                    if (indices[3] < 1)
                    {
                        encoding = encodingNames[Array.IndexOf(encodings, s.ToLower())];
                        indices[3] = i;
                        i = -1;
                    }
                }
                else if (rgx.IsMatch(s))
                {
                    if (indices[4] < 1)
                    {
                        year = rgx.Matches(inputString)[rgx.Matches(inputString).Count - 1].ToString();
                        indices[4] = i;
                        i = -1;
                    }
                }
                else if (Array.IndexOf(titleArray, s) == -1)
                {
                    titleArray[iTitle] = s;
                    iTitle++;
                }
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
                        if (iOther == 0)
                        {
                            otherArray[iOther] = s;
                        }
                        else
                        {
                            otherArray[iOther] = " " + s;
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
