using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
		BackgroundWorker scanner = new BackgroundWorker();

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
			if (MainWindow.musicPath != null)
			{
				refreshTrackList();
			}
        }

		#region Build tracklist
		public void refreshTrackList()
		{
			_filePaths.Clear();

			scanner.DoWork += new DoWorkEventHandler(bw_DoWork);
			scanner.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
			scanner.RunWorkerAsync();
		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			e.Result = scan();
		}

        private List<string> scan()
        {
            string[] tmpFilePaths = Directory.GetFiles(MainWindow.musicPath, "*.*", SearchOption.AllDirectories);
			List<string> trackFilePaths = new List<string>();

            foreach (string filePath in tmpFilePaths)
            {
                if (formats.Contains(filePath.Substring(filePath.Length - 4, 4).ToLower()))
                {
                    trackFilePaths.Add(filePath);
                }
            }

			return trackFilePaths;
        }

		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			foreach (string filePath in e.Result as List<string>)
			{
				_filePaths.Add(filePath);
			}
			NumTracks = FilePaths.Count().ToString() + " entries found";
		}
		#endregion

		public void Dispose()
		{
			_filePaths = null;
			NumTracks = null;
		}
    }
}
