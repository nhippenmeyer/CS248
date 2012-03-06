using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

using Gaia.Voxels;
using Gaia.Rendering.RenderViews;
using Gaia.Rendering;
using Gaia.Resources;

namespace Gaia.SceneGraph.GameEntities
{
    public class Terrain : Entity
    {
        public byte IsoValue = 127; //Defines density field isosurface cutoff value (ie the transition between solid and empty space)
                                    //so if a voxel had an element of 127 or lower, that would be empty space. A value higher than 127
                                    //Would be solid space.
        public int VoxelGridSize = 8; //Defines how many voxel geometries we have (used to balance performance)
        public int DensityFieldSize = 129; //Density field is (2^n)+1 in size. (e.g. 65, 129, 257, 513) 

        VoxelGeometry[] Voxels;
        BoundingBox[] VoxelBounds;
        public byte[] DensityField;

        Material terrainMaterial;

        public override void OnAdd(Scene scene)
        {
            GenerateFloatingIslands(256);
            terrainMaterial = ResourceManager.Inst.GetMaterial("TerrainMaterial");
            base.OnAdd(scene);
        }

        void GenerateFloatingIslands(int size)
        {
            DensityFieldSize = size + 1;
            InitializeFieldData();


            //Here we generate our noise textures
            int nSize = 16;
            Texture3D[] noiseTextures = new Texture3D[3];
            float[] noiseData = new float[nSize * nSize * nSize];
            Random rand = new Random();
            for (int i = 0; i < noiseTextures.Length; i++)
            {
                noiseTextures[i] = new Texture3D(GFX.Device, nSize, nSize, nSize, 1, TextureUsage.None, SurfaceFormat.Single);
                for (int j = 0; j < noiseData.Length; j++)
                {
                    noiseData[j] = (float)(rand.NextDouble() * 2 - 1);
                }
                noiseTextures[i].SetData<float>(noiseData);
            }

            noiseData = null;

            //The program we'll be using
            Shader islandShader = ResourceManager.Inst.GetShader("ProceduralIsland");
            islandShader.SetupShader();

            GFX.Device.SetPixelShaderConstant(0, Vector3.One / (float)DensityFieldSize);
            //Lets activate our textures
            for (int i = 0; i < noiseTextures.Length; i++)
                GFX.Device.Textures[i] = noiseTextures[i];
            
            //Here we setup our render target. 
            //This is used to fetch what is rendered to our screen and store it in a texture.
            RenderTarget2D srcTarget = new RenderTarget2D(GFX.Device, DensityFieldSize, DensityFieldSize, 1, GFX.Inst.ByteSurfaceFormat);
            DepthStencilBuffer dsOld = GFX.Device.DepthStencilBuffer;
            GFX.Device.DepthStencilBuffer = GFX.Inst.dsBufferLarge;

            for (int z = 0; z < DensityFieldSize; z++)
            {
                Vector4 depth = Vector4.One * (float)z / (float)(DensityFieldSize - 1);
                GFX.Device.SetVertexShaderConstant(0, depth); //Set our current depth
                
                GFX.Device.SetRenderTarget(0, srcTarget);
                GFX.Device.Clear(Color.TransparentBlack);

                GFXPrimitives.Quad.Render();
    
                GFX.Device.SetRenderTarget(0, null);

                //Now the copying stage.
                //In the lines below, we copy the texture data into the density field buffer
                if (GFX.Inst.ByteSurfaceDataType == GFXTextureDataType.BYTE)
                    srcTarget.GetTexture().GetData<byte>(DensityField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                else
                {
                    byte[] densityData = new byte[srcTarget.Width * srcTarget.Height];
                    switch (GFX.Inst.ByteSurfaceDataType)
                    {
                        case GFXTextureDataType.COLOR:
                            Color[] colorData = new Color[densityData.Length];
                            srcTarget.GetTexture().GetData<Color>(colorData);
                            for (int i = 0; i < colorData.Length; i++)
                                densityData[i] = colorData[i].R;
                            Array.Copy(densityData, 0, DensityField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                            break;
                        case GFXTextureDataType.HALFSINGLE:
                            HalfSingle[] hsingData = new HalfSingle[densityData.Length];
                            srcTarget.GetTexture().GetData<HalfSingle>(hsingData);
                            for (int i = 0; i < hsingData.Length; i++)
                                densityData[i] = (byte)(hsingData[i].ToSingle() * 255.0f);
                            Array.Copy(densityData, 0, DensityField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                            break;
                        case GFXTextureDataType.SINGLE:
                            float[] singData = new float[densityData.Length];
                            srcTarget.GetTexture().GetData<float>(singData);
                            for (int i = 0; i < singData.Length; i++)
                                densityData[i] = (byte)(singData[i] * 255.0f);
                            Array.Copy(densityData, 0, DensityField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                            break;
                    }
                }

            }
            GFX.Device.DepthStencilBuffer = dsOld;

            InitializeVoxels();
        }

        void InitializeFieldData()
        {
            int fieldSize = DensityFieldSize * DensityFieldSize * DensityFieldSize;
            DensityField = new byte[fieldSize];
        }

        void InitializeVoxels()
        {
            int voxelCount = (DensityFieldSize - 1) / VoxelGridSize;
            Voxels = new VoxelGeometry[voxelCount * voxelCount * voxelCount];
            VoxelBounds = new BoundingBox[Voxels.Length];
            float ratio = 2.0f * (float)VoxelGridSize / (float)(DensityFieldSize - 1);

            Transformation.SetScale(Vector3.One*512.0f);

            for (int z = 0; z < voxelCount; z++)
            {
                int zOff = voxelCount * voxelCount * z;
                for (int y = 0; y < voxelCount; y++)
                {
                    int yOff = voxelCount * y;

                    for (int x = 0; x < voxelCount; x++)
                    {
                        int idx = x + yOff + zOff;

                        VoxelBounds[idx] = new BoundingBox(new Vector3(x, y, z) * ratio - Vector3.One, new Vector3(x + 1, y + 1, z + 1) * ratio - Vector3.One);
                        VoxelBounds[idx].Min = Vector3.Transform(VoxelBounds[idx].Min, Transformation.GetTransform());
                        VoxelBounds[idx].Max = Vector3.Transform(VoxelBounds[idx].Max, Transformation.GetTransform());

                        Voxels[idx] = new VoxelGeometry();
                        Voxels[idx].renderElement.Transform = new Matrix[1] { Transformation.GetTransform() };
                        Voxels[idx].GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, VoxelGridSize, VoxelGridSize, VoxelGridSize, x * VoxelGridSize, y * VoxelGridSize, z * VoxelGridSize, 2.0f / (float)(DensityFieldSize - 1));
                    }
                }
            }
        }

        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustm = view.GetFrustum();
            for (int i = 0; i < Voxels.Length; i++)
            {
                if (frustm.Contains(VoxelBounds[i]) != ContainmentType.Disjoint && Voxels[i].CanRender)
                {
                    view.AddElement(terrainMaterial, Voxels[i].renderElement);
                }
            }
            base.OnRender(view);
        }
    }
}
