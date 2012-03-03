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
                    /*
                    switch (attrib.Name.ToLower())
                    {
                        case "psfilename":
                            psFileName = attrib.Value;
                            break;
                        case "vsfilename":
                            vsFileName = attrib.Value;
                            break;
                        case "pstarget":
                            PSTarget = int.Parse(attrib.Value);
                            break;
                        case "vstarget":
                            VSTarget = int.Parse(attrib.Value);
                            break;
                        case "name":
                            Name = attrib.Value;
                            break;
                    }*/
                }
                //CompileFromFiles(psFileName, vsFileName);
                //ResourceManager.Inst.AddShader(this);
            }
            catch { }
        }
    }
}
