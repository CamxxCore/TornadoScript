using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCore
{
    public delegate void ScriptExtensionEventHandler(ScriptExtension sender, EventArgs e);

    public class ScriptExtensionEventPool : Dictionary<string, ScriptExtensionEventHandler>
    {
        public ScriptExtensionEventPool() : base(StringComparer.OrdinalIgnoreCase)
        { }
    }
}
