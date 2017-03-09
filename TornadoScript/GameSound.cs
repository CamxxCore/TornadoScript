using GTA;
using GTA.Native;
using GTA.Math;

namespace TornadoScript
{
    public class GameSound
    {
        private string soundSet;
        private string sound;
        private int soundID;

        public bool Active { get; private set; }

        public GameSound(string sound, string soundSet)
        {
            this.Active = false;
            this.sound = sound;
            this.soundSet = soundSet;
            this.soundID = -1;
        }

        public static void Load(string audioBank)
        {
            Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, audioBank, false);
        }

        public static void Release(string audioBank)
        {
            Function.Call(Hash.RELEASE_NAMED_SCRIPT_AUDIO_BANK, audioBank);
        }

        public static void Load(GameSound sound)
        {
            Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, sound.soundSet, false);
        }

        public void Play(Entity ent)
        {
            soundID = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, soundID, sound, ent.Handle, 0, 0, 0);
            Active = true;
        }

        public void Play(Vector3 position)
        {
            soundID = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_COORD, soundID, sound, position.X, position.Y, position.Z, 0, 0, 0, 0);
            Active = true;
        }

        public void Destroy()
        {
            if (soundID == -1) return;
            Function.Call(Hash.STOP_SOUND, soundID);
            Function.Call(Hash.RELEASE_SOUND_ID, soundID);
            soundID = -1;
            Active = false;
        }
    }
}

