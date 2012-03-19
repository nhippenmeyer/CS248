<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="GemShader" psfilename="Shaders/GemP.hlsl" vsfilename="Shaders/GemV.hlsl" pstarget="3" vstarget="2"/>

  <Texture filename="Textures/Trees/ruby2_Color.jpg"/>

  <Material name="GemMaterial" shader="GemShader"
            texture_0 = "Textures/Trees/ruby2_Color.jpg"
            texture_1 = "Textures/Foliage/flowers_diffus.tga"/>
</Resources>