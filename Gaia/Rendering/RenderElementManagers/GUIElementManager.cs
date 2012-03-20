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
        public Vector4 Color;

        public GUIElement(Vector2 min, Vector2 max, TextureResource image)
        {
            Color = Vector4.One;
            ScaleOffset = Vector4.Zero;
            Image = image;
            SetDimensions(min, max);
        }

        public GUIElement(Vector2 min, Vector2 max, TextureResource image, Vector3 color)
        {
            Color = new Vector4(color,1.0f);
            ScaleOffset = Vector4.Zero;
            Image = image;
            SetDimensions(min, max);
        }

        public GUIElement(Vector2 min, Vector2 max, TextureResource image, Vector4 color)
        {
            Color = color;
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

    public struct GUITextElement
    {
        public Vector2 Position;
        public string Text;
        public Vector4 Color;

        public GUITextElement(Vector2 pos, string text)
        {
            Position = pos;
            Text = text;
            Color = Vector4.One;
        }

        public GUITextElement(Vector2 pos, string text, Vector3 color)
        {
            Position = pos;
            Text = text;
            Color = new Vector4(color, 1.0f);
        }
        public GUITextElement(Vector2 pos, string text, Vector4 color)
        {
            Position = pos;
            Text = text;
            Color = color;
        }
    }

    public class GUIElementManager
    {
        Shader basicImageShader;

        Queue<GUIElement> Elements = new Queue<GUIElement>();
        Queue<GUITextElement> TextElements = new Queue<GUITextElement>();

        Texture2D whiteTexture;

        SpriteBatch spriteBatch;

        public SpriteFont DefaultFont;

        public GUIElementManager()
        {
            basicImageShader = new Shader();
            basicImageShader.CompileFromFiles("Shaders/PostProcess/GUIP.hlsl", "Shaders/PostProcess/GUIV.hlsl");
            whiteTexture = new Texture2D(GFX.Device, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] color = new Color[] { Color.White };
            whiteTexture.SetData<Color>(color);
            spriteBatch = new SpriteBatch(GFX.Device);
        }

        public void AddElement(GUIElement element)
        {
            Elements.Enqueue(element);
        }

        public void AddElement(GUITextElement element)
        {
            TextElements.Enqueue(element);
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
                if (elem.Image != null)
                {
                    GFX.Device.Textures[0] = elem.Image.GetTexture();
                }
                else
                {
                    GFX.Device.Textures[0] = whiteTexture;
                }
                
                GFX.Device.SetVertexShaderConstant(0, elem.ScaleOffset);
                GFX.Device.SetPixelShaderConstant(0, elem.Color);
                GFXPrimitives.Quad.Render();
            }

            DrawTextElements();

            GFX.Device.RenderState.AlphaBlendEnable = false;

            GFX.Inst.ResetState();            
        }

        private void DrawTextElements()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            while (TextElements.Count > 0)
            {
                GUITextElement element = TextElements.Dequeue();
                Vector2 pos = element.Position * new Vector2(0.5f, -0.5f) + Vector2.One * 0.5f;
                pos *= GFX.Inst.DisplayRes;
                spriteBatch.DrawString(DefaultFont, element.Text, pos, new Color(element.Color));
            }
            spriteBatch.End();
        }
    }
}
