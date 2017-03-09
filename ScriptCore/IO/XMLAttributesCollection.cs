using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptCore.IO
{
    /// <summary>
    /// Class to hold XML attributes.
    /// </summary>
    public class XMLAttributesCollection : Dictionary<string, string>
    {
        public XMLAttributesCollection() : base(StringComparer.OrdinalIgnoreCase)
        { }
    }
}
