using System;
using System.Collections.Generic;

namespace TornadoScript.ScriptCore.Game
{
    public class ScriptVarCollection : Dictionary<string, IScriptVar>
    {
        public ScriptVarCollection() : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// Get an extension from the pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ScriptVar<T> Get<T>(string name)
        {
            if (TryGetValue(name, out IScriptVar result))
            {
                return result as ScriptVar<T>;
            }

            return null;
        }
    }
}
