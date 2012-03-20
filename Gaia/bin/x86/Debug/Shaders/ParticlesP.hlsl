#include "ShaderConst.h"
#include "Prepass.h"

float blendFunction(float x)
{

	float blend = 0.5*pow(saturate(2*(( x > 0.5) ? 1-x : x)), 0.75); 
	blend = ( x > 0.5) ? 1-blend : blend;
	return blend;
}

float4 main(float2 TexCoord : TEXCOORD0, float2 IndexCoord : TEXCOORD1, 
			float2 ScreenPos : VPOS,
			uniform sampler BaseMap : register(S1), uniform sampler ColorMap : register(S0),
			uniform sampler DepthMap: register(S2), uniform float2 InvRes : register(C0),
			uniform float4 lifetimeParams : register(C1), uniform float4 blendParams : register(C2),
			uniform float3 particleColor : register(C3),
			uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	float4 wp = tex2D(ColorMap, IndexCoord);
	float2 TC = ScreenPos*InvRes;
	TC = TC+0.5*InvRes;
	float z = tex2D(DepthMap, TC).r*EyePos.w;
	float depth = length(EyePos.xyz-wp.xyz);
	if(z <= EPS)
	 z = EyePos.w;
	clip(z-depth+EPS);
		
	float density = lifetimeParams.z;
	float blend = lerp(blendFunction(saturate(70*(z-depth)/EyePos.w)), 1, density);
	
	float age = saturate(wp.w/lifetimeParams.x);
	float alpha = 1.0f;
	if(age < blendParams.x)
	{
		alpha = blendParams.y*age;
	} 
	if(age > blendParams.z)
	{
		alpha = blendParams.w - blendParams.w*age;
	}
		
	return tex2D(BaseMap, TexCoord) * alpha * blend * float4(particleColor, 1.0f);
}