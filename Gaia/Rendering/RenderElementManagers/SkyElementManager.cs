﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Resources;
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

        Shader skyShaderPrepass;
        Shader skyShader;
        RenderTarget2D skyTexture;

        public SkyElementManager(RenderView renderView)
            : base(renderView)
        {
            skyShaderPrepass = ResourceManager.Inst.GetShader("SkyShaderPrepass");
            skyShader = ResourceManager.Inst.GetShader("SkyShader");

            skyTexture = new RenderTarget2D(GFX.Device, 64, 64, 1, SurfaceFormat.Color);
        }

        public override void Render()
        {
            GFX.Device.RenderState.CullMode = CullMode.None;
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;

            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PTDec;
            GFX.Device.SetRenderTarget(0, skyTexture);
            GFX.Device.Clear(Color.Black);

            skyShaderPrepass.SetupShader();

            GFX.Device.SetPixelShaderConstant(3, Vector3.One); //Light Direction
            GFX.Device.SetPixelShaderConstant(4, Vector4.One * -3.0f); //Exposure
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, renderView.GetViewProjectionLocal());
            
            for (int i = 0; i < Elements.Count; i++)
            {
                GFX.Device.SetPixelShaderConstant(0, Elements[i].rayleighColor); //Rayleigh Term
                GFX.Device.SetPixelShaderConstant(1, Elements[i].mieColor);      //Mie Term
                GFX.Device.SetPixelShaderConstant(2, new Vector2(Elements[i].rayleighHeight, Elements[i].mieHeight)); //Height Term
                GFXPrimitives.Cube.Render();
            }
            GFX.Device.SetRenderTarget(0, null);

            GFX.Device.Clear(Color.Black);

            skyShader.SetupShader();
 
            GFX.Device.Textures[0] = skyTexture.GetTexture();
            GFX.Device.SetPixelShaderConstant(0, Vector2.One / new Vector2(skyTexture.Width, skyTexture.Height));

            GFXPrimitives.Cube.Render();

            //RenderClouds();

            GFX.Inst.ResetState();
            Elements.Clear();
        }
    }
}
