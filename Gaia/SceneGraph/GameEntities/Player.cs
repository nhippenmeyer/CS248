using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Gaia.Input;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;
using Gaia.Physics;
using Gaia.Core;

namespace Gaia.SceneGraph.GameEntities
{
    public class Player : Entity
    {
        Vector3 position = Vector3.Zero;
        Vector3 rotation = Vector3.Zero;

        State physicsState;

        float hoverMagnitude = 2.5f;
        float hoverAngle = 0;

        float speed = 16f;
        float forwardAcceleration = 20; //15 units/second^2
        float backwardAcceleration = 8;
        float strafeAcceleration = 12;
        MainRenderView renderView;

        float aspectRatio;
        float fieldOfView;

        ParticleEmitter emitter;
        Light emitterLight;

        public override void OnAdd(Scene scene)
        {
            renderView = new MainRenderView(scene, Matrix.Identity, Matrix.Identity, Vector3.Zero, 1.0f, 1000);

            position = Vector3.Transform(Vector3.Up*0.25f, scene.MainTerrain.Transformation.GetTransform());
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

        public override void OnDestroy()
        {
            scene.RemoveRenderView(renderView);
            base.OnDestroy();
        }

        void FireGun(Vector3 forwardVector)
        {
            Projectile proj = new Projectile("TracerParticle", "ExplosionParticle");
            proj.Transformation.SetPosition(physicsState.position);
            proj.Transformation.SetRotation(rotation);
            proj.SetVelocity(forwardVector);
            this.scene.Entities.Add(proj);
            proj.OnAdd(this.scene);
        }

        public override void OnUpdate()
        {
            Vector2 centerCrd = GFX.Inst.DisplayRes / 2.0f;
            Vector2 delta = InputManager.Inst.GetMouseDisplacement();
            //delta.Y *= -1;
            rotation.Y += delta.X;
            rotation.X = MathHelper.Clamp(rotation.X + delta.Y, -1.4f, 1.4f);
            if (rotation.Y > MathHelper.TwoPi)
                rotation.Y -= MathHelper.TwoPi;
            if (rotation.Y < 0)
                rotation.Y += MathHelper.TwoPi;
            Mouse.SetPosition((int)(centerCrd.X + GFX.Inst.Origin.X), (int)(centerCrd.Y + GFX.Inst.Origin.Y));
            
            Matrix transform = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);

            Vector3 acceleration = Vector3.Zero;

            hoverAngle += Time.GameTime.ElapsedTime;
            if (hoverAngle >= MathHelper.TwoPi)
                hoverAngle -= MathHelper.TwoPi;

            Vector3 vel = Vector3.Zero;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveFoward))
                vel += transform.Forward * forwardAcceleration * (Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveFoward) / 3.0f));
            if (InputManager.Inst.IsKeyDown(GameKey.MoveBackward))
                vel -= transform.Forward * backwardAcceleration * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveBackward) / 1.75f);

            if (InputManager.Inst.IsKeyDown(GameKey.MoveRight))
                vel += transform.Right * strafeAcceleration * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveRight) / 1.25f);
            if (InputManager.Inst.IsKeyDown(GameKey.MoveLeft))
                vel -= transform.Right * strafeAcceleration * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveLeft) / 1.25f);

            physicsState.velocity = Vector3.Lerp(vel, physicsState.velocity, 0.45f) + (float)Math.Sin(hoverAngle) * hoverMagnitude * transform.Up;
            
            /*
            if(InputManager.Inst.IsKeyDown(GameKey.MoveFoward))
                acceleration += transform.Forward * forwardAcceleration * (1.0f - Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveFoward) / 3.0f));
            if (InputManager.Inst.IsKeyDown(GameKey.MoveBackward))
                acceleration -= transform.Forward * backwardAcceleration * (1.0f - Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveBackward) / 1.75f));
            
            if (InputManager.Inst.IsKeyDown(GameKey.MoveRight))
                acceleration += transform.Right * strafeAcceleration * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveRight) / 1.25f);
            if (InputManager.Inst.IsKeyDown(GameKey.MoveLeft))
                acceleration -= transform.Right * strafeAcceleration * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveLeft) / 1.25f);
            */

            if (InputManager.Inst.IsKeyDownOnce(GameKey.Fire))
            {
                FireGun(transform.Forward);
            }

            State newState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);

            Vector3 collNormal = Vector3.Zero;
            if (!scene.MainTerrain.IsCollision(newState.position, out collNormal))
            {
                physicsState = newState;
            }
            else
            {
                physicsState.velocity = Vector3.Reflect(physicsState.velocity, collNormal)*1.5f;
                physicsState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);
            }
            position = physicsState.position + transform.Up*4.5f - transform.Forward*0.25f;
            //position = physicsState.position -transform.Forward * 30;

            emitter.Transformation.SetPosition(physicsState.position);
            emitter.Transformation.SetRotation(rotation);

            emitterLight.Transformation.SetPosition(physicsState.position);

            float nearPlane = 0.15f;
            float farPlane = 2000;

            
            renderView.SetPosition(position);
            
            //renderView.SetView(Matrix.CreateLookAt(position, physicsState.position, Vector3.Up));
            renderView.SetView(Matrix.CreateLookAt(position, position + transform.Forward, Vector3.Up));
            
            renderView.SetProjection(Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane));
            renderView.SetNearPlane(nearPlane);
            renderView.SetFarPlane(farPlane);
            renderView.UpdateRenderViews(); //Update reflections

            base.OnUpdate();
        }
    }
}
