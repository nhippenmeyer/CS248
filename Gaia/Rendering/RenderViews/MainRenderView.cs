using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Gaia.Rendering.RenderViews
{
    public class MainRenderView : RenderView
    {
        public MainRenderView(Matrix view, Matrix projection, Vector3 position, float nearPlane, float farPlane)
            : base(view, projection, position, nearPlane, farPlane)
        {
            this.ElementManagers.Add(RenderPass.Sky, new SkyElementManager(this));
            this.ElementManagers.Add(RenderPass.Scene, new SceneElementManager(this));
        }

        public override void Render()
        {

            ElementManagers[RenderPass.Sky].Render(); //This'll change the modelview
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, GetViewProjection());
            ElementManagers[RenderPass.Scene].Render();
        }
    }
}
