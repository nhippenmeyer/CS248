using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.SceneGraph;
using Gaia.Resources;
namespace Gaia.Rendering.RenderViews
{
    public class MainRenderView : RenderView
    {
        public RenderTarget2D ColorMap;
        public RenderTarget2D DepthMap;
        public RenderTarget2D NormalMap;
        public RenderTarget2D DataMap;
        public RenderTarget2D LightMap;
        public RenderTarget2D GlowBuffer;

        public ResolveTexture2D BackBufferTexture;

        Matrix TexGen;
        public Scene scene;

        public MainRenderView(Scene scene, Matrix view, Matrix projection, Vector3 position, float nearPlane, float farPlane)
            : base(RenderViewType.MAIN, view, projection, position, nearPlane, farPlane)
        {
            this.scene = scene;
            this.ElementManagers.Add(RenderPass.Sky, new SkyElementManager(this));
            this.ElementManagers.Add(RenderPass.Scene, new SceneElementManager(this));
            this.ElementManagers.Add(RenderPass.Emissive, new SceneElementManager(this));
            this.ElementManagers.Add(RenderPass.PostProcess, new PostProcessElementManager(this));
            this.ElementManagers.Add(RenderPass.Light, new LightElementManager(this));

            InitializeTextures();
        }

        void InitializeTextures()
        {
            int width = GFX.Device.PresentationParameters.BackBufferWidth;
            int height = GFX.Device.PresentationParameters.BackBufferHeight;

            TexGen = GFX.Inst.ComputeTextureMatrix(new Vector2(width, height));

            ColorMap = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.Color);
            DepthMap = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.Single);
            NormalMap = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.HalfVector2);
            DataMap = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.Color);
            LightMap = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.Color);

            GlowBuffer = new RenderTarget2D(GFX.Device, width / 4, height / 4, 1, SurfaceFormat.Color);

            BackBufferTexture = new ResolveTexture2D(GFX.Device, width, height, 1, SurfaceFormat.Color);//GFX.Device.PresentationParameters.BackBufferFormat);
        }

        public override void AddElement(Material material, RenderElement element)
        {
            if (material.IsTranslucent)
            {
                TransparentElementManager transMgr = (TransparentElementManager)ElementManagers[RenderPass.Translucent];
                transMgr.AddElement(material, element);
            }
            else
            {
                if(material.IsEmissive)
                {
                    Material mat = ResourceManager.Inst.GetMaterial(material.EmissiveMaterial);
                    if(mat == null)
                        mat = material;
                    SceneElementManager glowMgr = (SceneElementManager)ElementManagers[RenderPass.Emissive];
                    glowMgr.AddElement(mat, element);
                }
                else
                {
                    SceneElementManager sceneMgr = (SceneElementManager)ElementManagers[RenderPass.Scene];
                    sceneMgr.AddElement(material, element);
                }
            }
        }

        public override void Render()
        {
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, GetViewProjection());
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_EYEPOS, GetEyePosShader());
            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_EYEPOS, GetEyePosShader());

            GFX.Device.SetRenderTarget(0, ColorMap);
            GFX.Device.SetRenderTarget(1, NormalMap);
            GFX.Device.SetRenderTarget(2, DepthMap);
            GFX.Device.SetRenderTarget(3, DataMap);

            GFX.Device.Clear(Color.TransparentBlack);

            ElementManagers[RenderPass.Scene].Render();

            GFX.Device.SetRenderTarget(0, null);
            GFX.Device.SetRenderTarget(1, null);
            GFX.Device.SetRenderTarget(2, null);
            GFX.Device.SetRenderTarget(3, null);


            GFX.Device.Textures[0] = NormalMap.GetTexture();
            GFX.Device.Textures[1] = DepthMap.GetTexture();
            GFX.Device.Textures[2] = DataMap.GetTexture();

            GFX.Device.SetRenderTarget(0, LightMap);
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_TEXGEN, TexGen);
            GFX.Device.Clear(Color.TransparentBlack);
            ElementManagers[RenderPass.Light].Render();
            GFX.Device.SetRenderTarget(0, null);

            GFX.Device.Clear(Color.TransparentBlack);
            GFX.Device.SetPixelShaderConstant(3, scene.MainLight.Transformation.GetPosition()); //Light Direction for sky
            ElementManagers[RenderPass.Sky].Render(); //This'll change the modelview

            ElementManagers[RenderPass.PostProcess].Render();
            
        }
    }
}
