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
    public class Tree : Entity
    {
        List<RenderElement> Voxels;
        Material treeMaterial;  // in Gaia.Resources
        Material leafMaterial;

        void generateTree()
        {
            // TODO: make a new Lsystem and generateGeometry for it
            Lsystem lSys = new Lsystem();
            lSys.setAxiom("F");
            lSys.setIterations(3);
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
        }

        public override void OnAdd(Scene scene)
        {
            generateTree();
            treeMaterial = ResourceManager.Inst.GetMaterial("TreeMat");
            leafMaterial = ResourceManager.Inst.GetMaterial("GrassMat1");
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {

            base.OnDestroy();
        }

        public override void OnRender(RenderView view)
        {

            view.AddElement(treeMaterial, Voxels[0]);
            view.AddElement(leafMaterial, Voxels[1]);
            base.OnRender(view);
        }

        public override void OnUpdate()
        {

            base.OnUpdate();
        }
    }
}