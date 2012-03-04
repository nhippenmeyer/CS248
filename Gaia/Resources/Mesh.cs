using System;
using System.Collections.Generic;
using System.Xml;

namespace Gaia.Resources
{
    public class Mesh : IResource
    {
        string name;
        public string Name { get { return name; } }

        void IResource.Destroy()
        {

        }

        void IResource.LoadFromXML(XmlNode node)
        {
            try
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {

                }

            }
            catch { }
        }
    }
}
