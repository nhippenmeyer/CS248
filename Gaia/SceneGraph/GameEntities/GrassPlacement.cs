using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;
using Gaia.Rendering.Geometry;
using Gaia.Rendering.RenderViews;
using Gaia.Rendering;
namespace Gaia.SceneGraph.GameEntities
{
    public class GrassPlacement : Entity
    {
        GrassGeometry[] grassGeometries;
        
        Material grassMaterial;

        RenderElement[] grassHighDetail;

        Random randomGen = new Random();

        float grassScale = 10;
        float grassDetailRange = 50;
        int gridWorldSize;
        GrassElement[] grassCells;
        int cellSize;
        BoundingBox cameraClipBounds;
        Vector3 prevPos = Vector3.One*float.NegativeInfinity;

        struct GrassElement
        {
            public Matrix Transform;
            public int GrassGeometryIndex;

            public BoundingBox Bounds;

            public GrassElement(Matrix transform, int index, BoundingBox bounds)
            {
                Transform = transform;
                GrassGeometryIndex = index;
                Bounds = bounds;
            }
        };

        SortedList<int, GrassElement> grassTransforms = new SortedList<int, GrassElement>();

        public override void OnAdd(Scene scene)
        {
            grassMaterial = ResourceManager.Inst.GetMaterial("3DGrassMaterial");
            int grassCount = 15;
            grassGeometries = new GrassGeometry[grassCount];
            grassHighDetail = new RenderElement[grassCount];
            for (int i = 0; i < grassGeometries.Length; i++)
            {
                grassGeometries[i] = new GrassGeometry();
                grassHighDetail[i] = grassGeometries[i].GetHighDetail();
            }
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        void UpdateGrassPlacement()
        {
            Vector3 camPos = scene.MainCamera.GetPosition()/gridWorldSize;
            int grassCount = gridWorldSize / cellSize;

            grassCells = new GrassElement[grassCount * grassCount * grassCount];
            float ratio = 2.0f * (float)cellSize / (float)gridWorldSize;

            int initX = (int)camPos.X;
            int initY = (int)camPos.Y;
            int initZ = (int)camPos.Z;

            for (int z = initX; z < initX+grassCount; z++)
            {
                int zOff = grassCount * grassCount * z;
                for (int y = 0; y < grassCount; y++)
                {
                    int yOff = grassCount * y;
                    
                    for (int x = 0; x < grassCount; x++)
                    {
                        int idx = x + yOff + zOff;

                        if (!grassTransforms.ContainsKey(idx))
                        {
                            Vector3 pos, normal;
                            pos = Vector3.Zero;
                            pos.X = (x + 0.5f) * grassScale;
                            pos.Z = (z + 0.5f) * grassScale;
                            normal = Vector3.Up;
                            if (scene.MainTerrain.GetYPos(ref pos, out normal, (y) * grassScale, (y + 1.0f) * grassScale))
                            {
                                Matrix transform = Matrix.Identity;
                                transform.Up = normal;
                                transform *= Matrix.CreateScale(grassScale);
                                transform.Translation = pos;
                                BoundingBox bounds = new BoundingBox(new Vector3(x, y, z) * ratio - Vector3.One, new Vector3(x + 1, y + 1, z + 1) * ratio);
                                bounds.Min = Vector3.Transform(bounds.Min, Matrix.CreateScale(grassScale));
                                bounds.Max = Vector3.Transform(bounds.Max, Matrix.CreateScale(grassScale));
                                int randIndex = randomGen.Next(grassHighDetail.Length);
                                //grassTransforms.Add(index, new GrassElement(transform, randIndex, bounds, x, y, z));
                            }
                        }
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            Vector3 camPos = scene.MainCamera.GetPosition();

            cameraClipBounds.Min = camPos - Vector3.One * grassDetailRange;
            cameraClipBounds.Max = camPos + Vector3.One * grassDetailRange;

            for (int i = 0; i < grassTransforms.Keys.Count; i++)
            {
                int key = grassTransforms.Keys[i];
                GrassElement elem = grassTransforms[key];
                
                if (cameraClipBounds.Contains(elem.Bounds) == ContainmentType.Disjoint)
                {
                    grassTransforms.Remove(key);
                }
            }

            UpdateGrassPlacement();
            
            base.OnUpdate();
        }

        public override void OnRender(RenderView view)
        {
            List<Matrix>[] transforms = new List<Matrix>[grassHighDetail.Length]; 
            BoundingFrustum frustum  = view.GetFrustum();
            for (int i = 0; i < grassTransforms.Keys.Count; i++)
            {
                int key = grassTransforms.Keys[i];
                //if (frustum.Contains(grassTransforms[key].Bounds) != ContainmentType.Disjoint)
                {
                    int index = grassTransforms[key].GrassGeometryIndex;
                    if(transforms[index] == null)
                        transforms[index] = new List<Matrix>();
                    transforms[index].Add(grassTransforms[key].Transform);
                }
            }
            for (int i = 0; i < grassHighDetail.Length; i++)
            {
                if (transforms[i] != null)
                {
                    grassHighDetail[i].Transform = transforms[i].ToArray();
                    view.AddElement(grassMaterial, grassHighDetail[i]);
                }
            }
            base.OnRender(view);
        }
    }
}
