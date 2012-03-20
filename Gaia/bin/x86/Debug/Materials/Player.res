<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<Resources>
  <Texture filename ="Textures/Particles/BasicBlue.dds"/>
  <Texture filename ="Textures/Particles/BasicPlain.dds"/>

  <Material name="PlayerMaterial" shader="ParticleShader"
            texture_1 = "Textures/Particles/BasicPlain.dds"/>
  <ParticleEffect name="PlayerParticles" size="7" lifetime="3.4" lifetimeVariance="0.0" density="0.8"
                  randomInitSpeed="0.9" initialSpeedVariance="0.4" mass="6" massVariance="3"
                  initialspeed="5.6" initialDirection="0 -1 0" randomoffset="0.5 0.15 0.5" offsetMagnitude="1.5"
                  fadeInPercent="0.3" fadeOutPercent="0.7" material="PlayerMaterial"
                  />
</Resources>