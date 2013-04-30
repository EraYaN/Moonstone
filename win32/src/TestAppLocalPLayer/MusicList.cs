using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestAppLocalPLayer
{
    public class MusicList
    {
        #region properties

        private List<string> filePaths = new List<string>(); 
        public List<string> FilePaths
        {
            get
            {
                return filePaths;
            }
        }

        #endregion

        #region supported formats
        string[] formats = { ".wav", ".mp3", ".aac", ".wma", ".aiff" };
        #endregion

        public MusicList()
        {
            Thread _scanner = new Thread(new ThreadStart(scan));
            _scanner.Name = "Scanner";
            _scanner.IsBackground = true;
            _scanner.Start();
            _scanner.Join();
        }

        private void scan()
        {
            string[] tmpFilePaths = Directory.GetFiles(MainWindow.musicPath, "*.*", SearchOption.AllDirectories);

            foreach (string filePath in tmpFilePaths)
            {
                if (formats.Contains(filePath.Substring(filePath.Length - 4, 4).ToLower()))
                {
                    filePaths.Add(filePath);
                }
            }
        }
    }
}
