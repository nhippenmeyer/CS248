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
        List<Material> leafMaterials;
        BoundingBox boundingBox;

        void generateTree(Vector3 position)
        {
            Lsystem lSys = new Lsystem();
            lSys.setAxiom("F");
            lSys.setIterations(3);
            lSys.setSphereRadius(1.0f);
            lSys.setTurnValue(45);
            lSys.setForwardLength(5.0f);

            Lsystem.ReproductionRule r1;
            r1.from = 'F';
         //   r1.to = "FF-[\\F+F&F@]+[/F-F%F@]";

            // Regular tree:
            r1.to = "GF[%-F@]%G[&+F@][&\\F@]&&G[%/F@]F@";  // With randomization
            r1.to = "GF[-F@]%G[+F@][\\F@]&&G[/F@]F@";  // With some randomization 

          //    r1.to = "G[/GF][+F][-GF][\\F]G@";
           //  r1.to = "G-%[[F]+&F]+&G[+&GF]-%F";
            //r1.to = "G[+%F][-&F]GFF@";
            lSys.addRule(r1);

            Lsystem.ReproductionRule r2;
            r2.from = 'G';
            r2.to = "G";
            //r2.to = "G[+<TTTT]G";
            lSys.addRule(r2);
            
            Voxels = lSys.generateGeometry(position);
            boundingBox = lSys.getBoundingBox();
        }

        public override void OnAdd(Scene scene)
        {
            
            treeMaterial = ResourceManager.Inst.GetMaterial("TreeMat");
            leafMaterials = new List<Material>();
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat0"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat1"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat2"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat3"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat4"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat5"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat6"));
            leafMaterials.Add(ResourceManager.Inst.GetMaterial("LeafMat7"));

            Random randomHelper = new Random();
            Vector3 randPosition = Vector3.Zero;
            Vector3 randNormal = Vector3.Zero;
            scene.MainTerrain.GenerateRandomTransform(randomHelper, out randPosition, out randNormal);
            generateTree(randPosition);
            base.OnAdd(scene);
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            if (frustum.Contains(boundingBox) != ContainmentType.Disjoint)
            {
                view.AddElement(treeMaterial, Voxels[0]);
                view.AddElement(leafMaterial, Voxels[1]);
            }
            base.OnRender(view);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}