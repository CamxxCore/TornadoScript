using System.Windows.Forms;
using GTA;
using GTA.Native;
using ScriptCore;
using TornadoScript.Memory;
using TornadoScript.Config;
using TornadoScript.Commands;

namespace TornadoScript.Script
{
    public class TScript : ScriptThread
    {
        private readonly Monitor _tMonitor;

        public TScript()
        {
            SetupAssets();
            AddVars();
            _tMonitor = GetOrCreate<Monitor>();
            GetOrCreate<CommandManager>();
            KeyDown += KeyPressed;
        }

        private void SetupAssets()
        {
            MemoryAccess.PatchPtfx();
            GameSound.Load("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            GameSound.Load("BASEJUMPS_SOUNDS");
        }

        private static void AddVars()
        {
            RegisterVar("enableconsole", false);
            RegisterVar("toggleconsole", Keys.T);
            RegisterVar("togglescript", IniHelper.GetValue("KeyBinds", "ToggleScript", Keys.F6));
            RegisterVar("enablekeybinds", IniHelper.GetValue("KeyBinds", "KeybindsEnabled", true));
            RegisterVar("notifications", IniHelper.GetValue("Other", "Notifications", true));
            RegisterVar("spawninstorm", IniHelper.GetValue("Other", "SpawnInStorm", true));   
            RegisterVar("vortexMovementEnabled", IniHelper.GetValue("Vortex", "MovementEnabled", true));
            RegisterVar("vortexMoveSpeedScale", IniHelper.GetValue("Vortex", "MoveSpeedScale", 1.0f));
            RegisterVar("vortexTopEntitySpeed", IniHelper.GetValue("Vortex", "MaxEntitySpeed", 40.0f));
            RegisterVar("vortexMaxEntityDist", IniHelper.GetValue("Vortex", "MaxEntityDistance", 54.0f));
            RegisterVar("vortexHorizontalPullForce", IniHelper.GetValue("Vortex", "HorizontalForceScale", 1.7f));
            RegisterVar("vortexVerticalPullForce", IniHelper.GetValue("Vortex", "VerticalForceScale", 2.29f));
            RegisterVar("vortexReverseRotation", IniHelper.GetValue("Vortex", "ReverseRotation", false));
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (!GetVar<bool>("enablekeybinds")) return;

            if (e.KeyCode == Keys.L)
            {
                var p = Game.Player.Character.Position;
                //   int soundId = Function.Call<int>(Hash.GET_SOUND_ID);
                Function.Call(Hash.PLAY_SOUND_FROM_COORD, -1, "CR_WEAPONS_BURST_SHORT", p.X, p.Y, p.Z, 0, 0, 0, 0);
            }

            if (e.KeyCode != GetVar<Keys>("togglescript")) return;

            if (_tMonitor.ActiveVortexCount > 0)
            {
                _tMonitor.RemoveAll();
            }

            else
            {
                Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, 0, 0, 0, 100000.0f);

                Function.Call(Hash.SET_WIND, 70.0f);

                var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                _tMonitor.CreateVortex(position);
            }
        }

        private static void ReleaseAssets()
        {
            GameSound.Release("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            GameSound.Release("BASEJUMPS_SOUNDS");
        }

        protected override void Dispose(bool a0)
        {
            Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, 0f, 0f, 0f, 1000000.0f);
            ReleaseAssets();
            base.Dispose(a0);
        }
    }
}
