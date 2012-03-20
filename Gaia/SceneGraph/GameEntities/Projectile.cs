using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Resources;
using Gaia.Physics;
using Gaia.Core;

using Gaia.Rendering;

namespace Gaia.SceneGraph.GameEntities
{
    public class Projectile : Entity
    {
        //public ParticleEffect tracerEffect;
        public ParticleEffect explosionEffect;

        Material projectileMaterial;

        public static float EXPLOSION_MAX_MAGNITUDE = 6.5f;

        State physicsState;
        bool exploded = false;
        RenderElement renderElement;
        //ParticleEmitter tracerEmitter;
        ParticleEmitter explosionEmitter;
        Actor sender;
        bool released = false;
        float deleteTime;

        float explosionMagnitude = 1.0f;

        public Actor GetSender()
        {
            return sender;
        }

        public Projectile(Actor sender, string tracerEffectName, string explosionEffectName)
        {
            this.sender = sender;
            renderElement = new RenderElement();
            renderElement.IndexBuffer = GFXPrimitives.SphereGeometry.Geometry.renderElement.IndexBuffer;
            renderElement.VertexBuffer = GFXPrimitives.SphereGeometry.Geometry.renderElement.VertexBuffer;
            renderElement.VertexCount = GFXPrimitives.SphereGeometry.Geometry.renderElement.VertexCount;
            renderElement.VertexDec = GFXPrimitives.SphereGeometry.Geometry.renderElement.VertexDec;
            renderElement.VertexStride = GFXPrimitives.SphereGeometry.Geometry.renderElement.VertexStride;
            renderElement.StartVertex = GFXPrimitives.SphereGeometry.Geometry.renderElement.StartVertex;
            renderElement.PrimitiveCount = GFXPrimitives.SphereGeometry.Geometry.renderElement.PrimitiveCount;

            projectileMaterial = ResourceManager.Inst.GetMaterial("TerrainMaterial");

            //tracerEffect = ResourceManager.Inst.GetParticleEffect(tracerEffectName);
            explosionEffect = ResourceManager.Inst.GetParticleEffect(explosionEffectName);
        }

        public float GetDamage()
        {
            return explosionMagnitude * 7.5f;
        }

        public void SetMagnitude(float magnitude)
        {
            explosionMagnitude = magnitude;
        }

        public override void OnAdd(Scene scene)
        {
            physicsState.position = this.Transformation.GetPosition();

            //tracerEmitter = new ParticleEmitter(tracerEffect, 200);
            //scene.Entities.Add(tracerEmitter);
            //tracerEmitter.OnAdd(scene);
            Console.WriteLine("Added particle at position: \n {0}", physicsState.position);
            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            //scene.Entities.Remove(tracerEmitter);
            //tracerEmitter.OnDestroy();
            base.OnDestroy();
        }

        public void SetVelocity(Vector3 velocity)
        {
            released = true;
            physicsState.velocity = velocity * 100;
        }

        bool CheckCollision(State state, out Vector3 collNormal)
        {
            bool collision = scene.MainTerrain.IsCollision(state.position, out collNormal);
            if (collision)
                return true;
            Ray r = new Ray(state.position, Vector3.Normalize(state.velocity));
            int currActorIndex = 0;
            float maxValTimestep = state.velocity.Length()*Time.GameTime.ElapsedTime;
            while (!collision && currActorIndex < scene.Actors.Count)
            {
                if (scene.Actors[currActorIndex].GetTeam() != sender.GetTeam())
                {
                    float? intersectValue = r.Intersects(scene.Actors[currActorIndex].GetBounds());
                    if (intersectValue.HasValue)
                    {
                        if (intersectValue.Value <= maxValTimestep)
                        {
                            return true;
                        }
                    }
                }
                currActorIndex++;
            }
            return false;
        }

        public override void OnUpdate()
        {
            if (!released)
            {
                physicsState.position = this.Transformation.GetPosition();
                physicsState.velocity = Vector3.Zero;
            }

            Vector3 acceleration = Vector3.Up * -9.82f; //Gravity!
            State newState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);

            Vector3 collNormal = Vector3.Zero;

            bool collision = CheckCollision(newState, out collNormal);
            if (collision)
            {
                if(!exploded)
                {
                    exploded = true;
                    int minCells = 3;
                    int maxCells = 7;
                    int val = (int)MathHelper.Lerp(minCells, maxCells, explosionMagnitude / EXPLOSION_MAX_MAGNITUDE);
                    scene.MainTerrain.CarveTerrainAtPoint(newState.position, val, -45);
                    BoundingBox damageBounds = scene.MainTerrain.GetWorldSpaceBoundsAtPoint(newState.position, val);

                    Vector3 impulseVec = Vector3.Reflect(newState.velocity, collNormal);

                    for (int i = 0; i < scene.Actors.Count; i++)
                    {
                        if (damageBounds.Contains(scene.Actors[i].GetBounds()) != ContainmentType.Disjoint)
                        {
                            scene.Actors[i].ApplyDamage(this, impulseVec);
                        }
                    }

                    //tracerEmitter.EmitOnce = true;
                    this.scene.Entities.Remove(this);
                    this.OnDestroy();
                    Console.WriteLine("Destroyed some terrain!");
                }
            }
            else
            {
                physicsState = newState;
                //tracerEmitter.Transformation.SetPosition(physicsState.position);
                //tracerEmitter.Transformation.SetRotation(this.Transformation.GetRotation());
            }
            this.Transformation.SetPosition(physicsState.position);
            this.renderElement.Transform = new Matrix[] { this.Transformation.GetTransform() };
            base.OnUpdate();
        }

        public override void OnRender(Gaia.Rendering.RenderViews.RenderView view)
        {
            if(view.GetFrustum().Contains(this.Transformation.GetBounds()) != ContainmentType.Disjoint)
            {
                view.AddElement(projectileMaterial, renderElement);
            }
            base.OnRender(view);
        }
        
    }
}
