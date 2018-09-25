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
<<<<<<< HEAD
            if (TryGetValue(name, out var result))
=======
            if (TryGetValue(name, out IScriptVar result))
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
            {
                return result as ScriptVar<T>;
            }

            return null;
        }
    }
}
