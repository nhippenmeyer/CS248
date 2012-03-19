#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 LocalPos : TEXCOORD2;
};

PSIN main(float4 Position : POSITION0, float3 Normal : NORMAL0, 
				float Index : TEXCOORD0, uniform float4x4 ModelView : register(VC_MODELVIEW), 
				uniform float4x4 World[NUM_INSTANCES] : register(VC_WORLD))
{
    PSIN OUT;
    int index = 0; //Index;
    float4 worldPos = mul(float4(Position.xyz,1.0f), World[index]);
    OUT.Position = mul(worldPos, ModelView);
	OUT.WorldPos = worldPos.xyz;
	OUT.Normal = mul(Normal, World[index]).xyz;
	OUT.LocalPos = Position.xyz;
	return OUT;
}
