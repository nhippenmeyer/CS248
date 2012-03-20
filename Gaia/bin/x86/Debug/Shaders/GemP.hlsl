#include "Prepass.h"
#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float4 RefractTC: TEXCOORD2;
};

float4 main(PSIN IN, uniform sampler RefractMap : register(S0),
			uniform sampler DepthMap : register(S1),
			uniform samplerCUBE ReflectMap : register(S2),
			uniform sampler NormalMap : register(S3), 
			uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	float3 V = IN.WorldPos.xyz-EyePos.xyz;
	float z = length(V);
	
	float2 refractTC = IN.RefractTC.xy / IN.RefractTC.w;
	
	float depth = tex2D(DepthMap, refractTC).r * EyePos.w;
	if(depth - EPS < 0.0)
		depth = EyePos.w;
	clip(depth-z);
	
	V = normalize(V);
	
	float3 N = normalize(IN.Normal);
	
	float3 blend_weights = 7*(abs(N.xyz)-0.2);   // Tighten up the blending zone:  
	blend_weights = pow(blend_weights,3);
	//blend_weights = (blend_weights - 0.06) * 3;  
	blend_weights = max(blend_weights, 0);      
	// Force weights to sum to 1.0 (very important!)  
	blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z );
	float3 TC = IN.WorldPos*0.25;
	
	float2 b1 = tex2D(NormalMap, TC.zy).xy-0.5;
	float2 b2 = tex2D(NormalMap, TC.xz).xy-0.5;
	float2 b3 = tex2D(NormalMap, TC.xy).xy-0.5;
	
	float3 n1 = float3(0, b1.x, b1.y);
	float3 n2 = float3(b2.y, 0, b2.x);
	float3 n3 = float3(b3.x, b3.y, 0);
	float3 blendBump = n1.xyz * blend_weights.x +  
                   n2.xyz * blend_weights.y +  
                   n3.xyz * blend_weights.z;
    N = normalize(blendBump+N);
    
    float refractCoeff = saturate(1.0-length(EyePos.xyz-IN.WorldPos)/50)+0.15;
    
    refractTC.xy += N.xz*refractCoeff;
	
	float3 refractColor = tex2D(RefractMap, refractTC.xy);
	
	float3 reflectColor = texCUBE(ReflectMap, reflect(V, normalize(IN.Normal)));
	
	return float4(lerp(float3(172,24,24)/255.0f, lerp(reflectColor, refractColor, 0.25),0.78), 1.0f);
}