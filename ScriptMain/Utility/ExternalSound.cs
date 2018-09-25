using System;
using System.Media;

namespace TornadoScript.ScriptMain.Utility
{
    public class ExternalSound : IDisposable
    {
        public SoundPlayer Player { get; }

        public ExternalSound(string filePath) :
            this(new SoundPlayer(filePath))
        { }

        public ExternalSound(System.IO.Stream stream) :
            this(new SoundPlayer(stream))
        { }

        private ExternalSound(SoundPlayer soundPlayer)
        {
            Player = soundPlayer;
            Player.Load();
        }

        public void Play()
        {
            if (Player.IsLoadCompleted)
                Player.Play();
        }

        public void Dispose()
        {
            if (Player != null)
            {
                Player.Stop();
                Player.Dispose();
            }
        }
    }
}

