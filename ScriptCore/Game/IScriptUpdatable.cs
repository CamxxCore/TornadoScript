
namespace ScriptCore
{
    /// <summary>
    /// Represents an object that can be updated by a script.
    /// </summary>
    public interface IScriptUpdatable
    {
        /// <summary>
        /// Method to be fired each frame.
        /// </summary>
        void OnUpdate(int gameTime);
    }
}
