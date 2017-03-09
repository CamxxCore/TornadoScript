using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCore
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
            IScriptVar result;

            if (TryGetValue(name, out result))
            {
                return result as ScriptVar<T>;
            }

            return null;
        }
    }
}
