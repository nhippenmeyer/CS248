<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources>
  <Texture filename ="Textures/Sky/StarrySky.dds" type="texturecube"/>
  <Shader name="SkyShader" psfilename="Shaders/SkyP.hlsl" vsfilename="Shaders/SkyV.hlsl" pstarget="2" vstarget="2"/>
  <Shader name="SkyShaderPrepass" psfilename="Shaders/SkyPreP.hlsl" vsfilename="Shaders/SkyV.hlsl" pstarget="3" vstarget="2"/>
</Resources>