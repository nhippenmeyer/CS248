using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;
using Gaia.Physics;
using Gaia.Core;

namespace Gaia.SceneGraph.GameEntities
{
    public class Projectile : Entity
    {
        public ParticleEffect tracerEffect;
        public ParticleEffect explosionEffect;

        State physicsState;
        bool exploded = false;
        ParticleEmitter tracerEmitter;
        ParticleEmitter explosionEmitter;
        float deleteTime;

        public Projectile(string tracerEffectName, string explosionEffectName)
        {
            tracerEffect = ResourceManager.Inst.GetParticleEffect(tracerEffectName);
            explosionEffect = ResourceManager.Inst.GetParticleEffect(explosionEffectName);
        }

        public override void OnAdd(Scene scene)
        {
            physicsState.position = this.Transformation.GetPosition();

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

        public void SetVelocity(Vector3 velocity)
        {
            physicsState.velocity = velocity * tracerEffect.initialSpeed;
        }

        public override void OnUpdate()
        {
            Vector3 acceleration = Vector3.Up * -9.82f; //Gravity!
            State newState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);

            Vector3 collNormal = Vector3.Zero;
            if (scene.MainTerrain.IsCollision(newState.position, out collNormal))
            {
                if(!exploded)
                {
                    exploded = true;
                    scene.MainTerrain.CarveTerrainAtPoint(newState.position, 3, -45);
                    tracerEmitter.EmitOnce = true;
                    this.scene.Entities.Remove(this);
                    this.OnDestroy();
                    Console.WriteLine("Destroyed some terrain!");
                }
            }
            else
            {
                physicsState = newState;
                tracerEmitter.Transformation.SetPosition(physicsState.position);
                tracerEmitter.Transformation.SetRotation(this.Transformation.GetRotation());
                foreach (Entity e in scene.Entities)
                {
                    if (e.GetType().FullName == "Actor")
                    {
                        if ((e.Transformation.GetPosition() - this.Transformation.GetPosition()).Length() < 10)
                        {
                            this.scene.Entities.Remove(e);
                            this.scene.Entities.Remove(this);
                            e.OnDestroy();
                            this.OnDestroy();
                            Console.WriteLine("Destroyed an actor");
                        }
                    }
                }
            }

            base.OnUpdate();
        }
        
    }
}
