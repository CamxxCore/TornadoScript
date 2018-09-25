using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.IO;
using TornadoScript.ScriptCore;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Script
{
    /// <summary>
    /// Extension to manage the spawning of tornadoes.
    /// </summary>
    public class TornadoFactory : ScriptExtension
    {
        private WavePlayer _tornadoWarningSiren;

        private WavePlayer _tornadoLowRumble;

        private const int VortexLimit = 30;

        private const int TornadoSpawnDelayBase = 20000;

        private int _spawnDelayAdditive = 0;

        private int _spawnDelayStartTime = 0;

        private int _lastSpawnAttempt;

        public int ActiveVortexCount { get; private set; }

        private bool soundEnabled = true, sirenEnabled = true;

        private readonly TornadoVortex[] _activeVortexList = new TornadoVortex[VortexLimit];

        public TornadoVortex[] ActiveVortexList => _activeVortexList;

        /// <summary>
        /// Whether we are in the process of spawning a tornado.
        /// If this is true, the script will prepare to spawn a tornado based on the
        /// set parameters.
        /// </summary>
        private bool spawnInProgress = false;

        private bool delaySpawn = false;

        public TornadoFactory()
        {
            InitSounds();
        }

        private void InitSounds()
        {
            soundEnabled = ScriptThread.GetVar<bool>("soundenabled");

            if (!soundEnabled) return;

            sirenEnabled = ScriptThread.GetVar<bool>("sirenenabled");

            SoundLoad("tornado-weather-alert.wav", ref _tornadoWarningSiren, true);

            SoundLoad("rumble-bass-2.wav", ref _tornadoLowRumble, true);
        }

        /// <summary>
        /// Load a sound by name from the working directory of the program
        /// </summary>
        /// <param name="soundName"></param>
        private void SoundLoad(string soundName, ref WavePlayer loadedSound, bool looping = false)
        {
            string absolutePath =
               AppDomain.CurrentDomain.BaseDirectory + "\\TornadoScript\\sounds\\" + soundName;

            if (File.Exists(absolutePath))
            {
                loadedSound = new WavePlayer(absolutePath);

                loadedSound.SetLoopAudio(looping);
            }

            else
                Logger.Log("Could not load audio file '{0}'. Expected path: '{1}'", soundName, absolutePath);
        }

        /// <summary>
        /// Create a vortex at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public TornadoVortex CreateVortex(Vector3 position)
        {
            if (spawnInProgress) // tornado already spawning in
                return null;

            for (var i = _activeVortexList.Length - 1; i > 0; i--)
                _activeVortexList[i] = _activeVortexList[i - 1];

            position.Z = World.GetGroundHeight(position) - 10.0f;

            var tVortex = new TornadoVortex(position, false);

            tVortex.Build();

            _activeVortexList[0] = tVortex;

            ActiveVortexCount = Math.Min(ActiveVortexCount + 1, _activeVortexList.Length);

            if (soundEnabled)
            {
                if (_tornadoLowRumble != null)
                {
                    _tornadoLowRumble.SetVolume(0.0f);

                    var volumeLevel = 1.0f - (1.0f / 300.0f * Vector3.Distance2D(position, GameplayCamera.Position));

                    volumeLevel = volumeLevel < 0.0f ? 0.0f : volumeLevel > 1.0f ? 1.0f : volumeLevel;

                    _tornadoLowRumble.DoFadeIn(5000, volumeLevel);
                }
            }

            if (ScriptThread.GetVar<bool>("notifications"))
            {
                UI.Notify("Tornado spawned nearby.");
            }

            spawnInProgress = true;

            return null;
        }

        public override void OnUpdate(int gameTime)
        {
            //UI.ShowSubtitle("vcount: " + ActiveVortexCount + " spawning: " + spawnInProgress + " delay spawn: " + delaySpawn);

            if (ActiveVortexCount < 1)
            {
                if (World.Weather == Weather.ThunderStorm && ScriptThread.GetVar<bool>("spawnInStorm"))
                {
                    if (!spawnInProgress && Game.GameTime - _lastSpawnAttempt > 1000)
                    {
                        if (Probability.GetBoolean(0.05f))
                        {
                            _spawnDelayStartTime = Game.GameTime;

                            _spawnDelayAdditive = Probability.GetInteger(0, 40);

                            Function.Call(Hash.SET_WIND_SPEED, 70.0f); // add suspense :p

                            if (soundEnabled && sirenEnabled && _tornadoWarningSiren != null)
                            {
                                _tornadoWarningSiren.SetVolume(0.6f);

                                _tornadoWarningSiren.Play(true);
                            }

                            if (ScriptThread.GetVar<bool>("notifications"))
                            {
                                Helpers.NotifyWithIcon("Severe Weather Alert", "Tornado Warning issued for Los Santos and Blaine County", "char_milsite");
                            }

                            spawnInProgress = true;
                            delaySpawn = true;
                        }

                        _lastSpawnAttempt = Game.GameTime;
                    }
                }

                else
                {
                    delaySpawn = false;
                }

                if (delaySpawn)
                {
                    // UI.ShowSubtitle("current: " + (Game.GameTime - _spawnDelayStartTime) + " target: " + (TornadoSpawnDelayBase + _spawnDelayAdditive));

                    if (Game.GameTime - _spawnDelayStartTime > (TornadoSpawnDelayBase + _spawnDelayAdditive))
                    {
                        spawnInProgress = false;
                        delaySpawn = false;

                        var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                        CreateVortex(position.Around(150.0f).Around(175.0f));
                    }
                }
            }

            else
            {
                if (_activeVortexList[0].DespawnRequested || Game.Player.IsDead && Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT))
                {
                    RemoveAll();                  
                }

                else if (soundEnabled)
                {
                    if (_tornadoLowRumble != null)
                    {
                        var distance = Vector3.Distance2D(_activeVortexList[0].Position, GameplayCamera.Position); //attenuation factor

                        var volumeLevel = 1.0f - 1.0f / 800.0f * distance;

                        if (distance < 170.0f)
                            volumeLevel += 0.087f * (2.219f * volumeLevel);

                        volumeLevel = volumeLevel < 0.0f ? 0.0f : volumeLevel > 1.0f ? 1.0f : volumeLevel;             
                   
                        _tornadoLowRumble.SetVolume(volumeLevel);
                    }
                }
            }

            base.OnUpdate(gameTime);
        }

        public void RemoveAll()
        {
            spawnInProgress = false;

            if (_tornadoWarningSiren != null && _tornadoWarningSiren.IsPlaying())
                _tornadoWarningSiren.DoFadeOut(3000, 0.0f);

            if (_tornadoLowRumble != null && _tornadoLowRumble.IsPlaying())
                _tornadoLowRumble.DoFadeOut(3000, 0.0f);

            for (var i = 0; i < ActiveVortexCount; i++)
            {
                _activeVortexList[i].Dispose();

                _activeVortexList[i] = null;
            }


            ActiveVortexCount = 0;
        }

        public override void Dispose()
        {
            for (var i = 0; i < ActiveVortexCount; i++)
            {
                _activeVortexList[i].Dispose();
            }

            base.Dispose(); 
        }
    }
}
