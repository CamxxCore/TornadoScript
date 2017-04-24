using System;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using ScriptCore;

namespace TornadoScript.Script
{
    /// <summary>
    /// Extension to manage the spawning of tornadoes.
    /// </summary>
    public class Monitor : ScriptExtension
    {
        private const int VortexLimit = 30;

        private int _lastSpawnAttempt;

        public int ActiveVortexCount { get; private set; }

        private readonly Vortex[] _activeVortexList = new Vortex[VortexLimit];

        /// <summary>
        /// Create a vortex at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vortex CreateVortex(Vector3 position)
        {
            for (int i = _activeVortexList.Length - 1; i > 0; i--)
                _activeVortexList[i] = _activeVortexList[i - 1];

            position.Z = World.GetGroundHeight(position) - 10.0f;

            var tVortex = new Vortex(position);

            tVortex.Build();

            _activeVortexList[0] = tVortex;

            ActiveVortexCount = Math.Min(ActiveVortexCount + 1, _activeVortexList.Length);

            if (ScriptThread.GetVar<bool>("notifications"))
            {
                UI.Notify("Tornado spawned nearby.");
            }

            return tVortex;
        }

        public override void OnUpdate(int gameTime)
        {
            if (ActiveVortexCount > 0 && Game.Player.IsDead && Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT))
            {
                RemoveAll();
            }

            if (World.Weather == Weather.ThunderStorm && ScriptThread.GetVar<bool>("spawnInStorm"))
            {
                if (Game.GameTime - _lastSpawnAttempt > 1000)
                {
                    if (Probability.GetBoolean(0.005f))
                    {
                        Function.Call(Hash.SET_WIND_SPEED, 70.0f);

                        var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                        CreateVortex(position.Around(30f));
                    }

                    _lastSpawnAttempt = Game.GameTime;
                }
            }   

            base.OnUpdate(gameTime);
        }

        public void RemoveAll()
        {
            for (int i = 0; i < ActiveVortexCount; i++)
            {
                _activeVortexList[i].Dispose();

                _activeVortexList[i] = null;
            }

            ActiveVortexCount = 0;
        }

        public override void Dispose()
        {
            for (int i = 0; i < ActiveVortexCount; i++)
            {
                _activeVortexList[i].Dispose();
            }

            base.Dispose(); 
        }
    }
}
