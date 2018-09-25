using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using TornadoScript.ScriptCore.Game;

namespace TornadoScript.ScriptMain.Utility
{
    public class WavePlayer
    {
        private bool soundWasPlaying = false;

        private float fadeStartVolume = 0.0f, fadeTarget = 0.0f;

        private float currentVolume = 0.0f;

        private int fadeTime = 0;

        private bool soundFadingIn = false, soundFadingOut = false;

        private LoopStream _waveStream;

        private SampleChannel _waveChannel;

        private WaveOutEvent _waveOut;

        public WavePlayer(string audioFilename)
        {
            _waveStream = new LoopStream(new WaveFileReader(audioFilename));

            _waveChannel = new SampleChannel(_waveStream);

            _waveOut = new WaveOutEvent();

            _waveOut.Init(_waveChannel);

            var soundManager = ScriptThread.GetOrCreate<SoundManager>();

            soundManager.Add(this);
        }

        public void SetVolume(float volumeLevel)
        {
         //   GTA.UI.ShowSubtitle(_waveOut.Volume.ToString());
            if (soundFadingIn || soundFadingOut)
                return;
            _waveChannel.Volume = volumeLevel;
        }

        public bool IsPlaying()
        {
            return _waveOut.PlaybackState == PlaybackState.Playing;
        }

        public bool IsPaused()
        {
            return _waveOut.PlaybackState == PlaybackState.Paused;
        }

        public void SetLoopAudio(bool shouldLoopAudio)
        {
            _waveStream.EnableLooping = shouldLoopAudio;
        }

        public void DoFadeIn(int fadeTime, float fadeTarget)
        {
            this.fadeTime = fadeTime;
            this.fadeTarget = fadeTarget;
            soundFadingOut = false;
            soundFadingIn = true;
            currentVolume = _waveChannel.Volume;
            _waveOut.Play();
        }


        public void DoFadeOut(int fadeTime, float fadeTarget)
        {
            this.fadeTime = fadeTime;
            this.fadeTarget = fadeTarget;
            soundFadingOut = true;
            soundFadingIn = false;
            currentVolume = _waveChannel.Volume;
            _waveOut.Play();
        }

        public void Pause()
        {
            _waveOut.Pause();
        }

        public void Stop()
        {
            _waveOut.Stop();
        }

        public void Play(bool fromStart = false)
        {
            if (fromStart)
                _waveStream.CurrentTime = System.TimeSpan.Zero;
                    
            _waveOut.Play();

            soundWasPlaying = true;
        }

        public void Update()
        {
            if (soundFadingIn)
            {
                if (currentVolume < fadeTarget)
                {
                    currentVolume += GTA.Game.LastFrameTime * (1000.0f / fadeTime);

                    currentVolume = currentVolume < 0.0f ? 0.0f : currentVolume > 1.0f ? 1.0f : currentVolume;

                    _waveChannel.Volume = currentVolume;
                }

                else
                    soundFadingIn = false;
            }

            else if (soundFadingOut)
            {
                if (currentVolume > fadeTarget)
                {
                    currentVolume -= GTA.Game.LastFrameTime * (1000.0f / fadeTime);

                    currentVolume = currentVolume < 0.0f ? 0.0f : currentVolume > 1.0f ? 1.0f : currentVolume;

                    _waveChannel.Volume = currentVolume;
                }

                else
                {
                    _waveOut.Stop();

                    soundFadingOut = false;
                }
            }
        }
    }
}
