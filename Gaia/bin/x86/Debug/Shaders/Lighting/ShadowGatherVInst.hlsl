#include "../ShaderConst.h"

struct PSSHADOW
{
	float4 Position : POSITION0;
	float4 WorldPos : TEXCOORD0;
};

PSSHADOW main(float4 Position : POSITION0, float Index : TEXCOORD1, uniform float4x4 ModelView : register(VC_MODELVIEW), uniform float4x4 World[NUM_INSTANCES] : register(VC_WORLD))
{
    PSSHADOW output;

    float4 worldPos = mul(float4(Position.xyz,1.0f), World[Index]);
    output.Position = mul(worldPos, ModelView);
	output.WorldPos = output.Position;
	//output.WorldPos = worldPos;
    return output;
}
