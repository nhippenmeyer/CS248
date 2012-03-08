using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaia.Rendering
{
    public class ScreenAlignedQuad
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        VertexBuffer vertexBufferInstanced;
        IndexBuffer indexBufferInstanced;

        public ScreenAlignedQuad()
        {
            VertexPositionTexture[] verts = new VertexPositionTexture[]
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

            vertexBuffer = new VertexBuffer(GFX.Device, verts.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(verts);

            short[] ib = new short[] { 0, 1, 2, 2, 3, 0 };

            indexBuffer = new IndexBuffer(GFX.Device, sizeof(short) * ib.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
            indexBuffer.SetData<short>(ib);

            CreateInstancedBuffers(verts, ib);
        }

        ~ScreenAlignedQuad()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public VertexBuffer GetInstanceVertexBuffer()
        {
            return vertexBufferInstanced;
        }

        public IndexBuffer GetInstanceIndexBuffer()
        {
            return indexBufferInstanced;
        }

        void CreateInstancedBuffers(VertexPositionTexture[] verts, short[] ib)
        {
            VertexPTI[] instVerts = new VertexPTI[verts.Length * GFXShaderConstants.NUM_INSTANCES];
            for (int i = 0; i < GFXShaderConstants.NUM_INSTANCES; i++)
            {
                for (int j = 0; j < verts.Length; j++)
                {
                    instVerts[i * verts.Length + j] = new VertexPTI(verts[j].Position, verts[j].TextureCoordinate, i);
                }
            }

            ushort[] instIB = new ushort[ib.Length * GFXShaderConstants.NUM_INSTANCES];
            for (int i = 0; i < GFXShaderConstants.NUM_INSTANCES; i++)
            {
                for(int j = 0; j < ib.Length; j++)
                {
                    instIB[i * ib.Length + j] = (ushort)(ib[j] + i * verts.Length);
                }
            }

            vertexBufferInstanced = new VertexBuffer(GFX.Device, instVerts.Length * VertexPTI.SizeInBytes, BufferUsage.None);
            vertexBufferInstanced.SetData<VertexPTI>(instVerts);

            indexBufferInstanced = new IndexBuffer(GFX.Device, sizeof(ushort) * instIB.Length, BufferUsage.None, IndexElementSize.SixteenBits);
            indexBufferInstanced.SetData<ushort>(instIB);
        }


        public void Render()
        {
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PTDec;
            GFX.Device.Indices = indexBuffer;
            GFX.Device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            GFX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }
    }

    public class RenderCube
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public RenderCube()
        {
            VertexPositionTexture[] verts = new VertexPositionTexture[]
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

            vertexBuffer = new VertexBuffer(GFX.Device, verts.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(verts);

            short[] ib = new short[] { 0, 1, 2, 2, 3, 0, 6, 5, 4, 4, 7, 6, 
                                   3, 2, 6, 6, 7, 3, 5, 1, 0, 0, 4, 5, 
                                   6, 2, 1, 1, 5, 6, 0, 3, 7, 7, 4, 0};

            indexBuffer = new IndexBuffer(GFX.Device, sizeof(short) * ib.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
            indexBuffer.SetData<short>(ib);

        }

        ~RenderCube()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public void Render()
        {
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PTDec;
            GFX.Device.Indices = indexBuffer;
            GFX.Device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            GFX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
        }
    }

    public class LightCube
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public LightCube()
        {
            VertexPosition[] verts = new VertexPosition[]
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

            vertexBuffer = new VertexBuffer(GFX.Device, verts.Length * VertexPosition.SizeInBytes, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPosition>(verts);

            short[] ib = new short[] { 0, 1, 2, 2, 3, 0, 6, 5, 4, 4, 7, 6, 
                                   3, 2, 6, 6, 7, 3, 5, 1, 0, 0, 4, 5, 
                                   6, 2, 1, 1, 5, 6, 0, 3, 7, 7, 4, 0};
            indexBuffer = new IndexBuffer(GFX.Device, sizeof(short) * ib.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
            indexBuffer.SetData<short>(ib);
        }

        ~LightCube()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public void Render()
        {
            GFX.Device.VertexDeclaration = GFXVertexDeclarations.PDec;
            GFX.Device.Indices = indexBuffer;
            GFX.Device.Vertices[0].SetSource(vertexBuffer, 0, VertexPosition.SizeInBytes);
            GFX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
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
