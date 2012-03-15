<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="TreeShader" psfilename="Shaders/TreeP.hlsl" vsfilename="Shaders/TreeV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="LeafShader" psfilename="Shaders/LeafP.hlsl" vsfilename="Shaders/LeafV.hlsl" pstarget="3" vstarget="2"/>

  <Texture filename="Textures/Trees/bark_diffuse.tga"/>
  <Texture filename="Textures/Trees/bark_normal.tga"/>

  <Texture filename="Textures/Trees/Leaf_8_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_8_Normal.png"/>

  <Texture filename="Textures/Foliage/flowers_diffus.tga"/>
  <Texture filename="Textures/Foliage/flowers_normal.tga"/>

  <Material name="TreeMat" shader="TreeShader"
            texture_0 = "Textures/Trees/bark_diffuse.tga"
            texture_1 = "Textures/Trees/bark_normal.tga"/>
  <Material name="LeafMat" shader="LeafShader"
            texture_0 = "Textures/Trees/Leaf_8_Color.png"
            texture_1 = "Textures/Trees/Leaf_8_Normal.png"/>
</Resources>