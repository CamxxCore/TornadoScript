using System;

namespace TornadoScript.ScriptCore.Game
{
    /// <summary>
    /// Base class for all script components.
    /// </summary>
    public abstract class ScriptComponent : IScriptComponent, IScriptUpdatable
    {
        /// <summary>
        /// Name of the component.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Unique id to identify this instance.
        /// </summary>
        public Guid Guid { get; private set; }


        public ScriptComponent() : this(string.Empty)
        { }

        public ScriptComponent(string name)
        {
            Name = name ?? GetType().Name;
            Guid = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            var component = obj as ScriptComponent;

            if (component == null)
            {
                return false;
            }

            return Guid.GetHashCode() == component.Guid.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public virtual void OnUpdate(int gameTime)
        { }
    }
}
