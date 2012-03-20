using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Resources;
using Gaia.Physics;
using Gaia.Core;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;


namespace Gaia.SceneGraph.GameEntities
{
    public class Health : Entity
    {
        public ParticleEffect tracerEffect;
        public ParticleEffect explosionEffect;

        State physicsState;
        ParticleEmitter tracerEmitter;

        Random randomHelper;
        
        BoundingBox boundingBox;
        bool collected;

        public Health(Random random)
        {
            randomHelper = random;
            tracerEffect = ResourceManager.Inst.GetParticleEffect("HealthParticle");
        }

        public override void OnAdd(Scene scene)
        {
            Vector3 randPosition = Vector3.Zero;
            Vector3 randNormal = Vector3.Zero;
            randomHelper.NextDouble();
            scene.MainTerrain.GenerateRandomTransform(randomHelper, out randPosition, out randNormal);

            physicsState.position = randPosition;

            tracerEmitter = new ParticleEmitter(tracerEffect, 8);
            scene.Entities.Add(tracerEmitter);
            tracerEmitter.OnAdd(scene);

            boundingBox = new BoundingBox();
            float size = tracerEmitter.GetTextureSize();
            boundingBox.Min = randPosition - Vector3.One * Vector3.Up * size;
            boundingBox.Min.X -= (size);// / 2.0f);
            boundingBox.Min.Z -= (size); // 2.0f);
            boundingBox.Max = randPosition + Vector3.One * Vector3.Up * size;
            boundingBox.Max.X += (size); // 2.0f);
            boundingBox.Max.Z += (size); // 2.0f);

            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            scene.Entities.Remove(tracerEmitter);
            tracerEmitter.OnDestroy();
            base.OnDestroy();
        }
        
        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            if (frustum.Contains(boundingBox) != ContainmentType.Disjoint && !collected)
            {

            }
            base.OnRender(view);
        }

        public void SetVelocity(Vector3 velocity)
        {
            physicsState.velocity = velocity * tracerEffect.initialSpeed;
        }

        public override void OnUpdate()
        {
            Vector3 acceleration = Vector3.Zero; //Gravity!
            State newState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);
           
                physicsState = newState;
                tracerEmitter.Transformation.SetPosition(physicsState.position);
                tracerEmitter.Transformation.SetRotation(this.Transformation.GetRotation());


            if (boundingBox.Contains(scene.MainCamera.GetPosition()) != ContainmentType.Disjoint && !collected)
            {
                collected = true;
                scene.Entities.Remove(tracerEmitter);
                Console.WriteLine("Heath absorbed");
                scene.MainPlayer.ApplyHealth(20.0f);
                // increase player's health
            }
            base.OnUpdate();
        }
    }
}