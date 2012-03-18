using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;


using Gaia.Input;
using Gaia.Physics;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public class Player : Entity
    {
        Vector3 position = Vector3.Zero;
        Vector3 rotation = Vector3.Zero;
        float speed = 0.5f;
        MainRenderView renderView;

        float aspectRatio;
        float fieldOfView;

        CharacterBody CharacterBody { get; set; }
        public Body body;
        public CollisionSkin collision;
        float playerSpeed = 4.5f;
        Vector3 com;

        bool attachCameraToPlayer = false;

        public override void OnAdd(Scene scene)
        {
            renderView = new MainRenderView(scene, Matrix.Identity, Matrix.Identity, Vector3.Zero, 1.0f, 1000);

            scene.MainCamera = renderView;
            scene.AddRenderView(renderView);

            fieldOfView = MathHelper.ToRadians(70);
            aspectRatio = GFX.Inst.DisplayRes.X / GFX.Inst.DisplayRes.Y;

            Vector3 pos = Vector3.Up * 512;
            body = new CharacterBody();
            collision = new CollisionSkin(body);

            Capsule capsule = new Capsule(Vector3.Zero, Matrix.CreateRotationX(MathHelper.PiOver2), 1.5f, 1.6f);
            collision.AddPrimitive(capsule, (int)MaterialTable.MaterialID.NormalRough);//.Player);
            //collision.AddPrimitive(new Box(Vector3.Zero, Matrix.Identity, Vector3.One), (int)MaterialTable.MaterialID.NotBouncyNormal);
            body.CollisionSkin = this.collision;
            com = PhysicsHelper.SetMass(75.0f, body, collision);

            body.MoveTo(pos + com, Matrix.Identity);
            collision.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));

            body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);

            body.AllowFreezing = false;
            body.EnableBody();
            CharacterBody = body as CharacterBody;


            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            scene.RemoveRenderView(renderView);
            base.OnDestroy();
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
            
            Vector3 moveDir = Vector3.Zero;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveFoward))
                moveDir += transform.Forward;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveBackward))
                moveDir -= transform.Forward;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveRight))
                moveDir += transform.Right;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveLeft))
                moveDir -= transform.Right;

            moveDir *= speed;
            position += moveDir;

            Matrix temp = Matrix.CreateRotationY(rotation.Y);
            
            if (InputManager.Inst.IsKeyDown(GameKey.Jump))
                CharacterBody.Jump(15);

            Vector3 vel = JigLibX.Math.JiggleMath.NormalizeSafe(Vector3.Zero);
            if (InputManager.Inst.IsKeyDown(GameKey.MoveFoward))
                vel += temp.Forward * playerSpeed;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveBackward))
                vel -= temp.Forward * playerSpeed;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveLeft))
                vel -= temp.Right * playerSpeed;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveRight))
                vel += temp.Right * playerSpeed;

            //vel);
            CharacterBody.DesiredVelocity = vel;

            if (attachCameraToPlayer)
            {
                position = CharacterBody.Position;
            }

            if (InputManager.Inst.IsKeyDownOnce(GameKey.ToggleCamera))
            {
                attachCameraToPlayer = !attachCameraToPlayer;
            }

            if (InputManager.Inst.IsKeyDownOnce(GameKey.DropPlayerAtCamera))
            {
                CharacterBody.MoveTo(position, Matrix.Identity);
                attachCameraToPlayer = true;
            }


            float nearPlane = 0.5f;
            float farPlane = 2000;

            renderView.SetPosition(position);
            renderView.SetView(Matrix.CreateLookAt(position, position + transform.Forward, Vector3.Up));
            renderView.SetProjection(Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane));
            renderView.SetNearPlane(nearPlane);
            renderView.SetFarPlane(farPlane);
            renderView.UpdateRenderViews(); //Update reflections

            base.OnUpdate();
        }
    }
}
