using System;
using System.Collections.Generic;

namespace TornadoScript.ScriptCore.Game
{
    public delegate void ScriptExtensionEventHandler(ScriptExtension sender, ScriptEventArgs e);

    public class ScriptExtensionEventPool : Dictionary<string, ScriptExtensionEventHandler>
    {
        public ScriptExtensionEventPool() : base(StringComparer.OrdinalIgnoreCase)
        { }
    }
}
