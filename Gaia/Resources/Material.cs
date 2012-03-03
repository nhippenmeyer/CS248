using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

using Gaia.Rendering;

namespace Gaia.Resources
{
    public class Material : IResource
    {
        Shader shader;
        string name;

        public string Name { get { return name; } }

        Material prepassMaterial;
        string prepassMaterialName;

        const int textureCounts = 8;

        TextureResource[] textures = new TextureResource[textureCounts];

        bool Transparent;
        bool Refract;
        bool Reflect;
        bool Transmissive;

        float kReflect;
        float kRefract;
        float kIOR;
        float kTrans;
        Vector3 kAmbient;
        Vector3 kDiffuse;
        Vector3 kSpecular;
        float kSpecularPower;
        float kRimCoeff;

        void IResource.Destroy()
        {

        }

        void IResource.LoadFromXML(XmlNode node)
        {
            foreach (XmlAttribute attrib in node.Attributes)
            {
                switch (attrib.Name.ToLower())
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

                    case "texture0":
                        textures[0] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture1":
                        textures[1] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture2":
                        textures[2] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture3":
                        textures[3] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture4":
                        textures[4] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture5":
                        textures[5] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture6":
                        textures[6] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;
                    case "texture7":
                        textures[7] = ResourceManager.Inst.GetTexture(attrib.Value);
                        break;

                    case "prepassmaterial":
                        name = attrib.Value;
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
                    GFX.Device.Textures[i] = textures[i].GetTexture();
            }
        }

        public void SetupMaterial()
        {
            shader.SetupShader();

            SetupTextures();
        }
    }
}
