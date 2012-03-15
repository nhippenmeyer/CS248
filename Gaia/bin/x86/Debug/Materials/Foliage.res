<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="FoliageShader" psfilename="Shaders/FoliageP.hlsl" vsfilename="Shaders/FoliageV.hlsl" pstarget="3" vstarget="2"/>
  
  <Texture filename="Textures/Foliage/bush5_diffus.tga"/>
  <Texture filename="Textures/Foliage/bush5_normal.tga"/>
  
  <Texture filename="Textures/Foliage/bush4_diffus.tga"/>
  <Texture filename="Textures/Foliage/bush4_normal.tga"/>

  <Texture filename="Textures/Foliage/glow3_diffus.tga"/>
  <Texture filename="Textures/Foliage/glow3_normal.tga"/>

  <Material name="GrassMat0" shader="FoliageShader"
            texture_0 = "Textures/Foliage/bush5_diffus.tga"
            texture_1 = "Textures/Foliage/bush5_normal.tga"/>
  <Material name="GrassMat1" shader="FoliageShader"
            texture_0 = "Textures/Foliage/bush4_diffus.tga"
            texture_1 = "Textures/Foliage/bush4_normal.tga"/>
  <Material name="GrassMat2" shader="FoliageShader"
            texture_0 = "Textures/Foliage/glow3_diffus.tga"
            texture_1 = "Textures/Foliage/glow3_normal.tga"/>
</Resources>