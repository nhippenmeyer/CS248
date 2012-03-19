using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;

namespace Gaia.SceneGraph.GameEntities
{
    public class Projectile : Entity
    {
        public ParticleEffect tracerEffect;
        public ParticleEffect explosionEffect;

        /*
        public override void OnAdd(Scene scene)
        {
            renderView = new MainRenderView(scene, Matrix.Identity, Matrix.Identity, Vector3.Zero, 1.0f, 1000);

            position = Vector3.Transform(Vector3.Up * 0.25f, scene.MainTerrain.Transformation.GetTransform());
            scene.MainCamera = renderView;
            scene.AddRenderView(renderView);

            fieldOfView = MathHelper.ToRadians(70);
            aspectRatio = GFX.Inst.DisplayRes.X / GFX.Inst.DisplayRes.Y;

            emitter = new ParticleEmitter(Resources.ResourceManager.Inst.GetParticleEffect("PlayerParticles"), 60);
            emitterLight = new Light(LightType.Point, new Vector3(0.13f, 0.86f, 1.26f), position, false);
            emitterLight.Parameters = new Vector4(55, 50, 0, 0);
            scene.Entities.Add(emitter);
            scene.Entities.Add(emitterLight);
            physicsState.position = position;
            physicsState.velocity = Vector3.Zero;

            base.OnAdd(scene);
        }
        */
    }
}
