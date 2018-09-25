using System;
using System.Collections.Generic;

namespace TornadoScript.ScriptCore.IO
{
    /// <summary>
    /// Class to hold XML attributes.
    /// </summary>
    [Serializable]
    public class XMLAttributesCollection : Dictionary<string, string>
    {
        public XMLAttributesCollection() : base(StringComparer.OrdinalIgnoreCase)
        { }
    }
}
