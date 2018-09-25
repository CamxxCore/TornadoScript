namespace TornadoScript.ScriptCore.Game
{
    public abstract class ScriptExtension : ScriptComponent, IScriptEventHandler
    {
        public ScriptExtensionEventPool Events { get; } = 
            new ScriptExtensionEventPool();

        public ScriptExtension()
        {
            ScriptThread.Add(this);
        }

        /// <summary>
        /// Raise an event with the specified name.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        public void NotifyEvent(string name)
        {
            NotifyEvent(name, new ScriptEventArgs());
        }

        /// <summary>
        /// Raise an event with the specified name and arguments.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="args">Event specific arguments.</param>
        public void NotifyEvent(string name, ScriptEventArgs args)
        {
            Events[name]?.Invoke(this, args);
        }

        /// <summary>
        /// Register a script event for the underlying extension.
        /// </summary>
        /// <param name="name"></param>
        public void RegisterEvent(string name)
        {
            Events.Add(name, default(ScriptExtensionEventHandler));
        }

        internal virtual void OnThreadAttached()
        { }

        internal virtual void OnThreadDetached()
        { }

        public virtual void Dispose()
        {
            ScriptThread.Remove(this);
        }
    }
}
