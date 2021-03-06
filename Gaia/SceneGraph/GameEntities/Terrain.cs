﻿using System;
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
        public int VoxelGridSize = 16; //Defines how many voxel geometries we have (used to balance performance)
        public int DensityFieldSize = 129; //Density field is (2^n)+1 in size. (e.g. 65, 129, 257, 513) 

        VoxelGeometry[] Voxels;
        BoundingBox[] VoxelBounds;

        public byte[] DensityField;

        Material terrainMaterial;

        Matrix textureMatrix = Matrix.Identity;
        float TerrainSize = 1024;

        RenderTarget2D srcTarget;
        Texture3D[] noiseTextures;

        Texture3D blendTexture;
        Texture3D textureAtlas;

        public Terrain()
        {
            Transformation.SetScale(Vector3.One * TerrainSize);
            Transformation.SetPosition(Vector3.Up * TerrainSize * 0.45f);
            GenerateFloatingIslands(256);
            terrainMaterial = ResourceManager.Inst.GetMaterial("TerrainMaterial");
        }

        public bool GetYPos(ref Vector3 pos, out Vector3 normal, float minY, float maxY)
        {

            Vector3 minPos = new Vector3(pos.X, minY, pos.Z);
            Vector3 maxPos = new Vector3(pos.X, maxY, pos.Z);
            minPos = Vector3.Transform(minPos, Transformation.GetObjectSpace());
            maxPos = Vector3.Transform(maxPos, Transformation.GetObjectSpace());
            minPos = minPos * 0.5f + Vector3.One * 0.5f;
            maxPos = maxPos * 0.5f + Vector3.One * 0.5f;

            int yBegin = (int)MathHelper.Clamp(minPos.Y * DensityFieldSize, 0, DensityFieldSize - 1);
            int yEnd = (int)MathHelper.Clamp(maxPos.Y * DensityFieldSize, 0, DensityFieldSize - 1);
            int xCrd = (int)MathHelper.Clamp(DensityFieldSize * minPos.X, 0, DensityFieldSize - 1);
            int zCrd = (int)MathHelper.Clamp(DensityFieldSize * minPos.Z, 0, DensityFieldSize - 1);
            int baseIndex = xCrd + zCrd * DensityFieldSize * DensityFieldSize;

            int yIndex = yBegin;
            bool freeSpaceFound = false;
            bool solidSpaceFound = false;
            for (int i = yBegin; i < yEnd; i++)
            {
                if (DensityField[baseIndex + i * DensityFieldSize] <= IsoValue)
                {
                    yIndex = i;
                    freeSpaceFound = true;
                }
                else
                {
                    solidSpaceFound = true;
                }
                if (freeSpaceFound && solidSpaceFound)
                    break;
            }
            minPos.Y = ((float)yIndex / (float)DensityFieldSize);
            minPos = minPos * 2.0f - Vector3.One;
            pos = Vector3.Transform(minPos, Transformation.GetTransform());
            normal = ComputeNormal(xCrd, yIndex, zCrd);
            return (freeSpaceFound && solidSpaceFound);
        }

        public void GenerateRandomTransform(Random rand, out Vector3 position, out Vector3 normal)
        {
            int bestY = -1;
            int randX = 0;
            int randZ = 0;
            while(bestY == -1)
            {
                randX = rand.Next(DensityFieldSize-1);
                randZ = rand.Next(DensityFieldSize-1);
                int index = randX + randZ * DensityFieldSize * DensityFieldSize;
                
                for (int i = 0; i < DensityFieldSize; i++)
                {
                    if(DensityField[index+i*DensityFieldSize] <= IsoValue)
                    {
                        bestY = i;
                        break;
                    }
                }
            }

            Vector3 vec = new Vector3((float)randX, 1.0f+(float)bestY, (float)randZ) / (float)(DensityFieldSize - 1);
            position = Vector3.Transform(2.0f * vec - Vector3.One, Transformation.GetTransform());
            normal = ComputeNormal(randX, bestY, randZ);
        }

        Vector3 ComputeNormal(int x, int y, int z)
        {
            int sliceArea = DensityFieldSize * DensityFieldSize;
            int idx = x + DensityFieldSize * y + z * sliceArea;
            int x0 = (x - 1 >= 0) ? -1 : 0;
            int x1 = (x + 1 < DensityFieldSize) ? 1 : 0;
            int y0 = (y - 1 >= 0) ? -DensityFieldSize : 0;
            int y1 = (y + 1 < DensityFieldSize) ? DensityFieldSize : 0;
            int z0 = (z - 1 >= 0) ? -sliceArea : 0;
            int z1 = (z + 1 < DensityFieldSize) ? sliceArea : 0;

            //Take the negative gradient (hence the x0-x1)
            Vector3 nrm = new Vector3(DensityField[idx + x0] - DensityField[idx + x1], DensityField[idx + y0] - DensityField[idx + y1], DensityField[idx + z0] - DensityField[idx + z1]);

            double magSqr = nrm.X * nrm.X + nrm.Y * nrm.Y + nrm.Z * nrm.Z + 0.0001; //Regularization constant (very important!)
            double invMag = 1.0 / Math.Sqrt(magSqr);
            nrm.X = (float)(nrm.X * invMag);
            nrm.Y = (float)(nrm.Y * invMag);
            nrm.Z = (float)(nrm.Z * invMag);

            return nrm;
        }

        public bool IsCollision(Vector3 pos, out Vector3 normal)
        {
            Vector3 posObjSpace = Vector3.Transform(pos, Transformation.GetObjectSpace());
 
            posObjSpace = posObjSpace * 0.5f + Vector3.One * 0.5f;
            posObjSpace *= DensityFieldSize;

            int xP = (int)MathHelper.Clamp(posObjSpace.X, 0, DensityFieldSize - 1);
            int yP = (int)MathHelper.Clamp(posObjSpace.Y, 0, DensityFieldSize - 1);
            int zP = (int)MathHelper.Clamp(posObjSpace.Z, 0, DensityFieldSize - 1);
            bool isCollision = (DensityField[xP + (yP + zP * DensityFieldSize) * DensityFieldSize] > IsoValue);
            normal = (isCollision)?ComputeNormal(xP, yP, zP):Vector3.Zero;
            return isCollision;
        }

        void GenerateFloatingIslands(int size)
        {
            DensityFieldSize = size + 1;
            InitializeFieldData();
            
            //Here we generate our noise textures
            int nSize = 16;
            noiseTextures = new Texture3D[4];
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

            GFX.Device.SetVertexShaderConstant(1, textureMatrix);

            //Set swizzle axis to the z axis
            GFX.Device.SetPixelShaderConstant(1, Vector4.One * 2);
            
            //Here we setup our render target. 
            //This is used to fetch what is rendered to our screen and store it in a texture.
            srcTarget = new RenderTarget2D(GFX.Device, DensityFieldSize, DensityFieldSize, 1, GFX.Inst.ByteSurfaceFormat);
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
                ExtractDensityTextureData(ref DensityField, z);

            }
            GFX.Device.DepthStencilBuffer = dsOld;

            InitializeVoxels();
        }

        void ExtractDensityTextureData(ref byte[] byteField, int z)
        {
            //In the lines below, we copy the texture data into the density field buffer
            if (GFX.Inst.ByteSurfaceDataType == GFXTextureDataType.BYTE)
                srcTarget.GetTexture().GetData<byte>(byteField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
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
                        Array.Copy(densityData, 0, byteField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                        break;
                    case GFXTextureDataType.HALFSINGLE:
                        HalfSingle[] hsingData = new HalfSingle[densityData.Length];
                        srcTarget.GetTexture().GetData<HalfSingle>(hsingData);
                        for (int i = 0; i < hsingData.Length; i++)
                            densityData[i] = (byte)(hsingData[i].ToSingle() * 255.0f);
                        Array.Copy(densityData, 0, byteField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                        break;
                    case GFXTextureDataType.SINGLE:
                        float[] singData = new float[densityData.Length];
                        srcTarget.GetTexture().GetData<float>(singData);
                        for (int i = 0; i < singData.Length; i++)
                            densityData[i] = (byte)(singData[i] * 255.0f);
                        Array.Copy(densityData, 0, byteField, z * DensityFieldSize * DensityFieldSize, DensityFieldSize * DensityFieldSize);
                        break;
                }
            }
        }

        void EvaluateDensity(int axis, int sign)
        {
            ResourceManager.Inst.GetShader("ProceduralIsland").SetupShader();
            GFX.Device.SetPixelShaderConstant(0, Vector3.One / (float)DensityFieldSize);
            //Lets activate our textures
            for (int i = 0; i < noiseTextures.Length; i++)
                GFX.Device.Textures[i] = noiseTextures[i];

            GFX.Device.SetVertexShaderConstant(1, textureMatrix);

            GFX.Device.SetPixelShaderConstant(1, Vector4.One * axis);

            DepthStencilBuffer dsOld = GFX.Device.DepthStencilBuffer;
            GFX.Device.DepthStencilBuffer = GFX.Inst.dsBufferLarge;

            int zOrg = (sign < 0) ? 0 : DensityFieldSize - VoxelGridSize;
            byte[] densityDataTemp = new byte[DensityFieldSize * DensityFieldSize * VoxelGridSize];
            for (int z = 0; z < VoxelGridSize; z++)
            {
                Vector4 depth = Vector4.One * (float)(zOrg+z) / (float)(DensityFieldSize - 1);
                GFX.Device.SetVertexShaderConstant(0, depth); //Set our current depth

                GFX.Device.SetRenderTarget(0, srcTarget);
                GFX.Device.Clear(Color.TransparentBlack);

                GFXPrimitives.Quad.Render();

                GFX.Device.SetRenderTarget(0, null);

                //Now the copying stage.
                ExtractDensityTextureData(ref densityDataTemp, z);
            }
            GFX.Device.DepthStencilBuffer = dsOld;

            int stride = (axis==0)?1:((axis==1)?DensityFieldSize:DensityFieldSize*DensityFieldSize);
            int offset = (sign < 0) ? 0 : (DensityFieldSize - VoxelGridSize - 1) * stride;
            switch (axis)
            {
                case 0:
                    for (int i = 0; i < DensityFieldSize; i++)
                    {
                        for (int j = 0; j < DensityFieldSize; j++)
                        {
                            int index = i + j * DensityFieldSize;
                            int indexDField = index + offset;
                            for(int k = 0; k < VoxelGridSize; k++)
                            {
                                DensityField[indexDField] = densityDataTemp[index];
                                index+=DensityFieldSize*DensityFieldSize;
                                indexDField++;
                            }
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < DensityFieldSize; i++)
                    {
                        for (int j = 0; j < DensityFieldSize; j++)
                        {
                            int index = i + j * DensityFieldSize;
                            int indexDField = i + j * DensityFieldSize * DensityFieldSize + offset;
                            for (int k = 0; k < VoxelGridSize; k++)
                            {
                                DensityField[indexDField] = densityDataTemp[index];
                                index += DensityFieldSize*DensityFieldSize;
                                indexDField += DensityFieldSize;
                            }
                        }
                    }
                    break;
                case 2:
                    
                    Array.Copy(densityDataTemp, 0, DensityField, offset, densityDataTemp.Length);
                    break;
            }
            
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

        void MoveTerrainsAlongAxis(int axis, int sign)
        {
            int shifter = (axis == 0) ? 1 : ((axis == 1) ? VoxelGridSize : VoxelGridSize * VoxelGridSize);

            for (int i = 0; i < VoxelGridSize; i++)
            {
                for (int j = 0; j < VoxelGridSize; j++)
                {
                    int idx = (axis == 0) ? VoxelGridSize * (j + i * VoxelGridSize)
                        : (axis == 1) ? j + i * VoxelGridSize * VoxelGridSize : (j + i * VoxelGridSize);

                    int index = ((sign < 0) ? (VoxelGridSize - 1) : 0) * shifter + idx;
                    int xOrg=0, yOrg=0, zOrg=0;
                    switch (axis)
                    {
                        case 0:
                            yOrg = i;
                            zOrg = j;
                            xOrg = ((sign < 0) ? (VoxelGridSize - 1) : 0);
                            break;
                        case 1:
                            xOrg = i;
                            zOrg = j;
                            yOrg = ((sign < 0) ? (VoxelGridSize - 1) : 0);
                            break;
                        case 2:
                            yOrg = i;
                            xOrg = j;
                            zOrg = ((sign < 0) ? (VoxelGridSize - 1) : 0);
                            break;
                    }
                    float ratio = 2.0f * (float)VoxelGridSize / (float)(DensityFieldSize - 1);
                    Voxels[index].GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, VoxelGridSize, VoxelGridSize, VoxelGridSize, xOrg, yOrg, zOrg, ratio);

                    for (int k = 0; k < VoxelGridSize - 1; k++)
                    {
                        int indexA = (k + 1) * shifter + idx;
                        int indexB = k * shifter + idx;
                        if (sign > 0)
                        {
                            indexB = indexA;
                            indexA = k * shifter + idx;
                        }
                        Voxels[indexA] = Voxels[indexB];
                    }
                }
            }
        }

        void HandleCameraMotion()
        {
            Vector3 delta = (scene.MainCamera.GetPosition() - Transformation.GetPosition())/(TerrainSize*VoxelGridSize);
            int[] diffs = new int[3] { (int)Math.Floor(delta.X), (int)Math.Floor(delta.Y), (int)Math.Floor(delta.Z) };

            for (int i = 0; i < diffs.Length; i++)
            {
                while (diffs[i] != 0)
                {
                    int sign = (diffs[i] < 0)?-1:1;
                    int origin = (sign < 0)?DensityFieldSize-1:0;
                    int shifter = (i == 0) ? 1 : ((i == 1) ? DensityFieldSize : DensityFieldSize * DensityFieldSize);
                    for (int k = 0; k < DensityFieldSize; k++)
                    {
                        for (int j = 0; j < DensityFieldSize; j++)
                        {
                            int idx = (i == 0) ? DensityFieldSize * (j + k * DensityFieldSize)
                                   : (i == 1) ? j + k * DensityFieldSize * DensityFieldSize : (j + k * DensityFieldSize);

                            for (int l = 0; l < DensityFieldSize-VoxelGridSize; l++)
                            {
                                int pos = origin + l * sign;
                                int indexA = (pos+sign) * shifter + idx;
                                int indexB = pos * shifter + idx;
                                if (sign > 0)
                                {
                                    indexB = indexA;
                                    indexA = pos * shifter + idx;
                                }
                                DensityField[indexA] = DensityField[indexB];
                            }
                        }
                    }

                    Vector3 transVec = (i == 0) ? Vector3.Right : ((i == 1) ? Vector3.Up : Vector3.Forward);
                    if (sign < 0)
                        transVec *= -1;

                    float textureScale = 2.0f * ((float)VoxelGridSize / (float)DensityFieldSize) - 1.0f;
                    float worldScale = textureScale * TerrainSize;

                    textureMatrix.Translation = textureMatrix.Translation + transVec * textureScale;
                    Transformation.SetPosition(Transformation.GetPosition() + transVec * worldScale);
                    EvaluateDensity(i, sign);

                    MoveTerrainsAlongAxis(i, sign);
                    diffs[i] -= sign;
                }
            }
        }

        public BoundingBox GetWorldSpaceBoundsAtPoint(Vector3 point, int size)
        {
            Vector3 offset = TerrainSize * Vector3.One * (float)size / (float)(DensityFieldSize-1);
            BoundingBox bounds = new BoundingBox();
            bounds.Min = point - offset;
            bounds.Max = point + offset;

            return bounds;
        }

        public void CarveTerrainAtPoint(Vector3 point, int size, int isoBrush)
        {
            List<VoxelGeometry> UpdateVoxels = new List<VoxelGeometry>();
            int DensityFieldSqr = DensityFieldSize * DensityFieldSize;

            Vector3 pointObjSpace = Vector3.Transform(point, Transformation.GetObjectSpace()) * 0.5f + Vector3.One * 0.5f;
            pointObjSpace *= DensityFieldSize;

            int xW = (int)MathHelper.Clamp(pointObjSpace.X, 0, DensityFieldSize - 1);
            int yW = (int)MathHelper.Clamp(pointObjSpace.Y, 0, DensityFieldSize - 1);
            int zW = (int)MathHelper.Clamp(pointObjSpace.Z, 0, DensityFieldSize - 1);

            List<int[]> UpdateShifts = new List<int[]>();
            int xStart = (int)MathHelper.Clamp(xW - size, 0, DensityFieldSize - 1);
            int xEnd = (int)MathHelper.Clamp(xW + size, 0, DensityFieldSize - 1);
            int yStart = (int)MathHelper.Clamp(yW - size, 0, DensityFieldSize - 1);
            int yEnd = (int)MathHelper.Clamp(yW + size, 0, DensityFieldSize - 1);
            int zStart = (int)MathHelper.Clamp(zW - size, 0, DensityFieldSize - 1);
            int zEnd = (int)MathHelper.Clamp(zW + size, 0, DensityFieldSize - 1);
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    int yStride = y * DensityFieldSize;
                    for (int z = zStart; z < zEnd; z++)
                    {
                        int idx = x + yStride + z * DensityFieldSqr;
                        
                        int density = (int)(isoBrush * MathHelper.Clamp(1 - Vector3.Distance(new Vector3(x, y, z), new Vector3(xW, yW, zW)) / (float)size, 0.0f, 1.0f)) + (int)DensityField[idx];
                        DensityField[idx] = (byte)MathHelper.Clamp(density, 0, 255);
                    }
                }
            }

            int voxelCount = (DensityFieldSize - 1) / VoxelGridSize;
            int voxelCountSqr = voxelCount * voxelCount;

            int bSizeP1 = (int)(size * 1.5f);
            int bSizeN1 = bSizeP1;// brushSize * 2;
            for (int x = xW - bSizeN1; x < xW + bSizeP1; x++)
            {
                for (int y = yW - bSizeN1; y < yW + bSizeP1; y++)
                {
                    for (int z = zW - bSizeN1; z < zW + bSizeP1; z++)
                    {
                        int xV = (int)MathHelper.Clamp((float)voxelCount * ((float)x / (float)DensityFieldSize), 0, voxelCount - 1);
                        int yV = (int)MathHelper.Clamp((float)voxelCount * ((float)y / (float)DensityFieldSize), 0, voxelCount - 1);
                        int zV = (int)MathHelper.Clamp((float)voxelCount * ((float)z / (float)DensityFieldSize), 0, voxelCount - 1);
                        int voxelIndex = xV + yV * voxelCount + zV * voxelCountSqr;
                        if (!UpdateVoxels.Contains(Voxels[voxelIndex]))
                        {
                            UpdateVoxels.Add(Voxels[voxelIndex]);
                            UpdateShifts.Add(new int[] { xV, yV, zV });
                        }
                    }
                }
            }

            float ratio = 2.0f * (float)VoxelGridSize / (float)(DensityFieldSize - 1);
            for (int i = 0; i < UpdateVoxels.Count; i++)
            {
                UpdateVoxels[i].GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, VoxelGridSize, VoxelGridSize, VoxelGridSize, UpdateShifts[i][0] * VoxelGridSize, UpdateShifts[i][1] * VoxelGridSize, UpdateShifts[i][2] * VoxelGridSize, 2.0f / (float)(DensityFieldSize - 1));
            }
        }

        public override void OnUpdate()
        {
            //HandleCameraMotion();
           
            base.OnUpdate();
        }

        public override void OnRender(RenderView view)
        {
            BoundingFrustum frustum = view.GetFrustum();
            for (int i = 0; i < Voxels.Length; i++)
            {
                if (frustum.Contains(VoxelBounds[i]) != ContainmentType.Disjoint && Voxels[i].CanRender)
                {
                    view.AddElement(terrainMaterial, Voxels[i].renderElement);
                }
            }
            base.OnRender(view);
        }
    }
}
