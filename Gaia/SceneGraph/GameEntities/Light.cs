﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Rendering;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public enum LightType
    {
        Directional,
        Point,
        Spot,
    };

    public class Light : Entity
    {
        Vector4 parameters;
        public Vector3 Color;

        LightType type = LightType.Directional;

        bool castsShadows;
        ShadowRenderView renderView;

        public Light(LightType lightType, Vector3 color, Vector3 position, bool castsShadows)
            : base()
        {
            this.type = lightType;
            this.Color = color;
            this.Transformation.SetPosition(position);
            this.castsShadows = castsShadows;
        }

        public Vector4 Parameters
        {
            get { return parameters; }
            set 
            {
                parameters = value;
                float maxScale = Math.Max(parameters.X, parameters.Y);
                Transformation.SetScale(Vector3.One * maxScale);
            }
        }

        public override void OnRender(RenderView view)
        {
            bool canRender = (view.GetFrustum().Contains(Transformation.GetBounds()) != ContainmentType.Disjoint);
            canRender |= (type == LightType.Directional);
            if (canRender)
            {
                LightElementManager lightMgr = (LightElementManager)view.GetRenderElementManager(RenderPass.Light);
                if (lightMgr != null)
                {
                    switch (type)
                    {
                        case LightType.Directional:
                            lightMgr.DirectionalLights.Enqueue(this);
                            break;
                        case LightType.Point:
                            lightMgr.PointLights.Enqueue(this);
                            break;
                        case LightType.Spot:
                            lightMgr.SpotLights.Enqueue(this);
                            break;
                    }
                }
            }
            base.OnRender(view);
        }


    }
}
