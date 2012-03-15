#include "Prepass.h"
#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal	: TEXCOORD1;
    float3 Tangent  : TEXCOORD2;
    float3 WorldPos : TEXCOORD3;
};

GBUFFER main(PSIN IN, uniform sampler BaseMap : register(S0),
			uniform sampler NormalMap : register(S1), 
			uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	GBUFFER OUT;
	float4 baseColor = tex2D(BaseMap, IN.TexCoord);
    float3 N = float3(0,1,0);
	float3 T = normalize(IN.Tangent);
	float3x3 TBN = float3x3(T,cross(N,T), N);
	
	//N = normalize(mul(tex2D(NormalMap, IN.TexCoord).rgb*2-1, TBN));
	
    OUT.Color = baseColor * 0.3f;
    OUT.Normal = float4(CompressNormal(N),0,0);
    OUT.Depth = length(IN.WorldPos-EyePos.xyz)/EyePos.w;
    OUT.Data = 0;
    return OUT;
}