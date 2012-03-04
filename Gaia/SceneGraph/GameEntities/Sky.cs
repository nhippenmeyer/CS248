using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Rendering;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public class Sky : Entity
    {
        const float invFactor = 0.0001f;
        const float Factor = 1.0f / invFactor;
        SkyRenderElement renderElement;

        public override void OnAdd(Scene scene)
        {
            renderElement = new SkyRenderElement();
            renderElement.mieHeight = 0.0015f;
            renderElement.rayleighHeight = 0.0055f;
            SetColor(new Vector3(0.65f, 0.57f, 0.475f));

            base.OnAdd(scene);
        }

        public override void OnRender(RenderView view)
        {
            SkyElementManager s = (SkyElementManager)view.GetRenderElementManager(RenderPass.Sky);
            if (s != null)
                s.Elements.Add(renderElement);
            base.OnRender(view);
        }

        public float MieHeight
        {
            get { return renderElement.mieHeight * Factor; }
            set { renderElement.mieHeight = value * invFactor; }
        }

        public float RayleighHeight
        {
            get { return renderElement.rayleighHeight * Factor; }
            set { renderElement.rayleighHeight = value * invFactor; }
        }

        void SetColor(Vector3 color)
        {
            renderElement.rayleighColor.X = 1.0f / (float)Math.Pow((double)color.X, 4.0);
            renderElement.rayleighColor.Y = 1.0f / (float)Math.Pow((double)color.Y, 4.0);
            renderElement.rayleighColor.Z = 1.0f / (float)Math.Pow((double)color.Z, 4.0);

            renderElement.mieColor.X = (float)Math.Pow((double)color.X, -0.84);
            renderElement.mieColor.Y = (float)Math.Pow((double)color.Y, -0.84);
            renderElement.mieColor.Z = (float)Math.Pow((double)color.Z, -0.84);
        }
    }
}
