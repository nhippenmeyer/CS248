#include "../ShaderConst.h"

float4 main(PSSHADOW input, uniform float4 LightPos : register(PC_EYEPOS) ) : COLOR
{
    return length(LightPos.xyz-input.WorldPos.xyz)/LightPos.w);
}