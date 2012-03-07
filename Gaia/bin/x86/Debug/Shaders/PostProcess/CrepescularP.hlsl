#include "../ShaderConst.h"
#include "../Prepass.h"

float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0),
			uniform sampler DepthMap : register(S1), uniform float4 EyePos : register(PC_EYEPOS),
			uniform float2 SunDirSS : register(C0), uniform float3 SunParams : register(C1), 
			uniform float3 SunColor : register(C2)
) : COLOR
{
	const int SAMPLES = 16;
	float2 deltaTC = TexCoord-SunDirSS;
	deltaTC *= 1.0/(float)SAMPLES * SunParams.x;
	float4 finalColor = 0;
	float illumDecay = 1.0f;
	float2 TC = TexCoord;
	for(int i = 0; i < SAMPLES; i++)
	{
		TC -= deltaTC;
		float samp = (tex2D(DepthMap, TexCoord)*EyePos.w-EPS <= 0.0)?1:0;
		samp *= illumDecay;
		illumDecay *= SunParams.y;
		finalColor += samp;
	}
	
	return tex2D(BaseMap, TexCoord) - finalColor * float4(SunColor*SunParams.z,1);
	
}