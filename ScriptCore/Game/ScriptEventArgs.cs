using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCore
{
    public class ScriptEventArgs : EventArgs
    {
        public object Data { get; private set; }

        public ScriptEventArgs() : this(null)
        { }

        public ScriptEventArgs(object data)
        {
            Data = data;
        }
    }

    public class ScriptEventArgs<T> : EventArgs
    {
        public T Data { get; private set; }

        public ScriptEventArgs(T data)
        {
            Data = data;
        }
    }
}
