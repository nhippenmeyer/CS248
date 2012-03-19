#include "ShaderConst.h"

struct PSIN_Particle
{
	float4 Position : POSITION0;
	float Size		: PSIZE0;
	float2 TexCoord : TEXCOORD0;
	float2 IndexCoord : TEXCOORD1;
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
	
    OUT.Position = mul(float4(worldPos.xyz,1), ModelView);
	OUT.IndexCoord = Index.xy;
	OUT.TexCoord = TexCoord;
	OUT.Size = lerp(ParticleSize, 4, OUT.Position.z/OUT.Position.w);
	
    return OUT;
}
