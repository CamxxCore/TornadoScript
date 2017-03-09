using System;

namespace ScriptCore
{
    public delegate void ScriptEntityEventHandler(IScriptEntity sender, ScriptEntityEventArgs args);

    /// <summary>
    /// Event args for a script entity event.
    /// </summary>
    public sealed class ScriptEntityEventArgs : EventArgs
    {
        public ScriptEntityEventArgs(int gameTime)
        {
            GameTime = gameTime;
        }

        /// <summary>
        /// The entity that fired the event
        /// </summary>
        public int GameTime { get; private set; }
    }
}
