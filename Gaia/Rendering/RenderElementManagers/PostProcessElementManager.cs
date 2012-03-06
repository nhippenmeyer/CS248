using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Resources;
using Gaia.Rendering.RenderViews;

namespace Gaia.Rendering
{
    public class PostProcessElementManager : RenderElementManager
    {
        Shader compositeShader;
        Shader fogShader;
        Shader motionBlurShader;
        Shader colorCorrectShader;
        TextureResource colorCorrectTexture;

        Matrix prevViewProjection = Matrix.Identity; //Used for motion blur

        MainRenderView mainRenderView; //Used to access GBuffer

        public PostProcessElementManager(MainRenderView renderView)
            : base(renderView)
        {
            mainRenderView = renderView;
            motionBlurShader = ResourceManager.Inst.GetShader("MotionBlur");
            compositeShader = ResourceManager.Inst.GetShader("Composite");
            fogShader = ResourceManager.Inst.GetShader("Fog");
            colorCorrectShader = ResourceManager.Inst.GetShader("ColorCorrect");
            colorCorrectTexture = ResourceManager.Inst.GetTexture("Textures/Color Correction/colorRamp0.dds");
        }

        void RenderComposite()
        {
            GFX.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            GFX.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            
            compositeShader.SetupShader();
            GFX.Device.Textures[0] = mainRenderView.ColorMap.GetTexture();
            GFX.Device.Textures[1] = mainRenderView.LightMap.GetTexture();
            GFX.Device.Textures[2] = mainRenderView.DepthMap.GetTexture();

            GFXPrimitives.Quad.Render();
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

        public override void Render()
        {
            for(int i = 0; i < 4; i++)
            {
                GFX.Device.SamplerStates[i].AddressU = TextureAddressMode.Clamp;
                GFX.Device.SamplerStates[i].AddressV = TextureAddressMode.Clamp;
                GFX.Device.SamplerStates[i].MagFilter = TextureFilter.Point;
                GFX.Device.SamplerStates[i].MinFilter = TextureFilter.Point;
                GFX.Device.SamplerStates[i].MipFilter = TextureFilter.None;
            }
            GFX.Device.RenderState.CullMode = CullMode.None;
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;
            GFX.Device.RenderState.AlphaBlendEnable = true;

            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_INVTEXRES, Vector2.One / GFX.Inst.DisplayRes);

            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, mainRenderView.GetInverseViewProjectionLocal());
            
            RenderComposite();

            //RenderFog();

            GFX.Device.RenderState.AlphaBlendEnable = false;

            RenderColorCorrection();

            RenderMotionBlur();

            GFX.Inst.ResetState();
        }
    }
}
