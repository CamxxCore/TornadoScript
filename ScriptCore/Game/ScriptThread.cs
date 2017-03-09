using System;
using System.Diagnostics;
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
        private static ScriptExtensionPool extensions;

        /// <summary>
        /// Script vars.
        /// </summary>
        private static ScriptVarCollection vars;

        public ScriptThread() : base()
        {
            extensions = new ScriptExtensionPool();
            vars = new ScriptVarCollection();
            Tick += (s,e) => OnUpdate(Game.GameTime);;
        }

        /// <summary>
        /// Get a script extension from the underlying pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : ScriptExtension
        {
            return extensions.Get<T>();
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public static void Add(ScriptExtension extension)
        {
            if (!extensions.Contains(extension))
            {
                extensions.Add(extension);

                extension.OnThreadAttached();
            }
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public static void Create<T>() where T : ScriptExtension, new()
        {
            T extension = Get<T>();

            if (extension == null)
            {
                extension = new T();

                Add(extension);
            }
        }

        /// <summary>
        /// Get an extension, or create it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetOrCreate<T>() where T : ScriptExtension, new()
        {
            var extension = Get<T>();

            if (extension == null)
            {
                extension = new T();

                Add(extension);
            }

            return extension;
        }

        internal static void Remove(ScriptExtension extension)
        {
            extension.OnThreadDetached();

            extensions.Remove(extension);
        }

        /// <summary>
        /// Register a new script variable and add it to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the var</param>
        /// <param name="value">The initial value</param>
        /// <param name="defaultValue">The default (reset) value</param>
        public static void RegisterVar<T>(string name, T defaultValue, bool readOnly = false)
        {
            vars.Add(name, new ScriptVar<T>(defaultValue, readOnly));
        }

        /// <summary>
        /// Get a script variable attached to this thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ScriptVar<T> GetVar<T>(string name)
        {
            ScriptVar<T> foundVar = vars.Get<T>(name);

            Debug.Assert(foundVar != null, "Script variable \"" + name + "\" not found.");

            return foundVar;
        }

        /// <summary>
        /// Set the value of a script variable attached to this thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool SetVar<T>(string name, T value)
        {
            ScriptVar<T> foundVar = GetVar<T>(name);

            if (!foundVar.ReadOnly)
            {
                foundVar.Value = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the thread.
        /// </summary>
        public virtual void OnUpdate(int gameTime)
        {
            for (int i = 0; i < extensions.Count; i++)
            {
                extensions[i].OnUpdate(gameTime);
            }
        }

        /// <summary>
        /// Removes the thread and all extensions.
        /// </summary>
        /// <param name="A_0"></param>
        protected override void Dispose(bool A_0)
        {
            Tick -= (s, e) => OnUpdate(Game.GameTime);

            for (int i = 0; i < extensions.Count; i++)
            {
                extensions[i].Dispose();
            }

            base.Dispose(A_0);
        }
    }
}
