﻿using System;
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
    public class Tree : Entity
    {
        List<VoxelGeometry> Voxels;
        Material terrainMaterial;  // in Gaia.Resources

        void generateTree()
        {
            // TODO: make a new Lsystem and generateGeometry for it
            Lsystem lSys = new Lsystem();
            lSys.setAxiom("F");
            lSys.setIterations(5);
            lSys.setSphereRadius(0.0025f);

            Lsystem.ReproductionRule r1;
            r1.from = 'F';
            r1.to = "G[+&F][-%F]GFF@";
            lSys.addRule(r1);

            Lsystem.ReproductionRule r2;
            r2.from = 'G';
            r2.to = "GG";
            lSys.addRule(r2);

            Voxels = lSys.generateGeometry();

            int polyCount = 0;
            int vertexCount = 0;
            for (int i = 0; i < Voxels.Count; i++)
            {
                polyCount += Voxels[i].renderElement.PrimitiveCount;
                vertexCount += Voxels[i].renderElement.VertexCount;
            }

            Console.WriteLine("Polygon count is {0} and vertex count is {1}", polyCount, vertexCount);

        }

        public override void OnAdd(Scene scene)
        {
            generateTree();
            terrainMaterial = ResourceManager.Inst.GetMaterial("TerrainMaterial");
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {

            base.OnDestroy();
        }

        public override void OnRender(RenderView view)
        {
            //   BoundingFrustum frustm = view.GetFrustum();
            for (int i = 0; i < Voxels.Count; i++)
            {
                //     if (frustm.Contains(VoxelBounds[i]) != ContainmentType.Disjoint && Voxels[i].CanRender)
                //    {
                view.AddElement(terrainMaterial, Voxels[i].renderElement);
                //    }
            }

            base.OnRender(view);
        }

        public override void OnUpdate()
        {

            base.OnUpdate();
        }
    }
}