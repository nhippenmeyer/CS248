<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources>
  <Texture filename ="Textures/Particles/SmokeWhite.dds"/>
  <Texture filename ="Textures/Particles/SmokeWhite1.dds"/>
  <Texture filename ="Textures/Particles/SmokeWhite2.dds"/>
  <Texture filename ="Textures/Particles/SmokeWhite3.dds"/>
  <Texture filename ="Textures/Particles/SmokeWhite4.dds"/>
  
  <Shader name="ParticleShader" psfilename="Shaders/ParticlesP.hlsl" vsfilename="Shaders/ParticlesV.hlsl" pstarget="3" vstarget="3"/>
  <Material name="ParticleMaterial" shader="ParticleShader"
            texture_1 = "Textures/Particles/SmokeWhite.dds"/>
  <Material name="ParticleMaterial1" shader="ParticleShader"
            texture_1 = "Textures/Particles/SmokeWhite1.dds"/>
  <Material name="ParticleMaterial2" shader="ParticleShader"
            texture_1 = "Textures/Particles/SmokeWhite2.dds"/>
  <Material name="ParticleMaterial3" shader="ParticleShader"
            texture_1 = "Textures/Particles/SmokeWhite3.dds"/>
  <Material name="ParticleMaterial4" shader="ParticleShader"
            texture_1 = "Textures/Particles/SmokeWhite4.dds"/>
  <ParticleEffect name="Smoke0" size="60" lifetime="11" lifetimeVariance="0.0"
                  randomInitSpeed="0.9" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="1.6" initialDirection="0 1 0" randomoffset="1 1 1" offsetMagnitude="150.5"
                  fadeInPercent="0.2" fadeOutPercent="0.6" material="ParticleMaterial"
                  />
  <ParticleEffect name="Smoke1" size="70" lifetime="11" lifetimeVariance="0.0"
                  randomInitSpeed="0.9" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="0.6" initialDirection="0 1 0" randomoffset="0.25 0.25 0.25" offsetMagnitude="190.5"
                  fadeInPercent="0.25" fadeOutPercent="0.65" material="ParticleMaterial1"
                  />
  <ParticleEffect name="Smoke2" size="80" lifetime="15" lifetimeVariance="0.0"
                  randomInitSpeed="0.9" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="0.6" initialDirection="0 1 0" randomoffset="1 0 1" offsetMagnitude="200.5"
                  fadeInPercent="0.25" fadeOutPercent="0.65" material="ParticleMaterial2"
                  />
  <ParticleEffect name="Smoke3" size="55" lifetime="11" lifetimeVariance="0.0"
                  randomInitSpeed="0.9" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="2.6" initialDirection="0 1 0" randomoffset="0.75 0.75 0.75" offsetMagnitude="170.5"
                  fadeInPercent="0.3" fadeOutPercent="0.6" material="ParticleMaterial3"
                  />
  <ParticleEffect name="Smoke4" size="65" lifetime="11" lifetimeVariance="0.0"
                  randomInitSpeed="0.9" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="0.6" initialDirection="0 1 0" randomoffset="0.5 0.5 0.5" offsetMagnitude="180.5"
                  fadeInPercent="0.3" fadeOutPercent="0.6" material="ParticleMaterial4"
                  />

  <Texture filename ="Textures/Particles/BurstGold.dds"/>
  <Texture filename ="Textures/Particles/FireGold.dds"/>
  <Texture filename ="Textures/Particles/FireRed.dds"/>

  <Material name="FlameMaterial0" shader="ParticleShader"
            texture_1 = "Textures/Particles/BurstGold.dds"/>
  <Material name="FlameMaterial1" shader="ParticleShader"
            texture_1 = "Textures/Particles/FireGold.dds"/>
  <Material name="FlameMaterial2" shader="ParticleShader"
            texture_1 = "Textures/Particles/FireRed.dds"/>

  <ParticleEffect name="Fire0" size="55" lifetime="1.5" lifetimeVariance="0.0"
                  randomInitSpeed="0.19" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="0.36" initialDirection="0 1 0" randomoffset="1 0 1" offsetMagnitude="12"
                  fadeInPercent="0.1" fadeOutPercent="0.7" material="FlameMaterial0"
                  />

  <ParticleEffect name="Fire1" size="45" lifetime="1.65" lifetimeVariance="0.0"
                  randomInitSpeed="0.19" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="0.36" initialDirection="0 1 0" randomoffset="0.5 0.35 0.5" offsetMagnitude="12"
                  fadeInPercent="0.1" fadeOutPercent="0.7" material="FlameMaterial1"
                  />
  <ParticleEffect name="Fire2" size="35" lifetime="1.45" lifetimeVariance="0.0"
                  randomInitSpeed="0.19" initialSpeedVariance="0.4" mass="40" massVariance="5"
                  initialspeed="0.36" initialDirection="0 1 0" randomoffset="0.15 1 0.15" offsetMagnitude="8"
                  fadeInPercent="0.1" fadeOutPercent="0.7" material="FlameMaterial2"
                  />
  
</Resources>