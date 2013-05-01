using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire;
using System.Threading;

using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace TestAppWPF
{
    class NAudioPlayer : IDisposable
    {
        private IWavePlayer waveOut;
        private BufferedWaveProvider buffer;
        private WaveFormat waveFormat;
        private Session session;
        public PlayQueue pq;
        private bool _playing = false;

        public Boolean IsPlaying
        {
            get
            {
                return _playing;
            }
        }

        public NAudioPlayer(Session _session)
        {
            session = _session;
            //waveOut = new WasapiOut(AudioClientShareMode.Shared, false, 20);
            //buffer = new BufferedWaveProvider(waveFormat);
            pq = new PlayQueue();
        }
        public void Init()
        {
            waveOut = new WasapiOut(AudioClientShareMode.Shared, false, 20);
            session.MusicDelivered += session_MusicDelivered;
            session.EndOfTrack += session_EndOfTrack;
        }

        void session_EndOfTrack(Session sender, SessionEventArgs e)
        {
            if (pq.Count > 0)
            {
                LoadTrack(pq.Dequeue());
            }
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
        }
        public void LoadTrack(Track track){
            try
            {
                session.PlayerUnload();
                session.PlayerLoad(track);
            }
            catch (Exception ex)
            {
                //
            }
        }
        public void Enqueue(Track track)
        {
            try
            {
                session.PlayerUnload();
                session.PlayerLoad(track);
            }
            catch (Exception ex)
            {
                //
            }
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
