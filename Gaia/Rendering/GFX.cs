using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaia.Rendering
{

    public enum GFXTextureDataType
    {
        BYTE,
        COLOR,
        SINGLE,
        HALFSINGLE,
    };

    public enum RenderPass
    {
        Shadows = 0,
        Sky,
        Scene,
        Light,
        Translucent,
        UI,
        Count
    };

    public class GFX
    {
        public Vector2 Origin = Vector2.Zero;
        public Vector2 DisplayRes
        {
            get { return new Vector2(Device.PresentationParameters.BackBufferWidth, Device.PresentationParameters.BackBufferHeight); }
        }

        GraphicsDevice device;
        public DepthStencilBuffer dsBufferLarge;
        public static GFX Inst = null;

        public static GraphicsDevice Device
        {
            get { return Inst.device; }
        }

        public SurfaceFormat ByteSurfaceFormat = SurfaceFormat.Luminance8;
        public GFXTextureDataType ByteSurfaceDataType = GFXTextureDataType.BYTE;

        RenderTarget2D GBuffer;
        DepthStencilBuffer DSBufferScene;

        public GFX(GraphicsDevice _device, IServiceProvider _services)
        {
            Inst = this;
            GFXShaderConstants.AuthorShaderConstantFile();
            RegisterDevice(_device);

        }
        ~GFX()
        {
        }

        public void RegisterDevice(GraphicsDevice device)
        {
            this.device = device;
            GFXVertexDeclarations.Initialize();
            GFXPrimitives.Initialize();
            InitializeSurfaceModes();
            InitializeSamplerStates();
            InitializeTextures();
        }

        public void Render()
        {
            DepthStencilBuffer dsOld = GFX.Device.DepthStencilBuffer;

            /*
            Device.SetVertexShaderConstant(GFXShaderConstants.ModelView, ActiveView.ViewProjection);
            Device.SetVertexShaderConstant(GFXShaderConstants.EyePos, new Vector4(ActiveView.Position, ActiveView.FarPlane));
            Device.SetPixelShaderConstant(GFXShaderConstants.EyePos, new Vector4(ActiveView.Position, ActiveView.FarPlane));
            */
            /*
            Device.SetVertexShaderConstant(GFXShaderConstants.VC_MODELVIEW, DefaultRenderView.GetViewProjection());
            GFX.Device.DepthStencilBuffer = DSBufferScene;
            GFX.Device.SetRenderTarget(0, GBuffer);
            GFX.Device.Clear(Color.TransparentBlack);
            GFX.Device.SetRenderTarget(0, null);
            GFX.Device.DepthStencilBuffer = dsOld;

            GFX.Device.Clear(Color.CornflowerBlue);
            ElementManager[(int)RenderPass.Sky].Render();
            ElementManager[(int)RenderPass.Voxels].Render();
            ElementManager[(int)RenderPass.Scene].Render();
            */

            //GFX.Device.Vertices[0].SetSource(null, 0, 0);
        }

        public void ResetState()
        {
            Device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            Device.RenderState.DepthBufferEnable = true;
            Device.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
            Device.RenderState.DepthBufferWriteEnable = true;
            Device.RenderState.AlphaBlendEnable = false;
        }

        void InitializeSurfaceModes()
        {
            SurfaceFormat[] formatEnum = { SurfaceFormat.Luminance8, SurfaceFormat.HalfSingle, SurfaceFormat.Color, SurfaceFormat.Single };
            GFXTextureDataType[] formatDataType = { GFXTextureDataType.BYTE, GFXTextureDataType.HALFSINGLE, GFXTextureDataType.COLOR, GFXTextureDataType.SINGLE };

            int i = 0;
            while (i < formatEnum.Length && !GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format,
                    TextureUsage.None, QueryUsages.None, ResourceType.RenderTarget, formatEnum[i]))
            {
                i++;
            }
            ByteSurfaceFormat = formatEnum[i];
            ByteSurfaceDataType = formatDataType[i];
        }

        void InitializeSamplerStates()
        {
            for (int i = 0; i < 8; i++)
            {
                GFX.Device.SamplerStates[i].AddressU = TextureAddressMode.Wrap;
                GFX.Device.SamplerStates[i].AddressV = TextureAddressMode.Wrap;
                GFX.Device.SamplerStates[i].MagFilter = TextureFilter.Linear;
                GFX.Device.SamplerStates[i].MinFilter = TextureFilter.Linear;
                GFX.Device.SamplerStates[i].MipFilter = TextureFilter.Linear;
            }
        }

        void InitializeTextures()
        {
            int width = (int)DisplayRes.X;
            int height = (int)DisplayRes.Y;

            DSBufferScene = new DepthStencilBuffer(GFX.Device, width, height, Device.DepthStencilBuffer.Format);
            GBuffer = new RenderTarget2D(GFX.Device, width, height, 1, SurfaceFormat.HalfVector4);

            dsBufferLarge = new DepthStencilBuffer(Device, 2048, 2048, Device.DepthStencilBuffer.Format);

        }
    }
}
