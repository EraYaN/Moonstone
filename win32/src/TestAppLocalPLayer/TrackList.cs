using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace TestAppLocalPLayer
{
	/// <summary>
	/// Build a tracklist from files found in indicated directory
	/// </summary>
    public class TrackList : IDisposable, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

        #region properties
		private ObservableCollection<string> _filePaths = new ObservableCollection<string>();
		public ObservableCollection<string> FilePaths
        {
            get
            {
                return _filePaths;
            }
        }

		private string _numTracks;
		public string NumTracks
		{
			get
			{
				return _numTracks;
			}
			set
			{
				_numTracks = value;
				NotifyPropertyChanged();
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

			NumTracks = _filePaths.Count().ToString() + " entries found";
        }

		public void Dispose()
		{
			_filePaths = null;
		}
    }
}
