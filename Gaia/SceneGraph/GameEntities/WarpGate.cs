using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;
using Gaia.Voxels;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public class WarpGate : Entity
    {
        VoxelGeometry gateGeometry;
        Material gateMaterial;
        BoundingBox bounds;

        public override void OnAdd(Scene scene)
        {
            this.Transformation.SetScale(30*Vector3.One);
            this.Transformation.SetPosition(Vector3.Transform(Vector3.Up * 0.45f, scene.MainTerrain.Transformation.GetTransform()));
            gateMaterial = ResourceManager.Inst.GetMaterial("GateMaterial");
            GenerateGeometry();
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            if (frustum.Contains(bounds) != ContainmentType.Disjoint)
            {
                view.AddElement(gateMaterial, gateGeometry.renderElement);
            }
            base.OnRender(view);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        void GenerateGeometry()
        {
            byte[] DensityField;
            int DensityFieldSize = 17;
            byte IsoValue = 127;

            DensityField = new byte[DensityFieldSize * DensityFieldSize * DensityFieldSize];
            Vector3 center = Vector3.One * DensityFieldSize * 0.5f;
            Vector3 minPos = center;
            Vector3 maxPos = center;

            float radiusInner = DensityFieldSize / 6;

            float numSpikes = 5;

            float period = MathHelper.TwoPi * numSpikes;

            float minRad = DensityFieldSize / 4;

            float maxRad = DensityFieldSize / 3;

            for (int x = 0; x < DensityFieldSize; x++)
            {
                for (int y = 1; y < (DensityFieldSize - 1); y++)
                {
                    for (int z = 0; z < DensityFieldSize; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        float innerRingTerm = (1.0f - new Vector2(pos.X-center.X, pos.Y-center.Y).Length() / radiusInner);
                        float innerRing = (innerRingTerm > 0.0f) ? 1 : 0;

                        float zElem = Math.Abs((z - center.Z) / center.Z);

                        float sinTerm = (float)Math.Sin(period * zElem);
                        float cosTerm = (float)Math.Cos(period * zElem);

                        float radiusOuter = minRad;

                        if (cosTerm <= 0.0f)
                        {
                            radiusOuter = MathHelper.Lerp(minRad, maxRad, 1.0f + cosTerm);
                        }

                        float outerRingTerm = (1.0f - new Vector2(pos.X - center.X, pos.Y - center.Y).Length() / radiusOuter);
                        float density = MathHelper.Clamp(outerRingTerm-innerRing, 0, 1);
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

            gateGeometry = new VoxelGeometry();
            gateGeometry.renderElement.Transform = new Matrix[1] { this.Transformation.GetTransform() };
            gateGeometry.GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, DensityFieldSize - 1, DensityFieldSize - 1, DensityFieldSize - 1, 0, 0, 0, 2.0f / (float)(DensityFieldSize - 1));

            bounds = new BoundingBox();
            bounds.Max = Vector3.Transform(maxPos, this.Transformation.GetTransform());
            bounds.Min = Vector3.Transform(minPos, this.Transformation.GetTransform());
        }

    }
}
