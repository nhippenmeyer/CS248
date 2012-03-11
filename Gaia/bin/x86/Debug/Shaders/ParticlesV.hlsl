#include "ShaderConst.h"

struct PSIN_Particle
{
	float4 Position : POSITION0;
	float Size		: PSIZE0;
	float2 TexCoord : TEXCOORD0;
	float2 IndexCoord : TEXCOORD1;
	float4 WorldPos	: TEXCOORD2;
};

PSIN_Particle main(float2 Index : POSITION0, float Size : PSIZE0, float2 TexCoord : TEXCOORD0, 
				uniform sampler2D PositionMap : register(S0),
				uniform float4x4 ModelView : register(VC_MODELVIEW),
				uniform float ParticleSize : register(C4),
				uniform float4 blendParams : register(C5),
				uniform float4 lifetimeParams: register(C6)
)
{
    PSIN_Particle OUT;

	float4 worldPos = tex2Dlod(PositionMap, float4(Index.xy, 0, 0)).xyzw;
	
	float age = saturate(1.0f-worldPos.w/lifetimeParams.x);
	float alpha = 1.0f;
	if(age < blendParams.x)
	{
		alpha = blendParams.y*age;
	} 
	if(age > blendParams.z)
	{
		alpha = blendParams.w - blendParams.w*age;
	}
	
    OUT.Position = mul(float4(worldPos.xyz,1), ModelView);
	OUT.TexCoord = TexCoord;
	OUT.IndexCoord = Index.xy;
	OUT.WorldPos = worldPos;//float4(worldPos.xyz, age);
	OUT.Size = lerp(ParticleSize, 32, OUT.Position.z/OUT.Position.w);
	
    return OUT;
}
