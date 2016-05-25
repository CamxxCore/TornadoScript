using System.Windows.Forms;
using TornadoScript.INI;

namespace TornadoScript
{
    public static class Config
    {
        public static readonly bool EnableKeybinds =
        INIHelper.GetConfigSetting("KeyBinds", "KeybindsEnabled", true);

        public static readonly Keys ToggleScript =
        INIHelper.GetConfigSetting("KeyBinds", "ToggleScript", Keys.F6);

        public static readonly bool SpawnInStorm =
         INIHelper.GetConfigSetting("Other", "SpawnInStorm", true);

    }
}
