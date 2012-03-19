using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;
using Gaia.Physics;
using Gaia.Core;


namespace Gaia.SceneGraph.GameEntities
{
    public class Health : Entity
    {
        public ParticleEffect tracerEffect;
        public ParticleEffect explosionEffect;

        State physicsState;
        bool exploded = false;
        ParticleEmitter tracerEmitter;
        
        BoundingBox boundingBox;
        Vector3 minPos, maxPos;
        bool collected;

        public Health()
        {
            tracerEffect = ResourceManager.Inst.GetParticleEffect("HealthParticle");
        }

        public override void OnAdd(Scene scene)
        {
           
            Random randomHelper = new Random();
            Vector3 randPosition = Vector3.Zero;
            Vector3 randNormal = Vector3.Zero;
            randomHelper.NextDouble();
            randomHelper.NextDouble();
            scene.MainTerrain.GenerateRandomTransform(randomHelper, out randPosition, out randNormal);

            physicsState.position = randPosition;

            tracerEmitter = new ParticleEmitter(tracerEffect, 200);
            scene.Entities.Add(tracerEmitter);
            tracerEmitter.OnAdd(scene);
            Console.WriteLine("Added particle at position: \n {0}", physicsState.position);
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            scene.Entities.Remove(tracerEmitter);
            tracerEmitter.OnDestroy();
            base.OnDestroy();
        }
        /*
        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            if (frustum.Contains(boundingBox) != ContainmentType.Disjoint && !collected)
            {
             //   view.AddElement(gemMaterial, gemGeometry.renderElement);
                
            }
            base.OnRender(view);
        }*/

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
            //    scene.Entities.Remove(emitterLight);
                // increase player's health
            }
            base.OnUpdate();
        }
    }
}