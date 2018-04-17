using System;

namespace TornadoScript.ScriptCore.Game
{
    /// <summary>
    /// Represents a component of a script.
    /// </summary>
    public interface IScriptComponent : IScriptUpdatable
    {
        /// <summary>
        /// Gets the name of this component.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Unique Id.
        /// </summary>
        Guid Guid { get; }
    }
}
