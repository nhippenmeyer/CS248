#include "../ShaderConst.h"
#include "../Common.h"

PSSHADOW main(VS_COMMON input, uniform float4x4 ModelView : register(VC_MODELVIEW), uniform float4x4 World : register(VC_WORLD))
{
    PSSHADOW output;

    float4 worldPos = mul(float4(input.Position.xyz,1.0f),World);
    output.Position = mul(worldPos, ModelView);
	output.WorldPos = worldPos;
    return output;
}
