using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NAudio;
using NAudio.Wave;
using System.Runtime.CompilerServices;

namespace TestAppLocalPLayer
{
	//public delegate void PropertyChangedEventHandler(object sender, EventArgs e);

    public class Player : IDisposable, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region Properties
		//Declarations required for audio out and the MP3 stream
        private IWavePlayer _waveOutDevice;
        public IWavePlayer WaveOutDevice
        {
            get { return _waveOutDevice; }
        }

        private WaveStream _mainOutputStream;
        private WaveChannel32 _volumeStream;

		private PlaybackState _playbackState;
        public PlaybackState PlaybackState
        {
            get 
			{ 
				return _playbackState;
			}
			set
			{
				if (_playbackState != value)
				{
					_playbackState = value;
					NotifyPropertyChanged();
					updatePlaypauseButton();
				}		
			}
        }

		private string _playpauseButtonText;
		public string PlaypauseButtonText
		{
			get
			{
				return _playpauseButtonText;
			}
			set
			{
				if (_playpauseButtonText != value)
				{
					_playpauseButtonText = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion

		public Player()
        {
            _waveOutDevice = new WaveOut( );
			updatePlaypauseButton();
        }

        public void Play(string musicPath)
        {
			if (musicPath == null)
			{
				return;
			}

			Reset();
            _mainOutputStream = CreateInputStream(musicPath);
            _waveOutDevice = new WaveOut();
            _waveOutDevice.Init(_mainOutputStream);
            _waveOutDevice.Play();

			PlaybackState = _waveOutDevice.PlaybackState;
        }

        public void Resume()
        {
            _waveOutDevice.Play();

			PlaybackState = _waveOutDevice.PlaybackState;
        }

        public void Pause()
        {
            _waveOutDevice.Pause();

			PlaybackState = _waveOutDevice.PlaybackState;
        }

        public void Stop()
        {
            _waveOutDevice.Stop();

			PlaybackState = _waveOutDevice.PlaybackState;
        }

        private WaveStream CreateInputStream(string fileName)
        {
            WaveChannel32 inputStream;
            if (fileName.EndsWith(".mp3"))
            {
                WaveStream mp3Reader = new Mp3FileReader(fileName);
                inputStream = new WaveChannel32(mp3Reader);
            }
            else
            {
                throw new InvalidOperationException("Unsupported extension");
            }
            _volumeStream = inputStream;
            _volumeStream.PadWithZeroes = false;
            return _volumeStream;
        }

		private void updatePlaypauseButton()
		{
			if (PlaybackState == NAudio.Wave.PlaybackState.Playing)
			{
				PlaypauseButtonText = "Pause";
			}
			else
			{
				PlaypauseButtonText = "Play";
			}
		}

        public void Reset()
        {
            if (_waveOutDevice != null)
            {
                _waveOutDevice.Stop();
            }
            if (_mainOutputStream != null)
            {
                // this one really closes the file and ACM conversion
                _volumeStream.Close();
                //_volumeStream = null;
                // this one does the metering stream
                _mainOutputStream.Close();
                //_mainOutputStream = null;
            }
        }

        public void Dispose()
        {
            if (_waveOutDevice != null)
            {
                _waveOutDevice.Stop();
            }
            if (_mainOutputStream != null)
            {
                // this one really closes the file and ACM conversion
                _volumeStream.Close();
                _volumeStream = null;
                // this one does the metering stream
                _mainOutputStream.Close();
                _mainOutputStream = null;
            }
            if (_waveOutDevice != null)
            {
                _waveOutDevice.Dispose();
                _waveOutDevice = null;
            }
        }
    }
}
