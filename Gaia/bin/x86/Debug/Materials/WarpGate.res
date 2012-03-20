<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="ChromeShader" psfilename="Shaders/ChromeP.hlsl" vsfilename="Shaders/GemV.hlsl" pstarget="3" vstarget="2"/>

  <Material name="GateMaterial" shader="ChromeShader"
            texture_3 = "Textures/Trees/ruby2_Color.jpg"
            texture_4 = "Textures/Animal/hair2_normal.tga" refract="true"/>
</Resources>