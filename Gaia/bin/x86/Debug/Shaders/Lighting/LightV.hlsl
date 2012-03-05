#include "../ShaderConst.h"

struct PSIN_Light
{
    float4 Position : POSITION0;
    float4 TexCoord : TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
};

PSIN_Light main(float4 Position : POSITION0, uniform float4x4 ModelView : register(VC_MODELVIEW), 
				uniform float4x4 World : register(VC_WORLD), uniform float4x4 TexGen : register(VC_TEXGEN)
)
{
    PSIN_Light OUT;

    float4 worldPos = mul(float4(Position.xyz,1.0f),World);
    OUT.Position = mul(worldPos, ModelView);
	OUT.WorldPos = worldPos;
	
	OUT.TexCoord = mul(TexGen, OUT.Position);

    return OUT;
}
