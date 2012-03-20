#include "ShaderConst.h"
#include "Common.h"

struct PSIN_Voxel
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 LocalPos : TEXCOORD2;
};

PSIN_Voxel main(VS_COMMON input, uniform float4x4 ModelView : register(VC_MODELVIEW), uniform float4x4 World : register(VC_WORLD)
)
{
    PSIN_Voxel output;

    float4 worldPos = mul(float4(input.Position.xyz,1.0f),World);
    output.Position = mul(worldPos, ModelView);
	output.Normal = input.Normal;
	output.WorldPos = worldPos;
	output.LocalPos = input.Position;
    return output;
}
