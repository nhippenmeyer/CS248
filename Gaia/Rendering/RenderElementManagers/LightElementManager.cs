using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Resources;
using Gaia.Rendering.RenderViews;
using Gaia.SceneGraph.GameEntities;
namespace Gaia.Rendering
{
    public class LightElementManager : RenderElementManager
    {

        public Queue<Light> DirectionalLights = new Queue<Light>();
        public Queue<Light> PointLights = new Queue<Light>();
        public Queue<Light> SpotLights = new Queue<Light>();

        Shader directionalLightShader;
        Shader directionalLightShadowsShader;
        Shader pointLightShader;
        Shader spotLightShader;

        public LightElementManager(RenderView renderView)
            : base(renderView)
        {
            directionalLightShader = ResourceManager.Inst.GetShader("DirectionalLightShader");
            directionalLightShadowsShader = ResourceManager.Inst.GetShader("PointLightShader");
            pointLightShader = ResourceManager.Inst.GetShader("PointLightShader");
            spotLightShader = ResourceManager.Inst.GetShader("PointLightShader");
        }

        void SetupLightParameters(Light light)
        {
            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_LIGHTPOS, light.Transformation.GetPosition());
            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_LIGHTCOLOR, light.Color);
            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_LIGHTPARAMS, light.Parameters);
        }

        public override void Render()
        {

            GFX.Device.RenderState.AlphaBlendEnable = true;
            GFX.Device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            GFX.Device.RenderState.AlphaSourceBlend = Blend.One;
            GFX.Device.RenderState.AlphaDestinationBlend = Blend.One;
            GFX.Device.RenderState.BlendFunction = BlendFunction.Add;
            GFX.Device.RenderState.SourceBlend = Blend.One;
            GFX.Device.RenderState.DestinationBlend = Blend.One;
            GFX.Device.RenderState.DepthBufferEnable = false;

            GFX.Device.RenderState.CullMode = CullMode.None;

            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PDec;

            directionalLightShader.SetupShader();

            GFX.Device.SetPixelShaderConstant(3, Vector3.One); //Light Direction
            GFX.Device.SetPixelShaderConstant(4, Vector4.One * -3.0f); //Exposure
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, renderView.GetViewProjectionLocal());

            while (DirectionalLights.Count > 0)
            {
                Light currLight = DirectionalLights.Dequeue();
                SetupLightParameters(currLight);
                GFXPrimitives.Cube.Render();
            }

            GFX.Device.RenderState.CullMode = CullMode.CullClockwiseFace;
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, renderView.GetViewProjection());

            pointLightShader.SetupShader();
            while (PointLights.Count > 0)
            {
                Light currLight = PointLights.Dequeue();
                SetupLightParameters(currLight);
                GFXPrimitives.Cube.Render();
            }

            spotLightShader.SetupShader();
            while (SpotLights.Count > 0)
            {
                Light currLight = SpotLights.Dequeue();
                SetupLightParameters(currLight);
                GFXPrimitives.Cube.Render();
            }

            GFX.Inst.ResetState();
        }
    }
}
