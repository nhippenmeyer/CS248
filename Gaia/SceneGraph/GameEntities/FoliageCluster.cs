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

        public override void OnAdd(Scene scene)
        {
            base.OnAdd(scene);
            materials.Add(ResourceManager.Inst.GetMaterial("GrassMat0"));
            materials.Add(ResourceManager.Inst.GetMaterial("GrassMat1"));
            materials.Add(ResourceManager.Inst.GetMaterial("GrassMat2"));
            InitializeClusters();
            
        }

        void RandomizeOrientation(Cluster cluster, Vector3 position, Vector3 surfaceNormal)
        {
            cluster.Transform = new Matrix[randomHelper.Next(minSides, maxSides)];
            cluster.Bounds.Min = Vector3.One * float.PositiveInfinity;
            cluster.Bounds.Max = Vector3.One * float.NegativeInfinity;
            for (int i = 0; i < cluster.Transform.Length; i++)
            {
                Vector3 randScale;
                randScale.X = MathHelper.Lerp(minScale.X, maxScale.X, (float)randomHelper.NextDouble());
                randScale.Y = MathHelper.Lerp(minScale.Y, maxScale.Y, (float)randomHelper.NextDouble());
                randScale.Z = MathHelper.Lerp(minScale.Z, maxScale.Z, (float)randomHelper.NextDouble());

                float randAngle = MathHelper.TwoPi * (float)randomHelper.NextDouble();
                cluster.Transform[i] = Matrix.CreateScale(randScale) * Matrix.CreateFromAxisAngle(surfaceNormal, randAngle);
                cluster.Transform[i].Translation = position + surfaceNormal*0.35f;
                Vector3 min = Vector3.Transform(Vector3.One * -1, cluster.Transform[i]);
                Vector3 max = Vector3.Transform(Vector3.One, cluster.Transform[i]);
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

                Matrix[] transforms = elemsMatrix.ToArray();
                for (int j = 0; j < transforms.Length; j += GFXShaderConstants.NUM_INSTANCES)
                {
                    RenderElement renderElem = new RenderElement();
                    int binLength = transforms.Length - j;

                    if (binLength > GFXShaderConstants.NUM_INSTANCES)
                        binLength = GFXShaderConstants.NUM_INSTANCES;

                    renderElem.Transform = new Matrix[binLength];

                    // Upload transform matrices as shader constants.
                    Array.Copy(transforms, j, renderElem.Transform, 0, binLength);
                    renderElem.StartVertex = 0;
                    renderElem.VertexDec = GFXVertexDeclarations.PTIDec;
                    renderElem.VertexStride = VertexPTI.SizeInBytes;
                    renderElem.VertexCount = 4 * binLength;
                    renderElem.VertexBuffer = GFXPrimitives.Quad.GetInstanceVertexBuffer();
                    renderElem.IndexBuffer = GFXPrimitives.Quad.GetInstanceIndexBuffer();
                    renderElem.PrimitiveCount = 2 * binLength;
                    view.AddElement(key, renderElem);
                }
            }
            base.OnRender(view);
        }


    }
}
