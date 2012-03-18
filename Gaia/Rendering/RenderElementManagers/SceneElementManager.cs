using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gaia.Rendering.RenderViews;
using Gaia.Resources;
namespace Gaia.Rendering
{
    public class SceneElementManager : RenderElementManager
    {
        SortedList<Material, Queue<RenderElement>> Elements = new SortedList<Material, Queue<RenderElement>>();
        Matrix[] tempTransforms = new Matrix[GFXShaderConstants.NUM_INSTANCES];

        public SceneElementManager(RenderView renderView) : base(renderView) { }

        public void AddElement(Material material, RenderElement element)
        {
            if (!Elements.ContainsKey(material))
                Elements.Add(material, new Queue<RenderElement>());
            Elements[material].Enqueue(element);
        }

        void DrawElement(Material key)
        {
            while (Elements[key].Count > 0)
            {
                RenderElement currElem = Elements[key].Dequeue();
                if (currElem.VertexDec != GFX.Device.VertexDeclaration)
                    GFX.Device.VertexDeclaration = currElem.VertexDec;
                GFX.Device.Indices = currElem.IndexBuffer;
                GFX.Device.Vertices[0].SetSource(currElem.VertexBuffer, 0, currElem.VertexStride);
                for (int j = 0; j < currElem.Transform.Length; j += GFXShaderConstants.NUM_INSTANCES)
                {
                    int binLength = currElem.Transform.Length - j;

                    if (binLength > GFXShaderConstants.NUM_INSTANCES)
                        binLength = GFXShaderConstants.NUM_INSTANCES;
                    if (currElem.Transform.Length > 1)
                    {
                        // Upload transform matrices as shader constants.
                        Array.Copy(currElem.Transform, j, tempTransforms, 0, binLength);
                        GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_WORLD, tempTransforms);
                    }
                    else
                    {
                        GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_WORLD, currElem.Transform);
                    }
                    GFX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, currElem.StartVertex, currElem.VertexCount * binLength, 0, currElem.PrimitiveCount * binLength);
                }
            }
        }

        public override void Render()
        {
            GFX.Device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            GFX.Device.RenderState.DepthBufferEnable = true;
            GFX.Device.RenderState.DepthBufferWriteEnable = true;
            GFX.Device.RenderState.DepthBufferFunction = CompareFunction.Less;

            for (int i = 0; i < Elements.Keys.Count; i++)
            {
                Material key = Elements.Keys[i];

                if (Elements[key].Count > 0)
                    key.SetupMaterial();

                DrawElement(key);               
            }
        }
    }
}
