using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Rendering;
using Gaia.Resources;

namespace Gaia.SceneGraph.GameEntities
{
    public class Cluster
    {
        public BoundingBox Bounds;
        public Matrix[] Transform;
    }

    public class FoliageCluster : Entity
    {
        List<Material> materials = new List<Material>();
        SortedList<Material, List<Cluster>> clusters = new SortedList<Material,List<Cluster>>();
        RenderElement[] renderElements;
        int minSides = 1;
        int maxSides = 5;
        int clusterSize;
        public Vector3 minScale = Vector3.One * 5.35f;
        public Vector3 maxScale = Vector3.One * 37.5f;

        Random randomHelper = new Random();

        public FoliageCluster(int clusterSize, int minSides, int maxSides)
        {
            this.minSides = minSides;
            this.maxSides = maxSides;
            this.clusterSize = clusterSize;
            
        }

        void CreateRenderElements()
        {
            renderElements = new RenderElement[materials.Count];
            for (int i = 0; i < renderElements.Length; i++)
            {
                renderElements[i] = new RenderElement();
                renderElements[i].VertexCount = 4;
                renderElements[i].PrimitiveCount = 4;
                renderElements[i].StartVertex = 0;
                renderElements[i].VertexStride = VertexPTI.SizeInBytes;
                renderElements[i].VertexDec = GFXVertexDeclarations.PTIDec;
                renderElements[i].VertexBuffer = GFXPrimitives.Quad.GetInstanceVertexBuffer();
                renderElements[i].IndexBuffer = GFXPrimitives.Quad.GetInstanceIndexBufferDoubleSided();
            }
        }

        public override void OnAdd(Scene scene)
        {
            base.OnAdd(scene);
            materials.Add(ResourceManager.Inst.GetMaterial("GrassMat0"));
            materials.Add(ResourceManager.Inst.GetMaterial("GrassMat1"));
            materials.Add(ResourceManager.Inst.GetMaterial("GrassMat2"));
            CreateRenderElements();
            InitializeClusters();
            
        }

        void RandomizeOrientation(Cluster cluster, Vector3 position, Vector3 surfaceNormal)
        {
            cluster.Transform = new Matrix[randomHelper.Next(minSides, maxSides)];
            cluster.Bounds.Min = Vector3.One * float.PositiveInfinity;
            cluster.Bounds.Max = Vector3.One * float.NegativeInfinity;
            for (int i = 0; i < cluster.Transform.Length; i++)
            {
                Vector3 randScale = Vector3.Lerp(minScale, maxScale, (float)randomHelper.NextDouble());

                float randAngle = MathHelper.TwoPi * (float)randomHelper.NextDouble();
                cluster.Transform[i] = Matrix.CreateScale(randScale) * Matrix.CreateFromAxisAngle(surfaceNormal, randAngle);
                cluster.Transform[i].Translation = position + surfaceNormal*0.5f;
                Vector3 min = Vector3.Transform(new Vector3(-1,-1,0), cluster.Transform[i]);
                Vector3 max = Vector3.Transform(new Vector3(1, 1, 0), cluster.Transform[i]);
                cluster.Bounds.Min = Vector3.Min(min, cluster.Bounds.Min);
                cluster.Bounds.Max = Vector3.Max(max, cluster.Bounds.Max);
            }
        }

        void InitializeClusters()
        {
            for (int i = 0; i < clusterSize; i++)
            {
                Cluster cluster = new Cluster();
                Material mat = materials[randomHelper.Next(materials.Count)];
                Vector3 randPosition = Vector3.Zero;
                Vector3 randNormal = Vector3.Zero;
                scene.MainTerrain.GenerateRandomTransform(randomHelper, out randPosition, out randNormal);
                RandomizeOrientation(cluster, randPosition, randNormal);
                if (!clusters.ContainsKey(mat))
                {
                    clusters.Add(mat, new List<Cluster>());
                }
                clusters[mat].Add(cluster);
            }
        }

        public override void OnRender(Gaia.Rendering.RenderViews.RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            for (int i = 0; i < clusters.Keys.Count; i++)
            {
                Material key = clusters.Keys[i];
                
                List<Matrix> elemsMatrix = new List<Matrix>();
                
                for (int j = 0; j < clusters[key].Count; j++)
                {
                    if(frustum.Contains(clusters[key][j].Bounds) != ContainmentType.Disjoint)
                    {
                        elemsMatrix.AddRange(clusters[key][j].Transform);
                    }
                }

                renderElements[i].Transform = elemsMatrix.ToArray();
                view.AddElement(key, renderElements[i]);
            }
            base.OnRender(view);
        }


    }
}
