using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Rendering;
using Gaia.Rendering.RenderViews;
using Gaia.Resources;

namespace Gaia.SceneGraph.GameEntities
{
    public class ParticleEmitter : Entity
    {
        int particleCount = 3000;

        public RenderTarget2D sizeData;
        public Texture2D positionData;
        public Texture2D velocityData;
        public RenderTarget2D colorData;

        public RenderTarget2D positionTarget;
        public RenderTarget2D velocityTarget;

        public bool EmitOnce = false;

        ParticleEffect particleEffect;

        public ParticleEffect GetParticleEffect()
        {
            return particleEffect;
        }

        public int GetParticleCount()
        {
            return particleCount;
        }

        public int GetTextureSize()
        {
            return textureSize;
        }

        int textureSize;

        public ParticleEmitter(ParticleEffect particleEffect, int particleCount)
            : base()
        {
            this.particleEffect = particleEffect;
            this.particleCount = particleCount;
            ComputeTextures();
        }

        void ComputeTextures()
        {
            textureSize = (int)Math.Sqrt(particleCount - (particleCount % 2)) + 1;
            positionTarget = new RenderTarget2D(GFX.Device, textureSize, textureSize, 1, SurfaceFormat.Vector4);
            velocityTarget = new RenderTarget2D(GFX.Device, textureSize, textureSize, 1, SurfaceFormat.Vector4);
            colorData = new RenderTarget2D(GFX.Device, textureSize, textureSize, 1, SurfaceFormat.Color);
            sizeData = new RenderTarget2D(GFX.Device, textureSize, textureSize, 1, SurfaceFormat.Single);
        }

        public override void OnAdd(Scene scene)
        {
            GFX.Inst.particleSystem.AddEmitter(this);
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            GFX.Inst.particleSystem.RemoveEmitter(this);
            base.OnDestroy();
        }

        public override void OnRender(RenderView view)
        {
            ParticleElementManager particleMgr = (ParticleElementManager)view.GetRenderElementManager(RenderPass.Particles);
            if (particleMgr != null)
            {
                particleMgr.AddElement(particleEffect.material, this);
            }
            base.OnRender(view);
        }
    }
}
