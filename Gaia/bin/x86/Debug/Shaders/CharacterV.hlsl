#include "ShaderConst.h"
#include "Common.h"

struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal	: TEXCOORD1;
    float3 Tangent  : TEXCOORD2;
    float3 WorldPos : TEXCOORD3;
};

PSIN_Voxel main(VS_COMMON input, uniform float4x4 ModelView : register(VC_MODELVIEW), uniform float4x4 World : register(VC_WORLD))
{
    PSIN_Voxel output;

    float4 worldPos = mul(float4(input.Position.xyz,1.0f),World);
    output.Position = mul(worldPos, ModelView);
	output.Normal = mul(input.Normal, ModelView);
	output.Tangent = mul(input.Tangent, ModelView);
	output.WorldPos = worldPos;
    return output;
}
