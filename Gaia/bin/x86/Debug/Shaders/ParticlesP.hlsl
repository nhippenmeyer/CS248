#include "ShaderConst.h"

float blendFunction(float x)
{

	float blend = 0.5*pow(saturate(2*(( x > 0.5) ? 1-x : x)), 0.75); 
	blend = ( x > 0.5) ? 1-blend : blend;
	return blend;
}

float4 main(float2 TexCoord : TEXCOORD0, float2 IndexCoord : TEXCOORD1, 
			float2 ScreenPos : VPOS, float4 WorldPos : TEXCOORD2,
			uniform sampler BaseMap : register(S1), uniform sampler ColorMap : register(S0),
			uniform sampler DepthMap: register(S2), uniform float2 InvRes : register(C0),
			uniform float4 lifetimeParams : register(C1), uniform float4 blendParams : register(C2),
			uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	float2 TC = ScreenPos*InvRes;
	TC = TC+0.5*InvRes;
	float z = tex2D(DepthMap, TC).r*EyePos.w;
	float depth = length(EyePos.xyz-WorldPos.xyz);
	if(z == 0.0)
	 z = EyePos.w;
	if(depth > z)
		discard;
		
	float age = saturate(1.0f-WorldPos.w/lifetimeParams.x);
	float alpha = 1.0f;
	if(age < blendParams.x)
	{
		alpha = blendParams.y*age;
	} 
	if(age > blendParams.z)
	{
		alpha = blendParams.w - blendParams.w*age;
	}
	float density = lifetimeParams.z;
	float blend = lerp(blendFunction(saturate(70*(z-depth)/EyePos.w)), 1, density);
		
	return tex2D(BaseMap, TexCoord) * blend * alpha;
}