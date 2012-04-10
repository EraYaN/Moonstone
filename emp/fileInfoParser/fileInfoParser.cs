using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
    class fileInfoParser
    {
        public string debugString;
        public string fileName;
        public DirectoryInfo fileDir;
        public string fileDirName;

        public fileInfoParser(FileInfo fileInfo)
        {
            //Some obvious shit first
            fileName = fileInfo.Name;
            fileDir = fileInfo.Directory;
            fileDirName = fileDir.Name;

            parse(fileName);
            parse(fileDirName);

            debugString = "Processed file:\t\t" + fileName +
                "\nIn directory:\t\t" + fileDirName;

        }

        private void parse(string inputString)
        {
            string[] split = inputString.Split(new Char[] { ' ', '.', '-', '_' });

            for (int i = 0; i < split.Count(); i++)
            {
                string s = split[i];
            }
        }

        //Used for printing useful info stored in "debugString", can be called in another class by calling this constructor
        public override string ToString()
        {
            return "Debugging info:\n\n" + debugString + "\n";
        }
    }
}
