#include "../ShaderConst.h"
#include "../Prepass.h"

float4 main(float4 TexCoord : TEXCOORD0, float3 Direction : TEXCOORD1, uniform sampler GBuffer : register(S0), 
			uniform sampler DepthMap : register(S1), uniform sampler DataMap : register(S2),
			uniform float3 LightColor : register(PC_LIGHTCOLOR), uniform float4 LightParams : register(PC_LIGHTPARAMS), 
			uniform float3 LightPos : register(PC_LIGHTPOS)
) : COLOR
{
	clip(tex2D(DepthMap, TexCoord).r-EPS);
	float3 N = DecompressNormal(tex2D(GBuffer, TexCoord).xy);
	float3 L = normalize(LightPos);
	float3 V = normalize(Direction);
	
	float NDL = max(0.0, dot(N,L));
	float3 R = 2*N*NDL-L;
	float4 data = tex2D(DataMap, TexCoord);
	float spec = pow(dot(R,V),data.r*255)*data.g;
	
	float4 finalColor = 0;
	finalColor = float4(NDL*LightColor, spec);
    return finalColor;
}