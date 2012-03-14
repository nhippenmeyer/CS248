﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Resources;
using Gaia.Rendering.RenderViews;
using Gaia.Rendering.Geometry;

namespace Gaia.Rendering
{
    public class PostProcessElementManager : RenderElementManager
    {
        Shader basicImageShader;
        Shader compositeShader;
        Shader fogShader;
        Shader motionBlurShader;
        Shader colorCorrectShader;
        Shader godRayShader;
        TextureResource colorCorrectTexture;

        TextureResource oceanTexture;
        Shader oceanShader;
        Shader oceanScreenSpaceShader;
        ProjectiveOcean oceanGeometry;
        Vector4[] bumpCoords;
        Vector4[] waveDirs;
        Vector4[] waveAmplitudes;
        
        Matrix prevViewProjection = Matrix.Identity; //Used for motion blur

        MainRenderView mainRenderView; //Used to access GBuffer

        float waterScale = 120;
        float waveScale = 10;

        public PostProcessElementManager(MainRenderView renderView)
            : base(renderView)
        {
            mainRenderView = renderView;
            basicImageShader = ResourceManager.Inst.GetShader("Generic2D");
            motionBlurShader = ResourceManager.Inst.GetShader("MotionBlur");
            compositeShader = ResourceManager.Inst.GetShader("Composite");
            fogShader = ResourceManager.Inst.GetShader("Fog");
            colorCorrectShader = ResourceManager.Inst.GetShader("ColorCorrect");
            colorCorrectTexture = ResourceManager.Inst.GetTexture("Textures/Color Correction/colorRamp0.dds");
            godRayShader = ResourceManager.Inst.GetShader("GodRay");

            oceanShader = ResourceManager.Inst.GetShader("Ocean");
            oceanScreenSpaceShader = ResourceManager.Inst.GetShader("OceanPP");
            oceanTexture = ResourceManager.Inst.GetTexture("Textures/Water/noise02.dds");
            oceanGeometry = new ProjectiveOcean(64);
            bumpCoords = new Vector4[] 
            {
                new Vector4(0.264000f,0.178000f, 0.2f, 0.1f),
                new Vector4(-2.06840f, -1.52640f, -1.0f, 0.23f),
                new Vector4(1.87920f, 1.9232f, 0.2f, 0.15f),
                new Vector4(0.096000f, 0.04f, -0.3f, 0.1f),
            };
            waveDirs = new Vector4[]
            {
                new Vector4(-0.66f, -0.208f, 0.2f, MathHelper.TwoPi/0.4f),
                new Vector4(-1.62f, 0.1708f, 0.38f, MathHelper.TwoPi/2.2f),
                new Vector4(-2.16f, 1.338f, 0.48f, MathHelper.TwoPi/1.0f),
                new Vector4(-2.72f, 8.6108f, 0.15f, MathHelper.TwoPi/4.8f),
            };
            float ratio = waveScale / waterScale;
            waveAmplitudes = new Vector4[]
            {
                ratio*Vector4.One*0.8f,
                ratio*Vector4.One*0.4f,
                ratio*Vector4.One*0.4f,
                ratio*Vector4.One*0.8f,
            };
        }

        void RenderComposite()
        {
            GFX.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            GFX.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            
            compositeShader.SetupShader();
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_INVTEXRES, Vector2.One / GFX.Inst.DisplayRes);
            GFX.Device.Textures[0] = mainRenderView.ColorMap.GetTexture();
            GFX.Device.Textures[1] = mainRenderView.LightMap.GetTexture();
            GFX.Device.Textures[2] = mainRenderView.DepthMap.GetTexture();

            GFXPrimitives.Quad.Render();

