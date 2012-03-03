using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gaia.Rendering.RenderViews;

namespace Gaia.Rendering
{
    public struct SkyRenderElement
    {
        public Vector3 rayleighColor;

        public float rayleighHeight;

        public Vector3 mieColor;

        public float mieHeight;
    }

    public class SkyElementManager : RenderElementManager
    {
        public List<SkyRenderElement> Elements = new List<SkyRenderElement>();

        Effect SkyShader;
        RenderTarget2D SkyTexture;

        public SkyElementManager(RenderView renderView)
            : base(renderView)
        {
            //SkyShader = GFX.Inst.Content.Load<Effect>("Shaders/Sky");
            SkyTexture = new RenderTarget2D(GFX.Device, 64, 64, 1, SurfaceFormat.Color);
        }

        public override void Render()
        {
            GFX.Device.RenderState.CullMode = CullMode.None;
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;
            //GFX.Device.RenderState.
            //GFX.Device.RenderState.DepthBufferFunction = CompareFunction.Always;// = true;
            GFX.Device.RenderState.AlphaBlendEnable = true;
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PTDec;
            GFX.Device.SetRenderTarget(0, SkyTexture);
            GFX.Device.Clear(Color.Black);
            SkyShader.CurrentTechnique = SkyShader.Techniques["SkyEffect"];
            SkyShader.Parameters["LightPosition"].SetValue(Vector3.One);
            SkyShader.Parameters["fExposure"].SetValue(-3);
            SkyShader.Parameters["ModelView"].SetValue(renderView.GetViewProjectionLocal());
            for (int i = 0; i < Elements.Count; i++)
            {
                SkyShader.Parameters["InvWavelength"].SetValue(Elements[i].rayleighColor);
                SkyShader.Parameters["WavelengthMie"].SetValue(Elements[i].mieColor);
                SkyShader.Parameters["HeightParams"].SetValue(new Vector2(Elements[i].rayleighHeight, Elements[i].mieHeight));
                SkyShader.CommitChanges();
                SkyShader.Begin();
                SkyShader.CurrentTechnique.Passes[0].Begin();
                GFXPrimitives.Cube.Render();
                SkyShader.CurrentTechnique.Passes[0].End();
                SkyShader.End();
            }
            GFX.Device.SetRenderTarget(0, null);
            //GFX.Device.RenderState.AlphaBlendEnable = false;
            GFX.Device.Clear(Color.Black);
            GFX.Device.RenderState.CullMode = CullMode.None;
            //GFX.Device.RenderState.AlphaBlendEnable = true;
            SkyShader.Parameters["RayleighTexture"].SetValue(SkyTexture.GetTexture());
            SkyShader.CurrentTechnique = SkyShader.Techniques["SunEffect"];
            SkyShader.CommitChanges();
            for (int i = 0; i < Elements.Count; i++)
            {
                SkyShader.Begin();
                SkyShader.CurrentTechnique.Passes[0].Begin();
                GFXPrimitives.Cube.Render();
                SkyShader.CurrentTechnique.Passes[0].End();
                SkyShader.End();
            }

            //RenderClouds();

            GFX.Device.RenderState.AlphaBlendEnable = false;
            GFX.Inst.ResetState();
            Elements.Clear();
        }
    }
}
