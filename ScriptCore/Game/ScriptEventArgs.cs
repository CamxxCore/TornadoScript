using System;

namespace TornadoScript.ScriptCore.Game
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
