﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Gaia.Resources
{
    public enum ResourceTypes
    {
        Shader = 0,
        Texture,
        Material,
        Particle,
        Mesh,
        Count
    }

    public class ResourceManager
    {
        public static ResourceManager Inst = null;

        SortedList<string, IResource>[] resources;

        string[] ResourceTypeTokens;
        Type[] ResourceTypeDefs;

        public ResourceManager()
        {
            Inst = this;
            resources = new SortedList<string, IResource>[(int)ResourceTypes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = new SortedList<string, IResource>();
            }
            RegisterResourceTypes();
        }

        void RegisterResourceTypes()
        {
            ResourceTypeDefs = new Type[(int)ResourceTypes.Count];
            ResourceTypeTokens = new string[(int)ResourceTypes.Count];

            ResourceTypeDefs[(int)ResourceTypes.Material] = typeof(Material);
            ResourceTypeTokens[(int)ResourceTypes.Material] = "Material";

            ResourceTypeDefs[(int)ResourceTypes.Mesh] = typeof(Mesh);
            ResourceTypeTokens[(int)ResourceTypes.Mesh] = "Mesh";

            ResourceTypeDefs[(int)ResourceTypes.Particle] = typeof(ParticleEffect);
            ResourceTypeTokens[(int)ResourceTypes.Particle] = "ParticleEffect";

            ResourceTypeDefs[(int)ResourceTypes.Shader] = typeof(Shader);
            ResourceTypeTokens[(int)ResourceTypes.Shader] = "Shader";

            ResourceTypeDefs[(int)ResourceTypes.Texture] = typeof(TextureResource);
            ResourceTypeTokens[(int)ResourceTypes.Texture] = "Texture";
        }

        public void LoadResources()
        {
            string[] filePaths = Directory.GetFiles("./", "*.res", SearchOption.AllDirectories);
            Queue<XmlNodeList>[] nodesToProcess = new Queue<XmlNodeList>[(int)ResourceTypes.Count];
            for (int i = 0; i < filePaths.Length; i++)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePaths[i]);
                for (int j = 0; j < ResourceTypeTokens.Length; j++)
                {
                    if (nodesToProcess[j] == null)
                        nodesToProcess[j] = new Queue<XmlNodeList>();
                    XmlNodeList list = xmlDoc.GetElementsByTagName(ResourceTypeTokens[j]);
                    nodesToProcess[j].Enqueue(list);
                }
            }
            for (int i = 0; i < nodesToProcess.Length; i++)
            {
                while (nodesToProcess[i] != null && nodesToProcess[i].Count > 0)
                {
                    XmlNodeList nodeCollection = nodesToProcess[i].Dequeue();
                    foreach (XmlNode node in nodeCollection)
                    {
                        IResource res = (IResource)ResourceTypeDefs[i].GetConstructors()[0].Invoke(null);
                        res.LoadFromXML(node);
                        if (resources[i].ContainsKey(res.Name))
                        {
                            res.Destroy();
                            res = null;
                            return;
                        }
                        resources[i].Add(res.Name, res);
                    }
                }
            }
        }

        IResource GetResource(string key, ResourceTypes type)
        {
            if (resources[(int)type].ContainsKey(key))
                return resources[(int)type][key];
            return null;
        }

        public Material GetMaterial(string key)
        {
            return (Material)GetResource(key, ResourceTypes.Material);
        }

        public TextureResource GetTexture(string key)
        {
            return (TextureResource)GetResource(key, ResourceTypes.Texture);
        }

        public Shader GetShader(string key)
        {
            return (Shader)GetResource(key, ResourceTypes.Shader);
        }

        public ParticleEffect GetParticleEffect(string key)
        {
            return (ParticleEffect)GetResource(key, ResourceTypes.Particle);
        }

        public Mesh GetMesh(string key)
        {
            return (Mesh)GetResource(key, ResourceTypes.Mesh);
        }

        public Vector2 ParseVector2(string text)
        {
            string[] splits = text.Split(' ');
            float x = 0, y = 0;
            float.TryParse(splits[0], out x);
            if (splits.Length > 1)
                float.TryParse(splits[1], out y);
            return new Vector2(x, y);
        }

        public Vector3 ParseVector3(string text)
        {
            string[] splits = text.Split(' ');
            float x = 0, y = 0, z = 0;
            float.TryParse(splits[0], out x);
            if (splits.Length > 1)
                float.TryParse(splits[1], out y);
            if (splits.Length > 2)
                float.TryParse(splits[2], out z);
            return new Vector3(x, y, z);
        }

        public Vector4 ParseVector4(string text)
        {
            string[] splits = text.Split(' ');
            float x = 0, y = 0, z = 0, w = 0;
            float.TryParse(splits[0], out x);
            if (splits.Length > 1)
                float.TryParse(splits[1], out y);
            if (splits.Length > 2)
                float.TryParse(splits[2], out z);
            if (splits.Length > 3)
                float.TryParse(splits[3], out w);
            return new Vector4(x, y, z, w);
        }
    }
}
