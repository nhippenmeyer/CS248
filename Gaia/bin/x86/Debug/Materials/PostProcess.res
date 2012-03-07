<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="GaussH" psfilename="Shaders/PostProcess/GaussH.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="GaussV" psfilename="Shaders/PostProcess/GaussV.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="BilateralH" psfilename="Shaders/PostProcess/BilateralH.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="BilateralV" psfilename="Shaders/PostProcess/BilateralV.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="BoxH" psfilename="Shaders/PostProcess/BoxH.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="BoxV" psfilename="Shaders/PostProcess/BoxV.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>

  <Shader name="MotionBlur" psfilename="Shaders/PostProcess/MotionBlurP.hlsl" vsfilename="Shaders/PostProcess/GenericSpatialV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="Composite" psfilename="Shaders/PostProcess/CompositeP.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="2" vstarget="2"/>
  <Shader name="Fog" psfilename="Shaders/PostProcess/FogP.hlsl" vsfilename="Shaders/PostProcess/GenericSpatialV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="ColorCorrect" psfilename="Shaders/PostProcess/ColorCorrectP.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="2" vstarget="2"/>
  <Shader name="GodRay" psfilename="Shaders/PostProcess/CrepescularP.hlsl" vsfilename="Shaders/PostProcess/GenericV.hlsl" pstarget="3" vstarget="2"/>
  <Texture filename="Textures/Color Correction/colorRamp0.dds"/>
</Resources>