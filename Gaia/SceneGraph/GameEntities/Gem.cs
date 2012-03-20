﻿﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Gaia.Resources;
using Gaia.Voxels;
using Gaia.Input;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;
using Gaia.SceneGraph.GameEntities;


namespace Gaia.SceneGraph.GameEntities
{
    public class Gem : Entity
    {
        VoxelGeometry gemGeometry;
        Material gemMaterial;
        byte[] DensityField;
        int DensityFieldSize = 17;
        byte IsoValue = 127;

        Light emitterLight;

        Random randomHelper;
        
        BoundingBox boundingBox;
        Vector3 minPos, maxPos;
        bool collected;

        public Gem(Random random)
        {
            randomHelper = random;
        }

        void generateGem(Vector3 position)
        {
            position.Y = position.Y + 2.0f;
            float radiusMax = (DensityFieldSize / 2);
            float radiusMin = (DensityFieldSize / 16);
            Vector3 cylinderCenter = Vector3.One * DensityFieldSize * 0.5f;
            minPos = maxPos = cylinderCenter;
            DensityField = new byte[DensityFieldSize * DensityFieldSize * DensityFieldSize];

            for (int x = 0; x < DensityFieldSize; x++)
            {
                for (int y = 1; y < (DensityFieldSize - 1); y++)
                {
                    for (int z = 0; z < DensityFieldSize; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        float offset = Math.Abs(pos.Y - cylinderCenter.Y);
                        pos.Y = cylinderCenter.Y;
                        float radius = MathHelper.Lerp(radiusMax, radiusMin, offset/cylinderCenter.Y);
                        float density = Math.Max(1.0f - (pos - cylinderCenter).Length() / (radius), 0.0f);
                        if (density > 0.0f)
                        {
                            pos = (pos / DensityFieldSize) * 2.0f - Vector3.One;
                            minPos = Vector3.Min(pos, minPos);
                            maxPos = Vector3.Max(pos, maxPos);
                        }
                        DensityField[x + (y + z * DensityFieldSize) * DensityFieldSize] = (byte)(density * 255.0f);
                    }
                }
            }
            
            gemGeometry = new VoxelGeometry();
            gemGeometry.renderElement.Transform = new Matrix[1] { Matrix.CreateScale(5.0f) * Matrix.CreateTranslation(position) };
            gemGeometry.GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, DensityFieldSize - 1, DensityFieldSize - 1, DensityFieldSize - 1, 0, 0, 0, 2.0f / (float)(DensityFieldSize - 1));

            boundingBox = new BoundingBox();
            boundingBox.Max = Vector3.Transform(maxPos, gemGeometry.renderElement.Transform[0]);
            boundingBox.Min = Vector3.Transform(minPos, gemGeometry.renderElement.Transform[0]);

            collected = false;

            Vector3 lightPosition = position;
            lightPosition.X = position.X + 12.0f;
            emitterLight = new Light(LightType.Point, new Vector3(0.83f, 0.06f, 0.36f), lightPosition, false);
            emitterLight.Parameters = new Vector4(70, 45, 0, 0);
        }

        public override void OnAdd(Scene scene)
        {
            gemMaterial = ResourceManager.Inst.GetMaterial("GemMaterial");

            Vector3 randPosition = Vector3.Zero;
            Vector3 randNormal = Vector3.Zero;
            randomHelper.NextDouble();
            randomHelper.NextDouble();
            scene.MainTerrain.GenerateRandomTransform(randomHelper, out randPosition, out randNormal);
            while (randPosition.Y < 2.0f || Vector3.Dot(randNormal, Vector3.Up) < 0.3f)
            {
                scene.MainTerrain.GenerateRandomTransform(randomHelper, out randPosition, out randNormal);
            }
            base.OnAdd(scene);
            generateGem(randPosition);
            scene.Entities.Add(emitterLight);
        }

        public override void OnDestroy()
        {
            scene.Entities.Remove(emitterLight);
            base.OnDestroy();
        }

        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            if (frustum.Contains(boundingBox) != ContainmentType.Disjoint && !collected)
            {
                view.AddElement(gemMaterial, gemGeometry.renderElement);
                
            }
            base.OnRender(view);
        }

        public override void OnUpdate()
        {

            if (boundingBox.Contains(scene.MainCamera.GetPosition()) != ContainmentType.Disjoint && !collected)
            {
                collected = true;
                scene.Entities.Remove(this);
                this.OnDestroy();
                scene.MainPlayer.OnGemCollected(1.25f);
            }
            base.OnUpdate();
        }
    }
}