using System.Windows.Forms;
using GTA;
using GTA.Native;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Commands;
using TornadoScript.ScriptMain.Config;
using TornadoScript.ScriptMain.Utility;
using TornadoScript.ScriptMain.Memory;

namespace TornadoScript.ScriptMain.Script
{
    public class MainScript : ScriptThread
    {
        private readonly TornadoFactory _factory;

        public MainScript()
        {
            RegisterVars();
            SetupAssets();    
            _factory = GetOrCreate<TornadoFactory>();
            GetOrCreate<CommandManager>();           
            KeyDown += KeyPressed;
        }

        private static void SetupAssets()
        {
            if (GetVar<bool>("vortexParticleMod"))
            {
                MemoryAccess.Initialize();
                MemoryAccess.PatchPtfx();
            }

            GameSound.Load("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            GameSound.Load("BASEJUMPS_SOUNDS");
        }

        private static void RegisterVars()
        {
            RegisterVar("toggleconsole", Keys.T, true);
            RegisterVar("enableconsole", IniHelper.GetValue("Other", "EnableConsole", false));
            RegisterVar("notifications", IniHelper.GetValue("Other", "Notifications", true));
            RegisterVar("spawninstorm", IniHelper.GetValue("Other", "SpawnInStorm", true));      
            RegisterVar("togglescript", IniHelper.GetValue("KeyBinds", "ToggleScript", Keys.F6), true);
            RegisterVar("enablekeybinds", IniHelper.GetValue("KeyBinds", "KeybindsEnabled", true));
            RegisterVar("multiVortex", IniHelper.GetValue("VortexAdvanced", "MultiVortexEnabled", true));
            RegisterVar("vortexMovementEnabled", IniHelper.GetValue("Vortex", "MovementEnabled", true));
            RegisterVar("vortexMoveSpeedScale", IniHelper.GetValue("Vortex", "MoveSpeedScale", 1.0f));
            RegisterVar("vortexTopEntitySpeed", IniHelper.GetValue("Vortex", "MaxEntitySpeed", 40.0f));
            RegisterVar("vortexMaxEntityDist", IniHelper.GetValue("Vortex", "MaxEntityDistance", 57.0f));
            RegisterVar("vortexHorizontalPullForce", IniHelper.GetValue("Vortex", "HorizontalForceScale", 1.7f));
            RegisterVar("vortexVerticalPullForce", IniHelper.GetValue("Vortex", "VerticalForceScale", 2.29f));
            RegisterVar("vortexRotationSpeed", IniHelper.GetValue("Vortex", "RotationSpeed", 2.4f));
            RegisterVar("vortexRadius", IniHelper.GetValue("Vortex", "VortexRadius", 9.40f));
            RegisterVar("vortexReverseRotation", IniHelper.GetValue("Vortex", "ReverseRotation", false));
            RegisterVar("vortexMaxParticleLayers", IniHelper.GetValue("VortexAdvanced", "MaxParticleLayers", 47));
            RegisterVar("vortexParticleCount", IniHelper.GetValue("VortexAdvanced", "ParticlesPerLayer", 9));
            RegisterVar("vortexLayerSeperationScale", IniHelper.GetValue("VortexAdvanced", "LayerSeperationAmount", 22.0f));
            RegisterVar("vortexParticleName", IniHelper.GetValue("VortexAdvanced", "ParticleName", "ent_amb_smoke_foundry"));
            RegisterVar("vortexParticleAsset", IniHelper.GetValue("VortexAdvanced", "ParticleAsset", "core"));
            RegisterVar("vortexParticleMod", IniHelper.GetValue("VortexAdvanced", "ParticleMod", true));
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (!GetVar<bool>("enablekeybinds")) return;

            if (e.KeyCode != GetVar<Keys>("togglescript")) return;

            if (_factory.ActiveVortexCount > 0 && !GetVar<bool>("multiVortex"))
            {
                _factory.RemoveAll();
            }

            else
            {
                Function.Call(Hash.SET_WIND, 70.0f);

                var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                _factory.CreateVortex(position);
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
