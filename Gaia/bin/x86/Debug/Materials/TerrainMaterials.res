<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="TerrainShader" psfilename="Shaders/VoxelP.hlsl" vsfilename="Shaders/VoxelV.hlsl" pstarget="3" vstarget="2"/>
  
  <Texture filename="Textures/Terrain/sand3_diffus.dds"/>
  <Texture filename="Textures/Terrain/sand3_normal.dds"/>
  
  <Texture filename="Textures/Terrain/rock2_diffus.dds"/>
  <Texture filename="Textures/Terrain/rock2_normal.dds"/>

  <Texture filename="Textures/Terrain/grass_diffus.dds"/>
  <Texture filename="Textures/Terrain/grass_normal.dds"/>

  <Texture filename="Textures/Terrain/grass4_diffus.dds"/>
  <Texture filename="Textures/Terrain/grass4_normal.dds"/>

  <Texture filename="Textures/Terrain/rock_diffus.dds"/>
  <Texture filename="Textures/Terrain/rock_normal.dds"/>

  <Texture filename="Textures/Climates/climate_volcano.dds"/>
  
  <Material name="TerrainMaterial" shader="TerrainShader" 
            texture_4 = "Textures/Terrain/sand3_diffus.dds"
            texture_5 = "Textures/Terrain/sand3_normal.dds"
            
            texture_2 = "Textures/Terrain/rock2_diffus.dds"
            texture_3 = "Textures/Terrain/rock2_normal.dds"
            
            texture_0 = "Textures/Terrain/grass_diffus.dds"
            texture_1 = "Textures/Terrain/grass_normal.dds"
            
            texture_6 = "Textures/Terrain/grass4_diffus.dds"
            texture_7 = "Textures/Terrain/grass4_normal.dds"
            
            texture_8 = "Textures/Terrain/rock_diffus.dds"
            texture_9 = "Textures/Terrain/rock_normal.dds"
         
            texture_10 = "Textures/Climates/climate_volcano.dds" />
</Resources>