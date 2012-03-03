using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaia.Rendering
{
    public class ScreenAlignedQuad
    {
        #region Private Members
        VertexPositionTexture[] verts = null;
        short[] ib = null;
        #endregion


        #region Constructor
        public ScreenAlignedQuad()
        {
            verts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(1,-1,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,-1,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,1,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(1,1,0),
                                new Vector2(1,0))
                        };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        ~ScreenAlignedQuad()
        {
            verts = null;
            ib = null;
        }
        #endregion

        public void Render()
        {
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PTDec;
            GFX.Device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }

    public class RenderCube
    {
        #region Private Members
        VertexPositionTexture[] verts = null;
        short[] ib = null;
        #endregion

        #region Constructor
        public RenderCube()
        {
            verts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(1,-1,1),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,-1,1),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,1,1),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(1,1,1),
                                new Vector2(1,0)),
                            new VertexPositionTexture(
                                new Vector3(1,-1,-1),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,-1,-1),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,1,-1),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(1,1,-1),
                                new Vector2(1,0))
                        };

            ib = new short[] { 0, 1, 2, 2, 3, 0, 6, 5, 4, 4, 7, 6, 
                                   3, 2, 6, 6, 7, 3, 5, 1, 0, 0, 4, 5, 
                                   6, 2, 1, 1, 5, 6, 0, 3, 7, 7, 4, 0};
        }

        ~RenderCube()
        {
            verts = null;
            ib = null;
        }
        #endregion

        public void Render()
        {
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PTDec;
            GFX.Device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 8, ib, 0, 12);
        }
    }

    public class LightCube
    {
        #region Private Members
        VertexPosition[] verts = null;
        short[] ib = null;
        #endregion

        #region Constructor
        public LightCube()
        {
            verts = new VertexPosition[]
                        {
                            new VertexPosition(
                                new Vector3(1,-1,1)),
                            new VertexPosition(
                                new Vector3(-1,-1,1)),
                            new VertexPosition(
                                new Vector3(-1,1,1)),
                            new VertexPosition(
                                new Vector3(1,1,1)),
                            new VertexPosition(
                                new Vector3(1,-1,-1)),
                            new VertexPosition(
                                new Vector3(-1,-1,-1)),
                            new VertexPosition(
                                new Vector3(-1,1,-1)),
                            new VertexPosition(
                                new Vector3(1,1,-1))
                        };

            ib = new short[] { 0, 1, 2, 2, 3, 0, 6, 5, 4, 4, 7, 6, 
                                   3, 2, 6, 6, 7, 3, 5, 1, 0, 0, 4, 5, 
                                   6, 2, 1, 1, 5, 6, 0, 3, 7, 7, 4, 0};
        }

        ~LightCube()
        {
            verts = null;
            ib = null;
        }
        #endregion

        public void Render()
        {
            GFX.Device.DrawUserIndexedPrimitives<VertexPosition>(PrimitiveType.TriangleList, verts, 0, 8, ib, 0, 12);
        }
    }

    public static class GFXPrimitives
    {
        public static ScreenAlignedQuad Quad;
        public static RenderCube CubePT;
        public static LightCube Cube;
        public static void Initialize()
        {
            Quad = new ScreenAlignedQuad();
            Cube = new LightCube();
            CubePT = new RenderCube();
        }
    }
}
