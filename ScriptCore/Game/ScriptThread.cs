using System.Windows.Forms;
using GTA;

namespace ScriptCore
{
    /// <summary>
    /// Base class for a script thread.
    /// </summary>
    public abstract class ScriptThread : Script
    {
        /// <summary>
        /// Script extension pool.
        /// </summary>
        private static ScriptExtensionPool _extensions;

        /// <summary>
        /// Script vars.
        /// </summary>
        private static ScriptVarCollection _vars;

        protected ScriptThread()
        {
            _extensions = new ScriptExtensionPool();
            _vars = new ScriptVarCollection();
            Tick += (s, e) => OnUpdate(Game.GameTime);
            KeyDown += KeyPressedInternal;
        }

        /// <summary>
        /// Get a script extension from the underlying pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : ScriptExtension
        {
            return _extensions.Get<T>();
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public static void Add(ScriptExtension extension)
        {
            if (_extensions.Contains(extension)) return;

            extension.RegisterEvent("keydown");

            _extensions.Add(extension);

            extension.OnThreadAttached();
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        public static void Create<T>() where T : ScriptExtension, new()
        {
            var extension = Get<T>();

            if (extension != null) return;

            extension = new T();

            Add(extension);
        }

        /// <summary>
        /// Get an extension, or create it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetOrCreate<T>() where T : ScriptExtension, new()
        {
            var extension = Get<T>();

            if (extension != null)
                return extension;

            extension = new T();

            Add(extension);

            return extension;
        }

        internal static void Remove(ScriptExtension extension)
        {
            extension.OnThreadDetached();

            _extensions.Remove(extension);
        }

        /// <summary>
        /// Register a new script variable and add it to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the var</param>
        /// <param name="defaultValue">The default (reset) value</param>
        /// <param name="readOnly"></param>
        public static void RegisterVar<T>(string name, T defaultValue, bool readOnly = false)
        {
            _vars.Add(name, new ScriptVar<T>(defaultValue, readOnly));
        }

        /// <summary>
        /// Get a script variable attached to this thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ScriptVar<T> GetVar<T>(string name)
        {
            return _vars.Get<T>(name);
        }

        /// <summary>
        /// Set the value of a script variable attached to this thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetVar<T>(string name, T value)
        {
            var foundVar = GetVar<T>(name);

            if (foundVar.ReadOnly)
                return false;

            foundVar.Value = value;

            return true;
        }

        internal virtual void KeyPressedInternal(object sender, KeyEventArgs e)
        {
            foreach (ScriptExtension s in _extensions)
            {
                s.NotifyEvent("keydown", new ScriptEventArgs(e));
            }
        }

        /// <summary>
        /// Updates the thread.
        /// </summary>
        public virtual void OnUpdate(int gameTime)
        {
            for (int i = 0; i < _extensions.Count; i++)
            {
                _extensions[i].OnUpdate(gameTime);
            }
        }

        /// <summary>
        /// Removes the thread and all extensions.
        /// </summary>
        /// <param name="A_0"></param>
        protected override void Dispose(bool A_0)
        {
            for (int i = _extensions.Count - 1; i > -1; i--)
            {
                _extensions[i].Dispose();
            }

            base.Dispose(A_0);
        }
    }
}
