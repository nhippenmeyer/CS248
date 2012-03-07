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
        Shader godRayShader;
        TextureResource colorCorrectTexture;

        public RenderTarget2D ShadowMap;

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
            godRayShader = ResourceManager.Inst.GetShader("GodRay");
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
            //GFX.Device.Textures[0] = ShadowMap.GetTexture();
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

            //RenderGodRays();
            
            GFX.Device.RenderState.AlphaBlendEnable = false;            

            RenderColorCorrection();

            RenderMotionBlur();

            GFX.Inst.ResetState();
        }
    }
}
