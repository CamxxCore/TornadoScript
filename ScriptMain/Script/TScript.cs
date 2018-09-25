<<<<<<< HEAD
﻿using GTA;
using GTA.Native;
using System.Windows.Forms;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Commands;
using TornadoScript.ScriptMain.Config;
using TornadoScript.ScriptMain.Memory;
using TornadoScript.ScriptMain.Utility;
=======
﻿using System.Windows.Forms;
using GTA;
using GTA.Native;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Commands;
using TornadoScript.ScriptMain.Config;
using TornadoScript.ScriptMain.Utility;
using TornadoScript.ScriptMain.Memory;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

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
<<<<<<< HEAD
            MemoryAccess.Initialize();

            if (GetVar<bool>("vortexParticleMod"))
            {
                Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, "core"); // asset must be loaded before we can access its internal handle
                MemoryAccess.SetPtfxColor("core", "ent_amb_smoke_foundry", 1, System.Drawing.Color.Black);
                MemoryAccess.SetPtfxColor("core", "ent_amb_smoke_foundry", 2, System.Drawing.Color.Black);
            }
=======
            if (GetVar<bool>("vortexParticleMod"))
            {
                MemoryAccess.Initialize();
                MemoryAccess.PatchPtfx();
            }

            GameSound.Load("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            GameSound.Load("BASEJUMPS_SOUNDS");
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        private static void RegisterVars()
        {
            RegisterVar("toggleconsole", Keys.T, true);
            RegisterVar("enableconsole", IniHelper.GetValue("Other", "EnableConsole", false));
            RegisterVar("notifications", IniHelper.GetValue("Other", "Notifications", true));
<<<<<<< HEAD
            RegisterVar("spawninstorm", IniHelper.GetValue("Other", "SpawnInStorm", true));
            RegisterVar("soundenabled", IniHelper.GetValue("Other", "SoundEnabled", true));
            RegisterVar("sirenenabled", IniHelper.GetValue("Other", "SirenEnabled", true));
=======
            RegisterVar("spawninstorm", IniHelper.GetValue("Other", "SpawnInStorm", true));      
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
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
<<<<<<< HEAD
            RegisterVar("vortexMaxParticleLayers", IniHelper.GetValue("VortexAdvanced", "MaxParticleLayers", 48));
=======
            RegisterVar("vortexMaxParticleLayers", IniHelper.GetValue("VortexAdvanced", "MaxParticleLayers", 47));
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
            RegisterVar("vortexParticleCount", IniHelper.GetValue("VortexAdvanced", "ParticlesPerLayer", 9));
            RegisterVar("vortexLayerSeperationScale", IniHelper.GetValue("VortexAdvanced", "LayerSeperationAmount", 22.0f));
            RegisterVar("vortexParticleName", IniHelper.GetValue("VortexAdvanced", "ParticleName", "ent_amb_smoke_foundry"));
            RegisterVar("vortexParticleAsset", IniHelper.GetValue("VortexAdvanced", "ParticleAsset", "core"));
            RegisterVar("vortexParticleMod", IniHelper.GetValue("VortexAdvanced", "ParticleMod", true));
<<<<<<< HEAD
            RegisterVar("vortexEnableCloudTopParticle", IniHelper.GetValue("VortexAdvanced", "CloudTopEnabled", true));
            RegisterVar("vortexEnableCloudTopParticleDebris", IniHelper.GetValue("VortexAdvanced", "CloudTopDebrisEnabled", true));
            RegisterVar("vortexEnableSurfaceDetection", IniHelper.GetValue("VortexAdvanced", "EnableSurfaceDetection", true));
            RegisterVar("vortexUseEntityPool", IniHelper.GetValue("VortexAdvanced", "UseInternalPool", true));
=======
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
<<<<<<< HEAD
            if (!GetVar<bool>("enablekeybinds")) return;     
=======
            if (!GetVar<bool>("enablekeybinds")) return;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

            if (e.KeyCode != GetVar<Keys>("togglescript")) return;

            if (_factory.ActiveVortexCount > 0 && !GetVar<bool>("multiVortex"))
            {
                _factory.RemoveAll();
            }

            else
            {
<<<<<<< HEAD
                Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, 0f, 0f, 0f, 1000000.0f);

                Function.Call(Hash.SET_WIND, 70.0f);

                var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 180f;
=======
                Function.Call(Hash.SET_WIND, 70.0f);

                var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

                _factory.CreateVortex(position);
            }
        }

<<<<<<< HEAD
        private bool didInitTlsAlloc = false;

        public override void OnUpdate(int gameTime)
        {
            if (!didInitTlsAlloc)
            {
                WinHelper.CopyTlsValues(WinHelper.GetProcessMainThreadId(), Win32Native.GetCurrentThreadId(), 0xC8, 0xC0, 0xB8);
                didInitTlsAlloc = true;
            }

            base.OnUpdate(gameTime);
        }

        private static void ReleaseAssets()
        {      
            //
=======
        private static void ReleaseAssets()
        {
            GameSound.Release("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");

            GameSound.Release("BASEJUMPS_SOUNDS");
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        protected override void Dispose(bool a0)
        {
            Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, 0f, 0f, 0f, 1000000.0f);

            ReleaseAssets();

            base.Dispose(a0);
        }
    }
}
