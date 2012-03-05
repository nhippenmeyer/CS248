#include "../ShaderConst.h"
#include "../Common.h"

float2 ComputeMoments(float3 Depth)
{
	float2 Moments;
	float D = length(Depth);
	Moments.x = D;
	float dx = ddx(D);
	float dy = ddy(D);
	Moments.y = D*D + 0.25*(dx*dx + dy*dy);

	return Moments;
}

float4 main(PSSHADOW input, uniform float4 LightPos : register(PC_EYEPOS) ) : COLOR
{
    return float4(ComputeMoments((LightPos.xyz-input.WorldPos.xyz)/LightPos.w),0,0);
}