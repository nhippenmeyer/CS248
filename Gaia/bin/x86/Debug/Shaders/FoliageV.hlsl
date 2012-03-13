#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal	: TEXCOORD1;
    float3 Tangent  : TEXCOORD2;
    float3 WorldPos : TEXCOORD3;
};

PSIN main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, 
				float Index : TEXCOORD1, uniform float4x4 ModelView : register(VC_MODELVIEW), 
				uniform float4x4 World[NUM_INSTANCES] : register(VC_WORLD))
{
    PSIN OUT;

	float4x4 worldMat = World[Index];
    float4 worldPos = mul(float4(Position.xyz,1.0f), World[Index]);
    OUT.Position = mul(worldPos, ModelView);
	OUT.TexCoord = TexCoord;
	OUT.WorldPos = worldPos.xyz;
	OUT.Normal = mul(float4(0,1,0,0), World[Index]).xyz;
	OUT.Tangent = mul(float4(0,0,1,0), World[Index]).xyz;
	return OUT;
}
