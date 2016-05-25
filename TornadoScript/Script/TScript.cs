using System;
using GTA;
using GTA.Native;
using System.Windows.Forms;

namespace TornadoScript.Script
{
    public class TScript : GTA.Script
    {
        private TVortex tVortex;

        private int lastSpawnAttempt = 0;

        public TScript()
        {
            GameSound.Load("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");

            GameSound.Load("BASEJUMPS_SOUNDS");

            Tick += OnTick;
            KeyDown += KeyPressed;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (tVortex != null && Game.Player.IsDead && Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT))
            {
                tVortex.Dispose();
                tVortex = null;
                return;
            }

            tVortex?.Update();

            if (Config.SpawnInStorm && tVortex == null && World.Weather == Weather.ThunderStorm)
            {
                if (Game.GameTime - lastSpawnAttempt > 1000)
                {
                    if (new Random().Next(0, 1000) < 10)
                    {
                        Function.Call(Hash.SET_WIND_SPEED, 70.0f);

                        var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                        position = position.Around(30f);

                        tVortex = CreateVortex(position);
                    }

                    lastSpawnAttempt = Game.GameTime;
                }
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (!Config.EnableKeybinds) return;

            if (e.KeyCode == Config.ToggleScript)
            {
                if (tVortex == null)
                {
                    Function.Call(Hash.SET_WIND_SPEED, 70.0f);

                    var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                    tVortex = CreateVortex(position);

                    UI.Notify("Tornado spawned nearby.");
                }

                else
                {
                    tVortex.Dispose();

                    tVortex = null;

                    UI.Notify("Tornado despawned!");
                }
            }
        }

        private TVortex CreateVortex(GTA.Math.Vector3 position)
        {
            position.Z = World.GetGroundHeight(position) - 10f;

            var tVortex = new TVortex(position);

            tVortex.Build();

            return tVortex;
        }

        protected override void Dispose(bool A_0)
        {
            var pos = Game.Player.Character.Position;
            Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, pos.X, pos.Y, pos.Z, 10000.0f);

            tVortex?.Dispose();

            GameSound.Release("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");

            GameSound.Release("BASEJUMPS_SOUNDS");       

            base.Dispose(A_0);
        }
    }
}
