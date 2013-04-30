using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Threading;
using SpotiFire;

namespace TestAppWPF
{
    class NAudioPlayerOld : IDisposable
    {
        WaveOut _waveOut;
        private BufferedWaveProvider _waveProvider;
        Thread _t;
        private delegate bool Test();
        public delegate void PlayerThreadMessageDelegate(object[] args);
        private static object _syncObj = new object();
        private static bool _initted = false;
        private static bool _shutDown = false;
        private static bool _complete = false;
        private static bool _isRunning = false;
        //private static AutoResetEvent _playerSignal;
        private static Queue<PlayerThreadMessage> _mq = new Queue<PlayerThreadMessage>();
        private static Queue<byte[]> _bq = new Queue<byte[]>();
        private class PlayerThreadMessage
        {
            public PlayerThreadMessageDelegate d;
            public object[] payload;
        }
        public void Init()
        {
            if (_t == null)
            {
                _t = new Thread(new ThreadStart(_playerThread));
                _t.Start();
            }
            postMessage(_initialize, null);
        }
        public void LoadTrack(IntPtr trackPtr)
        {
            postMessage(_loadTrack, new object[] { trackPtr });
        }
        public void Play()
        {
            postMessage(_play, null);
        }
        public void Pause()
        {
            postMessage(_pause, null);
        }
        public void Dispose()
        {
            lock (_syncObj)
            {
                _shutDown = true;
            }
            if(_t != null)
                _t.Join(1000);
        }
        #region Methods on other thread
        private void _loadTrack(object[] args)
        {
            _bq.Clear();
            if (_waveProvider.BufferedBytes > 0)
            {
                _waveProvider.ClearBuffer();
            }
            //libspotify.sp_error error = Session.LoadPlayer((IntPtr)args[0]);
        }
        private void _play(object[] args)
        {
            //Session.Play();
            _waveOut.Play();
        }
        private void _pause(object[] args)
        {
           // Session.Pause();
            _waveOut.Pause();
        }
        
        private void _initialize(object[] args)
        {
            if (!_initted)
            {
                _initted = true;
               // Session.OnAudioDataArrived += Session_OnAudioDataArrived;
               // Session.OnAudioStreamComplete += Session_OnAudioStreamComplete;
                WaveFormat format = new WaveFormat();
                _waveOut = new WaveOut();
                _waveProvider = new BufferedWaveProvider(format);
                _waveProvider.BufferDuration = TimeSpan.FromSeconds(20);
                _waveProvider.DiscardOnBufferOverflow = true;
                _waveOut.Init(_waveProvider);
                _waveOut.PlaybackStopped += _waveOut_PlaybackStopped;
            }
        }

        void _waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Log.Info("WaveOut Playback Stopped");
        }

        void _playerThread()
        {
            try
            {
                //_playerSignal = new AutoResetEvent(false);
                //int timeout = Timeout.Infinite;
                //DateTime lastEvents = DateTime.MinValue;
                //_isRunning = true;
                //_programSignal.Set(); // this signals to program thread that loop is running   
                while (true)
                {
                    lock (_syncObj)
                    {
                        if (_bq.Count > 0 && _waveProvider != null)
                        {
                            if ((_waveProvider.BufferLength - _waveProvider.BufferedBytes) > _bq.ElementAt(0).Length)
                            {
                                byte[] obj = _bq.Dequeue();
                                _waveProvider.AddSamples(obj, 0, obj.Length);
                            }
                            else
                            {
                                //Thread.Sleep(100);
                            }
                        }
                        else
                        {
                            //Thread.Sleep(50);
                        }
                        
                        if (_bq.Count == 0 && _complete)
                        {
                            _waveOut.Stop();
                        }
                        while (_mq.Count > 0)
                        {
                            PlayerThreadMessage m = _mq.Dequeue();
                            m.d.Invoke(m.payload);
                        }
                        if (_shutDown)
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("playerThread() unhandled exception: \n {0}", ex);
            }
            finally
            {
                lock (_syncObj)
                {
                    if (_waveProvider != null)
                    {
                        if (_waveProvider.BufferedBytes > 0)
                        {
                            _waveProvider.ClearBuffer();
                        }
                        _waveProvider = null;
                    }
                    if (_waveOut != null)
                    {
                        _waveOut.PlaybackStopped -= _waveOut_PlaybackStopped;
                        _waveOut.Stop();
                        _waveOut.Dispose();
                        _waveOut = null;
                    }
                    _isRunning = false;
                }
            }
        }
        void Session_OnAudioStreamComplete(object obj)
        {
            lock (_syncObj)
            {
                //Session.UnloadPlayer();
                Log.Info("AudioStreamCompleted");
            }
        }

        void Session_OnAudioDataArrived(byte[] obj)
        {
            lock (_syncObj)
            {
                if (!_complete)
                {
                    _bq.Enqueue(obj);
                }                
            }
        }
        #endregion
        private static void postMessage(PlayerThreadMessageDelegate d, object[] payload)
        {
            lock (_syncObj)
            {
                _mq.Enqueue(new PlayerThreadMessage() { d = d, payload = payload });
            }
        }

    }
}
