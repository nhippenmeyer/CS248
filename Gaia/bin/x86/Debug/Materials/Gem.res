<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="GemShader" psfilename="Shaders/GemP.hlsl" vsfilename="Shaders/GemV.hlsl" pstarget="3" vstarget="2"/>

  <Texture name="GemTex" filename="Textures/Trees/ruby_Color.png"/>
  <Texture name="GemTex" filename="Textures/Trees/ruby2_Color.jpg"/>

  <Material name="GemMaterial" shader="GemShader"
            texture_3 = "Textures/Trees/ruby2_Color.jpg"
            texture_4 = "Textures/Animal/hair_normal.tga" refract="true"/>
</Resources>