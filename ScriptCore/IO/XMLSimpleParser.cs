using System.Collections.Generic;
using System.Xml;

namespace TornadoScript.ScriptCore.IO
{
    class XMLSimpleParser
    {
        /// <summary>
        /// Lightweight function for grabbing nested xml data in a file.
        /// </summary>
        /// <param name="fileName">The path to the file.</param>
        /// <param name="dataType">The name of the xml element to be parsed.</param>
        /// <returns></returns>
        public static IEnumerable<XMLAttributesCollection> GetNestedAttributes(string fileName, string dataType)
        {
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == dataType && reader.HasAttributes)
                    {
                        var childAttributes = new XMLAttributesCollection();

                        while (reader.MoveToNextAttribute())
                        {
                            childAttributes.Add(reader.Name, reader.Value);
                        }

                        yield return childAttributes;
                    }
                }
            }
        }        
    }
}
