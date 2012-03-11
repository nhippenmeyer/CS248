#include "../ShaderConst.h"

float2 ComputeMoments(float D)
{
	float2 Moments;
	Moments.x = D;
	float dx = ddx(D);
	float dy = ddy(D);
	Moments.y = D*D + 0.25*(dx*dx + dy*dy);

	return Moments;
}

float4 main(float4 WorldPos : TEXCOORD0, uniform float4 LightPos : register(PC_EYEPOS) ) : COLOR
{
    //return WorldPos.z / WorldPos.w;
    //return float4(ComputeMoments(length(WorldPos.xyz-LightPos.xyz)/LightPos.w),0,0);
    return float4(ComputeMoments(WorldPos.z / WorldPos.w),0,0);
}