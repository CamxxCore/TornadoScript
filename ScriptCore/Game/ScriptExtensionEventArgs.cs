using System;

namespace TornadoScript.ScriptCore.Game
{
    public delegate void ScriptComponentEventHandler(ScriptThread sender, ScriptComponentEventArgs args);

    /// <summary>
    /// Event args for a script extension event.
    /// </summary>
    public sealed class ScriptComponentEventArgs : EventArgs
    {
        public ScriptComponentEventArgs(ScriptComponent extension)
        {
            Extension = extension;
        }

        public ScriptComponent Extension { get; private set; }
    }
}
