using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NAudio;
using NAudio.Wave;

namespace TestAppLocalPLayer
{
    public class Player : IDisposable
    {
        //Declarations required for audio out and the MP3 stream
        private IWavePlayer _waveOutDevice;
        public IWavePlayer WaveOutDevice
        {
            get { return _waveOutDevice; }
        }

        private WaveStream _mainOutputStream;
        private WaveChannel32 _volumeStream;

        public PlaybackState PlaybackState
        {
            get { return _waveOutDevice.PlaybackState; }
        }

        public Player()
        {
            _waveOutDevice = new WaveOut( );
        }

        public void Play(string musicPath)
        {
            _mainOutputStream = CreateInputStream(musicPath);
            _waveOutDevice = new WaveOut();
            _waveOutDevice.Init(_mainOutputStream);
            _waveOutDevice.Play();
        }

        public void Resume()
        {
            _waveOutDevice.Play();
        }

        public void Pause()
        {
            _waveOutDevice.Pause();
        }

        public void Stop()
        {
            _waveOutDevice.Stop();
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
