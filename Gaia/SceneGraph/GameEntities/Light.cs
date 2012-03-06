using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Rendering;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public enum LightType
    {
        Directional,
        Point,
        Spot,
    };

    public class Light : Entity
    {

        Vector4 parameters;
        public Vector3 Color;

        LightType type = LightType.Directional;

        protected bool castsShadows;
        ShadowRenderView[] renderViews;
        RenderTarget2D shadowMap;
        DepthStencilBuffer dsShadowMap;

        Vector3[] frustumCornersVS = new Vector3[8];
        Vector3[] frustumCornersWS = new Vector3[8];
        Vector3[] frustumCornersLS = new Vector3[8];
        Vector3[] farFrustumCornersVS = new Vector3[4];
        Vector3[] splitFrustumCornersVS = new Vector3[8];
        Matrix[] lightViewProjectionMatrices = new Matrix[GFXShaderConstants.NUM_SPLITS];
        Vector2[] lightClipPlanes = new Vector2[GFXShaderConstants.NUM_SPLITS];
        Vector4[] lightClipPositions = new Vector4[GFXShaderConstants.NUM_SPLITS];
        float[] splitDepths = new float[GFXShaderConstants.NUM_SPLITS + 1];

        public Light(LightType lightType, Vector3 color, Vector3 position, bool castsShadows)
            : base()
        {
            this.type = lightType;
            this.Color = color;
            this.Transformation.SetPosition(position);
            this.castsShadows = castsShadows;
            if (castsShadows)
            {
                CreateCascadeShadows(1024);
            }
        }

        public Vector4 Parameters
        {
            get { return parameters; }
            set 
            {
                parameters = value;
                float maxScale = Math.Max(parameters.X, parameters.Y);
                Transformation.SetScale(Vector3.One * maxScale);
            }
        }

        public Texture2D GetShadowMap()
        {
            return shadowMap.GetTexture();
        }

        public Matrix[] GetModelViews()
        {
            return lightViewProjectionMatrices;
        }

        public Vector2[] GetClipPlanes()
        {
            return lightClipPlanes;
        }

        public Vector4[] GetClipPositions()
        {
            return lightClipPositions;
        }

        DepthStencilBuffer oldDepthStencil;
        Viewport oldViewPort;
        public void BeginShadowMapping()
        {
            oldDepthStencil = GFX.Device.DepthStencilBuffer;
            oldViewPort = GFX.Device.Viewport;
            GFX.Device.SetRenderTarget(0, shadowMap);
            GFX.Device.DepthStencilBuffer = dsShadowMap;
            GFX.Device.Clear(Microsoft.Xna.Framework.Graphics.Color.TransparentBlack);
        }

        public void EndShadowMapping()
        {
            GFX.Device.SetRenderTarget(0, null);
            GFX.Device.DepthStencilBuffer = oldDepthStencil;
            GFX.Device.Viewport = oldViewPort;
        }

        void CreateCascadeShadows(int shadowMapSize)
        {
            int width = shadowMapSize * GFXShaderConstants.NUM_SPLITS;
            int height = shadowMapSize;
            shadowMap = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.Vector2);
            dsShadowMap = new DepthStencilBuffer(GFX.Device, width, height, GFX.Device.DepthStencilBuffer.Format);
            renderViews = new ShadowRenderView[GFXShaderConstants.NUM_SPLITS];

            for (int i = 0; i < GFXShaderConstants.NUM_SPLITS; i++)
            {
                Viewport splitViewport = new Viewport();
                splitViewport.MinDepth = 0;
                splitViewport.MaxDepth = 1;
                splitViewport.Width = shadowMapSize;
                splitViewport.Height = shadowMapSize;
                splitViewport.X = i * shadowMapSize;
                splitViewport.Y = 0;
                renderViews[i] = new ShadowRenderView(this, splitViewport, i, Matrix.Identity, Matrix.Identity, Vector3.Zero, 0.1f, 1000.0f);
            }
        }

        void ComputeFrustum(float minZ, float maxZ, RenderView cameraRenderView, int split)
        {
            // Shorten the view frustum according to the shadow view distance
            Matrix cameraMatrix = cameraRenderView.GetWorldMatrix();

            Vector4 camPos = cameraRenderView.GetEyePosShader();

            for (int i = 0; i < 4; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i + 4] * (minZ / camPos.W);

            for (int i = 4; i < 8; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i] * (maxZ / camPos.W);

            Vector3.Transform(splitFrustumCornersVS, ref cameraMatrix, frustumCornersWS);

            // Find the centroid
            Vector3 frustumCentroid = new Vector3(0, 0, 0);
            for (int i = 0; i < 8; i++)
                frustumCentroid += frustumCornersWS[i];
            frustumCentroid /= 8.0f;

            // Position the shadow-caster camera so that it's looking at the centroid,
            // and backed up in the direction of the sunlight
            Vector3 lightDir = -this.Transformation.GetPosition();
            lightDir.Normalize();
            float distFromCentroid = MathHelper.Max((maxZ - minZ), Vector3.Distance(splitFrustumCornersVS[4], splitFrustumCornersVS[5])) + 50.0f;
            Matrix viewMatrix = Matrix.CreateLookAt(frustumCentroid - (lightDir * distFromCentroid), frustumCentroid, new Vector3(0, 1, 0));

            // Determine the position of the frustum corners in light space
            Vector3.Transform(frustumCornersWS, ref viewMatrix, frustumCornersLS);

            // Calculate an orthographic projection by sizing a bounding box 
            // to the frustum coordinates in light space
            Vector3 mins = frustumCornersLS[0];
            Vector3 maxes = frustumCornersLS[0];
            for (int i = 0; i < 8; i++)
            {
                if (frustumCornersLS[i].X > maxes.X)
                    maxes.X = frustumCornersLS[i].X;
                else if (frustumCornersLS[i].X < mins.X)
                    mins.X = frustumCornersLS[i].X;
                if (frustumCornersLS[i].Y > maxes.Y)
                    maxes.Y = frustumCornersLS[i].Y;
                else if (frustumCornersLS[i].Y < mins.Y)
                    mins.Y = frustumCornersLS[i].Y;
                if (frustumCornersLS[i].Z > maxes.Z)
                    maxes.Z = frustumCornersLS[i].Z;
                else if (frustumCornersLS[i].Z < mins.Z)
                    mins.Z = frustumCornersLS[i].Z;
            }

            // Create an orthographic camera for use as a shadow caster
            const float nearClipOffset = 300.0f;
            
            float nearPlane = -maxes.Z - nearClipOffset;
            float farPlane = -mins.Z;

            renderViews[split].SetPosition(viewMatrix.Translation);
            renderViews[split].SetView(viewMatrix);
            renderViews[split].SetNearPlane(nearPlane);
            renderViews[split].SetFarPlane(farPlane);
            renderViews[split].SetProjection(Matrix.CreateOrthographicOffCenter(mins.X, maxes.X, mins.Y, maxes.Y, nearPlane, farPlane));
            lightViewProjectionMatrices[split] = renderViews[split].GetViewProjection();
            lightClipPositions[split] = renderViews[split].GetEyePosShader();
        }

        void UpdateCascades()
        {
            RenderView mainCamera = scene.MainCamera;

            // Get corners of the main camera's bounding frustum
            Matrix cameraTransform = mainCamera.GetWorldMatrix();
            Matrix viewMatrix = mainCamera.GetView();

            mainCamera.GetFrustum().GetCorners(frustumCornersWS);
            Vector3.Transform(frustumCornersWS, ref viewMatrix, frustumCornersVS);
            for (int i = 0; i < 4; i++)
                farFrustumCornersVS[i] = frustumCornersVS[i + 4];

            // Calculate the cascade splits.  We calculate these so that each successive
            // split is larger than the previous, giving the closest split the most amount
            // of shadow detail.  
            float N = GFXShaderConstants.NUM_SPLITS;
            float near = 1.0f, far = mainCamera.GetFarPlane();
            splitDepths[0] = near;
            splitDepths[GFXShaderConstants.NUM_SPLITS] = far;
            const float splitConstant = 0.95f;
            for (int i = 1; i < splitDepths.Length - 1; i++)
                splitDepths[i] = splitConstant * near * (float)Math.Pow(far / near, i / N) + (1.0f - splitConstant) * ((near + (i / N)) * (far - near));

            // Render our scene geometry to each split of the cascade
            for (int i = 0; i < GFXShaderConstants.NUM_SPLITS; i++)
            {
                float minZ = splitDepths[i];
                float maxZ = splitDepths[i + 1];

                lightClipPlanes[i].X = -splitDepths[i];
                lightClipPlanes[i].Y = -splitDepths[i + 1];

                ComputeFrustum(minZ, maxZ, mainCamera, i);
            }
        }

        public override void OnAdd(Scene scene)
        {

            if (castsShadows)
            {
                for (int i = 0; i < renderViews.Length; i++)
                {
                    scene.AddRenderView(renderViews[i]);
                }
            }

            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            if (castsShadows)
            {
                for (int i = 0; i < renderViews.Length; i++)
                {
                    scene.RemoveRenderView(renderViews[i]);
                }
            }
            base.OnDestroy();
        }

        public override void OnUpdate()
        {
            if (type == LightType.Directional && castsShadows)
            {
                UpdateCascades();
            }

            base.OnUpdate();
        }

        public override void OnRender(RenderView view)
        {
            bool canRender = (view.GetFrustum().Contains(Transformation.GetBounds()) != ContainmentType.Disjoint);
            canRender |= (type == LightType.Directional);
            if (canRender)
            {
                LightElementManager lightMgr = (LightElementManager)view.GetRenderElementManager(RenderPass.Light);
                if (lightMgr != null)
                {
                    switch (type)
                    {
                        case LightType.Directional:
                            if (castsShadows)
                                lightMgr.DirectionalShadowLights.Enqueue(this);
                            else
                                lightMgr.DirectionalLights.Enqueue(this);
                            break;
                        case LightType.Point:
                            lightMgr.PointLights.Enqueue(this);
                            break;
                        case LightType.Spot:
                            lightMgr.SpotLights.Enqueue(this);
                            break;
                    }
                }
            }
            base.OnRender(view);
        }


    }
}
