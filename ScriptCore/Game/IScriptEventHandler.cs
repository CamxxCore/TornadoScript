using System;

namespace ScriptCore
{
    /// <summary>
    /// Represents a handler of script events.
    /// </summary>
    public interface IScriptEventHandler
    {
        ScriptExtensionEventPool Events { get; }

        void NotifyEvent(string name);

        void NotifyEvent(string name, EventArgs args);
    }
}
