using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Gaia.Resources;
using Gaia.Rendering.RenderViews;
using Gaia.Rendering.Geometry;

namespace Gaia.Rendering
{
    public struct GUIElement
    {
        public Vector4 ScaleOffset;
        public TextureResource Image;

        public GUIElement(Vector2 min, Vector2 max, TextureResource image)
        {
            ScaleOffset = Vector4.Zero;
            Image = image;
            SetDimensions(min, max);
        }

        public void SetDimensions(Vector2 min, Vector2 max)
        {
            Vector2 tempMin = Vector2.Min(min, max);
            Vector2 tempMax = Vector2.Max(min, max);

            Vector2 scale = (tempMax - tempMin) * 0.5f;
            Vector2 offset = (tempMax + tempMin) * 0.5f;
            ScaleOffset = new Vector4(scale.X, scale.Y, offset.X, offset.Y);
        }
    };

    public class GUIElementManager
    {
        Shader basicImageShader;

        Queue<GUIElement> Elements = new Queue<GUIElement>();

        public GUIElementManager()
        {
            basicImageShader = new Shader();
            basicImageShader.CompileFromFiles("Shaders/PostProcess/GenericP.hlsl", "Shaders/PostProcess/GUIV.hlsl");
        }

        public void AddElement(GUIElement element)
        {
            Elements.Enqueue(element);
        }

        public void Render()
        {
            for(int i = 0; i < 4; i++)
            {
                GFX.Inst.SetTextureFilter(i, TextureFilter.Anisotropic);
                GFX.Inst.SetTextureAddressMode(i, TextureAddressMode.Clamp);
            }
            GFX.Device.RenderState.CullMode = CullMode.None;
            GFX.Device.RenderState.DepthBufferEnable = false;
            GFX.Device.RenderState.DepthBufferWriteEnable = false;
            GFX.Device.RenderState.AlphaBlendEnable = true;

            GFX.Device.RenderState.SourceBlend = Blend.SourceAlpha;
            GFX.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            basicImageShader.SetupShader();
            GFX.Device.SetVertexShaderConstant(GFXShaderConstants.VC_INVTEXRES, Vector2.Zero);
            while (Elements.Count > 0)
            {
                GUIElement elem = Elements.Dequeue();
                GFX.Device.Textures[0] = elem.Image.GetTexture();
                
                GFX.Device.SetVertexShaderConstant(0, elem.ScaleOffset);
                GFXPrimitives.Quad.Render();
            }

            GFX.Device.RenderState.AlphaBlendEnable = false;

            GFX.Inst.ResetState();
        }
    }
}
