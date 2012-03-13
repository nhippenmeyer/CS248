﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.SceneGraph.GameEntities;
using Gaia.Rendering.RenderViews;
using Gaia.Resources;
namespace Gaia.Rendering
{
    public class ParticleElementManager : RenderElementManager
    {
        SortedList<Material, Queue<ParticleEmitter>> Elements = new SortedList<Material, Queue<ParticleEmitter>>();

        public ParticleElementManager(RenderView renderView) : base(renderView) { }

        public void AddElement(Material material, ParticleEmitter element)
        {
            if (!Elements.ContainsKey(material))
                Elements.Add(material, new Queue<ParticleEmitter>());
            Elements[material].Enqueue(element);
        }

        public override void Render()
        {
            GFX.Inst.ResetState();
            GFX.Inst.SetTextureFilter(0, TextureFilter.Point);
            GFX.Device.RenderState.CullMode = CullMode.None;
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;
            GFX.Device.RenderState.AlphaBlendEnable = true;

            GFX.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            GFX.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            GFX.Device.RenderState.SeparateAlphaBlendEnabled = true;
            GFX.Device.RenderState.AlphaSourceBlend = Blend.Zero;
            GFX.Device.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.ParticlesDec;
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, renderView.GetViewProjection());
            GFX.Device.SetPixelShaderConstant(GFXShaderConstants.PC_EYEPOS, renderView.GetEyePosShader());
            GFX.Device.RenderState.PointSpriteEnable = true;
            for (int i = 0; i < Elements.Keys.Count; i++)
            {
                Material key = Elements.Keys[i];

                if (Elements[key].Count > 0)
                    key.SetupMaterial();
                while(Elements[key].Count > 0)
                {
                    
                    ParticleEmitter emitter = Elements[key].Dequeue();
                    ParticleEffect effect = emitter.GetParticleEffect();
                    GFXPrimitives.Particle.UpdateParticles(emitter.GetTextureSize());
                    GFX.Device.VertexTextures[0] = emitter.positionData;
                    //GFX.Device.Textures[0] = emitter.colorData.GetTexture();
                    GFX.Device.SetVertexShaderConstant(4, Vector4.One*effect.size*GFX.Inst.DisplayRes.X);
                    GFX.Device.SetVertexShaderConstant(5, new Vector4(effect.fadeInPercent, effect.fadeInCoeff, effect.fadeOutPercent, effect.fadeOutCoeff));
                    GFX.Device.SetVertexShaderConstant(6, new Vector4(effect.lifetime, effect.lifetimeVariance, 0, 0));
                    
                    GFX.Device.SetPixelShaderConstant(1, new Vector4(effect.lifetime, effect.lifetimeVariance, effect.densityRatio, 0));
                    GFX.Device.SetPixelShaderConstant(2, new Vector4(effect.fadeInPercent, effect.fadeInCoeff, effect.fadeOutPercent, effect.fadeOutCoeff));
                    GFX.Device.DrawUserPrimitives<VertexParticles>(PrimitiveType.PointList, GFXPrimitives.Particle.particles, 0, emitter.GetParticleCount());
                }
            }

            GFX.Device.VertexTextures[0] = null;
            GFX.Device.Textures[0] = null;
            GFX.Device.RenderState.SeparateAlphaBlendEnabled = false;
            GFX.Device.RenderState.AlphaBlendEnable = false;
            GFX.Device.RenderState.PointSpriteEnable = false;
            GFX.Inst.ResetState();
        }
        
    }
}