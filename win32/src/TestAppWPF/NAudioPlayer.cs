using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire;
using NLog;
using System.Threading;

using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestAppWPF
{
    class NAudioPlayer : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private IWavePlayer waveOut;
        private BufferedWaveProvider buffer;
        private WaveFormat waveFormat;
        private Session session;
        private Logger logger;
        public PlayQueue pq;
        private bool _playing = false;
        private Track _currentTrack;
        private object _lock = new object();

        public Boolean IsPlaying
        {
            get
            {
                return _playing;
            }
            set
            {
                if (value != _playing)
                {
                    _playing = value;
                    NotifyPropertyChanged();
                }
            }            
<<<<<<< HEAD
        }

        public Track CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            set
            {
                if (value != _currentTrack)
                {
                    _currentTrack = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NowPlaying");
                }
            }   
        }

        public String NowPlaying
        {
            get
            {
                if (_currentTrack != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_currentTrack.Name);
                    sb.Append(" - ");
                    int c = _currentTrack.Artists.Count();
                    sb.Append(_currentTrack.Artists[0].Name);
                    if (c > 1)
                    {
                        for (int i = 1; i < c; i++)
                        {
                            sb.Append(" & " + _currentTrack.Artists[i].Name);
                        }
                    }
                    return sb.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }            
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public NAudioPlayer(Session _session)
        {
            session = _session;
            //waveOut = new WasapiOut(AudioClientShareMode.Shared, false, 20);
            //buffer = new BufferedWaveProvider(waveFormat);
            pq = new PlayQueue();
            logger = LogManager.GetCurrentClassLogger();
            //pq.Shuffle = true;
        }
        public void Init()
        {
            waveOut = new WasapiOut(AudioClientShareMode.Shared, false, 20);
            session.MusicDelivered += session_MusicDelivered;
            session.EndOfTrack += session_EndOfTrack;
=======
        }

        public Track CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            set
            {
                if (value != _currentTrack)
                {
                    _currentTrack = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NowPlaying");
                }
            }   
        }

        public String NowPlaying
        {
            get
            {

                if (_currentTrack != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_currentTrack.Name);
                    sb.Append(" - ");
                    int c = _currentTrack.Artists.Count();
                    sb.Append(_currentTrack.Artists[0].Name);
                    if (c > 1)
                    {
                        for (int i = 1; i < c; i++)
                        {
                            sb.Append(" & " + _currentTrack.Artists[i].Name);
                        }
                    }
                    return sb.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }            

        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public NAudioPlayer(Session _session)
        {
            session = _session;
            //waveOut = new WasapiOut(AudioClientShareMode.Shared, false, 20);
            //buffer = new BufferedWaveProvider(waveFormat);
            pq = new PlayQueue();
            logger = LogManager.GetCurrentClassLogger();
            //pq.Shuffle = true;

        }
        public void Init()
        {

            waveOut = new WasapiOut(AudioClientShareMode.Shared, false, 20);
            session.MusicDelivered += session_MusicDelivered;
            session.EndOfTrack += session_EndOfTrack;

        }

        void session_EndOfTrack(Session sender, SessionEventArgs e)
        {
            //LoadTrack(pq.Dequeue());
            NextTrack();

        }

        void session_MusicDelivered(Session sender, MusicDeliveryEventArgs e)
        {
            if (buffer == null)
            {
                waveFormat = new WaveFormat(e.Rate, 16, e.Channels);
                buffer = new BufferedWaveProvider(waveFormat);
                buffer.BufferDuration = TimeSpan.FromSeconds(0.5);
                waveOut.Init(buffer);
                waveOut.Play();
            }
            if (buffer.BufferLength - buffer.BufferedBytes > e.Samples.Length)
            {
                buffer.AddSamples(e.Samples, 0, e.Samples.Length);
                e.ConsumedFrames = e.Frames;
            }
            else
            {
                e.ConsumedFrames = 0;
            }
            if (waveOut.PlaybackState != PlaybackState.Playing)
                waveOut.Play();
>>>>>>> d5e52e99d1e6083cdaab47bc13c27bb14c3a2e85
        }
        public void LoadTrack(Track track)
        {

            _playing = false;
            CurrentTrack = track;
            session.PlayerUnload();
            session.PlayerLoad(track);
            logger.Debug("Loaded track: " + track.Name);

<<<<<<< HEAD
        void session_EndOfTrack(Session sender, SessionEventArgs e)
        {
            //LoadTrack(pq.Dequeue());
            NextTrack();

        }

        void session_MusicDelivered(Session sender, MusicDeliveryEventArgs e)
        {
            if (buffer == null)
            {
                waveFormat = new WaveFormat(e.Rate, 16, e.Channels);
                buffer = new BufferedWaveProvider(waveFormat);
                buffer.BufferDuration = TimeSpan.FromSeconds(0.5);
                waveOut.Init(buffer);
                waveOut.Play();
            }
            if (buffer.BufferLength - buffer.BufferedBytes > e.Samples.Length)
            {
                buffer.AddSamples(e.Samples, 0, e.Samples.Length);
                e.ConsumedFrames = e.Frames;
            }
            else
            {
                e.ConsumedFrames = 0;
=======
        }
        public void Enqueue(Track track)
        {
            pq.Enqueue(track);
            logger.Debug("Enqueued track: "+track.Name);
        }
        public async void NextTrack()
        {
            if (pq.Count>0)
            {
                LoadTrack(await pq.Dequeue());
                if (!_playing)
                {
                    Play();
                }
                logger.Debug("Next track: " + _currentTrack.Name);
            }
            else
            {
                Pause();
>>>>>>> d5e52e99d1e6083cdaab47bc13c27bb14c3a2e85
            }
            if (waveOut.PlaybackState != PlaybackState.Playing)
                waveOut.Play();
        }
<<<<<<< HEAD
        public void LoadTrack(Track track)
        {
            _playing = false;
            CurrentTrack = track;
            session.PlayerUnload();
            session.PlayerLoad(track);
            logger.Debug("Loaded track: " + track.Name);
        }
        public void Enqueue(Track track)
        {
            pq.Enqueue(track);
            logger.Debug("Enqueued track: "+track.Name);
        }
        public async void NextTrack()
        {
            if (pq.Count>0)
            {
                LoadTrack(await pq.Dequeue());
                if (!_playing)
                {
                    Play();
                }
                logger.Debug("Next track: " + _currentTrack.Name);
            }
            else
            {
                Pause();
            }
        }
        public void SetSeed(IEnumerable<Track> tracks)
        {
            pq.Seed = tracks;
        }
        public void Play()
        {
            session.PlayerPlay();
            _playing = true;
        }
        public void Pause()
        {
            session.PlayerPause();
            _playing = false;
        }
        public void Dispose()
        {
            session.PlayerPause();
            _playing = false;
=======
        public void SetSeed(IEnumerable<Track> tracks)
        {
            pq.Seed = tracks;
        }
        public void Play()
        {
            session.PlayerPlay();
            _playing = true;
        }
        public void Pause()
        {
            session.PlayerPause();
            _playing = false;
        }
        public void Dispose()
        {
            session.PlayerPause();
            _playing = false;
>>>>>>> d5e52e99d1e6083cdaab47bc13c27bb14c3a2e85
            session.PlayerUnload();
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }
            if (buffer != null)
            {
                buffer = null;
            }
        }
    }
}
