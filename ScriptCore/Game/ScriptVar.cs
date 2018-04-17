
namespace TornadoScript.ScriptCore.Game
{
    /// <summary>
    /// Represents a script variable object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScriptVar<T> : IScriptVar
    {
        /// <summary>
        /// The current value of the script var.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The default value of the script var.
        /// </summary>
        public T Default { get; }

        /// <summary>
        /// Whether the script var is read-only.
        /// </summary>
        public bool ReadOnly { get; }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="value">The initial value of the variable.</param>
        /// <param name="isReadonly">Whether the variable is readonly.</param>
        public ScriptVar(T value, bool isReadonly)
        {
            Value = value;
            Default = value;   
            ReadOnly = isReadonly;
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="value"></param>
        public ScriptVar(T value) : this(value, false)
        { }

        /// <summary>
        /// Implicit conversion from <see cref="ScriptVar{T}"/> to nested <see cref="T"/> value
        /// </summary>
        /// <param name="var"></param>
        public static implicit operator T(ScriptVar<T> var)
        {
            return var.Value;
        }
    }
}
