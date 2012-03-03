using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaia.Rendering.RenderViews
{
    public class ShadowRenderView : RenderView
    {
        public ShadowRenderView(Matrix view, Matrix projection, Vector3 position, float nearPlane, float farPlane)
            : base(view, projection, position, nearPlane, farPlane)
        {
            this.ElementManagers.Add(RenderPass.Shadows, new ShadowElementManager(this));
        }
    }
}
