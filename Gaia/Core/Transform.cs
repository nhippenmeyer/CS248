using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Gaia.Core
{
    public class Transform
    {
        Vector3 position;
        Vector3 rotation;
        Vector3 scale;

        bool dirtyMatrix;
        Matrix worldMatrix;
        Matrix objectMatrix;
        BoundingBox bounds;

        public Transform()
        {
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            scale = Vector3.One;
            worldMatrix = Matrix.Identity;
            objectMatrix = Matrix.Identity;
            dirtyMatrix = true;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
            dirtyMatrix = true;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public void SetRotation(Vector3 rotation)
        {
            this.rotation = rotation;
            dirtyMatrix = true;
        }

        public Vector3 GetRotation()
        {
            return rotation;
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
            dirtyMatrix = true;
        }

        public Vector3 GetScale()
        {
            return scale;
        }

        public Matrix GetTransform()
        {
            if (dirtyMatrix)
                UpdateMatrix();
            return worldMatrix;
        }

        public Matrix GetObjectSpace()
        {
            if (dirtyMatrix)
                UpdateMatrix();
            return objectMatrix;
        }

        public BoundingBox GetBounds()
        {
            return bounds;
        }

        void UpdateMatrix()
        {
            worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            worldMatrix.Translation = position;
            objectMatrix = Matrix.Invert(worldMatrix);
            dirtyMatrix = false;
            bounds.Min = Vector3.Transform(-Vector3.One, worldMatrix);
            bounds.Max = Vector3.Transform(Vector3.One, worldMatrix);
        }
    }
}
