<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="TerrainShader" psfilename="Shaders/VoxelP.hlsl" vsfilename="Shaders/VoxelV.hlsl" pstarget="3" vstarget="2"/>
  
  <Texture filename="Textures/Terrain/sand3_diffus.tga"/>
  <Texture filename="Textures/Terrain/sand3_normal.tga"/>
  
  <Texture filename="Textures/Terrain/rock2_diffus.tga"/>
  <Texture filename="Textures/Terrain/rock2_normal.tga"/>

  <Texture filename="Textures/Terrain/grass_diffus.tga"/>
  <Texture filename="Textures/Terrain/grass_normal.tga"/>

  <Texture filename="Textures/Terrain/grass4_diffus.tga"/>
  <Texture filename="Textures/Terrain/grass4_normal.tga"/>

  <Texture filename="Textures/Terrain/rock_diffus.tga"/>
  <Texture filename="Textures/Terrain/rock_normal.tga"/>

  <Texture filename="Textures/Climates/climate_volcano.dds"/>
  
  <Material name="TerrainMaterial" shader="TerrainShader" 
            texture_0 = "Textures/Terrain/sand3_diffus.tga"
            texture_1 = "Textures/Terrain/sand3_normal.tga"
            
            texture_2 = "Textures/Terrain/rock2_diffus.tga"
            texture_3 = "Textures/Terrain/rock2_normal.tga"
            
            texture_4 = "Textures/Terrain/grass_diffus.tga"
            texture_5 = "Textures/Terrain/grass_normal.tga"
            
            texture_6 = "Textures/Terrain/grass4_diffus.tga"
            texture_7 = "Textures/Terrain/grass4_normal.tga"
            
            texture_8 = "Textures/Terrain/rock_diffus.tga"
            texture_9 = "Textures/Terrain/rock_normal.tga"
         
            texture_10 = "Textures/Climates/climate_volcano.dds" />
</Resources>