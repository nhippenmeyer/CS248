using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Gaia.Input;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public class Player : Entity
    {
        Vector3 position = Vector3.Zero;
        Vector3 rotation = Vector3.Zero;
        float speed = 2.5f;
        MainRenderView renderView;

        float aspectRatio;
        float fieldOfView;

        public override void OnAdd(Scene scene)
        {
            renderView = new MainRenderView(scene, Matrix.Identity, Matrix.Identity, Vector3.Zero, 0.0001f, 1000);

            scene.RenderViews.Add(renderView);

            fieldOfView = MathHelper.ToRadians(70);
            aspectRatio = GFX.Inst.DisplayRes.X / GFX.Inst.DisplayRes.Y;

            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            scene.RenderViews.Remove(renderView);
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

            renderView.SetPosition(position);
            renderView.SetView(Matrix.CreateLookAt(position, position + transform.Forward, Vector3.Up));
            renderView.SetProjection(Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, 0.01f, 5000));

            base.OnUpdate();
        }
    }
}
