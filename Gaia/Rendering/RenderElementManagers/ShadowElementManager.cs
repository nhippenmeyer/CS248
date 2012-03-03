using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Gaia.Rendering.RenderViews;
using Gaia.Resources;
namespace Gaia.Rendering
{
    public class ShadowElementManager : RenderElementManager
    {
        public SortedList<Material, Queue<RenderElement>> Elements = new SortedList<Material, Queue<RenderElement>>();

        Shader shadowShader;

        public ShadowElementManager(RenderView renderView)
            : base(renderView)
        {
            shadowShader = new Shader();
            shadowShader.CompileFromFiles("Shaders/Lighting/ShadowP.hlsl", "Shaders/Lighting/ShadowV.hlsl");
        }

        public override void Render()
        {
            GFX.Device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            GFX.Device.RenderState.DepthBufferEnable = true;
            GFX.Device.RenderState.DepthBufferWriteEnable = true;
            GFX.Device.RenderState.DepthBufferFunction = CompareFunction.Less;

            shadowShader.SetupShader();
            for (int i = 0; i < Elements.Keys.Count; i++)
            {
                Material key = Elements.Keys[i];
                while (Elements[key].Count > 0)
                {
                    RenderElement currElem = Elements[key].Dequeue();
                    if (currElem.VertexDec != GFX.Device.VertexDeclaration)
                        GFX.Device.VertexDeclaration = currElem.VertexDec;
                    GFX.Device.Indices = currElem.IndexBuffer;
                    GFX.Device.Vertices[0].SetSource(currElem.VertexBuffer, 0, currElem.VertexStride);
                    GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_WORLD, currElem.Transform);
                    GFX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, currElem.StartVertex, currElem.VertexCount, 0, currElem.PrimitiveCount);
                }
            }
        }
    }
}
