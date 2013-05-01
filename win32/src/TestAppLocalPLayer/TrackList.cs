using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestAppLocalPLayer
{
	/// <summary>
	/// Build a tracklist from files found in indicated directory
	/// </summary>
    public class TrackList : IDisposable
    {

        #region properties
        private List<string> _filePaths = new List<string>(); 
        public List<string> FilePaths
        {
            get
            {
                return _filePaths;
            }
        }
        #endregion

        #region supported formats
        string[] formats = { ".wav", ".mp3", ".aac", ".wma", ".aiff" };
        #endregion

        public TrackList()
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
                    _filePaths.Add(filePath);
                }
            }
        }

		public void Dispose()
		{
			_filePaths = null;
		}
    }
}