            GFX.Device.RenderState.SourceBlend = Blend.One;
            GFX.Device.RenderState.DestinationBlend = Blend.One;
            basicImageShader.SetupShader();
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_INVTEXRES, Vector2.One / new Vector2(mainRenderView.ParticleBuffer.Width, mainRenderView.ParticleBuffer.Height));
            GFX.Device.Textures[0] = mainRenderView.ParticleBuffer.GetTexture();
            GFXPrimitives.Quad.Render();

            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_INVTEXRES, Vector2.One / GFX.Inst.DisplayRes);
        }

        void RenderFog()
        {
            GFX.Device.RenderState.SourceBlend = Blend.One;
            GFX.Device.RenderState.DestinationBlend = Blend.InverseDestinationColor;

            fogShader.SetupShader();
            GFX.Device.Textures[0] = mainRenderView.DepthMap.GetTexture();
            GFX.Device.SetPixelShaderConstant(0, Vector3.One * 0.5f);
            GFX.Device.SetPixelShaderConstant(1, new Vector3(3.71754f, 0.91412f, 0.197203f)); //Fog parameters 
            
            GFXPrimitives.Quad.Render();
        }

        void RenderMotionBlur()
        {
            GFX.Device.ResolveBackBuffer(mainRenderView.BackBufferTexture);

            motionBlurShader.SetupShader();
            GFX.Device.Textures[0] = mainRenderView.BackBufferTexture;
            GFX.Device.Textures[1] = mainRenderView.DepthMap.GetTexture();
            GFX.Device.SetPixelShaderConstant(0, mainRenderView.GetViewProjection());
            GFX.Device.SetPixelShaderConstant(4, prevViewProjection);

            GFXPrimitives.Quad.Render();
            prevViewProjection = mainRenderView.GetViewProjection();
        }

        void RenderColorCorrection()
        {
            GFX.Device.ResolveBackBuffer(mainRenderView.BackBufferTexture);

            colorCorrectShader.SetupShader();
            GFX.Device.Textures[0] = mainRenderView.BackBufferTexture;
            GFX.Device.Textures[1] = colorCorrectTexture.GetTexture();

            GFXPrimitives.Quad.Render();
        }

        void RenderGodRays()
        {
            GFX.Device.ResolveBackBuffer(mainRenderView.BackBufferTexture);
            
            godRayShader.SetupShader();
            GFX.Device.Textures[0] = mainRenderView.BackBufferTexture;
            GFX.Device.Textures[1] = mainRenderView.DepthMap.GetTexture();
            Vector4 lightVec = new Vector4(mainRenderView.scene.MainLight.Transformation.GetPosition(), 1);
            Vector4 ssSunPos = Vector4.Transform(lightVec, mainRenderView.GetViewProjectionLocal());
            ssSunPos /= ssSunPos.W;
            ssSunPos = ssSunPos*0.5f+Vector4.One*0.5f;
            GFX.Device.SetPixelShaderConstant(0, ssSunPos);
            GFX.Device.SetPixelShaderConstant(1, new Vector3(1.0f, 0.36f, 0.7f));
            GFX.Device.SetPixelShaderConstant(2, Vector3.One);

            GFXPrimitives.Quad.Render();
        }

        void RenderOcean()
        {
            
            GFX.Device.ResolveBackBuffer(mainRenderView.BackBufferTexture);
            GFX.Device.Clear(Color.TransparentBlack);

            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_LIGHTPOS, Vector3.Normalize(mainRenderView.scene.MainLight.Transformation.GetPosition()));
            GFX.Device.SetPixelShaderConstant(0, bumpCoords);
            GFX.Device.Textures[0] = mainRenderView.BackBufferTexture;
            GFX.Device.Textures[1] = mainRenderView.DepthMap.GetTexture();
            GFX.Device.Textures[2] = oceanTexture.GetTexture();
            GFX.Device.Textures[3] = mainRenderView.planarReflection.ReflectionMap.GetTexture();
            GFX.Inst.SetTextureFilter(3, TextureFilter.Anisotropic);
            GFX.Inst.SetTextureAddressMode(3, TextureAddressMode.Clamp);
            GFX.Inst.SetTextureFilter(2, TextureFilter.Anisotropic);
            GFX.Inst.SetTextureAddressMode(2, TextureAddressMode.Wrap);
            GFX.Inst.SetTextureAddressMode(1, TextureAddressMode.Wrap);

            GFX.Device.RenderState.SourceBlend = Blend.One;
            GFX.Device.RenderState.DestinationBlend = Blend.One;

            basicImageShader.SetupShader();
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_INVTEXRES, Vector2.One / GFX.Inst.DisplayRes);
            GFXPrimitives.Quad.Render();

            GFX.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            GFX.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            oceanScreenSpaceShader.SetupShader();
            GFXPrimitives.Quad.Render();
            
            oceanShader.SetupShader();
            GFX.Device.SetVertexShaderConstant(4, waveDirs);
            GFX.Device.SetVertexShaderConstant(33, Vector4.One * waterScale);
            GFX.Device.SetVertexShaderConstant(34, waveAmplitudes);
            Matrix worldMatrix = Matrix.CreateScale(waterScale);
            worldMatrix.Translation = new Vector3(1, 0, 1) * mainRenderView.GetPosition();
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_WORLD, worldMatrix);
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, mainRenderView.GetViewProjection());
            GFX.Device.RenderState.DepthBufferEnable = true;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;
            GFX.Device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            oceanGeometry.Render();
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.CullMode = CullMode.None;

            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, mainRenderView.GetInverseViewProjectionLocal());
            GFX.Inst.SetTextureFilter(3, TextureFilter.Point);
            GFX.Inst.SetTextureAddressMode(3, TextureAddressMode.Clamp);
            GFX.Inst.SetTextureFilter(2, TextureFilter.Point);
            GFX.Inst.SetTextureAddressMode(2, TextureAddressMode.Clamp);
            GFX.Inst.SetTextureAddressMode(1, TextureAddressMode.Clamp);


        }

        public override void Render()
        {
            for(int i = 0; i < 4; i++)
            {
                GFX.Inst.SetTextureFilter(i, TextureFilter.Point);
                GFX.Inst.SetTextureAddressMode(i, TextureAddressMode.Clamp);
            }
            GFX.Device.RenderState.CullMode = CullMode.None;
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;
            GFX.Device.RenderState.AlphaBlendEnable = true;

            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, mainRenderView.GetInverseViewProjectionLocal());
            
            RenderComposite();

            RenderOcean();

            //RenderFog();

            //RenderGodRays();

            GFX.Device.RenderState.AlphaBlendEnable = false;

            RenderColorCorrection();

            RenderMotionBlur();

            GFX.Inst.ResetState();
        }
    }
}
