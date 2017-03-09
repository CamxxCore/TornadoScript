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
    public class TMonitor : ScriptExtension
    {
        const int VortexLimit = 30;

        private int lastSpawnAttempt = 0;

        private int activeVortexCount = 0;

        public int ActiveVortexCount {  get { return activeVortexCount; } }

        private TVortex[] activeVortexList = new TVortex[VortexLimit];

        private readonly Ped Player = Game.Player.Character;

        /// <summary>
        /// Create a vortex at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public TVortex CreateVortex(Vector3 position)
        {
      //w      World.GetAllEntities().Select(x => { x.Delete(); return true; });

            for (int i = activeVortexList.Length - 1; i > 0; i--)
                activeVortexList[i] = activeVortexList[i - 1];

            position.Z = World.GetGroundHeight(position);

            var tVortex = new TVortex(position);

            tVortex.Build();

            activeVortexList[0] = tVortex;

            activeVortexCount = Math.Min(activeVortexCount + 1, activeVortexList.Length);

            if (ScriptThread.GetVar<bool>("notifications"))
            {
                UI.Notify("Tornado spawned nearby.");
            }

            return tVortex;
        }

        public override void OnUpdate(int gameTime)
        {
            if (activeVortexCount > 0 && Game.Player.IsDead && Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT))
            {
                RemoveAll();
            }

            if (World.Weather == Weather.ThunderStorm && ScriptThread.GetVar<bool>("spawnInStorm"))
            {
                if (Game.GameTime - lastSpawnAttempt > 1000)
                {
                    if (Probability.GetBoolean(0.005f))
                    {
                        Function.Call(Hash.SET_WIND_SPEED, 70.0f);

                        var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                        CreateVortex(position.Around(30f));
                    }

                    lastSpawnAttempt = Game.GameTime;
                }
            }   

            base.OnUpdate(gameTime);
        }

        public void RemoveAll()
        {
            for (int i = 0; i < activeVortexCount; i++)
            {
                activeVortexList[i].Dispose();

                activeVortexList[i] = null;
            }

            activeVortexCount = 0;
        }

        public override void Dispose()
        {
            for (int i = 0; i < activeVortexCount; i++)
            {
                activeVortexList[i].Dispose();
            }

            base.Dispose(); 
        }
    }
}
