using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

using Gaia.Rendering;

namespace Gaia.Resources
{
    public class Material : IResource, IComparable
    {
        Shader shader;
        string name;

        public string Name { get { return name; } }

        const int textureCounts = 16;

        TextureResource[] textures = new TextureResource[textureCounts];

        bool Transparent;
        bool Refract;
        bool Reflect;
        bool Transmissive;

        public bool IsTranslucent { get { return (Transmissive || Refract || Transparent); } }
        public bool IsEmissive;
        public string EmissiveMaterial;

        float kReflect;
        float kRefract;
        float kIOR;
        float kTrans;
        Vector3 kAmbient;
        Vector3 kDiffuse;
        Vector3 kSpecular;
        float kSpecularPower;
        float kRimCoeff;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Material otherMaterial = obj as Material;
            if (otherMaterial != null)
                return string.Compare(this.name, otherMaterial.name);
            else
                throw new ArgumentException("Object is not a materials");
        }

        void IResource.Destroy()
        {

        }

        void IResource.LoadFromXML(XmlNode node)
        {
            foreach (XmlAttribute attrib in node.Attributes)
            {
                string[] attribs = attrib.Name.ToLower().Split('_');
                switch (attribs[0])
                {
                    case "reflect":
                        Reflect = bool.Parse(attrib.Value);
                        break;
                    case "refract":
                        Refract = bool.Parse(attrib.Value);
                        break;
                    case "transmissive":
                        Transmissive = bool.Parse(attrib.Value);
                        break;
                    case "transparent":
                        Transparent = bool.Parse(attrib.Value);
                        break;
                    case "emissive":
                        IsEmissive = bool.Parse(attrib.Value);
                        break;
                    case "emissivematerial":
                        EmissiveMaterial = attrib.Value;
                        break;

                    case "kreflect":
                        kReflect = float.Parse(attrib.Value);
                        break;
                    case "krefract":
                        kRefract = float.Parse(attrib.Value);
                        break;
                    case "kior":
                        kIOR = float.Parse(attrib.Value);
                        break;
                    case "ktrans":
                        kTrans = float.Parse(attrib.Value);
                        break;

                    case "kambient":
                        kAmbient = ResourceManager.Inst.ParseVector3(attrib.Value);
                        break;
                    case "kdiffuse":
                        kDiffuse = ResourceManager.Inst.ParseVector3(attrib.Value);
                        break;
                    case "kspecular":
                        kSpecular = ResourceManager.Inst.ParseVector3(attrib.Value);
                        break;
                    case "kspecpower":
                        kSpecularPower = float.Parse(attrib.Value);
                        break;
                    case "krimcoeff":
                        kRimCoeff = float.Parse(attrib.Value);
                        break;

                    case "texture":
                        int index = int.Parse(attribs[1]);
                        textures[index] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;

                    case "shader":
                        shader = ResourceManager.Inst.GetShader(attrib.Value);
                        break;

                    case "name":
                        name = attrib.Value;
                        break;
                }
            }
        }

        public void SetupTextures()
        {
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] != null)
                {
                    GFX.Device.Textures[i] = textures[i].GetTexture();
                    GFX.Device.SamplerStates[i].MaxMipLevel = textures[i].GetTexture().LevelOfDetail;
                }
            }
        }

        public void SetupMaterial()
        {
            shader.SetupShader();

            SetupTextures();
        }
    }
}
