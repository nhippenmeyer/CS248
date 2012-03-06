#include "../ShaderConst.h"

struct PSSHADOW
{
	float4 Position : POSITION0;
	float4 WorldPos : TEXCOORD0;
};

PSSHADOW main(float4 Position : POSITION0, uniform float4x4 ModelView : register(VC_MODELVIEW), uniform float4x4 World : register(VC_WORLD))
{
    PSSHADOW output;

    float4 worldPos = mul(float4(Position.xyz,1.0f), World);
    output.Position = mul(worldPos, ModelView);
	output.WorldPos = output.Position;//worldPos;
    return output;
}
