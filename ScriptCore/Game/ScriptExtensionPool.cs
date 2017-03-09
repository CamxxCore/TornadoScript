using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCore
{
    public class ScriptExtensionPool : List<ScriptExtension>
    {              
        /// <summary>
        /// Get an extension from the pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : ScriptComponent
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i] as T;

                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
