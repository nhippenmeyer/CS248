#include "Prepass.h"
#include "ShaderConst.h"

struct PSIN_Voxel
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 LocalPos : TEXCOORD2;
};

GBUFFER main(PSIN_Voxel input, uniform sampler ColorSAMP0 : register(S0), uniform sampler NormalSAMP0 : register(S1), uniform sampler ColorSAMP1 : register(S2),
uniform sampler NormalSAMP1 : register(S3), uniform sampler ColorSAMP2 : register(S4), uniform sampler NormalSAMP2 : register(S5), uniform sampler ColorSAMP3 : register(S6),
uniform sampler NormalSAMP3 : register(S7), uniform sampler ColorSAMP4 : register(S8), uniform sampler NormalSAMP4 : register(S9), uniform sampler ClimateSAMP : register(S10),
uniform sampler SandCSAMP : register(S11), uniform sampler SandNSAMP : register(S12),
uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	GBUFFER OUT;
    float3 N = normalize(input.Normal);
	float3 T = N.zxy;
	float3x3 TBN = float3x3(T,cross(N,T), N);
	
	float3 blend_weights = 7*(abs(N.xyz)-0.2);   // Tighten up the blending zone:  
	blend_weights = pow(blend_weights,3);
	//blend_weights = (blend_weights - 0.06) * 3;  
	blend_weights = max(blend_weights, 0);      
	// Force weights to sum to 1.0 (very important!)  
	blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z );
	float3 TC = input.WorldPos*0.125;
	
	float altitude = input.LocalPos.y * 0.5 + 0.5;
	float minSandAlt = 0.26;
	float maxSandAlt = 0.32;
	float sandWeight = pow(saturate((altitude - minSandAlt) / (maxSandAlt - minSandAlt)), 3.0);
	
	// Compute sand contribution
	float4 col1 = lerp(tex2D(SandCSAMP, TC.zy), (N.x < 0.0f)? tex2D(ColorSAMP3, TC.zy) : tex2D(ColorSAMP4, TC.zy), sandWeight);
	float4 col2 = lerp(tex2D(SandCSAMP, TC.xz), (N.y < 0.0f)? tex2D(ColorSAMP1, TC.xz) : tex2D(ColorSAMP0, TC.xz), sandWeight);
	float4 col3 = lerp(tex2D(SandCSAMP, TC.xy), tex2D(ColorSAMP2, TC.xy), sandWeight);
	
	float2 b1 = -0.5+(N.x < 0.0f)?tex2D(NormalSAMP3, TC.zy).xy:tex2D(NormalSAMP4, TC.zy).xy;
	float2 b2 = -0.5+(N.y < 0.0f)?tex2D(NormalSAMP1, TC.xz).xy:tex2D(NormalSAMP0, TC.xz).xy;
	float2 b3 = tex2D(NormalSAMP2, TC.xy).xy-0.5;
	
	/*
	float3 n1 = (N.x < 0.0f)?tex2D(NormalSAMP3, TC.zy)*2-1:tex2D(NormalSAMP4, TC.zy)*2-1;
	float3 n2 = ((N.y < 0.0f)?tex2D(NormalSAMP1, TC.xz):tex2D(NormalSAMP0, TC.xz))*2-1;
	float3 n3 = tex2D(NormalSAMP2, TC.xy)*2-1;
	*/
	float3 n1 = float3(0, b1.x, b1.y);
	float3 n2 = float3(b2.y, 0, b2.x);
	float3 n3 = float3(b3.x, b3.y, 0);
	// Finally, blend the results of the 3 planar projections.  
	float4 blendColor = col1.xyzw * blend_weights.x +  
                col2.xyzw * blend_weights.y +  
                col3.xyzw * blend_weights.z;  
	float3 blendBump = n1.xyz * blend_weights.x +  
                   n2.xyz * blend_weights.y +  
                   n3.xyz * blend_weights.z;
    N = normalize(blendBump+N);//mul(blendBump, TBN)); 
    float3 climate = pow(tex2D(ClimateSAMP, float2(input.LocalPos.y*0.5+0.5, 0.5)), 2.2);
    OUT.Color = float4(blendColor*0.5 + climate*0.15,1);
    OUT.Normal = float4(CompressNormal(N),0,0);
    OUT.Depth = length(input.WorldPos-EyePos.xyz)/EyePos.w;
    OUT.Data = 0;
    return OUT;
}