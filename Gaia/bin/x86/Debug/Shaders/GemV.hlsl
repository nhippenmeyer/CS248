#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float4 RefractTC: TEXCOORD2;
};

PSIN main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, 
				float3 Normal : NORMAL, uniform float4x4 ModelView : register(VC_MODELVIEW), 
				uniform float4x4 World : register(VC_WORLD), uniform float2 InvRes : register(VC_INVTEXRES)
)
{
    PSIN OUT;

    float4 worldPos = mul(float4(Position.xyz,1.0f), World);
    OUT.Position = mul(worldPos, ModelView);
	OUT.WorldPos = worldPos.xyz;
	OUT.Normal = mul(Normal, World).xyz;
	
	float4x4 texMat = { 0.5,  0.0,  0.0,  0.5 + 0.5 * InvRes.x,
    0.0, -0.5,  0.0,  0.5 + 0.5 * InvRes.y,
    0.0,  0.0,  1.0,  0.0,
    0.0,  0.0,  0.0,  1.0 };
    OUT.RefractTC = mul(texMat, OUT.Position);
	return OUT;
}
