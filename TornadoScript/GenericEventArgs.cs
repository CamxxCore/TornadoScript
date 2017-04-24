using System;

namespace TornadoScript
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T Data { get; private set; }

        public GenericEventArgs(T data)
        {
            Data = data;
        }
    }
}
