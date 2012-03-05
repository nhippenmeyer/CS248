#include "../ShaderConst.h"
#include "../Prepass.h"

float4 main(float4 TexCoord : TEXCOORD0, float3 ObjPosition : TEXCOORD1, uniform sampler GBuffer : register(S0), 
			uniform sampler DepthMap : register(S1), uniform sampler DataMap : register(S2),
			uniform float3 LightColor : register(PC_LIGHTCOLOR), uniform float4 LightParams : register(PC_LIGHTPARAMS), 
			uniform float3 LightPos : register(PC_LIGHTPOS), uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	float2 TC = TexCoord.xy/TexCoord.w;
	float z = tex2D(DepthMap, TC).r*EyePos.w;
	clip(z-EPS);
	
	float3 V = normalize(EyePos.xyz-ObjPosition);
	float3 WorldPos = V*z+EyePos;
	float3 N = DecompressNormal(tex2D(GBuffer, TC).xy);
	float3 LightDir = LightPos-WorldPos;
	float3 L = normalize(LightDir);
	float len2 = dot(LightDir, LightDir);
	float attn = saturate(1-len2/LightParams.x)+saturate(1-sqrt(len2)/LightParams.y);
	
	float NDL = max(0.0, dot(N,L));
	float3 R = 2*N*NDL-L;
	float4 data = tex2D(DataMap, TC);
	float spec = pow(dot(R,V),data.r*255)*data.g;
	
	float4 finalColor = float4(NDL*LightColor, spec)*attn;
    return finalColor;
}