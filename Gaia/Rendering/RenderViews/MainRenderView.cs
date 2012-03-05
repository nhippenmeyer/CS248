using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;
namespace Gaia.Rendering.RenderViews
{
    public class MainRenderView : RenderView
    {
        public MainRenderView(Matrix view, Matrix projection, Vector3 position, float nearPlane, float farPlane)
            : base(view, projection, position, nearPlane, farPlane)
        {
            this.ElementManagers.Add(RenderPass.Sky, new SkyElementManager(this));
            this.ElementManagers.Add(RenderPass.Scene, new SceneElementManager(this));
            this.ElementManagers.Add(RenderPass.Emissive, new SceneElementManager(this));
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
            ElementManagers[RenderPass.Sky].Render(); //This'll change the modelview
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, GetViewProjection());
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_EYEPOS, GetEyePosShader());
            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_EYEPOS, GetEyePosShader());
            ElementManagers[RenderPass.Scene].Render();
        }
    }
}
