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
    public class TerrainGenerator : Entity
    {
        byte IsoValue = 127; //Defines density field isosurface cutoff value (ie the transition between solid and empty space)
        //so if a voxel had an element of 127 or lower, that would be empty space. A value higher than 127
        //Would be solid space.
        int VoxelGridSize = 32; //Defines how many voxel geometries we have (used to balance performance)
        int WorldFieldSize = 129; //Density field is (2^n)+1 in size. (e.g. 65, 129, 257, 513) 
        byte[] DensityField;
        int DensityFieldSize;

        SortedList<int, VoxelGeometry> Voxels = new SortedList<int, VoxelGeometry>();
        SortedList<int, BoundingBox> VoxelBounds = new SortedList<int, BoundingBox>();

        Material terrainMaterial;

        float TerrainSize = 1024.0f;

        RenderTarget2D srcTarget;
        Texture3D[] noiseTextures;

        public TerrainGenerator(int fieldSize) : base()
        {
            WorldFieldSize = fieldSize;
        }

        public override void OnAdd(Scene scene)
        {

            InitializeVoxels();
            base.OnAdd(scene);
        }

        void EvaluateDensityFunction(Matrix textureMatrix)
        {
            ResourceManager.Inst.GetShader("ProceduralIsland").SetupShader();
            GFX.Device.SetPixelShaderConstant(0, Vector3.One / (float)DensityFieldSize);
            //Lets activate our textures
            for (int i = 0; i < noiseTextures.Length; i++)
                GFX.Device.Textures[i] = noiseTextures[i];

            GFX.Device.SetVertexShaderConstant(1, textureMatrix);

            DepthStencilBuffer dsOld = GFX.Device.DepthStencilBuffer;
            GFX.Device.DepthStencilBuffer = GFX.Inst.dsBufferLarge;

            for (int z = 0; z < srcTarget.Width; z++)
            {
                Vector4 depth = Vector4.One * (float)z / (float)(DensityFieldSize - 1);
                GFX.Device.SetVertexShaderConstant(0, depth); //Set our current depth

                GFX.Device.SetRenderTarget(0, srcTarget);
                GFX.Device.Clear(Color.TransparentBlack);

                GFXPrimitives.Quad.Render();

                GFX.Device.SetRenderTarget(0, null);

                //Now the copying stage.
                ExtractDensityTextureData(z);
            }
            GFX.Device.DepthStencilBuffer = dsOld;
        }

        void ExtractDensityTextureData(int z)
        {
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

        void GenerateNoiseTextures()
        {
            int nSize = 16;
            noiseTextures = new Texture3D[3];
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
        }

        void InitializeVoxels()
        {
            terrainMaterial = ResourceManager.Inst.GetMaterial("TerrainMaterial");
            int voxelCount = WorldFieldSize / VoxelGridSize;
            float blockScale = WorldFieldSize * TerrainSize / VoxelGridSize;

            GenerateNoiseTextures();

            DensityFieldSize = VoxelGridSize + 1;
            srcTarget = new RenderTarget2D(GFX.Device, DensityFieldSize, DensityFieldSize, 1, GFX.Inst.ByteSurfaceFormat);
            DensityField = new byte[DensityFieldSize * DensityFieldSize * DensityFieldSize];

            Vector3 halfShift = Vector3.One*voxelCount*0.5f;

            for (int z = 0; z < voxelCount; z++)
            {
                int zOff = voxelCount * voxelCount * z;
                for (int y = 0; y < voxelCount; y++)
                {
                    int yOff = voxelCount * y;

                    for (int x = 0; x < voxelCount; x++)
                    {
                        int idx = x + yOff + zOff;

                        BoundingBox bounds = new BoundingBox(Vector3.One*-1, Vector3.One);
                      
                        Vector3 pos = new Vector3(x,y,z)-halfShift;
                        Matrix worldMat = Matrix.CreateScale(blockScale);
                        worldMat.Translation = pos * blockScale;

                        bounds.Min = Vector3.Transform(bounds.Min, worldMat);
                        bounds.Max = Vector3.Transform(bounds.Max, worldMat);

                        VoxelBounds.Add(idx, bounds);

                        EvaluateDensityFunction(Matrix.CreateTranslation(pos / halfShift));
                                                
                        Voxels.Add(idx, new VoxelGeometry());
                        Voxels[idx].renderElement.Transform = new Matrix[1] { worldMat };
                        Voxels[idx].GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, VoxelGridSize, VoxelGridSize, VoxelGridSize, 0, 0, 0, 2.0f);
                    }
                }
            }
        }

        public override void OnUpdate()
        {

            /*
            float camX, camY, camZ;

            camX = (scene.MainCamera.GetPosition().X / TerrainSize) + visibleRange / 2;
            camY = (scene.MainCamera.GetPosition().Y / TerrainSize) + visibleRange / 2;
            camZ = (scene.MainCamera.GetPosition().Z / TerrainSize) + visibleRange / 2;

            float[] diff = new float[3] { camX-camOrigin[0], camY-camOrigin[1], camZ-camOrigin[2] };

            float visOver2 = visibleRange / 2;
            float visOver4 = visibleRange / 4;
            for (int i = 0; i < diff.Length; i++)
            {
                if (diff[i] <= -visOver4)
                {
                    MoveTerrainsAlongAxis(i, -1);
                    camOrigin[i] += visOver4;
                }
                else if (diff[i] >= visOver4)
                {
                    MoveTerrainsAlongAxis(i, 1);
                    camOrigin[i] -= visOver4;
                }
            }
            */
            base.OnUpdate();
        }

        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustm = view.GetFrustum();
            for (int i = 0; i < Voxels.Count; i++)
            {
                if(Voxels.Values[i].CanRender)//if (frustm.Contains(VoxelBounds.Values[i]) != ContainmentType.Disjoint && Voxels.Values[i].CanRender)
                {
                    view.AddElement(terrainMaterial, Voxels.Values[i].renderElement);
                }
            }
            base.OnRender(view);
        }
    }
}
