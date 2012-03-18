<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources> 
  <Shader name="TreeShader" psfilename="Shaders/TreeP.hlsl" vsfilename="Shaders/TreeV.hlsl" pstarget="3" vstarget="2"/>
  <Shader name="LeafShader" psfilename="Shaders/LeafP.hlsl" vsfilename="Shaders/LeafV.hlsl" pstarget="3" vstarget="2"/>

  <Texture filename="Textures/Trees/bark2_diffuse.tga"/>
  <Texture filename="Textures/Trees/bark2_normal.tga"/>

  <Texture filename="Textures/Trees/Leaf_1_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_1_Normal.png"/>

  <Texture filename="Textures/Trees/Leaf_3_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_3_Normal.png"/>

  <Texture filename="Textures/Trees/Leaf_4_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_4_Normal.png"/>

  <Texture filename="Textures/Trees/Leaf_5_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_5_Normal.png"/>

  <Texture filename="Textures/Trees/Leaf_6_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_6_Normal.png"/>
  
  <Texture filename="Textures/Trees/Leaf_7_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_7_Normal.png"/>

  <Texture filename="Textures/Trees/Leaf_8_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_8_Normal.png"/>

  <Texture filename="Textures/Trees/Leaf_9_Color.png"/>
  <Texture filename="Textures/Trees/Leaf_9_Normal.png"/>

  <Texture filename="Textures/Foliage/flowers_diffus.tga"/>
  <Texture filename="Textures/Foliage/flowers_normal.tga"/>

  <Material name="TreeMat" shader="TreeShader"
            texture_0 = "Textures/Trees/bark2_diffuse.tga"
            texture_1 = "Textures/Trees/bark2_normal.tga"/>
  <Material name="LeafMat0" shader="LeafShader"
            texture_0 = "Textures/Trees/Leaf_1_Color.png"
            texture_1 = "Textures/Trees/Leaf_1_Normal.png"/>
  <Material name="LeafMat1" shader="LeafShader"
            texture_0 = "Textures/Trees/Leaf_3_Color.png"
            texture_1 = "Textures/Trees/Leaf_3_Normal.png"/>
  <Material name="LeafMat2" shader="LeafShader"
            texture_0 = "Textures/Trees/Leaf_4_Color.png"
            texture_1 = "Textures/Trees/Leaf_4_Normal.png"/>
  <Material name="LeafMat3" shader="LeafShader"
            texture_0 = "Textures/Trees/Leaf_5_Color.png"
            texture_1 = "Textures/Trees/Leaf_5_Normal.png"/>
  <Material name="LeafMat4" shader="LeafShader"          
            texture_0 = "Textures/Trees/Leaf_6_Color.png"
            texture_1 = "Textures/Trees/Leaf_6_Normal.png"/>
  <Material name="LeafMat5" shader="LeafShader"          
            texture_0 = "Textures/Trees/Leaf_7_Color.png"
            texture_1 = "Textures/Trees/Leaf_7_Normal.png"/>
  <Material name="LeafMat6" shader="LeafShader"           
            texture_0 = "Textures/Trees/Leaf_8_Color.png"
            texture_1 = "Textures/Trees/Leaf_8_Normal.png" />
  <Material name="LeafMat7" shader="LeafShader"
            texture_0 = "Textures/Trees/Leaf_9_Color.png"
            texture_1 = "Textures/Trees/Leaf_9_Normal.png"/>
</Resources>