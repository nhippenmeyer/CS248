#include "../Prepass.h"
#include "../ShaderConst.h"
float4 main(float2 TexCoord : TEXCOORD0, float3 Direction : TEXCOORD1, 
			uniform sampler DepthMap : register(S0), uniform float4 EyePos : register(PC_EYEPOS),
			uniform float3 fogColor : register(C0), uniform float3 fogParams : register(C1)
) : COLOR
{
    float2 TC = TexCoord.xy;
    float z = tex2D(DepthMap, TC).r*EyePos.w;
    clip(z-EPS);
    
    float3 worldPos = normalize(Direction)*z+EyePos.xyz;
    
	float fogInt = max(EyePos.w-z,1) * (worldPos.y-fogParams.x);
	float t = fogParams.y * worldPos.y;
	fogInt *= ( 1.0 - exp( -t ) ) / t;
	float4 finalColor = float4(fogColor,1)*saturate(exp( -fogParams.z * fogInt ));
    
	return finalColor;
}