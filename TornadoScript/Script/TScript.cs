using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using ScriptCore;
using TornadoScript.Memory;
using TornadoScript.Config;

namespace TornadoScript.Script
{
    public class TScript : ScriptThread
    {
        private TMonitor tMonitor;

        public TScript()
        {
            SetupAssets();
            AddVars();
            KeyDown += KeyPressed;
            tMonitor = GetOrCreate<TMonitor>();    
        }

        private void SetupAssets()
        {
            MemoryAccess.PatchPTFX();
            GameSound.Load("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            GameSound.Load("BASEJUMPS_SOUNDS");
        }

        public void AddVars()
        {
            RegisterVar("enablekeybinds", IniHelper.GetConfigSetting("KeyBinds", "KeybindsEnabled", true));
            RegisterVar("togglescript", IniHelper.GetConfigSetting("KeyBinds", "ToggleScript", Keys.F6));
            RegisterVar("vortexHorzizontalPullForce", IniHelper.GetConfigSetting("Vortex", "HorizontalForceScale", 1.1f));
            RegisterVar("vortexVerticalPullForce", IniHelper.GetConfigSetting("Vortex", "VerticalForceScale", 1.169f));
            RegisterVar("spawninstorm", IniHelper.GetConfigSetting("Other", "SpawnInStorm", true));
            RegisterVar("notifications", IniHelper.GetConfigSetting("Other", "Notifications", true));        
        }

        GameSound sound;

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (!GetVar<bool>("enablekeybinds")) return;

            if (e.KeyCode == Keys.Add)
            {
                MemoryAccess.PatchPTFX();

               /* if (sound != null)
                {
                    sound.Destroy();
                }

                GameSound.Load("FBI_HEIST_RAID");
                

                sound = new GameSound("debris", "EXTREME_01_SOUNDSET");

                sound.Play(Game.Player.Character.Position);

                UI.ShowSubtitle("done");
                       */                 //  var result = MemoryAccess.Func();

                //  UI.ShowSubtitle(result.Count().ToString());

                // Function.Call((Hash)0x552369F549563AD5, true);
                //  UI.ShowSubtitle(Probability.GetScalar().ToString());
                // MemoryAccess.Func("core", "ent_amb_smoke_foundry");
            }

            if (e.KeyCode == GetVar<Keys>("togglescript"))
            {
                if (tMonitor.ActiveVortexCount > 0)
                {
                    tMonitor.RemoveAll();
                }

                else
                {
                    Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, 0, 0, 0, 100000.0f);

                    Function.Call(Hash.SET_WIND, 70.0f);

                    var position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100f;

                    tMonitor.CreateVortex(position);
                }
            }
        }

        private void ReleaseAssets()
        {
            GameSound.Release("FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            GameSound.Release("BASEJUMPS_SOUNDS");
        }

        protected override void Dispose(bool A_0)
        {
            if (sound != null)
            {
                sound.Destroy();

                sound = null;
            }

            Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, 0f, 0f, 0f, 1000000.0f);

            ReleaseAssets();

            base.Dispose(A_0);
        }
    }
}
