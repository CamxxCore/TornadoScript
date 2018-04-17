using GTA;
using GTA.Math;
using GTA.Native;

namespace TornadoScript.ScriptMain.Utility
{
    public class GameSound
    {
        private int _soundId;
        private readonly string _soundSet, _sound;

        public bool Active { get; private set; }

        public GameSound(string sound, string soundSet)
        {
            Active = false;
            _sound = sound;
            _soundSet = soundSet;
            _soundId = -1;
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
            Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, sound._soundSet, false);
        }

        public void Play(Entity ent)
        {
            _soundId = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, _soundId, _sound, ent.Handle, 0, 0, 0);
            Active = true;
        }

        public void Play(Vector3 position)
        {
            _soundId = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_COORD, _soundId, _sound, position.X, position.Y, position.Z, 0, 0, 0, 0);
            Active = true;
        }

        public void Destroy()
        {
            if (_soundId == -1) return;
            Function.Call(Hash.STOP_SOUND, _soundId);
            Function.Call(Hash.RELEASE_SOUND_ID, _soundId);
            _soundId = -1;
            Active = false;
        }
    }
}

