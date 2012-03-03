using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Gaia.Rendering.RenderViews
{
    public enum RenderViewType
    {
        SHADOWS = 0,
        REFLECTION,
        MAIN
    };

    public abstract class RenderView
    {
        BoundingFrustum frustum;
        Vector3 position;
        float farPlane;
        float nearPlane;

        Matrix projection;
        Matrix view;
        Matrix viewProjection;
        Matrix viewLocal;
        Matrix viewProjectionLocal;
        Matrix inverseViewProjection;
        Matrix inverseViewProjectionLocal;

        protected SortedList<RenderPass, RenderElementManager> ElementManagers;

        bool dirtyMatrix;

        public RenderView(Matrix view, Matrix projection, Vector3 position, float nearPlane, float farPlane)
        {
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.position = position;
            this.view = view;
            this.projection = projection;
            dirtyMatrix = true;
            ElementManagers = new SortedList<RenderPass, RenderElementManager>();
        }

        public virtual void Render()
        {
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, GetViewProjection());
            for (int i = 0; i < ElementManagers.Keys.Count; i++)
            {
                RenderPass pass = ElementManagers.Keys[i];
                ElementManagers[pass].Render();
            }
        }

        public RenderElementManager GetRenderElementManager(RenderPass type)
        {
            if (ElementManagers.ContainsKey(type))
                return ElementManagers[type];

            return null;
        }

        void ComputeMatrix()
        {
            if (!dirtyMatrix)
                return;

            dirtyMatrix = false;
            viewProjection = view * projection;
            viewLocal = view;
            viewLocal.Translation = Vector3.Zero;
            viewProjectionLocal = viewLocal * projection;
            frustum = new BoundingFrustum(viewProjection);
            inverseViewProjection = Matrix.Invert(viewProjection);
            inverseViewProjectionLocal = Matrix.Invert(viewProjectionLocal);
        }

        public void SetView(Matrix view)
        {
            this.view = view;
            dirtyMatrix = true;
        }

        public Matrix GetView()
        {
            return view;
        }

        public void SetProjection(Matrix projection)
        {
            this.projection = projection;
            dirtyMatrix = true;
        }

        public Matrix GetProjection()
        {
            return projection;
        }

        public Matrix GetViewProjection()
        {
            if (dirtyMatrix)
                ComputeMatrix();
            return viewProjection;
        }

        public Matrix GetViewProjectionLocal()
        {
            if (dirtyMatrix)
                ComputeMatrix();
            return viewProjectionLocal;
        }

        public Matrix GetInverseViewProjection()
        {
            if (dirtyMatrix)
                ComputeMatrix();
            return inverseViewProjection;
        }

        public Matrix GetInverseViewProjectionLocal()
        {
            if (dirtyMatrix)
                ComputeMatrix();
            return inverseViewProjectionLocal;
        }

        public BoundingFrustum GetFrustum()
        {
            if (dirtyMatrix)
                ComputeMatrix();
            return frustum;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector4 GetEyePosShader()
        {
            return new Vector4(position, farPlane);
        }

    }
}
